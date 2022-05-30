﻿using RGPopup.Animations;
using RGPopup.Enums;
using RGPopup.Interfaces;
using RGPopup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RGPopup.Pages;

public class PopupPage : ContentPage
{
    //#region Private

    //private const string IsAnimatingObsoleteText =
    //    nameof(IsAnimating) +
    //    " is obsolute as of v1.1.5. Please use "
    //    + nameof(IsAnimationEnabled) +
    //    " instead. See more info: "
    //    + Config.MigrationV1_0_xToV1_1_xUrl;

    //#endregion

    #region Internal Properties

    internal Task? AppearingTransactionTask { get; set; }

    internal Task? DisappearingTransactionTask { get; set; }

    #endregion

    #region Events

    public event EventHandler? BackgroundClicked;

    #endregion

    #region Bindable Properties

    //[Obsolete(IsAnimatingObsoleteText)]
    //public static readonly BindableProperty IsAnimatingProperty = BindableProperty.Create(nameof(IsAnimating), typeof(bool), typeof(PopupPage), true);

    //[Obsolete(IsAnimatingObsoleteText)]
    //public bool IsAnimating
    //{
    //    get { return (bool)GetValue(IsAnimatingProperty); }
    //    set { SetValue(IsAnimatingProperty, value); }
    //}

    public static readonly BindableProperty IsAnimationEnabledProperty = BindableProperty.Create(nameof(IsAnimationEnabled), typeof(bool), typeof(PopupPage), true);

    public bool IsAnimationEnabled
    {
        get { return (bool)GetValue(IsAnimationEnabledProperty); }
        set { SetValue(IsAnimationEnabledProperty, value); }
    }

    public static readonly BindableProperty HasSystemPaddingProperty = BindableProperty.Create(nameof(HasSystemPadding), typeof(bool), typeof(PopupPage), true);

    public bool HasSystemPadding
    {
        get { return (bool)GetValue(HasSystemPaddingProperty); }
        set { SetValue(HasSystemPaddingProperty, value); }
    }

    public static readonly BindableProperty AnimationProperty = BindableProperty.Create(nameof(Animation), typeof(IPopupAnimation), typeof(PopupPage), new ScaleAnimation());

    public IPopupAnimation Animation
    {
        get { return (IPopupAnimation)GetValue(AnimationProperty); }
        set { SetValue(AnimationProperty, value); }
    }

    public static readonly BindableProperty SystemPaddingProperty = BindableProperty.Create(nameof(SystemPadding), typeof(Thickness), typeof(PopupPage), default(Thickness), BindingMode.OneWayToSource);

    public Thickness SystemPadding
    {
        get { return (Thickness)GetValue(SystemPaddingProperty); }
        internal set { SetValue(SystemPaddingProperty, value); }
    }

    public static readonly BindableProperty SystemPaddingSidesProperty = BindableProperty.Create(nameof(SystemPaddingSides), typeof(PaddingSide), typeof(PopupPage), PaddingSide.All);

    public PaddingSide SystemPaddingSides
    {
        get { return (PaddingSide)GetValue(SystemPaddingSidesProperty); }
        set { SetValue(SystemPaddingSidesProperty, value); }
    }

    public static readonly BindableProperty CloseWhenBackgroundIsClickedProperty = BindableProperty.Create(nameof(CloseWhenBackgroundIsClicked), typeof(bool), typeof(PopupPage), true);

    public bool CloseWhenBackgroundIsClicked
    {
        get { return (bool)GetValue(CloseWhenBackgroundIsClickedProperty); }
        set { SetValue(CloseWhenBackgroundIsClickedProperty, value); }
    }

    public static readonly BindableProperty BackgroundInputTransparentProperty = BindableProperty.Create(nameof(BackgroundInputTransparent), typeof(bool), typeof(PopupPage), false);

    public bool BackgroundInputTransparent
    {
        get { return (bool)GetValue(BackgroundInputTransparentProperty); }
        set { SetValue(BackgroundInputTransparentProperty, value); }
    }

    public static readonly BindableProperty HasKeyboardOffsetProperty = BindableProperty.Create(nameof(HasKeyboardOffset), typeof(bool), typeof(PopupPage), true);

    public bool HasKeyboardOffset
    {
        get { return (bool)GetValue(HasKeyboardOffsetProperty); }
        set { SetValue(HasKeyboardOffsetProperty, value); }
    }

    public static readonly BindableProperty KeyboardOffsetProperty = BindableProperty.Create(nameof(KeyboardOffset), typeof(double), typeof(PopupPage), 0d, BindingMode.OneWayToSource);

    public double KeyboardOffset
    {
        get { return (double)GetValue(KeyboardOffsetProperty); }
        private set { SetValue(KeyboardOffsetProperty, value); }
    }

    public static readonly BindableProperty BackgroundClickedCommandProperty = BindableProperty.Create(nameof(BackgroundClickedCommand), typeof(ICommand), typeof(PopupPage));

    public ICommand BackgroundClickedCommand
    {
        get => (ICommand)GetValue(BackgroundClickedCommandProperty);
        set => SetValue(BackgroundClickedCommandProperty, value);
    }

