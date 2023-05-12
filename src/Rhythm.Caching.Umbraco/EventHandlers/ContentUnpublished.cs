namespace Rhythm.Caching.Umbraco.EventHandlers;

// Namespaces.
using global::Umbraco.Cms.Core.Events;
using global::Umbraco.Cms.Core.Notifications;

/// <summary>
/// Handles the ContentUnpublishedNotification.
/// </summary>
public class ContentUnpublished : INotificationHandler<ContentUnpublishedNotification>
{
    /// <summary>
    /// Handle the ContentUnpublished notification.
    /// </summary>
    /// <param name="notification">
    /// The notification.
    /// </param>
    public void Handle(ContentUnpublishedNotification notification)
    {
        var nodes = notification.UnpublishedEntities;

        UmbracoCachingHandlers.HandleChangedContent(nodes);
    }
}
