using Lummo.Mobile.ViewModels;

namespace Lummo.Mobile.Views.Pages;

public partial class ResetPasswordPage : ContentPage
{
	private ResetPasswordPageViewModel _vm;
    public ResetPasswordPage(ResetPasswordPageViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = _vm = vm;
	}
}