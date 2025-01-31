// <copyright file="ReverseCircleRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreAnimation;

namespace RetroTransition;

/// <summary>
/// Reverse Circle Retro Transition.
/// </summary>
public class ReverseCircleRetroTransition : RetroTransition
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

        // Add views in reverse order compared to CircleRetroTransition
        containerView.AddSubview(fromVC.View);
        containerView.AddSubview(toVC.View);

        // Calculate the radius that will fully encompass the view
        var radius = (nfloat)Math.Sqrt(
            Math.Pow(toVC.View.Bounds.Height / 2, 2) +
            Math.Pow(toVC.View.Bounds.Width / 2, 2));

        // Create center point
        var center = new CGPoint(
            toVC.View.Bounds.Width / 2,
            toVC.View.Bounds.Height / 2);

        // Create start and end circle paths (reversed from CircleRetroTransition)
        var circlePathStart = UIBezierPath.FromArc(
            center,
            1, // Start with minimum radius
            0,
            (nfloat)(Math.PI * 2),
            true);

        var circlePathEnd = UIBezierPath.FromArc(
            center,
            radius, // End with maximum radius
            0,
            (nfloat)(Math.PI * 2),
            true);

        // Create and configure shape layer
        var shapeLayer = new CAShapeLayer
        {
            Path = circlePathStart.CGPath,
            Bounds = new CGRect(0, 0, toVC.View.Bounds.Width, toVC.View.Bounds.Height),
            Position = new CGPoint(
                toVC.View.Bounds.Width / 2,
                toVC.View.Bounds.Height / 2),
        };

        // Apply mask to the toVC view
        toVC.View.Layer.Mask = shapeLayer;

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
                toVC.View.Layer.Mask = null;
            },
        };

        // Add animation to the shape layer
        shapeLayer.AddAnimation(animation, "path");
    }
}