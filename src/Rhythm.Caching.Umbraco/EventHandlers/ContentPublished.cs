namespace Rhythm.Caching.Umbraco.EventHandlers;

// Namespaces.
using global::Umbraco.Cms.Core.Events;
using global::Umbraco.Cms.Core.Notifications;

/// <summary>
/// Handles the ContentPublishedNotification.
/// </summary>
public class ContentPublished : INotificationHandler<ContentPublishedNotification>
{
    /// <summary>
    /// Handle the ContentPublished notification.
    /// </summary>
    /// <param name="notification">
    /// The notification.
    /// </param>
    public void Handle(ContentPublishedNotification notification)
    {
        var nodes = notification.PublishedEntities;

        UmbracoCachingHandlers.HandleChangedContent(nodes);
    }
}
