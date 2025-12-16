using launchmaui.VM;

namespace launchmaui;

public partial class DetailsPage : ContentPage
{
  public DetailsPage(DetailsVM vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }
}
