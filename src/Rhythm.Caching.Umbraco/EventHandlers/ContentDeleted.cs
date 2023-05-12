namespace Rhythm.Caching.Umbraco.EventHandlers;

// Namespaces.
using global::Umbraco.Cms.Core.Events;
using global::Umbraco.Cms.Core.Notifications;

/// <summary>
/// Handles the ContentDeletedNotification.
/// </summary>
public class ContentDeleted : INotificationHandler<ContentDeletedNotification>
{
    /// <summary>
    /// Handle the ContentDeleted notification.
    /// </summary>
    /// <param name="notification">
    /// The notification.
    /// </param>
    public void Handle(ContentDeletedNotification notification)
    {
        var nodes = notification.DeletedEntities;

        UmbracoCachingHandlers.HandleChangedContent(nodes);
    }
}
