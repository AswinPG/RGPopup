using RGPopup.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGPopup.Services
{
    public static class PopupNavigation
    {
        private static IPopupNavigation? _popupNavigation;
        private static IPopupNavigation? _customNavigation;

        public static IPopupNavigation Instance
        {
            get
            {
                if (_customNavigation != null)
                    return _customNavigation;

                if (_popupNavigation == null)
                    _popupNavigation = new PopupNavigationImpl();

                return _popupNavigation;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetInstance(IPopupNavigation instance)
        {
            _customNavigation = instance;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void RestoreDefaultInstance()
        {
            _customNavigation = null;
        }
    }

}
