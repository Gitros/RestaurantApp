using CommunityToolkit.Mvvm.Messaging.Messages;

namespace RestaurantApp.Messages;

public enum UiNotificationType
{
    Info,
    Success,
    Warning,
    Error
}

public sealed class UiNotificationMessage : ValueChangedMessage<(UiNotificationType Type, string Text)>
{
    public UiNotificationMessage(UiNotificationType type, string text)
        : base((type, text))
    {
    }
}
