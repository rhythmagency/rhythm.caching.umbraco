namespace Rhythm.Caching.Umbraco.EventHandlers;

using global::Umbraco.Cms.Core.Composing;
using global::Umbraco.Cms.Core.DependencyInjection;
using global::Umbraco.Cms.Core.Notifications;

/// <summary>
/// A composer which registers caching services.
/// </summary>
public sealed class CachingComposer : IComposer
{
    /// <inheritdoc />
    public void Compose(IUmbracoBuilder builder)
	{
        // Register notification handlers.
        builder.AddNotificationHandler<ContentMovingNotification, ContentMoving>();
        builder.AddNotificationHandler<ContentMovedNotification, ContentMoved>();
        builder.AddNotificationHandler<ContentPublishedNotification, ContentPublished>();
        builder.AddNotificationHandler<ContentUnpublishedNotification, ContentUnpublished>();
        builder.AddNotificationHandler<ContentDeletedNotification, ContentDeleted>();
        builder.AddNotificationHandler<ContentCacheRefresherNotification, ContentCacheRefresher>();
    }
}
