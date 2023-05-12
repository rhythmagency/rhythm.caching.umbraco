namespace Rhythm.Caching.Umbraco.EventHandlers;

// Namespaces.
using global::Umbraco.Cms.Core.Events;
using global::Umbraco.Cms.Core.Notifications;
using System.Linq;

/// <summary>
/// Handles the ContentMovingNotification.
/// </summary>
public class ContentMoving : INotificationHandler<ContentMovingNotification>
{
    /// <summary>
    /// Handle the ContentMoving notification.
    /// </summary>
    /// <param name="notification">
    /// The notification.
    /// </param>
    public void Handle(ContentMovingNotification notification)
    {
        var nodes = notification
            .MoveInfoCollection
            .Select(x => x.Entity);

        UmbracoCachingHandlers.HandleChangedContent(nodes);
    }
}
