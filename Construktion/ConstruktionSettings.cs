namespace Construktion
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public interface ConstruktionSettings
    {
        /// <summary>
        /// The configured blueprints. The pipeline will evaluate them in the returned order.
        /// </summary>
        IEnumerable<Blueprint> Blueprints { get; }

        /// <summary>
        /// The configured exit blueprints. These are evaluated the same way the normal ones are.
        /// </summary>
        IEnumerable<ExitBlueprint> ExitBlueprints { get; }

        /// <summary>
        /// When a key in the dictionary is requested, will construct the value. Usually used to construct interfaces.
        /// </summary>
        IDictionary<Type, Type> TypeMap { get; }

        /// <summary>
        /// Resolve the constructor (Greedy or Modest). Uses modest by default. The modest constructor is the one
        /// with the fewest arguments.
        /// </summary>
        ConstructorInfo CtorStrategy(IEnumerable<ConstructorInfo> constructors);

        /// <summary>
        /// Resolve an objects properties to construct. By default only properties with public setters are constructed.
        /// </summary>
        IEnumerable<PropertyInfo> PropertyStrategy(Type requestType);

        /// <summary>
        /// The amount of items to create when any IEnumerable (or array) is requested. The Default is 3.
        /// </summary>
        int EnumerableCount { get; }

        /// <summary>
        /// How many levels of recursion to construct. By default recursive properties are ignored.
        /// </summary>
        int RecursionDepth { get; }

        /// <summary>
        /// When true, an exception will be thrown when Recursion is detected. False by default.
        /// </summary>
        bool ThrowOnRecursion { get; }

        /// <summary>
        /// How many levels of nested properties that will be constructed
        /// </summary>
        int? MaxDepth { get; }
    }
}