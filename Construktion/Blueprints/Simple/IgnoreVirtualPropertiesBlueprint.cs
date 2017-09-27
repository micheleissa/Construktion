﻿using System.Reflection;

namespace Construktion.Blueprints.Simple
{
    public class IgnoreVirtualPropertiesBlueprint : Blueprint
    {
        public bool Matches(ConstruktionContext context)
        {
            var getMethod = context.PropertyInfo?.GetGetMethod();

            return getMethod != null && (getMethod.IsVirtual && !getMethod.IsFinal);
        }

        public object Construct(ConstruktionContext context, ConstruktionPipeline pipeline)
        {
            return default(object);
        }
    }
}