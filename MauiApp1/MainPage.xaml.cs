using RGPopup.Services;
using System.Net.Http.Json;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
        
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
        //await CounterBtn.RelRotateTo(90,2000);

        await PopupNavigation.Instance.PushAsync(new NewPage1());
    }   

    protected override void LayoutChildren(double x, double y, double width, double height)
    {
        base.LayoutChildren(x, 200, width, height - 200);
    }
    protected override Size ArrangeOverride(Rect bounds)
    {
        return base.ArrangeOverride(bounds);
    }
}
