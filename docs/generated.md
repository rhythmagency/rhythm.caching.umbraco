# Rhythm.Caching.Umbraco

<table>
<tbody>
<tr>
<td><a href="#cachehelper">CacheHelper</a></td>
<td><a href="#umbracocachinghandlers">UmbracoCachingHandlers</a></td>
</tr>
<tr>
<td><a href="#invalidatorbypage\`1">InvalidatorByPage\`1</a></td>
<td><a href="#invalidatorbypagealiases\`1">InvalidatorByPageAliases\`1</a></td>
</tr>
<tr>
<td><a href="#invalidatorbyparentpage\`1">InvalidatorByParentPage\`1</a></td>
</tr>
</tbody>
</table>


## CacheHelper

Assists with caching operations.

### PreviewCacheKeys

Returns the keys to be used when caching, depending on if the site is currently in preview mode or not.

#### Remarks

This allows for multiple caches to be used (one for live, and one for preview mode).


## UmbracoCachingHandlers

Handles application events used by caching.

#### Remarks

Invalidators are stored in weak references because there is no way to unregister an invalidator, which could lead to memory building up if lots of unused invalidators remain in memory. By using weak references that are periodically pruned, we can avoid that buildup of memory.

### #cctor

Static constructor.

### ApplicationStarting(Umbraco.Core.UmbracoApplicationBase,Umbraco.Core.ApplicationContext)

Handles application startup.

### ContentService_Deleted(Umbraco.Core.Services.IContentService,Umbraco.Core.Events.DeleteEventArgs{Umbraco.Core.Models.IContent})

Content nodes were deleted.

### ContentService_Moved(Umbraco.Core.Services.IContentService,Umbraco.Core.Events.MoveEventArgs{Umbraco.Core.Models.IContent})

Content nodes were moved.

### ContentService_Moving(Umbraco.Core.Services.IContentService,Umbraco.Core.Events.MoveEventArgs{Umbraco.Core.Models.IContent})

Content nodes are moving.

### ContentService_Published(Umbraco.Core.Publishing.IPublishingStrategy,Umbraco.Core.Events.PublishEventArgs{Umbraco.Core.Models.IContent})

Content nodes were published.

### GetKeyByPageInvalidators

Returns the invalidators that are stored by the page's content node ID.

#### Returns

The invalidators.

### GetKeyByParentInvalidators

Returns the invalidators that are stored by the parent's content node ID.

#### Returns

The invalidators.

### GetLiveValues\`\`1(items)

Gets the values in a list of weak references that aren't stale yet.

#### Type Parameters

- T - The type of item stored by each weak reference.

| Name | Description |
| ---- | ----------- |
| items | *System.Collections.Generic.List{System.WeakReference{\`\`0}}@*<br>The list of weak references. |

#### Returns

The list of values that aren't stale.

#### Remarks

The supplied item list will have stale references removed.

### GetPageInvalidatorsForAliases

Returns the invalidators that are based on the page's content type alias.

#### Returns

The invalidators.

### HandleChangedContent(ids)

Handles changed content.

| Name | Description |
| ---- | ----------- |
| ids | *System.Collections.Generic.IEnumerable{Umbraco.Core.Models.IContent}*<br>The ID's of the content items that were changed. |

### InvalidatorsForAliases

Invalidators for caches that are stored by key and that relate to the content node of interest.

### InvalidatorsForAliasesLock

Lock object to prevent cross-thread issues.

### KeyByPageInvalidators

Invalidators for caches that are stored by key and that relate to the content node of interest.

### KeyByPageInvalidatorsLock

Lock object to prevent cross-thread issues.

### KeyByParentInvalidators

Invalidators for caches that are stored by key and that relate to the parent of the content node of interest.

### KeyByParentInvalidatorsLock

Lock object to prevent cross-thread issues.

### PageCacheRefresher_CacheUpdated(Umbraco.Web.Cache.PageCacheRefresher,Umbraco.Core.Cache.CacheRefresherEventArgs)

Content cache was updated.

### RegisterContentByPageInvalidator(invalidator)

Registers a cache by key invalidator so that it can be notified to invalidate any content that is changed.

| Name | Description |
| ---- | ----------- |
| invalidator | *Rhythm.Caching.Core.Invalidators.ICacheByKeyInvalidator*<br>The invalidator to notify when changes occur. |

#### Remarks

For example, if the "Home > About" content node changes, the invalidator will be notified based on the ID of the "Home > About" node.

### RegisterContentByParentInvalidator(invalidator)

Registers a cache by key invalidator so that it can be notified to invalidate any content that is the parent of a changed content.

| Name | Description |
| ---- | ----------- |
| invalidator | *Rhythm.Caching.Core.Invalidators.ICacheByKeyInvalidator*<br>The invalidator to notify when changes occur. |

#### Remarks

For example, if the "Home > About" content node changes, the invalidator will be notified based on the ID of the "Home" node (because it is the parent of the changed content node).

### RegisterContentInvalidatorForAliases(invalidator)

Registers a cache invalidator so that it can be notified to invalidate any content that is changed.

| Name | Description |
| ---- | ----------- |
| invalidator | *Rhythm.Caching.Core.Invalidators.ICacheInvalidator*<br>The invalidator to notify when changes occur. |

#### Remarks

For example, if the "Home > About" content node changes, the invalidator will be notified based on the content type alias of the "Home > About" node.


## InvalidatorByPage\`1

Invalidates a cache based on the ID of any content node that is changed.

#### Type Parameters

- T - The type of item stored in the cache.
- TKey - The type of key used by the cache.

### Constructor(cache)

Primary constructor.

| Name | Description |
| ---- | ----------- |
| cache | *Rhythm.Caching.Core.Caches.InstanceByKeyCache{\`0,System.Int32}*<br>The cache to invalidate. |

### Cache

The cache to invalidate.

### InvalidateForKeys(keys)

Invalidates the stored cache by the specified keys.

| Name | Description |
| ---- | ----------- |
| keys | *System.Collections.Generic.IEnumerable{System.Object}*<br>The keys to invalidate the cache for. |


## InvalidatorByPageAliases\`1

Invalidates a cache based on the content type alias of any content node that is changed.

#### Type Parameters

- T - The type of item stored in the cache.

### Constructor(cache, aliases)

Primary constructor.

| Name | Description |
| ---- | ----------- |
| cache | *Rhythm.Caching.Core.Caches.InstanceCache{\`0}*<br>The cache to invalidate. |
| aliases | *System.String[]*<br>The content type aliases to monitor for changes that will initiate an invalidation. |

### Cache

The cache to invalidate.

### Invalidate

Invalidate the cache unconditionally.

### InvalidateForAliases(aliases)

Invalidates the cache if the specified aliases match the monitored aliases.

| Name | Description |
| ---- | ----------- |
| aliases | *System.Collections.Generic.IEnumerable{System.String}*<br>The aliases that should cause the cache to invalidate. |

### MonitoredAliases

The aliases to monitor for cache invalidation.


## InvalidatorByParentPage\`1

Invalidates a cache based on the parent ID of any content node that is changed.

#### Type Parameters

- T - The type of item stored in the cache.
- TKey - The type of key used by the cache.

### Constructor(cache)

Primary constructor.

| Name | Description |
| ---- | ----------- |
| cache | *Rhythm.Caching.Core.Caches.InstanceByKeyCache{\`0,System.Int32}*<br>The cache to invalidate. |

### Cache

The cache to invalidate.

### InvalidateForKeys(keys)

Invalidates the stored cache by the specified keys.

| Name | Description |
| ---- | ----------- |
| keys | *System.Collections.Generic.IEnumerable{System.Object}*<br>The keys to invalidate the cache for. |
