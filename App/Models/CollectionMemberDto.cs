using CommunityToolkit.Mvvm.ComponentModel;
using Shared.Models.Common;

namespace App.Models;

public partial class CollectionMemberDto : ObservableObject
{
    private Guid userId;
    public Guid UserId
    {
        get => userId;
        set => SetProperty(ref userId, value);
    }

    private string username = string.Empty;
    public string Username
    {
        get => username;
        set => SetProperty(ref username, value);
    }

    private CollectionRole role;
    public CollectionRole Role
    {
        get => role;
        set => SetProperty(ref role, value);
    }

    // True when this item corresponds to the logged-in user.
    private bool isSelf;
    public bool IsSelf
    {
        get => isSelf;
        set => SetProperty(ref isSelf, value);
    }
}
