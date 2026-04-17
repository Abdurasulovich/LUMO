using Lummo.Mobile.ViewModels;

namespace Lummo.Mobile.Views.Pages;

public partial class ForgotPasswordPage : ContentPage
{
	private ForgotPasswordPageViewModel _vm;
    public ForgotPasswordPage(ForgotPasswordPageViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = _vm = vm;
    }

    private void EmailAddress_TextChanged(object sender, TextChangedEventArgs e)
    {
        _vm.IsEmailAdddressNotInputOrNotCorrect = false;
        _vm.HasGeneralError = false;
    }
}