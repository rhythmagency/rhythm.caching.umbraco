namespace Rhythm.Caching.Umbraco.EventHandlers
{

    // Namespaces.
    using Core.Invalidators;
    using global::Umbraco.Core;
    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Events;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Publishing;
    using global::Umbraco.Core.Services;
    using global::Umbraco.Core.Sync;
    using global::Umbraco.Web.Cache;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Handles application events used by caching.
    /// </summary>
    /// <remarks>
    /// Invalidators are stored in weak references because there is no way to unregister an
    /// invalidator, which could lead to memory building up if lots of unused invalidators
    /// remain in memory. By using weak references that are periodically pruned, we can
    /// avoid that buildup of memory.
    /// </remarks>
    public class UmbracoCachingHandlers : ApplicationEventHandler
    {

        #region Private Properties

        /// <summary>
        /// Lock object to prevent cross-thread issues.
        /// </summary>
        private static object KeyByParentInvalidatorsLock { get; set; }

        /// <summary>
        /// Lock object to prevent cross-thread issues.
        /// </summary>
        private static object KeyByPageInvalidatorsLock { get; set; }

        /// <summary>
        /// Lock object to prevent cross-thread issues.
        /// </summary>
        private static object InvalidatorsForAliasesLock { get; set; }

        /// <summary>
        /// Invalidators for caches that are stored by key and that relate to the parent
        /// of the content node of interest.
        /// </summary>
        private static List<WeakReference<ICacheByKeyInvalidator>> KeyByParentInvalidators { get; set; }

        /// <summary>
        /// Invalidators for caches that are stored by key and that relate to the content
        /// node of interest.
        /// </summary>
        private static List<WeakReference<ICacheByKeyInvalidator>> KeyByPageInvalidators { get; set; }

        /// <summary>
        /// Invalidators for caches that are stored by key and that relate to the content
        /// node of interest.
        /// </summary>
        private static List<WeakReference<ICacheInvalidator>> InvalidatorsForAliases { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers a cache by key invalidator so that it can be notified to invalidate any content
        /// that is the parent of a changed content.
        /// </summary>
        /// <param name="invalidator">
        /// The invalidator to notify when changes occur.
        /// </param>
        /// <remarks>
        /// For example, if the "Home > About" content node changes, the invalidator will be notified
        /// based on the ID of the "Home" node (because it is the parent of the changed content node).
        /// </remarks>
        public static void RegisterContentByParentInvalidator(ICacheByKeyInvalidator invalidator)
        {
            if (invalidator != null)
            {
                lock (KeyByParentInvalidatorsLock)
                {
                    var weakReference = new WeakReference<ICacheByKeyInvalidator>(invalidator);
                    KeyByParentInvalidators.Add(weakReference);
                }
            }
        }

        /// <summary>
        /// Registers a cache by key invalidator so that it can be notified to invalidate any content
        /// that is changed.
        /// </summary>
        /// <param name="invalidator">
        /// The invalidator to notify when changes occur.
        /// </param>
        /// <remarks>
        /// For example, if the "Home > About" content node changes, the invalidator will be notified
        /// based on the ID of the "Home > About" node.
        /// </remarks>
        public static void RegisterContentByPageInvalidator(ICacheByKeyInvalidator invalidator)
        {
            if (invalidator != null)
            {
                lock (KeyByPageInvalidatorsLock)
                {
                    var weakReference = new WeakReference<ICacheByKeyInvalidator>(invalidator);
                    KeyByPageInvalidators.Add(weakReference);
                }
            }
        }

        /// <summary>
        /// Registers a cache invalidator so that it can be notified to invalidate any content
        /// that is changed.
        /// </summary>
        /// <param name="invalidator">
        /// The invalidator to notify when changes occur.
        /// </param>
        /// <remarks>
        /// For example, if the "Home > About" content node changes, the invalidator will be notified
        /// based on the content type alias of the "Home > About" node.
        /// </remarks>
        public static void RegisterContentInvalidatorForAliases(ICacheInvalidator invalidator,
            IEnumerable<string> aliases)
        {
            if (invalidator != null)
            {
                lock (InvalidatorsForAliasesLock)
                {
                    var weakReference = new WeakReference<ICacheInvalidator>(invalidator);
                    InvalidatorsForAliases.Add(weakReference);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor.
        /// </summary>
        static UmbracoCachingHandlers()
        {
            KeyByParentInvalidatorsLock = new object();
            KeyByParentInvalidators = new List<WeakReference<ICacheByKeyInvalidator>>();
            KeyByPageInvalidatorsLock = new object();
            KeyByPageInvalidators = new List<WeakReference<ICacheByKeyInvalidator>>();
            InvalidatorsForAliasesLock = new object();
            InvalidatorsForAliases = new List<WeakReference<ICacheInvalidator>>();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles application startup.
        /// </summary>
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {

            // Listen for content change events.
            ContentService.Moving += ContentService_Moving;
            ContentService.Moved += ContentService_Moved;
            ContentService.Published += ContentService_Published;
            ContentService.Deleted += ContentService_Deleted;
            PageCacheRefresher.CacheUpdated += PageCacheRefresher_CacheUpdated;

            // Boilerplate.
            base.ApplicationStarting(umbracoApplication, applicationContext);

        }

        /// <summary>
        /// Content cache was updated.
        /// </summary>
        private void PageCacheRefresher_CacheUpdated(PageCacheRefresher sender,
            CacheRefresherEventArgs e)
        {
            var kind = e.MessageType;
            if (kind == MessageType.RefreshById || kind == MessageType.RemoveById)
            {
                var id = e.MessageObject as int?;
                if (id.HasValue)
                {
                    var contentService = ApplicationContext.Current.Services.ContentService;
                    var node = contentService.GetById(id.Value);
                    if (node != null)
                    {
                        HandleChangedContent(new[] { node });
                    }
                }
            }
        }

        /// <summary>
        /// Content nodes were deleted.
        /// </summary>
        private void ContentService_Deleted(IContentService sender,
            DeleteEventArgs<IContent> e)
        {
            var nodes = e.DeletedEntities;
            HandleChangedContent(nodes);
        }

        /// <summary>
        /// Content nodes were published.
        /// </summary>
        private void ContentService_Published(IPublishingStrategy sender,
            PublishEventArgs<IContent> e)
        {
            var nodes = e.PublishedEntities;
            HandleChangedContent(nodes);
        }

        /// <summary>
        /// Content nodes are moving.
        /// </summary>
        private void ContentService_Moving(IContentService sender,
            MoveEventArgs<IContent> e)
        {
            var nodes = e.MoveInfoCollection.Select(x => x.Entity);
            HandleChangedContent(nodes);
        }

        /// <summary>
        /// Content nodes were moved.
        /// </summary>
        private void ContentService_Moved(IContentService sender,
            MoveEventArgs<IContent> e)
        {
            var nodes = e.MoveInfoCollection.Select(x => x.Entity);
            HandleChangedContent(nodes);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Handles changed content.
        /// </summary>
        /// <param name="ids">The ID's of the content items that were changed.</param>
        private void HandleChangedContent(IEnumerable<IContent> ids)
        {

            // Variables.
            var pageIds = ids.Select(x => x.Id).Cast<object>().ToList();
            var foundByPageInvaldiators = GetKeyByPageInvalidators();
            var aliases = ids.Select(x => x.ContentType.Alias).Distinct().ToList();
            var foundByParentInvaldiators = GetKeyByParentInvalidators();
            var parentIds = ids.Select(x => x.ParentId)
                // Only include valid content ID's (i.e., to exclude the root content).
                .Where(x => x >= 0)
                .Cast<object>().ToList();
            var foundInvalidatorsForAliases = GetPageInvalidatorsForAliases();

            // Call invalidators for the changed ID's.
            foreach (var invalidator in foundByPageInvaldiators)
            {

                // For any content node that changed, invalidate by the page ID.
                invalidator.InvalidateForKeys(pageIds);

            }
            foreach (var invalidator in foundByParentInvaldiators)
            {

                // For any content node that changed, invalidate by the parent ID's.
                invalidator.InvalidateForKeys(parentIds);

            }

            // Call invalidators for the changed aliases.
            foreach (var invalidator in foundInvalidatorsForAliases)
            {

                // For any content node that changed, invalidate by the content type alias.
                invalidator.InvalidateForAliases(aliases);

            }

        }

        /// <summary>
        /// Returns the invalidators that are stored by the parent's content node ID.
        /// </summary>
        /// <returns>
        /// The invalidators.
        /// </returns>
        private List<ICacheByKeyInvalidator> GetKeyByParentInvalidators()
        {

            // Variables.
            var foundByParentInvaldiators = new List<ICacheByKeyInvalidator>();

            // Find invalidators that are still in memory.
            lock (KeyByParentInvalidatorsLock)
            {
                var invalidators = KeyByParentInvalidators;
                foundByParentInvaldiators = GetLiveValues(ref invalidators);
                KeyByParentInvalidators = invalidators;
            }

            // Return the found "key by parent" invalidators.
            return foundByParentInvaldiators;

        }

        /// <summary>
        /// Returns the invalidators that are stored by the page's content node ID.
        /// </summary>
        /// <returns>
        /// The invalidators.
        /// </returns>
        private List<ICacheByKeyInvalidator> GetKeyByPageInvalidators()
        {

            // Variables.
            var liveInvalidators = new List<ICacheByKeyInvalidator>();

            // Find invalidators that are still in memory.
            lock (KeyByPageInvalidatorsLock)
            {
                var invalidators = KeyByPageInvalidators;
                liveInvalidators = GetLiveValues(ref invalidators);
                KeyByPageInvalidators = invalidators;
            }

            // Return the found "key by page" invalidators.
            return liveInvalidators;

        }

        /// <summary>
        /// Returns the invalidators that are based on the page's content type alias.
        /// </summary>
        /// <returns>
        /// The invalidators.
        /// </returns>
        private List<ICacheInvalidator> GetPageInvalidatorsForAliases()
        {

            // Variables.
            var liveInvalidators = new List<ICacheInvalidator>();

            // Find invalidators that are still in memory.
            lock (InvalidatorsForAliasesLock)
            {
                var invalidators = InvalidatorsForAliases;
                liveInvalidators = GetLiveValues(ref invalidators);
                InvalidatorsForAliases = invalidators;
            }

            // Return the found invalidators.
            return liveInvalidators;

        }

        /// <summary>
        /// Gets the values in a list of weak references that aren't stale yet.
        /// </summary>
        /// <typeparam name="T">
        /// The type of item stored by each weak reference.
        /// </typeparam>
        /// <param name="items">
        /// The list of weak references.
        /// </param>
        /// <returns>
        /// The list of values that aren't stale.
        /// </returns>
        /// <remarks>
        /// The supplied item list will have stale references removed.
        /// </remarks>
        private List<T> GetLiveValues<T>(ref List<WeakReference<T>> items) where T : class
        {

            // Variables.
            var foundItems = new List<T>();
            var newItems = new List<WeakReference<T>>();

            // Find invalidators that are still in memory.
            for (var i = 0; i < items.Count; i++)
            {
                var weakItem = items[i];
                if (weakItem != null)
                {
                    var item = default(T);
                    if (weakItem.TryGetTarget(out item))
                    {
                        newItems.Add(weakItem);
                        foundItems.Add(item);
                    }
                }
            }

            // Remove stale references.
            items = newItems;

            // Return found items.
            return foundItems;

        }

        #endregion

    }

}