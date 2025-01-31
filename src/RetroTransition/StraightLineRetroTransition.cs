// <copyright file="StraightLineRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreAnimation;

namespace RetroTransition;

/// <summary>
/// Straight Line Retro Transition.
/// </summary>
public class StraightLineRetroTransition : RetroTransition
{
    /// <summary>
    /// Gets or sets the side to slide from.
    /// </summary>
    public UIRectEdge SideToSlideFrom { get; set; } = UIRectEdge.Left;

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
        var fromPath = UIBezierPath.FromRect(fromVC.View.Bounds);
        UIBezierPath toPath;

        switch (this.SideToSlideFrom)
        {
            case UIRectEdge.Top:
                toPath = UIBezierPath.FromRect(new CGRect(0, size.Height, size.Width, 0));
                break;
            case UIRectEdge.Left:
                toPath = UIBezierPath.FromRect(new CGRect(size.Width, 0, size.Width, size.Height));
                break;
            case UIRectEdge.Right:
                toPath = UIBezierPath.FromRect(new CGRect(0, 0, 0, size.Height));
                break;
            case UIRectEdge.Bottom:
                toPath = UIBezierPath.FromRect(new CGRect(0, 0, size.Width, 0));
                break;
            default:
                toPath = UIBezierPath.FromRect(new CGRect(size.Width, 0, size.Width, size.Height));
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