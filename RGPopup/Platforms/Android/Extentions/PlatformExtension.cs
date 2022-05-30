using RGPopup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XFPlatform = Microsoft.Maui.MauiApplication;

namespace RGPopup.Platforms.Android.Extentions;

internal static class PlatformExtension
{
    public static IViewHandler GetOrCreateHandler(this VisualElement bindable)
    {
        try
        {
            if(bindable.Handler == null)
            {
                bindable.Handler = new PopupPageHandler();
            }
            return bindable.Handler;

            //return bindable.Handler ??= new PopupPageHandler();
        }
        catch (Exception g)
        {
            throw;
        }
    }
}
