namespace Rhythm.Caching.Umbraco.Invalidators
{

    // Namespaces.
    using Core.Caches;
    using Core.Invalidators;
    using EventHandlers;
    using Rhythm.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Invalidates a cache based on the content type alias of any content node that is changed.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item stored in the cache.
    /// </typeparam>
    public class InvalidatorByPageAliases<T> : ICacheInvalidator
    {

        #region Properties

        /// <summary>
        /// The cache to invalidate.
        /// </summary>
        private InstanceCache<T> Cache { get; set; }

        /// <summary>
        /// The aliases to monitor for cache invalidation.
        /// </summary>
        private IEnumerable<string> MonitoredAliases { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="cache">
        /// The cache to invalidate.
        /// </param>
        /// <param name="aliases">
        /// The content type aliases to monitor for changes that will initiate an invalidation.
        /// </param>
        public InvalidatorByPageAliases(InstanceCache<T> cache, params string[] aliases)
        {
            Cache = cache;
            MonitoredAliases = aliases.MakeSafe().ToList();
            UmbracoCachingHandlers.RegisterContentInvalidatorForAliases(this, aliases);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Invalidate the cache unconditionally.
        /// </summary>
        public void Invalidate()
        {
            this.Cache.Clear();
        }

        /// <summary>
        /// Invalidates the cache if the specified aliases match the monitored aliases.
        /// </summary>
        /// <param name="aliases">
        /// The aliases that should cause the cache to invalidate.
        /// </param>
        public void InvalidateForAliases(IEnumerable<string> aliases)
        {
            var ignoreCase = StringComparer.InvariantCultureIgnoreCase;
            var noAliases = !MonitoredAliases.Any();
            var hasMatchingAliases = MonitoredAliases.Intersect(aliases, ignoreCase).Any();
            var shouldClearCache = noAliases || hasMatchingAliases;
            if (shouldClearCache)
            {
                this.Cache.Clear();
            }
        }

        #endregion

    }

}