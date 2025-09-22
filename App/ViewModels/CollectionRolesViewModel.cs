using App.Models;
using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Models.Common;
using System.Collections.ObjectModel;
using System.Windows.Input;

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
    public ICommand AssignCommand { get; init; }
    public ICommand UpdateCommand { get; init; }

    public CollectionRolesViewModel(ICollectionService collectionService)
    {
        _collectionService = collectionService;

        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        AssignCommand = new AsyncRelayCommand(AssignAsync);
        UpdateCommand = new AsyncRelayCommand<CollectionMemberDto>(UpdateAsync);
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

    private async Task UpdateAsync(CollectionMemberDto? member)
    {
        if (member is null) return;

        if (member.IsSelf)
        {
            await Shell.Current.DisplayAlert("Not allowed", "You can't change your own role.", "OK");
            return;
        }

        var myRole = Members.FirstOrDefault(m => m.IsSelf)?.Role;
        if (myRole is null)
        {
            await Shell.Current.DisplayAlert("Not allowed", "Your role is not known yet. Please refresh and try again.", "OK");
            return;
        }

        if (member.Role >= myRole.Value)
        {
            await Shell.Current.DisplayAlert("Not allowed", "You can't change the role of a user with equal or higher role than yours.", "OK");
            return;
        }

        var roles = Roles
            .Where(r => r < myRole.Value)
            .ToList();

        if (roles.Count == 0)
        {
            await Shell.Current.DisplayAlert("Not allowed", "No roles available to assign.", "OK");
            return;
        }

        var roleOptions = roles
            .Select(role => new
            {
                Display = role == member.Role ? $"✓ {role} (current)" : role.ToString(),
                Value = role
            })
            .ToList();

        var selectedText = await Shell.Current.DisplayActionSheet(
            $"Update role for {member.Username}",
            "Cancel",
            null,
            roleOptions.Select(o => o.Display).ToArray()
        );

        if (string.IsNullOrWhiteSpace(selectedText) || selectedText == "Cancel")
            return;

        var selected = roleOptions.FirstOrDefault(o => o.Display == selectedText);
        if (selected is null) return;

        var newRole = selected.Value;
        if (newRole == member.Role) return;

        var oldRole = member.Role;

        var confirm = await Shell.Current.DisplayAlert(
            "Confirm role change",
            $"Are you sure you want to change {member.Username}'s role from {oldRole} to {newRole}?",
            "Change",
            "Cancel"
        );
        if (!confirm) return;

        var ok = await UpdateRoleAsync(member, newRole);

        if (ok)
        {
            member.Role = newRole;
        }
    }

    private async Task AssignAsync()
    {
        var myRole = Members.FirstOrDefault(m => m.IsSelf)?.Role;
        if (myRole is null)
        {
            await Shell.Current.DisplayAlert("Not allowed", "Your role is not known yet. Please refresh and try again.", "OK");
            return;
        }

        var allowedRoles = Roles
            .Where(r => r < myRole.Value)
            .ToList();

        if (allowedRoles.Count == 0)
        {
            await Shell.Current.DisplayAlert("Not allowed", "No roles available to assign.", "OK");
            return;
        }

        var username = await Shell.Current.DisplayPromptAsync(
            "Assign role",
            "Enter the username to assign a role to:",
            "Continue",
            "Cancel",
            keyboard: Keyboard.Text);

        if (string.IsNullOrWhiteSpace(username)) return;
        username = username.Trim();

        if (Members.Any(m => string.Equals(m.Username, username)))
        {
            var overwrite = await Shell.Current.DisplayAlert(
                "User already a member",
                "User already exists in this collection. Do you want to change their role instead?",
                "Yes",
                "No");
            if (!overwrite) return;

            var existing = Members.First(m => string.Equals(m.Username, username));
            await UpdateAsync(existing);
            return;
        }

        var roleOptions = allowedRoles
            .Select(r => r.ToString())
            .ToArray();

        var selectedRoleText = await Shell.Current.DisplayActionSheet(
            $"Select role for {username}",
            "Cancel",
            null,
            roleOptions);

        if (string.IsNullOrWhiteSpace(selectedRoleText) || selectedRoleText == "Cancel")
            return;

        if (!Enum.TryParse(selectedRoleText, out CollectionRole selectedRole))
            return;

        var confirm = await Shell.Current.DisplayAlert(
            "Confirm assignment",
            $"Assign role {selectedRole} to user '{username}'?",
            "Assign",
            "Cancel");

        if (!confirm) return;

        var okAssign = await AssignRoleAsync(username, selectedRole);
        if (okAssign)
        {
            await Shell.Current.DisplayAlert("Success", $"Role {selectedRole} assigned to {username}.", "OK");
        }
    }
}
