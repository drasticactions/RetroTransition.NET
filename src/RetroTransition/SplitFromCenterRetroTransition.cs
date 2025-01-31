// <copyright file="SplitFromCenterRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreAnimation;

namespace RetroTransition;

/// <summary>
/// Split From Center Retro Transition.
/// </summary>
public class SplitFromCenterRetroTransition : CollidingDiamondsRetroTransition
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

        var diamondSize = new CGSize(
            fromVC.View.Bounds.Width * 2,
            fromVC.View.Bounds.Height * 2);

        var containerView = transitionContext.ContainerView;
        containerView.AddSubview(toVC.View);
        containerView.AddSubview(fromVC.View);

        var containerLayer = new CALayer
        {
            Bounds = new CGRect(0, 0, fromVC.View.Bounds.Width, fromVC.View.Bounds.Height),
            Position = new CGPoint(
                fromVC.View.Bounds.Width / 2,
                fromVC.View.Bounds.Height / 2),
        };

        Action completion = () =>
        {
            transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled);
            fromVC.View.Layer.Mask = null;
        };

        // Top diamond
        var start = new CGPoint(
            fromVC.View.Bounds.Width / 2,
            (fromVC.View.Bounds.Height / 2) - (diamondSize.Height / 2));
        var layer = this.AnimatedDiamondPath(
            start,
            new CGPoint(start.X, start.Y - (diamondSize.Height / 2)),
            diamondSize,
            fromVC.View.Bounds,
            completion);
        containerLayer.AddSublayer(layer);

        // Bottom diamond
        start = new CGPoint(
            fromVC.View.Bounds.Width / 2,
            (fromVC.View.Bounds.Height / 2) + (diamondSize.Height / 2));
        layer = this.AnimatedDiamondPath(
            start,
            new CGPoint(start.X, start.Y + (diamondSize.Height / 2)),
            diamondSize,
            fromVC.View.Bounds,
            () => { });
        containerLayer.AddSublayer(layer);

        // Right diamond
        start = new CGPoint(
            (fromVC.View.Bounds.Width / 2) + (diamondSize.Width / 2),
            fromVC.View.Bounds.Height / 2);
        layer = this.AnimatedDiamondPath(
            start,
            new CGPoint(start.X + (diamondSize.Width / 2), start.Y),
            diamondSize,
            fromVC.View.Bounds,
            () => { });
        containerLayer.AddSublayer(layer);

        // Left diamond
        start = new CGPoint(
            (fromVC.View.Bounds.Width / 2) - (diamondSize.Width / 2),
            fromVC.View.Bounds.Height / 2);
        layer = this.AnimatedDiamondPath(
            start,
            new CGPoint(start.X - (diamondSize.Width / 2), start.Y),
            diamondSize,
            fromVC.View.Bounds,
            () => { });
        containerLayer.AddSublayer(layer);

        fromVC.View.Layer.Mask = containerLayer;
    }
}