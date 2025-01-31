// <copyright file="AngleLineRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreAnimation;

namespace RetroTransition;

/// <summary>
/// Angle Line Retro Transition.
/// </summary>
public class AngleLineRetroTransition : RetroTransition
{
    /// <summary>
    /// Gets or sets the corner to slide from.
    /// </summary>
    public UIRectCorner CornerToSlideFrom { get; set; } = UIRectCorner.TopLeft;

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

        var size = fromVC.View.Frame.Size;
        var toPath = new UIBezierPath();
        var fromPath = new UIBezierPath();

        switch (this.CornerToSlideFrom)
        {
            case UIRectCorner.TopRight:
                toPath.MoveTo(new CGPoint(-size.Width, 0));
                toPath.AddLineTo(new CGPoint(size.Width, size.Height * 2));
                toPath.AddLineTo(new CGPoint(-size.Width, size.Height * 2));
                toPath.ClosePath();

                fromPath.MoveTo(new CGPoint(0, -size.Height));
                fromPath.AddLineTo(new CGPoint(size.Width * 2, size.Height));
                fromPath.AddLineTo(new CGPoint(0, size.Height));
                fromPath.ClosePath();
                break;

            case UIRectCorner.BottomLeft:
                toPath.MoveTo(new CGPoint(size.Width * 2, size.Height));
                toPath.AddLineTo(new CGPoint(0, -size.Height));
                toPath.AddLineTo(new CGPoint(size.Width * 2, -size.Height));
                toPath.ClosePath();

                fromPath.MoveTo(new CGPoint(size.Width, size.Height * 2));
                fromPath.AddLineTo(new CGPoint(-size.Width, 0));
                fromPath.AddLineTo(new CGPoint(size.Width, 0));
                fromPath.ClosePath();
                break;

            case UIRectCorner.BottomRight:
                toPath.MoveTo(new CGPoint(-size.Width, size.Height));
                toPath.AddLineTo(new CGPoint(size.Width, -size.Height));
                toPath.AddLineTo(new CGPoint(-size.Width, -size.Height));
                toPath.ClosePath();

                fromPath.MoveTo(new CGPoint(0, size.Height * 2));
                fromPath.AddLineTo(new CGPoint(size.Width * 2, 0));
                fromPath.AddLineTo(CGPoint.Empty);
                fromPath.ClosePath();
                break;

            case UIRectCorner.TopLeft:
            default:
                toPath.MoveTo(new CGPoint(size.Width * 2, size.Height));
                toPath.AddLineTo(new CGPoint(0, size.Height * 2));
                toPath.AddLineTo(new CGPoint(size.Width * 2, size.Height * 2));
                toPath.ClosePath();

                fromPath.MoveTo(new CGPoint(size.Width, size.Height * -2));
                fromPath.AddLineTo(new CGPoint(-size.Width, size.Height));
                fromPath.AddLineTo(new CGPoint(size.Width, size.Height));
                fromPath.ClosePath();
                break;
        }

        var shapeLayer = new CAShapeLayer
        {
            Path = fromPath.CGPath,
            Bounds = new CGRect(0, 0, fromVC.View.Bounds.Width, fromVC.View.Bounds.Height),
            Position = new CGPoint(
                fromVC.View.Bounds.Width / 2,
                fromVC.View.Bounds.Height / 2),
        };

        fromVC.View.Layer.Mask = shapeLayer;

        var animation = new RetroBasicAnimation
        {
            KeyPath = "path",
            FillMode = CAFillMode.Forwards,
            RemovedOnCompletion = false,
            Duration = this.Duration,
            From = NSObject.FromObject(fromPath.CGPath),
            To = NSObject.FromObject(toPath.CGPath),
            AutoReverses = false,
            OnFinish = () =>
            {
                transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled);
                fromVC.View.Layer.Mask = null;
            },
        };

        shapeLayer.AddAnimation(animation, "path");
    }
}