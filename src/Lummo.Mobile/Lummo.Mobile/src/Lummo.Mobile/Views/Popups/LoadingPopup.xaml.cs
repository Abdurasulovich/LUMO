using Lummo.Mobile.Views.Pages;
using Mopups.Services;

namespace Lummo.Mobile.Views.Popups;

public partial class LoadingPopup
{
	public LoadingPopup()
	{
		InitializeComponent();
		_ = Count5Sec();
	}

	async Task Count5Sec()
	{
		await Task.Delay(5000);
		await Shell.Current.GoToAsync(nameof(VerificationPage), true);
		await MopupService.Instance.PopAllAsync();
	}
}