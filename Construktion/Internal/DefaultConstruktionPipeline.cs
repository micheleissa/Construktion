﻿namespace Construktion.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DefaultConstruktionPipeline : ConstruktionPipeline
    {
        private readonly List<Type> _underConstruction = new List<Type>();
        private readonly InternalConstruktionSettings _settings;
        public ConstruktionSettings Settings => _settings;

        public DefaultConstruktionPipeline() : this(new DefaultConstruktionSettings()) { }

        internal DefaultConstruktionPipeline(InternalConstruktionSettings settings)
        {
            _settings = settings;
        }

        public object Send(ConstruktionContext context)
        {
            var blueprint = _settings.Blueprints.First(x => x.Matches(context));

            var result = Construct(context, blueprint);

            var exitBlueprint = _settings.ExitBlueprints.FirstOrDefault(x => x.Matches(result, context));

            result = exitBlueprint?.Construct(result, this) ?? result;

            return result;
        }

        public void Inject(Type type, object value)
        {
            _settings.Inject(type, value);
        }

        private object Construct(ConstruktionContext context, Blueprint blueprint)
        {
            if (RecurssionDetected(context))
            {
                return _settings.ThrowOnRecurrsion
                           ? throw new Exception($"Recursion Detected: {context.RequestType.FullName}")
                           : default(object);
            }

            _underConstruction.Add(context.RequestType);

            var result = blueprint.Construct(context, this);

            _underConstruction.Remove(context.RequestType);

            return result;
        }

        private bool RecurssionDetected(ConstruktionContext context)
        {
            var depth = _underConstruction.Count(x => context.RequestType == x);

            return depth > _settings.RecurssionDepth || (depth > 0 && _settings.ThrowOnRecurrsion);
        }
    }
}