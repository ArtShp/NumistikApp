using CommunityToolkit.Mvvm.ComponentModel;

namespace App.Models;

public partial class CreateCollectionForm : ObservableObject
{
    private bool _isVisible;
    private string _name = string.Empty;
    private string? _description;
    private bool _isCreating;

    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public bool IsCreating
    {
        get => _isCreating;
        set => SetProperty(ref _isCreating, value);
    }

    public void Reset()
    {
        Name = string.Empty;
        Description = null;
    }
}
