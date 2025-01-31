// <copyright file="RectanglerRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreAnimation;
using CoreFoundation;

namespace RetroTransition;

/// <summary>
/// Rectangle Retro Transition.
/// </summary>
public class RectanglerRetroTransition : RetroTransition
{
    private static readonly nfloat RectangleGrowthDistance = 60f;

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

        CAShapeLayer? CreateRectOutlinePath(CGRect outerRect, Action? completion)
        {
            var magnitude = RectangleGrowthDistance * 0.2f;
            if (outerRect.Width <= RectangleGrowthDistance || outerRect.Height <= RectangleGrowthDistance)
            {
                return null;
            }

            var innerRect = RectMovedIn(outerRect, magnitude);
            if (innerRect.Width <= RectangleGrowthDistance || innerRect.Height <= RectangleGrowthDistance)
            {
                return null;
            }

            var path = UIBezierPath.FromRect(outerRect);
            path.AppendPath(UIBezierPath.FromRect(innerRect));
            path.UsesEvenOddFillRule = true;

            var finalPath = UIBezierPath.FromRect(outerRect);
            finalPath.AppendPath(UIBezierPath.FromRect(RectMovedIn(innerRect, RectangleGrowthDistance)));
            finalPath.UsesEvenOddFillRule = true;

            void RunAnimationToPathWithCompletion(CGPath pathEnd, CAShapeLayer layer, Action? animCompletion)
            {
                var animation = new RetroBasicAnimation
                {
                    KeyPath = "path",
                    Duration = this.Duration,
                    From = NSObject.FromObject(pathEnd),
                    To = NSObject.FromObject(path.CGPath),
                    AutoReverses = false,
                    RemovedOnCompletion = false,
                    OnFinish = animCompletion,
                };

                layer.AddAnimation(animation, "path");
            }

            var shapeLayer = new CAShapeLayer
            {
                Bounds = new CGRect(0, 0, outerRect.Width, outerRect.Height),
                Position = new CGPoint(outerRect.Width / 2, outerRect.Height / 2),
                FillRule = CAShapeLayer.FillRuleEvenOdd,
                Path = path.CGPath,
            };

            RunAnimationToPathWithCompletion(finalPath.CGPath!, shapeLayer, completion);
            return shapeLayer;
        }

        Action cleanup = () =>
        {
            transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled);
            fromVC.View.Alpha = 1;
            fromVC.View.Layer.Mask = null;
        };

        var maskLayer = new CALayer
        {
            Bounds = new CGRect(0, 0, fromVC.View.Bounds.Width, fromVC.View.Bounds.Height),
            Position = new CGPoint(fromVC.View.Bounds.Width / 2, fromVC.View.Bounds.Height / 2),
        };

        for (int i = 0; i < 8; i++)
        {
            var magnitude = i * RectangleGrowthDistance;
            if (magnitude <= fromVC.View.Bounds.Width && magnitude <= fromVC.View.Bounds.Height)
            {
                var startRect = RectMovedIn(fromVC.View.Bounds, magnitude);
                var sublayer = CreateRectOutlinePath(startRect, null);
                if (sublayer != null)
                {
                    maskLayer.AddSublayer(sublayer);
                }
            }
        }

        fromVC.View.Layer.Mask = maskLayer;

        UIView.Animate(this.Duration, () =>
        {
            fromVC.View.Alpha = 0.0f;
        });

        DispatchQueue.MainQueue.DispatchAfter(
            new DispatchTime(DispatchTime.Now, TimeSpan.FromSeconds(this.Duration)),
            cleanup);
    }
}