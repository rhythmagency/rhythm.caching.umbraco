namespace Rhythm.Caching.Umbraco.Invalidators
{

    // Namespaces.
    using Core.Caches;
    using Core.Invalidators;
    using EventHandlers;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Invalidates a cache based on the parent ID of any content node that is changed.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item stored in the cache.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// The type of key used by the cache.
    /// </typeparam>
    public class InvalidatorByParentPage<T> : ICacheByKeyInvalidator
    {

        #region Properties

        /// <summary>
        /// The cache to invalidate.
        /// </summary>
        private InstanceByKeyCache<T, int> Cache { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="cache">
        /// The cache to invalidate.
        /// </param>
        public InvalidatorByParentPage(InstanceByKeyCache<T, int> cache)
        {
            this.Cache = cache;
            UmbracoCachingHandlers.RegisterContentByParentInvalidator(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Invalidates the stored cache by the specified keys.
        /// </summary>
        /// <param name="keys">
        /// The keys to invalidate the cache for.
        /// </param>
        public void InvalidateForKeys(IEnumerable<object> keys)
        {
            var castedKeys = keys.Where(x => x is int).Cast<int>();
            this.Cache.ClearKeys(castedKeys);
        }

        #endregion

    }

}