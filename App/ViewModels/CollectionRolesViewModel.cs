using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Models;
using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Models.Common;

namespace App.ViewModels;

[QueryProperty(nameof(CollectionId), "collectionId")]
public partial class CollectionRolesViewModel : ObservableObject
{
    private readonly ICollectionService _collectionService;

    public ObservableCollection<CollectionMemberDto> Members { get; init; } = [];

    public ObservableCollection<CollectionRole> Roles { get; init; } = [];

    private Guid _collectionId;
    public string? CollectionId
    {
        set
        {
            if (Guid.TryParse(value, out var id))
            {
                _collectionId = id;
            }
        }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand RefreshCommand { get; init; }

    public CollectionRolesViewModel(ICollectionService collectionService)
    {
        _collectionService = collectionService;

        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
    }

    public async Task InitializeAsync()
    {
        if (Members.Count == 0 || Roles.Count == 0)
            await RefreshAsync();
    }

    public async Task<bool> UpdateRoleAsync(CollectionMemberDto member, CollectionRole newRole)
    {
        if (_collectionId == Guid.Empty) return false;

        var ok = await _collectionService.UpdateCollectionRoleAsync(_collectionId, member.UserId, newRole);

        if (!ok)
        {
            await Shell.Current.DisplayAlert("Error", "Unable to update role. Not enough permissions or invalid request.", "OK");
            await RefreshAsync();

            return false;
        }

        return true;
    }

    public async Task<bool> AssignRoleAsync(string username, CollectionRole role)
    {
        if (_collectionId == Guid.Empty) return false;

        var ok = await _collectionService.AssignCollectionRoleAsync(_collectionId, username, role);

        if (!ok)
        {
            await Shell.Current.DisplayAlert("Error", "Unable to assign role. User may not exist or you lack permissions.", "OK");
            return false;
        }

        await RefreshAsync();
        return true;
    }

    private async Task RefreshAsync()
    {
        if (_collectionId == Guid.Empty) return;

        IsLoading = true;
        try
        {
            Members.Clear();
            Roles.Clear();

            var membersTask = _collectionService.GetCollectionMembersAsync(_collectionId);
            var rolesTask = _collectionService.GetAssignableRolesAsync(_collectionId);

            await Task.WhenAll(membersTask, rolesTask);

            foreach (var role in rolesTask.Result)
            {
                Roles.Add(role);
            }

            var currentUsername = AppSettings.Username;
            foreach (var member in membersTask.Result)
            {
                member.IsSelf = string.Equals(member.Username, currentUsername);
                Members.Add(member);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}
