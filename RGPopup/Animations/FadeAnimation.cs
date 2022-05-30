﻿using System.Threading.Tasks;
using RGPopup.Animations.Base;
using RGPopup.Pages;

namespace RGPopup.Animations
{
    public class FadeAnimation : BaseAnimation
    {
        private double _defaultOpacity;

        public bool HasBackgroundAnimation { get; set; } = true;

        public override void Preparing(View content, PopupPage page)
        {
            if (HasBackgroundAnimation)
            {
                _defaultOpacity = page.Opacity;
                page.Opacity = 0;
            }
            else if (content != null)
            {
                _defaultOpacity = content.Opacity;
                content.Opacity = 0;
            }
        }

        public override void Disposing(View content, PopupPage page)
        {
            if (HasBackgroundAnimation)
            {
                page.Opacity = _defaultOpacity;
            }
            else if (content != null)
            {
                page.Opacity = _defaultOpacity;
            }
        }

        public override Task Appearing(View content, PopupPage page)
        {
            if (HasBackgroundAnimation)
            {
                return page.FadeTo(1, DurationIn, EasingIn);
            }
            if (content != null)
            {
                return content.FadeTo(1, DurationIn, EasingIn);
            }

            return Task.FromResult(0);
        }

        public override Task Disappearing(View content, PopupPage page)
        {
            _defaultOpacity = page.Opacity;
            if (double.IsNaN(_defaultOpacity))
                _defaultOpacity = 1;

            if (HasBackgroundAnimation)
            {
                return page.FadeTo(0, DurationOut, EasingOut);
            }
            if (content != null)
            {
                return content.FadeTo(0, DurationOut, EasingOut);
            }

            return Task.FromResult(0);
        }
    }
}
