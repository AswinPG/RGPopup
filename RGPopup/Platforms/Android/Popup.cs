using Android.Content;
using RGPopup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGPopup.Platforms.Android;

//public static class Popup
//{
//    internal static event EventHandler? OnInitialized;

//    internal static bool IsInitialized { get; private set; }

//    internal static Context? Context { get; private set; }

//    public static void Init(Context context)
//    {
//        //LinkAssemblies();

//        Context = context;

//        IsInitialized = true;
//        OnInitialized?.Invoke(null, EventArgs.Empty);
//    }

//    public static bool SendBackPressed(Action? backPressedHandler = null)
//    {
//        var popupNavigationInstance = PopupNavigation.Instance;

//        if (popupNavigationInstance.PopupStack.Count > 0)
//        {
//            var lastPage = popupNavigationInstance.PopupStack.Last();

//            var isPreventClose = lastPage.DisappearingTransactionTask != null || lastPage.SendBackButtonPressed();

//            if (!isPreventClose)
//            {
//                MainThread.BeginInvokeOnMainThread(async () =>
//                {
//                    await popupNavigationInstance.RemovePageAsync(lastPage);
//                });
//            }

//            return true;
//        }

//        backPressedHandler?.Invoke();

//        return false;
//    }

//    //private static void LinkAssemblies()
//    //{
//    //    if (false.Equals(true))
//    //    {
//    //        var i = new PopupPlatformDroid();
//    //        var r = new PopupPageRenderer(null!);
//    //    }
//    //}
//}

