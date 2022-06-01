﻿using CoreGraphics;
using RGPopup.Pages;
using RGPopup.Platforms.iOS.Handler;
using UIKit;

namespace RGPopup.Platforms.iOS
{
    internal class PopupWindow : UIWindow
    {
        public PopupWindow(IntPtr handle) : base(handle)
        {
            // Fix #307
        }

        public PopupWindow()
        {

        }

        public PopupWindow(UIWindowScene uiWindowScene) : base(uiWindowScene)
        {

        }

        public override UIView HitTest(CGPoint point, UIEvent? uievent)
        {
            var platformHandler = (PopupPlatformHandler?)RootViewController;
            var renderer = platformHandler?.Handler;
            var hitTestResult = base.HitTest(point, uievent);

            if (!(platformHandler?.Handler?.VirtualView is PopupPage formsElement))
                return hitTestResult;

            if (formsElement.InputTransparent)
                return null!;

            if (formsElement.BackgroundInputTransparent && renderer?.PlatformView == hitTestResult)
            {
                formsElement.SendBackgroundClick();
                return null!;
            }

            return hitTestResult;
        }
    }

}