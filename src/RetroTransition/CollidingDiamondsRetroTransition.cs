// <copyright file="CollidingDiamondsRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreAnimation;

namespace RetroTransition;

/// <summary>
/// Colliding Diamonds Retro Transition.
/// </summary>
public class CollidingDiamondsRetroTransition : RetroTransition
{
    /// <summary>
    /// Colliding Diamonds Orientation.
    /// </summary>
    public enum CollidingDiamondsOrientation
    {
        /// <summary>
        /// Vertical Orientation.
        /// </summary>
        Vertical,

        /// <summary>
        /// Horizontal Orientation.
        /// </summary>
        Horizontal,
    }

    /// <summary>
    /// Gets or sets the orientation of the diamonds.
    /// </summary>
    public CollidingDiamondsOrientation Orientation { get; set; } = CollidingDiamondsOrientation.Horizontal;

    /// <inheritdoc/>
    public override double DefaultDuration()
    {
        return 1.0;
    }

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
        containerView.AddSubview(fromVC.View);
        containerView.AddSubview(toVC.View);

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
            toVC.View.Layer.Mask = null;
        };

        if (this.Orientation == CollidingDiamondsOrientation.Vertical)
        {
            // Top diamond
            var start = new CGPoint(
                fromVC.View.Bounds.Width / 2,
                diamondSize.Height / -2);
            var layer = this.AnimatedDiamondPath(
                start,
                new CGPoint(start.X, start.Y + diamondSize.Height),
                diamondSize,
                fromVC.View.Bounds,
                () => { });
            containerLayer.AddSublayer(layer);

            // Bottom diamond
            start = new CGPoint(
                fromVC.View.Bounds.Width / 2,
                (diamondSize.Height * 0.5f) + fromVC.View.Bounds.Height);
            layer = this.AnimatedDiamondPath(
                start,
                new CGPoint(start.X, start.Y - diamondSize.Height),
                diamondSize,
                fromVC.View.Bounds,
                completion);
            containerLayer.AddSublayer(layer);
        }
        else
        {
            // Left diamond
            var start = new CGPoint(
                diamondSize.Width / -2,
                fromVC.View.Bounds.Height / 2);
            var layer = this.AnimatedDiamondPath(
                start,
                new CGPoint(start.X + diamondSize.Width, start.Y),
                diamondSize,
                fromVC.View.Bounds,
                () => { });
            containerLayer.AddSublayer(layer);

            // Right diamond
            start = new CGPoint(
                fromVC.View.Bounds.Width + (diamondSize.Width * 0.5f),
                fromVC.View.Bounds.Height / 2);
            layer = this.AnimatedDiamondPath(
                start,
                new CGPoint(start.X - diamondSize.Width, start.Y),
                diamondSize,
                fromVC.View.Bounds,
                completion);
            containerLayer.AddSublayer(layer);
        }

        toVC.View.Layer.Mask = containerLayer;
    }

    /// <summary>
    /// Animated Diamond Path.
    /// </summary>
    /// <param name="startCenter">Start Center.</param>
    /// <param name="endCenter">End Center.</param>
    /// <param name="size">Size.</param>
    /// <param name="screenBounds">Screen Bounds.</param>
    /// <param name="completion">Completion.</param>
    /// <returns>CALayer.</returns>
    protected CALayer AnimatedDiamondPath(
        CGPoint startCenter,
        CGPoint endCenter,
        CGSize size,
        CGRect screenBounds,
        Action completion)
    {
        // Create start path
        var pathStart = new UIBezierPath();
        pathStart.MoveTo(new CGPoint(startCenter.X - (size.Width / 2), startCenter.Y));
        pathStart.AddLineTo(new CGPoint(startCenter.X, startCenter.Y - (size.Height / 2)));
        pathStart.AddLineTo(new CGPoint(startCenter.X + (size.Width / 2), startCenter.Y));
        pathStart.AddLineTo(new CGPoint(startCenter.X, startCenter.Y + (size.Height / 2)));
        pathStart.ClosePath();

        // Create end path
        var pathEnd = new UIBezierPath();
        pathEnd.MoveTo(new CGPoint(endCenter.X - (size.Width / 2), endCenter.Y));
        pathEnd.AddLineTo(new CGPoint(endCenter.X, endCenter.Y - (size.Height / 2)));
        pathEnd.AddLineTo(new CGPoint(endCenter.X + (size.Width / 2), endCenter.Y));
        pathEnd.AddLineTo(new CGPoint(endCenter.X, endCenter.Y + (size.Height / 2)));
        pathEnd.ClosePath();

        // Create shape layer
        var shapeLayer = new CAShapeLayer
        {
            Path = pathStart.CGPath,
            Bounds = new CGRect(0, 0, screenBounds.Width, screenBounds.Height),
            Position = new CGPoint(screenBounds.Width / 2, screenBounds.Height / 2),
        };

        // Create and configure animation
        var animation = new RetroBasicAnimation
        {
            KeyPath = "path",
            FillMode = CAFillMode.Forwards,
            RemovedOnCompletion = false,
            Duration = this.Duration,
            From = NSObject.FromObject(pathStart.CGPath),
            To = NSObject.FromObject(pathEnd.CGPath),
            AutoReverses = false,
            OnFinish = completion,
        };

        shapeLayer.AddAnimation(animation, "path");
        return shapeLayer;
    }
}