namespace Construktion.Blueprints
{
    using System;

    /// <summary>
    /// A blueprint that will construct the specified type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractBlueprint<T> : Blueprint
    {
        internal readonly Random _random = new Random();

        /// <summary>
        /// Mathces types of the closed generic. Can be overridden in derived classes
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual bool Matches(ConstruktionContext context)
        {
            return context.RequestType == typeof(T);
        }

        /// <summary>
        /// Contruct an object of T
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pipeline"></param>
        /// <returns></returns>
        public abstract object Construct(ConstruktionContext context, ConstruktionPipeline pipeline);
    }
}