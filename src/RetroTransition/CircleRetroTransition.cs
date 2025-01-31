// <copyright file="CircleRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreAnimation;

namespace RetroTransition;

/// <summary>
/// Circle Retro Transition.
/// </summary>
public class CircleRetroTransition : RetroTransition
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
        containerView.AddSubview(toVC.View);
        containerView.AddSubview(fromVC.View);

        // Calculate the radius that will fully encompass the view
        var radius = (nfloat)Math.Sqrt(
            Math.Pow(fromVC.View.Bounds.Height / 2, 2) +
            Math.Pow(fromVC.View.Bounds.Width / 2, 2));

        // Create center point
        var center = new CGPoint(
            fromVC.View.Bounds.Width / 2,
            fromVC.View.Bounds.Height / 2);

        // Create start and end circle paths
        var circlePathStart = UIBezierPath.FromArc(
            center,
            radius,
            0,
            (nfloat)(Math.PI * 2),
            true);

        var circlePathEnd = UIBezierPath.FromArc(
            center,
            1, // Minimum radius
            0,
            (nfloat)(Math.PI * 2),
            true);

        // Create and configure shape layer
        var shapeLayer = new CAShapeLayer
        {
            Path = circlePathStart.CGPath,
            Bounds = new CGRect(0, 0, fromVC.View.Bounds.Width, fromVC.View.Bounds.Height),
            Position = new CGPoint(
                fromVC.View.Bounds.Width / 2,
                fromVC.View.Bounds.Height / 2),
        };

        // Apply mask to the fromVC view
        fromVC.View.Layer.Mask = shapeLayer;

        // Create and configure animation
        var animation = new RetroBasicAnimation
        {
            KeyPath = "path",
            FillMode = CAFillMode.Forwards,
            RemovedOnCompletion = false,
            Duration = this.Duration,
            From = NSObject.FromObject(circlePathStart.CGPath),
            To = NSObject.FromObject(circlePathEnd.CGPath),
            AutoReverses = false,
            OnFinish = () =>
            {
                transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled);
                fromVC.View.Layer.Mask = null;
            },
        };

        // Add animation to the shape layer
        shapeLayer.AddAnimation(animation, "path");
    }
}