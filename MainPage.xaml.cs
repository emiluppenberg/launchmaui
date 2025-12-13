using launchmaui.VM;

namespace launchmaui;

public partial class MainPage : ContentPage
{
	public MainPage(MainVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
