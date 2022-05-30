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
using RGPopup.Platforms.Android;
using Android.Widget;

namespace RGPopup.Pages;

public class PopupPageHandler : PageHandler
{
    //private readonly RgGestureDetectorListener _gestureDetectorListener;
    //private readonly GestureDetector _gestureDetector;

    private DateTime _downTime;
    private Microsoft.Maui.Graphics.Point _downPosition;
    public bool _disposed;

    public PopupPageHandler()
    {

        this.SetMauiContext(new MauiContext(MauiApplication.Current.Services, MauiApplication.Current.ApplicationContext));
        //_gestureDetectorListener = new RgGestureDetectorListener();

        //_gestureDetectorListener.Clicked += OnBackgroundClick;

        //_gestureDetector = new GestureDetector(Context, _gestureDetectorListener);
    }

    //private async void OnBackgroundClick(object sender, MotionEvent e)
    //{

    //    var isInRegion = IsInRegion(e.RawX, e.RawY, this.PlatformView.GetChildAt(0));

    //    if (!isInRegion)
    //    {
    //        await (this.VirtualView as PopupPage).SendBackgroundClick();
    //    }
    //}

    //private bool IsInRegion(float x, float y, AndroidView.View v)
    //{
    //    var mCoordBuffer = new int[2];

    //    v.GetLocationOnScreen(mCoordBuffer);
    //    return mCoordBuffer[0] + v.Width > x &&    // right edge
    //           mCoordBuffer[1] + v.Height > y &&   // bottom edge
    //           mCoordBuffer[0] < x &&              // left edge
    //           mCoordBuffer[1] < y;                // top edge
    //}

    protected override void ConnectHandler(ContentViewGroup platformView)
    {
        (platformView as PopupContentViewGroup).PopupHandler = this;
        base.ConnectHandler(platformView);
    }

    protected override ContentViewGroup CreatePlatformView()
    {
        var item = new PopupContentViewGroup(Context);
        return item;
    }
    //bool DispatchTouchEvent(MotionEvent e)
    //{

    //    if (e.Action == MotionEventActions.Down)
    //    {
    //        _downTime = DateTime.UtcNow;
    //        _downPosition = new Microsoft.Maui.Graphics.Point(e.RawX, e.RawY);
    //    }
    //    //if (e.Action != MotionEventActions.Up)
    //    //    return this.PlatformView.DispatchTouchEvent(e);

    //    if (_disposed)
    //        return false;

    //    AndroidView.View? currentFocus1 = Platform.CurrentActivity?.CurrentFocus;

    //    if (currentFocus1 is EditText)
    //    {
    //        AndroidView.View? currentFocus2 = Platform.CurrentActivity?.CurrentFocus;
    //        if (currentFocus1 == currentFocus2 && _downPosition.Distance(new Microsoft.Maui.Graphics.Point(e.RawX, e.RawY)) <= Context.ToPixels(20.0) && !(DateTime.UtcNow - _downTime > TimeSpan.FromMilliseconds(200.0)))
    //        {
    //            var location = new int[2];
    //            currentFocus1.GetLocationOnScreen(location);
    //            var num1 = e.RawX + currentFocus1.Left - location[0];
    //            var num2 = e.RawY + currentFocus1.Top - location[1];
    //            if (!new Rectangle(currentFocus1.Left, currentFocus1.Top, currentFocus1.Width, currentFocus1.Height).Contains((int)num1, (int)num2))
    //            {
    //                Context.HideKeyboard(currentFocus1);
    //                currentFocus1.ClearFocus();
    //            }
    //        }
    //    }

    //    if (_disposed)
    //        return false;
    //    return true;
    //}

    //private void PlatformView_Touch(object sender, AndroidView.View.TouchEventArgs e)
    //{
    //    try
    //    {
    //        if (_disposed)
    //            return;

    //        var baseValue = base.OnTouchEvent(e);

    //        _gestureDetector.OnTouchEvent(e);

    //        if (CurrentElement != null && CurrentElement.BackgroundInputTransparent)
    //        {
    //            if ((ChildCount > 0 && !IsInRegion(e.RawX, e.RawY, GetChildAt(0)!)) || ChildCount == 0)
    //            {
    //                CurrentElement.SendBackgroundClick();
    //                return false;
    //            }
    //        }

    //        return baseValue;


    //    }
    //    catch (Exception)
    //    {
    //        throw;
    //    }

    //}

    //private bool OnTouchEvent(object? sender, MotionEvent? e)
    //{
    //    if (_disposed)
    //    {
    //        return false;
    //    }

    //    //var baseValue = (sender as AndroidView.View).OnTouchEvent(e);

    //    _gestureDetector.OnTouchEvent(e);

    //    if ((this.VirtualView as PopupPage)?.BackgroundInputTransparent == true)
    //    {
    //        OnBackgroundClick(sender, e);
    //    }

    //    return false;
    //}

    protected override void DisconnectHandler(ContentViewGroup platformView)
    {
        //platformView.Dispose();
        base.DisconnectHandler(platformView);

    }
}

