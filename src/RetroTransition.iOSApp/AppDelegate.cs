// <copyright file="AppDelegate.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using RetroTransition.Apple;

namespace RetroTransition.IOSApp;

/// <summary>
/// App Delegate.
/// </summary>
[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    /// <inheritdoc/>
    public override UIWindow? Window
    {
        get;
        set;
    }

    /// <inheritdoc/>
    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        this.Window = new UIWindow(UIScreen.MainScreen.Bounds);

        var vc = new UINavigationController(new ColorGridViewController());
        this.Window.RootViewController = vc;

        this.Window.MakeKeyAndVisible();

        return true;
    }
}
