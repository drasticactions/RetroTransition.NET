// <copyright file="CrossFadeRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace RetroTransition;

/// <summary>
/// Cross Fade Retro Transition.
/// </summary>
public class CrossFadeRetroTransition : RetroTransition
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

        fromVC.View.Alpha = 1.0f;
        toVC.View.Alpha = 0.0f;

        var containerView = transitionContext.ContainerView;
        containerView.AddSubview(fromVC.View);
        containerView.AddSubview(toVC.View);

        UIView.Animate(
            this.Duration,
            () =>
            {
                fromVC.View.Alpha = 0.0f;
                toVC.View.Alpha = 1.0f;
            },
            () =>
            {
                transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled);
                fromVC.View.Alpha = 1.0f;
            });
    }
}