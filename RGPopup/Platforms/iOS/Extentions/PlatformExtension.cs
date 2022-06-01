﻿using RGPopup.Pages;
using RGPopup.Platforms.iOS.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace RGPopup.Platforms.iOS.Extentions;

internal static class PlatformExtension
{
    public static PopupPageHandler GetOrCreateHandler(this VisualElement bindable)
    {
        try
        {
            if (bindable.Handler == null)
            {
                bindable.Handler = new PopupPageHandler();
            }
            return (PopupPageHandler)bindable.Handler;

            //return bindable.Handler ??= new PopupPageHandler();
        }
        catch (Exception g)
        {
            throw;
        }
    }

    //public static void DisposeModelAndChildrenHandlers(this VisualElement view)
    //{
    //    IVisualElementHandler handler;
    //    foreach (var child in view.GetVisualTreeDescendants().OfType<VisualElement>())
    //    {
    //        handler = view.Handler;
    //        XFPlatform.SetHandler(child, null);

    //        if (handler == null)
    //            continue;

    //        handler.NativeView.RemoveFromSuperview();
    //        handler.Dispose();
    //    }

    //    handler = XFPlatform.GetHandler(view);
    //    if (handler != null)
    //    {
    //        handler.NativeView.RemoveFromSuperview();
    //        handler.Dispose();
    //    }
    //    XFPlatform.SetHandler(view, null);
    //}

    public static void UpdateSize(this PopupPlatformHandler handler)
    {
        var currentElement = handler.CurrentElement;

        if (handler.PlatformView?.Superview?.Frame == null || currentElement == null)
            return;

        var superviewFrame = handler.PlatformView.Superview.Frame;
        var applicationFrame = UIScreen.MainScreen.ApplicationFrame;

        var systemPadding = new Thickness
        {
            Left = applicationFrame.Left,
            Top = applicationFrame.Top,
            Right = applicationFrame.Right - applicationFrame.Width - applicationFrame.Left,
            Bottom = applicationFrame.Bottom - applicationFrame.Height - applicationFrame.Top + handler.KeyboardBounds.Height
        };

        if ((handler.VirtualView.Width != superviewFrame.Width && handler.VirtualView.Height != superviewFrame.Height)
            || currentElement.SystemPadding.Bottom != systemPadding.Bottom)
        {
            currentElement.BatchBegin();
            currentElement.SystemPadding = systemPadding;
            currentElement.Layout(new Rect(currentElement.X, currentElement.Y, superviewFrame.Width, superviewFrame.Height));
            currentElement.BatchCommit();
        }
    }
}
