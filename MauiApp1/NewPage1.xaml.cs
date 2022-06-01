using RGPopup.Pages;

namespace MauiApp1;

public partial class NewPage1 : PopupPage
{
	public NewPage1()
	{
		InitializeComponent();
	}

    private void PopupPage_BackgroundClicked(object sender, EventArgs e)
    {

    }

    private async void blahButton_Clicked(object sender, EventArgs e)
    {
        await blahButton.RelRotateTo(180,1000);
    }
}