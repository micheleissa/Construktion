﻿// ReSharper disable UseMethodAny.2
namespace Construktion.Blueprints.Simple
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Internal;

    public class OmitPropertyBlueprint : Blueprint
    {
        private readonly Func<PropertyInfo, bool> _convention;
        private readonly IEnumerable<Type> _propertyTypes;

        public OmitPropertyBlueprint(Func<PropertyInfo, bool> convention, Type propertyType) : this(convention,
            new List<Type> { propertyType }) { }

        public OmitPropertyBlueprint(Func<PropertyInfo, bool> convention, IEnumerable<Type> propertyTypes)
        {
            _convention = convention ?? throw new ArgumentNullException(nameof(convention));
            _propertyTypes = propertyTypes ?? throw new ArgumentNullException(nameof(propertyTypes));
        }

        public bool Matches(ConstruktionContext context)
        {
            var matchesType = _propertyTypes.Contains(context.RequestType) ||
                              _propertyTypes.Count() == 0 ||
                              containsGeneric();

            return context.PropertyInfo != null && _convention(context.PropertyInfo) && matchesType;

            bool containsGeneric()
            {
                var typeInfo = context.RequestType.GetTypeInfo();

                return typeInfo.IsGenericType && _propertyTypes.Contains(typeInfo.GetGenericTypeDefinition());
            }
        }

        public object Construct(ConstruktionContext context, ConstruktionPipeline pipeline) => context.RequestType.GetTypeInfo().IsValueType
                                                                                                   ? Activator.CreateInstance(context.RequestType)
                                                                                                   : null;
    }
}