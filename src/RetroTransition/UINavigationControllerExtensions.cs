// <copyright file="UINavigationControllerExtensions.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace RetroTransition
{
    /// <summary>
    /// UINavigationController Extensions.
    /// </summary>
    public static class UINavigationControllerExtensions
    {
        /// <summary>
        /// Push a view controller with a retro transition.
        /// </summary>
        /// <param name="navigationController">The UINavigationController.</param>
        /// <param name="viewController">The View Controller to push.</param>
        /// <param name="transition">The Transition.</param>
        public static void PushViewController(
            this UINavigationController navigationController,
            UIViewController viewController,
            RetroTransition transition)
        {
            RetroTransitionNavigationDelegate.Shared.PushTransition(transition, navigationController);
            navigationController.PushViewController(viewController, true);
        }

        /// <summary>
        /// Pop a view controller with a retro transition.
        /// </summary>
        /// <param name="navigationController">The UINavigationController.</param>
        /// <param name="transition">The Transition.</param>
        /// <returns>The popped view controller.</returns>
        public static UIViewController PopViewController(
            this UINavigationController navigationController,
            RetroTransition transition)
        {
            RetroTransitionNavigationDelegate.Shared.PushTransition(transition, navigationController);
            return navigationController.PopViewController(true);
        }
    }
}