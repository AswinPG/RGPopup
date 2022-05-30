using RGPopup.Events;
using RGPopup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGPopup.Contracts;

public interface IPopupNavigation
{
    event EventHandler<PopupNavigationEventArgs> Pushing;

    event EventHandler<PopupNavigationEventArgs> Pushed;

    event EventHandler<PopupNavigationEventArgs> Popping;

    event EventHandler<PopupNavigationEventArgs> Popped;

    IReadOnlyList<PopupPage> PopupStack { get; }

    Task PushAsync(PopupPage page, bool animate = true);

    Task PopAsync(bool animate = true);

    Task PopAllAsync(bool animate = true);

    Task RemovePageAsync(PopupPage page, bool animate = true);
}
