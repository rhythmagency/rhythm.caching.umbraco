namespace Rhythm.Caching.Umbraco.EventHandlers;

// Namespaces.
using global::Umbraco.Cms.Core.Events;
using global::Umbraco.Cms.Core.Notifications;
using System.Linq;

/// <summary>
/// Handles the ContentMovedNotification.
/// </summary>
public class ContentMoved : INotificationHandler<ContentMovedNotification>
{
    /// <summary>
    /// Handle the ContentMoved notification.
    /// </summary>
    /// <param name="notification">
    /// The notification.
    /// </param>
    public void Handle(ContentMovedNotification notification)
    {
        var nodes = notification
            .MoveInfoCollection
            .Select(x => x.Entity);

        UmbracoCachingHandlers.HandleChangedContent(nodes);
    }
}
