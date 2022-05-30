using RGPopup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGPopup.Interfaces;

public interface IPopupAnimation
{
    void Preparing(View content, PopupPage page);
    void Disposing(View content, PopupPage page);
    Task Appearing(View content, PopupPage page);
    Task Disappearing(View content, PopupPage page);
}
