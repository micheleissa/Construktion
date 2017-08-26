﻿namespace Construktion.Debug
{
    using System.Collections.Generic;
    using global::Construktion;

    public class DebuggingConstruktion
    {
        private readonly Construktion _construktion;

        public DebuggingConstruktion() : this (new Construktion())
        {
            
        }

        public DebuggingConstruktion(Construktion construktion)
        {
            _construktion = construktion;
        }

        /// <summary>
        /// DO NOT use for normal operations. Should be used for ad hoc debugging ONLY.
        /// </summary>
        /// <returns></returns>
        public object DebuggingConstruct(ConstruktionContext context, out string debugLog)
        {
            var pipeline = new DebuggingConstruktionPipeline(_construktion._registry.ToSettings());

            var result = pipeline.DebugSend(context, out List<string> log);

            debugLog = string.Join("\n", log);

            return result;
        }
    }
}