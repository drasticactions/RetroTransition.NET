// <copyright file="FlipRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace RetroTransition;

/// <summary>
/// Flip Retro Transition.
/// </summary>
public class FlipRetroTransition : RetroTransition
{
    /// <summary>
    /// Animate the transition.
    /// </summary>
    /// <param name="transitionContext">The transition context.</param>
    [Export("animateTransition:")]
    public new void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
    {
        var fromVC = transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);
        var toVC = transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);

        if (fromVC?.View == null || toVC.View == null)
        {
            return;
        }

        var containerView = transitionContext.ContainerView;
        containerView.AddSubview(fromVC.View);

        var transitionOptions = UIViewAnimationOptions.TransitionFlipFromRight |
                                UIViewAnimationOptions.CurveEaseInOut;

        UIView.Transition(
            containerView,
            this.Duration,
            transitionOptions,
            () =>
            {
                containerView.AddSubview(toVC.View);
            },
            () =>
            {
                transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled);
            });
    }
}