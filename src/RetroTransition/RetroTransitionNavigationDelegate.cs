// <copyright file="RetroTransitionNavigationDelegate.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace RetroTransition
{
    internal class RetroTransitionNavigationDelegate : NSObject, IUINavigationControllerDelegate
    {
        private static readonly RetroTransitionNavigationDelegate shared = new RetroTransitionNavigationDelegate();

        public static RetroTransitionNavigationDelegate Shared => shared;

        private readonly System.Collections.Generic.List<RetroTransition> transitions = new System.Collections.Generic.List<RetroTransition>();
        private IUINavigationControllerDelegate oldNavigationDelegate;

        public void PushTransition(RetroTransition transition, UINavigationController navigationController)
        {
            this.transitions.Add(transition);
            this.oldNavigationDelegate = navigationController.Delegate;
            navigationController.Delegate = RetroTransitionNavigationDelegate.Shared;
        }

        [Export("navigationController:animationControllerForOperation:fromViewController:toViewController:")]
        public IUIViewControllerAnimatedTransitioning GetAnimationControllerForOperation(
            UINavigationController navigationController,
            UINavigationControllerOperation operation,
            UIViewController fromViewController,
            UIViewController toViewController)
        {
            var transition = this.transitions.Count > 0 ? this.transitions[this.transitions.Count - 1] : null;
            this.transitions.RemoveAt(this.transitions.Count - 1);

            navigationController.Delegate = this.oldNavigationDelegate;

            return transition;
        }
    }
}