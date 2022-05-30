using RGPopup.Contracts;
using RGPopup.Events;
using RGPopup.Exceptions;
using RGPopup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGPopup.Services;

internal class PopupNavigationImpl : IPopupNavigation
{
    private readonly object _locker = new object();

    private readonly List<PopupPage> _popupStack = new List<PopupPage>();

    public event EventHandler<PopupNavigationEventArgs>? Pushing;

    public event EventHandler<PopupNavigationEventArgs>? Pushed;

    public event EventHandler<PopupNavigationEventArgs>? Popping;

    public event EventHandler<PopupNavigationEventArgs>? Popped;


    private static readonly Lazy<IPopupPlatform> lazyImplementation = new(() => GeneratePopupPlatform(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    private static readonly IPopupPlatform PopupPlatform = lazyImplementation.Value;

    private static IPopupPlatform GeneratePopupPlatform()
    {
        return PullPlatformImplementation();


        static IPopupPlatform PullPlatformImplementation()
        {
#if ANDROID
            return new RGPopup.Platforms.Android.PopupPlatform();
#endif

            throw new PlatformNotSupportedException();
        }
    }


//    private static IPopupPlatform PopupPlatform
//    {
//        get
//        {
//            return PullPlatformImplementation();


//            static IPopupPlatform PullPlatformImplementation()
//            {
//#if ANDROID
//            return new RGPopup.Platforms.Android.PopupPlatform();
//#endif
//                throw new PlatformNotSupportedException();
//            }
//            //if (popupPlatform == null)
//            //    throw new RGInitialisationException("You MUST install RGPopup to each project and call RGPopup.Popup.Init(); prior to using it.\nSee more info: " );

//            ////if (!popupPlatform.IsInitialized)
//            ////    throw new RGInitialisationException("You MUST call RGPopup.Popup.Init(); prior to using it.\nSee more info: ");

//            //return popupPlatform;
//        }
//    }

    public IReadOnlyList<PopupPage> PopupStack => _popupStack;

    public PopupNavigationImpl()
    {
        //PopupPlatform.OnInitialized += OnInitialized;
    }

    private async void OnInitialized(object sender, EventArgs e)
    {
        if (PopupStack.Count > 0)
            await PopAllAsync(false);
    }

    public Task PushAsync(PopupPage page, bool animate = true)
    {
        lock (_locker)
        {
            if (_popupStack.Contains(page))
                throw new RGPageInvalidException("The page has been pushed already. Pop or remove the page before to push it again");

            Pushing?.Invoke(this, new PopupNavigationEventArgs(page, animate));

            _popupStack.Add(page);

            var task = InvokeThreadSafe(async () =>
            {
                animate = CanBeAnimated(animate);

                if (animate)
                {
                    page.PreparingAnimation();
                    await AddAsync(page);
                    await page.AppearingAnimation();
                }
                else
                {
                    await AddAsync(page);
                }

                page.AppearingTransactionTask = null;

                Pushed?.Invoke(this, new PopupNavigationEventArgs(page, animate));
            });

            page.AppearingTransactionTask = task;

            return task;
        }
    }

    public Task PopAsync(bool animate = true)
    {
        lock (_locker)
        {
            animate = CanBeAnimated(animate);
            if (PopupStack.Count == 0)
                throw new RGPopupStackInvalidException("No Page in PopupStack");

            return RemovePageAsync(PopupStack.Last(), animate);
        }
    }

    public Task PopAllAsync(bool animate = true)
    {
        lock (_locker)
        {
            animate = CanBeAnimated(animate);

            if (PopupStack.Count == 0)
                throw new RGPopupStackInvalidException("No Page in PopupStack");

            var popupTasks = PopupStack.ToList().Select(page => RemovePageAsync(page, animate));

            return Task.WhenAll(popupTasks);
        }
    }

    public Task RemovePageAsync(PopupPage page, bool animate = true)
    {
        lock (_locker)
        {
            if (page == null)
                throw new RGPageInvalidException("Page cannot be null");

            if (!_popupStack.Contains(page))
                throw new RGPopupStackInvalidException("The page has not been pushed yet or has been removed already");

            if (page.DisappearingTransactionTask != null)
                return page.DisappearingTransactionTask;

            var task = InvokeThreadSafe(async () =>
            {
                if (page.AppearingTransactionTask != null)
                    await page.AppearingTransactionTask;

                lock (_locker)
                {
                    if (!_popupStack.Contains(page))
                        return;
                }

                Popping?.Invoke(this, new PopupNavigationEventArgs(page, animate));

                animate = CanBeAnimated(animate);

                if (animate)
                    await page.DisappearingAnimation();

                await RemoveAsync(page);

                if (animate)
                    page.DisposingAnimation();

                lock (_locker)
                {
                    _popupStack.Remove(page);
                    page.DisappearingTransactionTask = null;

                    Popped?.Invoke(this, new PopupNavigationEventArgs(page, animate));
                }
            });

            page.DisappearingTransactionTask = task;

            return task;
        }
    }

    // Private

    private static Task AddAsync(PopupPage page)
    {
        return PopupPlatform.AddAsync(page);
    }

    private static Task RemoveAsync(PopupPage page)
    {
        return PopupPlatform.RemoveAsync(page);
    }

    // Internal 

    internal void RemovePopupFromStack(PopupPage page)
    {
        if (_popupStack.Contains(page))
            _popupStack.Remove(page);
    }

#region Animation

    private static bool CanBeAnimated(bool animate)
    {
        return animate;
        //return animate && PopupPlatform.IsSystemAnimationEnabled;
    }

#endregion

#region Helpers

    private static Task InvokeThreadSafe(Func<Task> action)
    {
        var tcs = new TaskCompletionSource<bool>();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                await action.Invoke();
                tcs.SetResult(true);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        });

        return tcs.Task;
    }

#endregion
}

