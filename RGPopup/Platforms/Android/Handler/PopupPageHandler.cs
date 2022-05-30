using Android.Views;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using RGPopup.Pages;
using RGPopup.Platforms.Android.Gestures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AndroidView = Android.Views;
using AndroidGraphics = Android.Graphics;
using AndroidWidget = Android.Widget;
using Android.OS;
using System.Drawing;

namespace RGPopup.Pages;

public class PopupPageHandler : PageHandler
{
    private readonly RgGestureDetectorListener _gestureDetectorListener;
    private readonly GestureDetector _gestureDetector;

    private DateTime _downTime;
    private Microsoft.Maui.Graphics.Point _downPosition;
    private bool _disposed;

    public PopupPageHandler()
    {

        this.SetMauiContext(new MauiContext(MauiApplication.Current.Services, MauiApplication.Current.ApplicationContext));
        //_gestureDetectorListener = new RgGestureDetectorListener();

        //_gestureDetectorListener.Clicked += OnBackgroundClick;

        //_gestureDetector = new GestureDetector(Context, _gestureDetectorListener);
    }

    private async void OnBackgroundClick(object sender, MotionEvent e)
    {

        var isInRegion = IsInRegion(e.RawX, e.RawY, this.PlatformView.GetChildAt(0));

        if (!isInRegion)
        {
            await (this.VirtualView as PopupPage).SendBackgroundClick();
        }
    }

    private bool IsInRegion(float x, float y, AndroidView.View v)
    {
        var mCoordBuffer = new int[2];

        v.GetLocationOnScreen(mCoordBuffer);
        return mCoordBuffer[0] + v.Width > x &&    // right edge
               mCoordBuffer[1] + v.Height > y &&   // bottom edge
               mCoordBuffer[0] < x &&              // left edge
               mCoordBuffer[1] < y;                // top edge
    }

    protected override void ConnectHandler(ContentViewGroup platformView)
    {
        this.PlatformView.LayoutChange += PlatformView_LayoutChange;
        this.PlatformView.Touch += PlatformView_Touch;
        base.ConnectHandler(platformView);
    }

    private void PlatformView_Touch(object sender, AndroidView.View.TouchEventArgs e)
    {
        try
        {
            DispatchTouchEvent(e.Event);

            void DispatchTouchEvent(MotionEvent e)
            {

                if (e.Action == MotionEventActions.Down)
                {
                    _downTime = DateTime.UtcNow;
                    _downPosition = new Microsoft.Maui.Graphics.Point(e.RawX, e.RawY);
                    OnTouchEvent(sender, e);
                    OnBackgroundClick(sender, e);
                }
                if (e.Action != MotionEventActions.Up)
                {
                    return;
                }

                if (_disposed)
                    return;

                AndroidView.View? currentFocus1 = Platform.CurrentActivity.CurrentFocus;

                if (currentFocus1 is AndroidWidget.EditText)
                {
                    AndroidView.View? currentFocus2 = Platform.CurrentActivity.CurrentFocus;
                    if (currentFocus1 == currentFocus2 && _downPosition.Distance(new(e.RawX, e.RawY)) <= Context.ToPixels(20.0) && !(DateTime.UtcNow - _downTime > TimeSpan.FromMilliseconds(200.0)))
                    {
                        int[] location = new int[2];
                        currentFocus1.GetLocationOnScreen(location);
                        float num1 = e.RawX + currentFocus1.Left - location[0];
                        float num2 = e.RawY + currentFocus1.Top - location[1];
                        if (!new Rectangle(currentFocus1.Left, currentFocus1.Top, currentFocus1.Width, currentFocus1.Height).Contains((int)num1, (int)num2))
                        {
                            Context.HideKeyboard(currentFocus1);
                            currentFocus1.ClearFocus();
                        }
                    }
                }
            }

            
        }
        catch (Exception)
        {
            throw;
        }

    }

    private bool OnTouchEvent(object? sender, MotionEvent? e)
    {
        if (_disposed)
        {
            return false;
        }

        //var baseValue = (sender as AndroidView.View).OnTouchEvent(e);

        //_gestureDetector.OnTouchEvent(e);

        if ((this.VirtualView as PopupPage)?.BackgroundInputTransparent == true)
        {
            OnBackgroundClick(sender, e);
        }

        return false;
    }




    private void PlatformView_LayoutChange(object sender, AndroidView.View.LayoutChangeEventArgs e)
    {
        try
        {
            var activity = Platform.CurrentActivity;

            Thickness systemPadding;
            var keyboardOffset = 0d;

            var decoreView = activity.Window.DecorView;
            var decoreHeight = decoreView.Height;
            var decoreWidth = decoreView.Width;

            using var visibleRect = new AndroidGraphics.Rect();

            decoreView?.GetWindowVisibleDisplayFrame(visibleRect);

            using var screenSize = new AndroidGraphics.Point();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {

                var windowInsets = activity?.WindowManager?.DefaultDisplay?.Cutout;


                var bottomPadding = windowInsets?.SafeInsetBottom;

                if (screenSize.Y - visibleRect.Bottom > bottomPadding)
                {
                    keyboardOffset = Context.FromPixels(screenSize.Y - visibleRect.Bottom);
                }

                systemPadding = new Microsoft.Maui.Thickness
                {
                    Left = Context.FromPixels(windowInsets.SafeInsetLeft),
                    Top = Context.FromPixels(windowInsets.SafeInsetTop),
                    Right = Context.FromPixels(windowInsets.SafeInsetRight),
                    Bottom = Context.FromPixels(bottomPadding.Value)
                };
            }
            else
            {
                var keyboardHeight = 0d;

                if (visibleRect.Bottom < screenSize.Y)
                {
                    keyboardHeight = screenSize.Y - visibleRect.Bottom;
                    keyboardOffset = Context.FromPixels(decoreHeight - visibleRect.Bottom);
                }

                systemPadding = new Microsoft.Maui.Thickness
                {
                    Left = Context.FromPixels(visibleRect.Left),
                    Top = Context.FromPixels(visibleRect.Top),
                    Right = Context.FromPixels(decoreWidth - visibleRect.Right),
                    Bottom = Context.FromPixels(decoreHeight - visibleRect.Bottom - keyboardHeight)
                };
            }

            var page = this.VirtualView as Page;
            page.SetValue(PopupPage.SystemPaddingProperty, systemPadding);
            page.SetValue(PopupPage.KeyboardOffsetProperty, keyboardOffset);
            page.Layout(new Rect(Context.FromPixels(e.Left), Context.FromPixels(e.Top), Context.FromPixels(e.Right), Context.FromPixels(e.Bottom)));
            //this.PlatformView.Layout((int)Context.FromPixels(e.Left), (int)Context.FromPixels(e.Top), (int)Context.FromPixels(e.Right), (int)Context.FromPixels(e.Bottom));
            page.ForceLayout();
            //this.PlatformView.ForceLayout();

            //base.OnLayout(changed, l, t, r, b);
        }
        catch (Exception)
        {
            throw;
        }

    }

    protected override void DisconnectHandler(ContentViewGroup platformView)
    {
        _gestureDetectorListener.Clicked -= OnBackgroundClick;
        _gestureDetectorListener.Dispose();
        _gestureDetector.Dispose();
        _disposed = true;
        base.DisconnectHandler(platformView);
    }
}

