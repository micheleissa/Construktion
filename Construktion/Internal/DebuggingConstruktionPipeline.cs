﻿namespace Construktion.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Blueprints.Simple;

    public class DebuggingConstruktionPipeline : ConstruktionPipeline
    {
        private readonly List<string> _log;
        private readonly List<Type> _underConstruction;
        private int level;

        private readonly List<Blueprint> _blueprints;
        private readonly ConstruktionSettings _settings;
        public ConstruktionSettings Settings => _settings;

        public DebuggingConstruktionPipeline() : this (new DefaultConstruktionSettings())
        {
            
        }

        internal DebuggingConstruktionPipeline(ConstruktionSettings settings)
        {
            _settings = settings;
            _blueprints = settings.Blueprints.ToList();
            _log = new List<string>();
            _underConstruction = new List<Type>();
            level = -1;
        }

        public void Inject(Type type, object value) => _blueprints.Insert(0, new SingletonBlueprint(type, value));

        public object Send(ConstruktionContext context) => DebugSend(context, out List<string> debugLog);

        public object DebugSend(ConstruktionContext context, out List<string> debugLog)
        {
            var depth = _underConstruction.Count(x => context.RequestType == x);

            var blueprint = Settings.Blueprints.First(x => x.Matches(context));

            level++;
            var indent = new string(' ', level * 5);

            if (depth > Settings.RecursionDepth)
            {
                _log.Add($"{indent}Recursion detected over the allowed limit. Omitting {context.RequestType.FullName}");
                debugLog = _log;

                return default(object);
            }

            var requestName = StartLog(context, blueprint, indent);

            object result;
            try
            {
                result = blueprint.Construct(context, this);

                var exitBlueprint = _settings.ExitBlueprints.FirstOrDefault(x => x.Matches(result, context));

                if (exitBlueprint != null)
                    _log.Add($"{indent}ExitBlueprint: {exitBlueprint}");

                result = exitBlueprint?.Construct(result, this) ?? result;
            }
            catch (OutOfMemoryException)
            {
                throw new Exception(
                    "Oops you ran out of memory! It looks like this object graph is REALLY big. " +
                    "It might be best to add a blueprint that constructs your object manually, " +
                    "or revisit the design of your object. The current log can be found in the inner exception.",
                    new Exception(string.Join("\n", _log)));
            }
            catch (Exception ex)
            {
                _log.Add(
                    $"{indent}An exception occurred in blueprint {blueprint}, when constructing {requestName}");
                _log.Add($"{indent}Exception is: {ex.Message}");
                _log.Add("");
                debugLog = _log;

                return default(object);
            }

            EndLog(context, indent, requestName);

            debugLog = _log;

            return result;
        }

        private string StartLog(ConstruktionContext requestContext, Blueprint blueprint, string buffer)
        {
            var requestName = requestContext.RequestType.FullName;
            var start = $"Start: {requestName}";

            if (requestContext.PropertyInfo.IsNulloPropertyInfo() == false) 
            {
                requestName = requestContext.PropertyInfo.Name;
                start = $"Start Property: {requestName}";
            }
            else if (requestContext.ParameterInfo.IsNulloParameterInfo() == false)
            {
                requestName = requestContext.ParameterInfo.Name;
                start = $"Start Parameter: {requestName}";
            }

            _underConstruction.Add(requestContext.RequestType);

            if (level != 0)
                _log.Add("");

            _log.Add($"{buffer}{start}");
            _log.Add($"{buffer}Blueprint: {blueprint}");

            return requestName;
        }

        private void EndLog(ConstruktionContext context, string buffer, string requestName)
        {
            level--;
            _underConstruction.Remove(context.RequestType);

            if (_log.Last().Trim().StartsWith("End"))
                _log.Add("");

            _log.Add($"{buffer}End {requestName}");
        }
    }
}