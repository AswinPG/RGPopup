using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGPopup.Platforms.Android.Gestures
{
    internal class RgGestureDetectorListener : GestureDetector.SimpleOnGestureListener
    {
        public event EventHandler<MotionEvent>? Clicked;

        public override bool OnSingleTapUp(MotionEvent? e)
        {
            if (e != null) Clicked?.Invoke(this, e);

            return false;
        }
    }
}
