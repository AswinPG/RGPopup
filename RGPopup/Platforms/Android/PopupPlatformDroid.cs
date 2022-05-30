using Android.App;
using Android.Widget;
using AsyncAwaitBestPractices;
using RGPopup.Contracts;
using RGPopup.Exceptions;
using RGPopup.Pages;
using RGPopup.Platforms.Android;
using RGPopup.Platforms.Android.Extentions;
using RGPopup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using AndroidViews = Android.Views;
using System.Text;
using System.Threading.Tasks;

using XApplication = Microsoft.Maui.MauiApplication;

namespace RGPopup.Platforms.Android
{
    public class PopupPlatform : IPopupPlatform
    {
        private static FrameLayout? DecoreView => Platform.CurrentActivity.Window.DecorView as FrameLayout;

        //public event EventHandler OnInitialized
        //{
        //    add => Popup.OnInitialized += value;
        //    remove => Popup.OnInitialized -= value;
        //}

        //public bool IsInitialized => Popup.IsInitialized;


        public static bool SendBackPressed(Action? backPressedHandler = null)
        {
            var popupNavigationInstance = PopupNavigation.Instance;

            if (popupNavigationInstance.PopupStack.Count > 0)
            {
                var lastPage = popupNavigationInstance.PopupStack[popupNavigationInstance.PopupStack.Count - 1];

                var isPreventClose = lastPage.SendBackButtonPressed();

                if (!isPreventClose)
                {
                    popupNavigationInstance.PopAsync().SafeFireAndForget();
                }

                return true;
            }

            backPressedHandler?.Invoke();

            return false;
        }

        //public bool IsSystemAnimationEnabled => GetIsSystemAnimationEnabled();

        public Task AddAsync(PopupPage page)
        {
            try
            {
                var decoreView = DecoreView;

                //HandleAccessibilityWorkaround(page);

                page.Parent = XApplication.Current.Application.Windows[0].Content as Element;
                var handler = page.GetOrCreateHandler();

                decoreView?.AddView(handler.PlatformView as AndroidViews.View);
                return PostAsync(handler.PlatformView as AndroidViews.View);
            }
            catch (Exception d)
            {

            }
            return null;

            //static void HandleAccessibilityWorkaround(PopupPage page)
            //{
            //    if (page.AndroidTalkbackAccessibilityWorkaround)
            //    {
            //        var navCount = XApplication.Current.MainPage.Navigation.NavigationStack.Count;
            //        var modalCount = XApplication.Current.MainPage.Navigation.ModalStack.Count;
            //        XApplication.Current.MainPage.GetOrCreateRenderer().View.ImportantForAccessibility = ImportantForAccessibility.NoHideDescendants;

            //        if (navCount > 0)
            //        {
            //            XApplication.Current.MainPage.Navigation.NavigationStack[navCount - 1].GetOrCreateRenderer().View.ImportantForAccessibility = ImportantForAccessibility.NoHideDescendants;
            //        }
            //        if (modalCount > 0)
            //        {
            //            XApplication.Current.MainPage.Navigation.ModalStack[modalCount - 1].GetOrCreateRenderer().View.ImportantForAccessibility = ImportantForAccessibility.NoHideDescendants;
            //        }

            //        DisableFocusableInTouchMode(XApplication.Current.MainPage.GetOrCreateRenderer().View.Parent);
            //    }
            //}

            //static void DisableFocusableInTouchMode(IViewParent? parent)
            //{
            //    var view = parent;
            //    string className = $"{view?.GetType().Name}";

            //    while (!className.Contains("PlatformRenderer") && view != null)
            //    {
            //        view = view.Parent;
            //        className = $"{view?.GetType().Name}";
            //    }

            //    if (view is AndroidViews.View androidView)
            //    {
            //        androidView.Focusable = false;
            //        androidView.FocusableInTouchMode = false;
            //    }
            //}
        }

        public Task RemoveAsync(PopupPage page)
        {
            if (page == null)
                throw new RGPageInvalidException("Popup page is null");

            var handler = page.GetOrCreateHandler();
            if (handler != null)
            {
                //HandleAccessibilityWorkaround(page);

                //page.Parent = XApplication.Current.MainPage;
                //var element = handler.VirtualView;

                DecoreView?.RemoveView(handler.PlatformView as AndroidViews.View);
                handler.DisconnectHandler();

                page.Parent = null;
                if (DecoreView != null)
                    return PostAsync(DecoreView);
            }

            return Task.FromResult(true);

            //static void HandleAccessibilityWorkaround(PopupPage page)
            //{
            //    if (page.AndroidTalkbackAccessibilityWorkaround)
            //    {
            //        var navCount = XApplication.Current.MainPage.Navigation.NavigationStack.Count;
            //        var modalCount = XApplication.Current.MainPage.Navigation.ModalStack.Count;
            //        XApplication.Current.MainPage.GetOrCreateRenderer().View.ImportantForAccessibility = ImportantForAccessibility.Auto;

            //        if (navCount > 0)
            //        {
            //            XApplication.Current.MainPage.Navigation.NavigationStack[navCount - 1].GetOrCreateRenderer().View.ImportantForAccessibility = ImportantForAccessibility.Auto;
            //        }
            //        if (modalCount > 0)
            //        {
            //            XApplication.Current.MainPage.Navigation.ModalStack[modalCount - 1].GetOrCreateRenderer().View.ImportantForAccessibility = ImportantForAccessibility.Auto;
            //        }
            //    }
            //}
        }
        #region Helpers

        private static Task PostAsync(AndroidViews.View nativeView)
        {
            if (nativeView == null)
                return Task.FromResult(true);

            var tcs = new TaskCompletionSource<bool>();

            nativeView.Post(() => tcs.SetResult(true));

            return tcs.Task;
        }
        #endregion
    }

}
