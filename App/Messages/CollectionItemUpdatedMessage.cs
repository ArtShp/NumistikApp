using App.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace App.Messages;

public sealed class CollectionItemUpdatedMessage(CollectionItem value) : ValueChangedMessage<CollectionItem>(value)
{
}
