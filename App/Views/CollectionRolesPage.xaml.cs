using App.Models;
using App.ViewModels;
using Shared.Models.Common;

namespace App.Views;

public partial class CollectionRolesPage : ContentPage
{
    public CollectionRolesPage(CollectionRolesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((CollectionRolesViewModel)BindingContext).InitializeAsync();
    }

    private async void OnUpdateClicked(object sender, EventArgs e)
    {
        if (sender is not Button button) return;
        if (button.BindingContext is not CollectionMemberDto member) return;

        if (member.IsSelf)
        {
            await DisplayAlert("Not allowed", "You can't change your own role.", "OK");
            return;
        }

        var viewModel = (CollectionRolesViewModel)BindingContext;

        var myRole = viewModel.Members.FirstOrDefault(m => m.IsSelf)?.Role;
        if (myRole is null)
        {
            await DisplayAlert("Not allowed", "Your role is not known yet. Please refresh and try again.", "OK");
            return;
        }

        if (member.Role >= myRole.Value)
        {
            await DisplayAlert("Not allowed", "You can't change the role of a user with equal or higher role than yours.", "OK");
            return;
        }

        var roles = viewModel.Roles
            .Where(r => r < myRole.Value)
            .ToList();

        if (roles.Count == 0)
        {
            await DisplayAlert("Not allowed", "No roles available to assign.", "OK");
            return;
        }

        var roleOptions = roles
            .Select(role => new
            {
                Display = role == member.Role ? $"✓ {role} (current)" : role.ToString(),
                Value = role
            })
            .ToList();

        var selectedText = await DisplayActionSheet(
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

        // Confirm change
        var confirm = await DisplayAlert(
            "Confirm role change",
            $"Are you sure you want to change {member.Username}'s role from {oldRole} to {newRole}?",
            "Change",
            "Cancel"
        );
        if (!confirm) return;

        var ok = await viewModel.UpdateRoleAsync(member, newRole);

        if (ok)
        {
            member.Role = newRole;
        }
    }

    private async void OnAssignClicked(object sender, EventArgs e)
    {
        var viewModel = (CollectionRolesViewModel)BindingContext;

        var myRole = viewModel.Members.FirstOrDefault(m => m.IsSelf)?.Role;
        if (myRole is null)
        {
            await DisplayAlert("Not allowed", "Your role is not known yet. Please refresh and try again.", "OK");
            return;
        }

        var allowedRoles = viewModel.Roles
            .Where(r => r < myRole.Value)
            .ToList();

        if (allowedRoles.Count == 0)
        {
            await DisplayAlert("Not allowed", "No roles available to assign.", "OK");
            return;
        }

        var username = await DisplayPromptAsync(
            "Assign role",
            "Enter the username to assign a role to:",
            "Continue",
            "Cancel",
            keyboard: Keyboard.Text);

        if (string.IsNullOrWhiteSpace(username)) return;

        if (viewModel.Members.Any(m => string.Equals(m.Username, username)))
        {
            var overwrite = await DisplayAlert("User already a member",
                "User already exists in this collection. Do you want to change their role instead?",
                "Yes",
                "No");
            if (!overwrite) return;

            var existing = viewModel.Members.First(m => string.Equals(m.Username, username));
            OnUpdateClicked(new Button { BindingContext = existing }, EventArgs.Empty);

            return;
        }

        var roleOptions = allowedRoles
            .Select(r => r.ToString())
            .ToArray();

        var selectedRoleText = await DisplayActionSheet(
            $"Select role for {username}",
            "Cancel",
            null,
            roleOptions);

        if (string.IsNullOrWhiteSpace(selectedRoleText) || selectedRoleText == "Cancel")
            return;

        if (!Enum.TryParse(selectedRoleText, out CollectionRole selectedRole))
            return;

        var confirm = await DisplayAlert(
            "Confirm assignment",
            $"Assign role {selectedRole} to user '{username}'?",
            "Assign",
            "Cancel");

        if (!confirm) return;

        var okAssign = await viewModel.AssignRoleAsync(username.Trim(), selectedRole);
        if (okAssign)
        {
            await DisplayAlert("Success", $"Role {selectedRole} assigned to {username}.", "OK");
        }
    }
}
