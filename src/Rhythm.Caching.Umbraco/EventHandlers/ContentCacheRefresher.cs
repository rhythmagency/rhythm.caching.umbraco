namespace Rhythm.Caching.Umbraco.EventHandlers;

// Namespaces.
using global::Umbraco.Cms.Core.Events;
using global::Umbraco.Cms.Core.Notifications;
using global::Umbraco.Cms.Core.Services;
using global::Umbraco.Cms.Core.Sync;

/// <summary>
/// Handles the ContentCacheRefresherNotification.
/// </summary>
public class ContentCacheRefresher : INotificationHandler<ContentCacheRefresherNotification>
{
    private readonly IContentService _contentService;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="contentService">
    /// The Umbraco ContentService.
    /// </param>
    public ContentCacheRefresher(IContentService contentService)
    {
        _contentService = contentService;
    }

    /// <summary>
    /// Handle the ContentCacheRefresher notification.
    /// </summary>
    /// <param name="notification">
    /// The notification.
    /// </param>
    public void Handle(ContentCacheRefresherNotification notification)
    {
        var kind = notification.MessageType;
        if (kind == MessageType.RefreshById || kind == MessageType.RemoveById)
        {
            var id = notification.MessageObject as int?;
            if (id.HasValue)
            {
                var node = _contentService.GetById(id.Value);
                if (node != null)
                {
                    UmbracoCachingHandlers.HandleChangedContent(new[] { node });
                }
            }
        }
    }
}
