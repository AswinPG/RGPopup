using RGPopup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGPopup.Events;

public class PopupNavigationEventArgs : EventArgs
{
    public PopupPage Page { get; }

    public bool IsAnimated { get; }

    public PopupNavigationEventArgs(PopupPage page, bool isAnimated)
    {
        Page = page;
        IsAnimated = isAnimated;
    }
}