using Android.Content;
using Android.Views;
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
using Android.Widget;
using System.Drawing;
using Android.OS;

namespace RGPopup.Platforms.Android
{
    public class PopupContentViewGroup : ContentViewGroup
    {
        public PopupPageHandler PopupHandler;

        private readonly RgGestureDetectorListener _gestureDetectorListener;
        private readonly GestureDetector _gestureDetector;
        private DateTime _downTime;
        private Microsoft.Maui.Graphics.Point _downPosition;
        private bool _disposed;
        public PopupContentViewGroup(Context context) : base(context)
        {
            _gestureDetectorListener = new RgGestureDetectorListener();
            _gestureDetectorListener.Clicked += OnBackgroundClick;

            _gestureDetector = new GestureDetector(Context, _gestureDetectorListener);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposed = true;

                _gestureDetectorListener.Clicked -= OnBackgroundClick;
                _gestureDetectorListener.Dispose();
                _gestureDetector.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            try
            {
                var activity = Platform.CurrentActivity;
                var decoreView = activity?.Window?.DecorView;

                Thickness systemPadding;
                var keyboardOffset = 0d;


                var visibleRect = new AndroidGraphics.Rect();

                decoreView?.GetWindowVisibleDisplayFrame(visibleRect);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.M && RootWindowInsets != null)
                {
                    var h = bottom - top;

                    var windowInsets = RootWindowInsets;
                    var bottomPadding = Math.Min(windowInsets.StableInsetBottom, windowInsets.SystemWindowInsetBottom);

                    if (h - visibleRect.Bottom > windowInsets.StableInsetBottom)
                    {
                        keyboardOffset = Context.FromPixels(h - visibleRect.Bottom);
                    }

                    systemPadding = new Thickness
                    {
                        Left = Context.FromPixels(windowInsets.SystemWindowInsetLeft),
                        Top = Context.FromPixels(windowInsets.SystemWindowInsetTop),
                        Right = Context.FromPixels(windowInsets.SystemWindowInsetRight),
                        Bottom = Context.FromPixels(bottomPadding)
                    };
                }
                else if (Build.VERSION.SdkInt < BuildVersionCodes.M && decoreView != null)
                {
                    var screenSize = new AndroidGraphics.Point();
                    activity?.WindowManager?.DefaultDisplay?.GetSize(screenSize);

                    var keyboardHeight = 0d;

                    var decoreHeight = decoreView.Height;
                    var decoreWidht = decoreView.Width;

                    if (visibleRect.Bottom < screenSize.Y)
                    {
                        keyboardHeight = screenSize.Y - visibleRect.Bottom;
                        keyboardOffset = Context.FromPixels(decoreHeight - visibleRect.Bottom);
                    }

                    systemPadding = new Thickness
                    {
                        Left = Context.FromPixels(visibleRect.Left),
                        Top = Context.FromPixels(visibleRect.Top),
                        Right = Context.FromPixels(decoreWidht - visibleRect.Right),
                        Bottom = Context.FromPixels(decoreHeight - visibleRect.Bottom - keyboardHeight)
                    };
                }
                else
                {
                    systemPadding = new Thickness();
                }

            (PopupHandler.VirtualView as PopupPage).SetValue(PopupPage.SystemPaddingProperty, systemPadding);
                (PopupHandler.VirtualView as PopupPage).SetValue(PopupPage.KeyboardOffsetProperty, keyboardOffset);

                if (changed)
                    (PopupHandler.VirtualView as PopupPage).Layout(new Rect(Context.FromPixels(left), Context.FromPixels(top), Context.FromPixels(right), Context.FromPixels(bottom)));
                else
                    (PopupHandler.VirtualView as PopupPage).ForceLayout();

                base.OnLayout(changed, left, top, right, bottom);
            }
            catch (Exception)
            {
                throw;
            }

        }


        protected override void OnAttachedToWindow()
        {
            var activity = Platform.CurrentActivity;
            var decoreView = activity?.Window?.DecorView;
            Context.HideKeyboard(decoreView);
            base.OnAttachedToWindow();
        }

        protected override void OnDetachedFromWindow()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(0), () =>
            {
                var activity = Platform.CurrentActivity;
                var decoreView = activity?.Window?.DecorView;
                Context.HideKeyboard(decoreView);
                return false;
            });
            base.OnDetachedFromWindow();
        }

        protected override void OnWindowVisibilityChanged(ViewStates visibility)
        {
            base.OnWindowVisibilityChanged(visibility);

            // It is needed because a size of popup has not updated on Android 7+. See #209
            if (visibility == ViewStates.Visible)
                RequestLayout();
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down)
            {
                _downTime = DateTime.UtcNow;
                _downPosition = new Microsoft.Maui.Graphics.Point(e.RawX, e.RawY);
            }
            if (e.Action != MotionEventActions.Up)
                return base.DispatchTouchEvent(e);

            if (_disposed)
                return false;

            AndroidView.View? currentFocus1 = Platform.CurrentActivity?.CurrentFocus;

            if (currentFocus1 is EditText)
            {
                AndroidView.View? currentFocus2 = Platform.CurrentActivity?.CurrentFocus;
                if (currentFocus1 == currentFocus2 && _downPosition.Distance(new Microsoft.Maui.Graphics.Point(e.RawX, e.RawY)) <= Context.ToPixels(20.0) && !(DateTime.UtcNow - _downTime > TimeSpan.FromMilliseconds(200.0)))
                {
                    var location = new int[2];
                    currentFocus1.GetLocationOnScreen(location);
                    var num1 = e.RawX + currentFocus1.Left - location[0];
                    var num2 = e.RawY + currentFocus1.Top - location[1];
                    if (!new Rectangle(currentFocus1.Left, currentFocus1.Top, currentFocus1.Width, currentFocus1.Height).Contains((int)num1, (int)num2))
                    {
                        Context.HideKeyboard(currentFocus1);
                        currentFocus1.ClearFocus();
                    }
                }
            }

            if (_disposed)
                return false;

            var flag = base.DispatchTouchEvent(e);

            return flag;
        }


        public override bool OnTouchEvent(MotionEvent e)
        {
            try
            {
                if (_disposed)
                    return false;

                var baseValue = base.OnTouchEvent(e);

                _gestureDetector.OnTouchEvent(e);

                if ((PopupHandler?.VirtualView as PopupPage).BackgroundInputTransparent)
                {
                    if ((ChildCount > 0 && !IsInRegion(e.RawX, e.RawY, PopupHandler?.PlatformView.GetChildAt(0)!)) || ChildCount == 0)
                    {
                        (PopupHandler?.VirtualView as PopupPage).SendBackgroundClick();
                        return false;
                    }
                }
                return baseValue;
            }
            catch(Exception f)
            {

            }
            return base.OnTouchEvent(e);
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
        private async void OnBackgroundClick(object sender, MotionEvent e)
        {
            if (ChildCount == 0)
                return;

            var isInRegion = IsInRegion(e.RawX, e.RawY, PopupHandler.PlatformView.GetChildAt(0));

            if (!isInRegion)
            {
                await (PopupHandler.VirtualView as PopupPage).SendBackgroundClick();
            }
        }

    }
}
