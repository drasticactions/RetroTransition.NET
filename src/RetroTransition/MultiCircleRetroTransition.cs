// <copyright file="MultiCircleRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreAnimation;

namespace RetroTransition;

/// <summary>
/// Multi Circle Retro Transition.
/// </summary>
public class MultiCircleRetroTransition : RetroTransition
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

        Action cleanup = () =>
        {
            transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled);
            fromVC.View.RemoveFromSuperview();
            fromVC.View.Layer.Mask = null;
        };

        Func<CGPoint, CGSize, nfloat, Action, CAShapeLayer> createRectOutlinePath = (circleCenter, circleSize, circleRadius, completion) =>
        {
            var pathStart = new UIBezierPath();
            pathStart.AddArc(circleCenter, circleRadius, 0, (nfloat)(Math.PI * 2), true);

            var pathEnd = new UIBezierPath();
            pathEnd.AddArc(circleCenter, circleSize.Width, 0, (nfloat)(Math.PI * 2), true);

            var rect = new CGRect(
                circleCenter.X - (circleSize.Width / 2),
                circleCenter.Y - (circleSize.Height / 2),
                circleSize.Width,
                circleSize.Height);

            var shapeLayer = new CAShapeLayer
            {
                Bounds = new CGRect(0, 0, circleSize.Width, circleSize.Height),
                Position = new CGPoint(
                    (rect.X + rect.Width) - (circleSize.Width / 2),
                    (rect.Y + rect.Height) - (circleSize.Height / 2)),
                Path = pathStart.CGPath,
            };

            var animation = new RetroBasicAnimation
            {
                KeyPath = "path",
                Duration = this.Duration,
                From = NSObject.FromObject(pathEnd.CGPath),
                To = NSObject.FromObject(pathStart.CGPath),
                AutoReverses = false,
                OnFinish = completion,
            };

            shapeLayer.AddAnimation(animation, "path");
            return shapeLayer;
        };

        if (fromVC.View == null)
        {
            transitionContext.CompleteTransition(false);
            return;
        }

        var maskLayer = new CALayer
        {
            Bounds = new CGRect(0, 0, fromVC.View.Frame.Width, fromVC.View.Frame.Height),
            Position = new CGPoint(fromVC.View.Frame.Width / 2, fromVC.View.Frame.Height / 2),
        };

        var circleSize = new CGSize(20, 20);
        var rowCount = 1 + Math.Ceiling(fromVC.View.Bounds.Height / circleSize.Height);
        var colCount = 2 + Math.Ceiling(fromVC.View.Bounds.Width / circleSize.Width);

        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            for (int colIndex = 0; colIndex < colCount; colIndex++)
            {
                var circleCenter = new CGPoint(
                    (circleSize.Width / 2) + (colIndex * circleSize.Width),
                    (circleSize.Height / 2) + (rowIndex * circleSize.Height));

                var isFirst = rowIndex == 0 && colIndex == 0;
                maskLayer.AddSublayer(createRectOutlinePath(
                    circleCenter,
                    circleSize,
                    1,
                    isFirst ? cleanup : () => { }));
            }
        }

        fromVC.View.Layer.Mask = maskLayer;
    }
}