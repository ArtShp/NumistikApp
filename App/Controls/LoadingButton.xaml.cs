using System.Windows.Input;

namespace App.Controls;

public partial class LoadingButton : ContentView
{
    public LoadingButton()
    {
        InitializeComponent();
        UpdateVisualState();
    }

    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(LoadingButton), false, propertyChanged: OnAnyPropertyChanged);

    public bool IsLoading
    {
        get => (bool) GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public static readonly BindableProperty IdleTextProperty =
        BindableProperty.Create(nameof(IdleText), typeof(string), typeof(LoadingButton), "Load more", propertyChanged: OnAnyPropertyChanged);

    public string IdleText
    {
        get => (string) GetValue(IdleTextProperty);
        set => SetValue(IdleTextProperty, value);
    }

    public static readonly BindableProperty LoadingTextProperty =
        BindableProperty.Create(nameof(LoadingText), typeof(string), typeof(LoadingButton), "Loading...", propertyChanged: OnAnyPropertyChanged);

    public string LoadingText
    {
        get => (string) GetValue(LoadingTextProperty);
        set => SetValue(LoadingTextProperty, value);
    }

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(LoadingButton));

    public ICommand? Command
    {
        get => (ICommand?) GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly BindableProperty DisableWhenLoadingProperty =
        BindableProperty.Create(nameof(DisableWhenLoading), typeof(bool), typeof(LoadingButton), true, propertyChanged: OnAnyPropertyChanged);

    public bool DisableWhenLoading
    {
        get => (bool) GetValue(DisableWhenLoadingProperty);
        set => SetValue(DisableWhenLoadingProperty, value);
    }

    protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(IsEnabled))
        {
            UpdateVisualState();
        }
    }

    private static void OnAnyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((LoadingButton) bindable).UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        PART_Button.Text = IsLoading ? LoadingText : IdleText;
        PART_Button.IsEnabled = IsEnabled && (!IsLoading || !DisableWhenLoading);
    }
}
