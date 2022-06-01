using CoreGraphics;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using RGPopup.Contracts;
using RGPopup.Exceptions;
using RGPopup.Pages;
using RGPopup.Platforms.iOS.Extentions;
using RGPopup.Platforms.iOS.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace RGPopup.Platforms.iOS;

internal class PopupPlatformIos : IPopupPlatform
{
    // It's necessary because GC in Xamarin.iOS 13 removes all UIWindow if there are not any references to them. See #459
    private readonly List<UIWindow> _windows = new List<UIWindow>();

    private static bool IsiOS9OrNewer => UIDevice.CurrentDevice.CheckSystemVersion(9, 0);

    private static bool IsiOS13OrNewer => UIDevice.CurrentDevice.CheckSystemVersion(13, 0);

    //public event EventHandler OnInitialized
    //{
    //    add => Popup.OnInitialized += value;
    //    remove => Popup.OnInitialized -= value;
    //}

    //public bool IsInitialized => Popup.IsInitialized;

    public bool IsSystemAnimationEnabled => true;

    public Task AddAsync(PopupPage page)
    {
        page.Parent = Application.Current.MainPage;

        page.DescendantRemoved += HandleChildRemoved;

        if (UIApplication.SharedApplication.KeyWindow.WindowLevel == UIWindowLevel.Normal)
            UIApplication.SharedApplication.KeyWindow.WindowLevel = -1;

        var handler = page.GetOrCreateHandler();

        PopupWindow window;
        if (IsiOS13OrNewer)
        {
            var connectedScene = UIApplication.SharedApplication.ConnectedScenes.ToArray().FirstOrDefault(x => x.ActivationState == UISceneActivationState.ForegroundActive);
            if (connectedScene != null && connectedScene is UIWindowScene windowScene)
                window = new PopupWindow(windowScene);
            else
                window = new PopupWindow();

            _windows.Add(window);
        }
        else
            window = new PopupWindow();

        window.BackgroundColor = Colors.Transparent.ToUIColor();
        window.RootViewController = new PopupPlatformHandler(handler);
        if (window.RootViewController.View != null)
            window.RootViewController.View.BackgroundColor = Colors.Transparent.ToUIColor();
        window.WindowLevel = UIWindowLevel.Normal;
        window.MakeKeyAndVisible();

        if (!IsiOS9OrNewer)
            window.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);

        return window.RootViewController.PresentViewControllerAsync(handler.ViewController, false);
    }

    public async Task RemoveAsync(PopupPage page)
    {
        if (page == null)
            throw new RGPageInvalidException("Popup page is null");

        var handler = page.Handler as PopupPageHandler;
        var viewController = handler?.ViewController;

        await Task.Delay(50);

        page.DescendantRemoved -= HandleChildRemoved;

        if (handler != null && viewController != null && !viewController.IsBeingDismissed)
        {
            var window = viewController.View?.Window;
            page.Parent = null;
            if (window != null)
            {
                var rvc = window.RootViewController;
                if (rvc != null)
                {
                    await rvc.DismissViewControllerAsync(false);
                    DisposeModelAndChildrenHandlers(page);
                    rvc.Dispose();
                }
                window.RootViewController = null;
                window.Hidden = true;
                if (IsiOS13OrNewer && _windows.Contains(window))
                    _windows.Remove(window);
                window.Dispose();
                window = null;
            }

            if (_windows.Count > 0)
                _windows.Last().WindowLevel = UIWindowLevel.Normal;
            else if (UIApplication.SharedApplication.KeyWindow.WindowLevel == -1)
                UIApplication.SharedApplication.KeyWindow.WindowLevel = UIWindowLevel.Normal;
        }
    }

    private static void DisposeModelAndChildrenHandlers(VisualElement view)
    {
        //    IVisualElementHandler handler;
        //    foreach (VisualElement child in view.Descendants())
        //    {
        //        handler = XFPlatform.GetHandler(child);
        //        XFPlatform.SetHandler(child, null);

        //        if (handler != null)
        //        {
        //            handler.NativeView.RemoveFromSuperview();
        //            handler.Dispose();
        //        }
        //    }

        //    handler = XFPlatform.GetHandler(view);
        //    if (handler != null)
        //    {
        //        handler.NativeView.RemoveFromSuperview();
        //        handler.Dispose();
        //    }
        //    XFPlatform.SetHandler(view, null);
    }

    private void HandleChildRemoved(object sender, ElementEventArgs e)
    {
        var view = e.Element;
        DisposeModelAndChildrenHandlers((VisualElement)view);
    }
}