    public static readonly BindableProperty BackgroundClickedCommandParameterProperty = BindableProperty.Create(nameof(BackgroundClickedCommandParameter), typeof(object), typeof(PopupPage));

    public object BackgroundClickedCommandParameter
    {
        get => GetValue(BackgroundClickedCommandParameterProperty);
        set => SetValue(BackgroundClickedCommandParameterProperty, value);
    }

    public static readonly BindableProperty AndroidTalkbackAccessibilityWorkaroundProperty = BindableProperty.Create(nameof(AndroidTalkbackAccessibilityWorkaround), typeof(bool), typeof(PopupPage), false);

    public bool AndroidTalkbackAccessibilityWorkaround
    {
        get => (bool)GetValue(AndroidTalkbackAccessibilityWorkaroundProperty);
        set => SetValue(AndroidTalkbackAccessibilityWorkaroundProperty, value);
    }

    #endregion

    #region Main Methods

    public PopupPage()
    {
        BackgroundColor = Color.FromHex("#80000000");
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(HasSystemPadding):
            case nameof(HasKeyboardOffset):
            case nameof(SystemPaddingSides):
            case nameof(SystemPadding):
                ForceLayout();
                break;
            //case nameof(IsAnimating):
            //    IsAnimationEnabled = IsAnimating;
            //    break;
            //case nameof(IsAnimationEnabled):
            //    IsAnimating = IsAnimationEnabled;
            //    break;
        }
    }

    protected override bool OnBackButtonPressed()
    {
        return false;
    }

    #endregion

    #region Size Methods

    protected override void LayoutChildren(double x, double y, double width, double height)
    {
        if (HasSystemPadding)
        {
            var systemPadding = SystemPadding;
            var systemPaddingSide = SystemPaddingSides;
            var left = 0d;
            var top = 0d;
            var right = 0d;
            var bottom = 0d;

            if (systemPaddingSide.HasFlag(PaddingSide.Left))
                left = systemPadding.Left;
            if (systemPaddingSide.HasFlag(PaddingSide.Top))
                top = systemPadding.Top;
            if (systemPaddingSide.HasFlag(PaddingSide.Right))
                right = systemPadding.Right;
            if (systemPaddingSide.HasFlag(PaddingSide.Bottom))
                bottom = systemPadding.Bottom;

            x += left;
            y += top;
            width -= left + right;

            if (HasKeyboardOffset)
                height -= top + Math.Max(bottom, KeyboardOffset);
            else
                height -= top + bottom;
        }
        else if (HasKeyboardOffset)
        {
            height -= KeyboardOffset;
        }

        base.LayoutChildren(x, y, width, height);
    }

    #endregion

    #region Animation Methods

    internal void PreparingAnimation()
    {
        if (IsAnimationEnabled)
            Animation?.Preparing(Content, this);
    }

    internal void DisposingAnimation()
    {
        if (IsAnimationEnabled)
            Animation?.Disposing(Content, this);
    }

    internal async Task AppearingAnimation()
    {
        OnAppearingAnimationBegin();
        await OnAppearingAnimationBeginAsync();

        if (IsAnimationEnabled && Animation != null)
            await Animation.Appearing(Content, this);

        OnAppearingAnimationEnd();
        await OnAppearingAnimationEndAsync();
    }

    internal async Task DisappearingAnimation()
    {
        OnDisappearingAnimationBegin();
        await OnDisappearingAnimationBeginAsync();

        if (IsAnimationEnabled && Animation != null)
            await Animation.Disappearing(Content, this);

        OnDisappearingAnimationEnd();
        await OnDisappearingAnimationEndAsync();
    }

    #endregion

    #region Override Animation Methods

    protected virtual void OnAppearingAnimationBegin()
    {
    }

    protected virtual void OnAppearingAnimationEnd()
    {
    }

    protected virtual void OnDisappearingAnimationBegin()
    {
    }

    protected virtual void OnDisappearingAnimationEnd()
    {
    }

    protected virtual Task OnAppearingAnimationBeginAsync()
    {
        return Task.FromResult(0);
    }

    protected virtual Task OnAppearingAnimationEndAsync()
    {
        return Task.FromResult(0);
    }

    protected virtual Task OnDisappearingAnimationBeginAsync()
    {
        return Task.FromResult(0);
    }

    protected virtual Task OnDisappearingAnimationEndAsync()
    {
        return Task.FromResult(0);
    }

    #endregion

    #region Background Click

    protected virtual bool OnBackgroundClicked()
    {
        return CloseWhenBackgroundIsClicked;
    }

    #endregion

    #region Internal Methods

    internal async Task SendBackgroundClick()
    {
        BackgroundClicked?.Invoke(this, EventArgs.Empty);
        if (BackgroundClickedCommand?.CanExecute(BackgroundClickedCommandParameter) == true)
        {
            BackgroundClickedCommand.Execute(BackgroundClickedCommandParameter);
        }
        var isClose = OnBackgroundClicked();
        if (isClose)
        {
            await PopupNavigation.Instance.RemovePageAsync(this);
        }
    }

    #endregion
}
