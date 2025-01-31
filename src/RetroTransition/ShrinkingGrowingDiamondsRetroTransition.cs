// <copyright file="ShrinkingGrowingDiamondsRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreAnimation;

namespace RetroTransition;

/// <summary>
/// Shrinking Growing Diamonds Retro Transition.
/// </summary>
public class ShrinkingGrowingDiamondsRetroTransition : RetroTransition
{
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

        var diamondSize = fromVC.View.Bounds.Size;

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

        var start = new CGPoint(
            fromVC.View.Bounds.Width / 2,
            diamondSize.Height / 2);

        var layer = this.AnimatedDiamondPath(
            start,
            diamondSize,
            new CGSize(diamondSize.Width * 2, diamondSize.Height * 2),
            new CGSize(1, 1),
            fromVC.View.Bounds,
            completion);

        containerLayer.AddSublayer(layer);
        toVC.View.Layer.Mask = containerLayer;
    }

    private CALayer AnimatedDiamondPath(
        CGPoint startCenter,
        CGSize startSize,
        CGSize endSizeLarge,
        CGSize endSizeSmall,
        CGRect screenBounds,
        Action completion)
    {
        // Create start path with two diamonds
        var pathStart = new UIBezierPath();

        // Outer diamond
        pathStart.MoveTo(new CGPoint(startCenter.X - (startSize.Width / 2), startCenter.Y));
        pathStart.AddLineTo(new CGPoint(startCenter.X, startCenter.Y - (startSize.Height / 2)));
        pathStart.AddLineTo(new CGPoint(startCenter.X + (startSize.Width / 2), startCenter.Y));
        pathStart.AddLineTo(new CGPoint(startCenter.X, startCenter.Y + (startSize.Height / 2)));
        pathStart.ClosePath();

        // Inner diamond (same size initially)
        var pathStart2 = new UIBezierPath();
        pathStart2.MoveTo(new CGPoint(startCenter.X - (startSize.Width / 2), startCenter.Y));
        pathStart2.AddLineTo(new CGPoint(startCenter.X, startCenter.Y - (startSize.Height / 2)));
        pathStart2.AddLineTo(new CGPoint(startCenter.X + (startSize.Width / 2), startCenter.Y));
        pathStart2.AddLineTo(new CGPoint(startCenter.X, startCenter.Y + (startSize.Height / 2)));
        pathStart2.ClosePath();

        pathStart.AppendPath(pathStart2);
        pathStart.UsesEvenOddFillRule = true;

        // Create end path with two different sized diamonds
        var pathEnd = new UIBezierPath();

        // Larger diamond
        pathEnd.MoveTo(new CGPoint(startCenter.X - (endSizeLarge.Width / 2), startCenter.Y));
        pathEnd.AddLineTo(new CGPoint(startCenter.X, startCenter.Y - (endSizeLarge.Height / 2)));
        pathEnd.AddLineTo(new CGPoint(startCenter.X + (endSizeLarge.Width / 2), startCenter.Y));
        pathEnd.AddLineTo(new CGPoint(startCenter.X, startCenter.Y + (endSizeLarge.Height / 2)));
        pathEnd.ClosePath();

        // Smaller diamond
        var pathEnd2 = new UIBezierPath();
        pathEnd2.MoveTo(new CGPoint(startCenter.X - (endSizeSmall.Width / 2), startCenter.Y));
        pathEnd2.AddLineTo(new CGPoint(startCenter.X, startCenter.Y - (endSizeSmall.Height / 2)));
        pathEnd2.AddLineTo(new CGPoint(startCenter.X + (endSizeSmall.Width / 2), startCenter.Y));
        pathEnd2.AddLineTo(new CGPoint(startCenter.X, startCenter.Y + (endSizeSmall.Height / 2)));
        pathEnd2.ClosePath();

        pathEnd.AppendPath(pathEnd2);
        pathEnd.UsesEvenOddFillRule = true;

        // Create shape layer
        var shapeLayer = new CAShapeLayer
        {
            Path = pathStart.CGPath,
            FillRule = CAShapeLayer.FillRuleEvenOdd,
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