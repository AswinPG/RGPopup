using Microsoft.Maui.LifecycleEvents;
using RGPopup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGPopup.Hosting;

public static class AppHostBuilderExtensions
{
    /// <summary>
    /// Configures the implemented handlers in Syncfusion.Maui.Core.
    /// </summary>
    public static MauiAppBuilder ConfigurePopups(this MauiAppBuilder builder)
    {
        builder
            .ConfigureLifecycleEvents(lifecycle =>
            {
#if ANDROID
                lifecycle.AddAndroid(d =>
                {
                    d.OnBackPressed(activity => RGPopup.Platforms.Android.PopupPlatform.SendBackPressed(activity.OnBackPressed));
                });
#endif
            })
            .ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                handlers.AddHandler(typeof(PopupPage), typeof(PopupPageHandler));
#endif
            });
        return builder;
    }
}