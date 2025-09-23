using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Models.Auth;
using Shared.Models.Common;
using System.Windows.Input;

namespace App.ViewModels;

public partial class AdminViewModel : ObservableObject
{
    private readonly ILoginService _loginService;

    public ICommand BackToMainCommand { get; init; }
    public ICommand CreateInviteTokenCommand { get; init; }

    public AdminViewModel(ILoginService loginService)
    {
        _loginService = loginService;

        BackToMainCommand = new AsyncRelayCommand(OnBackToMain);
        CreateInviteTokenCommand = new AsyncRelayCommand(OnCreateInviteTokenAsync);
    }

    private async Task OnBackToMain()
    {
        await Shell.Current.GoToAsync("//MainTabs");
    }

    private async Task OnCreateInviteTokenAsync()
    {
        try
        {
            var availableRoles = Enum.GetValues<UserAppRole>()
                                     .Where(r => r <= UserAppRole.Admin);

            var roleNames = availableRoles.Select(r => r.ToString()).ToArray();
            var picked = await Shell.Current.DisplayActionSheet(
                "Assign role for the invite token",
                "Cancel",
                null,
                roleNames
            );

            if (string.IsNullOrWhiteSpace(picked) || picked == "Cancel")
                return;

            var selectedRole = Enum.Parse<UserAppRole>(picked);

            var response = await _loginService.CreateInviteToken(
                new InviteTokenDto.Request { AssignedRole = selectedRole }
            );

            if (response is null)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to create invite token.", "OK");
                return;
            }

            var acceptCopy = await Shell.Current.DisplayAlert(
                "Invite token created",
                $"Token: {response.Token}\nRole: {response.AssignedRole}\nExpires: {response.ExpirationDate:g}",
                "Copy",
                "Close"
            );

            if (acceptCopy)
            {
                try
                {
                    await Clipboard.Default.SetTextAsync(response.Token.ToString());
                }
                catch
                {
                    // Ignore clipboard errors silently
                }
            }
        }
        catch
        {
            await Shell.Current.DisplayAlert("Error", "Unexpected error while creating invite token.", "OK");
        }
    }
}
