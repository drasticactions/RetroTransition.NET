// <copyright file="StarRevealRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreAnimation;

namespace RetroTransition;

/// <summary>
/// Star Reveal Retro Transition.
/// </summary>
public class StarRevealRetroTransition : RetroTransition
{
    /// <summary>
    /// Gets or sets the number of points on the star.
    /// </summary>
    public nfloat StarPoints { get; set; } = 5;

    /// <summary>
    /// Gets or sets the inner ratio of the star.
    /// </summary>
    public nfloat StarInnerRatio { get; set; } = 0.5f;

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
        var maxRadius = (nfloat)Math.Sqrt(
            Math.Pow(fromVC.View.Bounds.Height / 2, 2) +
            Math.Pow(fromVC.View.Bounds.Width / 2, 2));

        // Create center point
        var center = new CGPoint(
            fromVC.View.Bounds.Width / 2,
            fromVC.View.Bounds.Height / 2);

        // Create start and end star paths
        var starPathStart = this.CreateStarPath(center, maxRadius);
        var starPathEnd = this.CreateStarPath(center, 1); // Minimum radius

        // Create and configure shape layer
        var shapeLayer = new CAShapeLayer
        {
            Path = starPathStart.CGPath,
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
            From = NSObject.FromObject(starPathStart.CGPath),
            To = NSObject.FromObject(starPathEnd.CGPath),
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

    private UIBezierPath CreateStarPath(CGPoint center, nfloat radius)
    {
        var path = new UIBezierPath();
        var angleIncrement = (nfloat)(Math.PI * 2.0 / (this.StarPoints * 2));
        var startAngle = -(nfloat)Math.PI / 2;
        var innerRadius = radius * this.StarInnerRatio;

        // Move to first outer point
        var firstX = center.X + (radius * (nfloat)Math.Cos(startAngle));
        var firstY = center.Y + (radius * (nfloat)Math.Sin(startAngle));
        path.MoveTo(new CGPoint(firstX, firstY));

        // Draw the star
        for (int i = 1; i < this.StarPoints * 2; i++)
        {
            var angle = startAngle + (angleIncrement * i);
            var currentRadius = i % 2 == 0 ? radius : innerRadius;

            var x = center.X + (currentRadius * (nfloat)Math.Cos(angle));
            var y = center.Y + (currentRadius * (nfloat)Math.Sin(angle));

            path.AddLineTo(new CGPoint(x, y));
        }

        path.ClosePath();
        return path;
    }
}