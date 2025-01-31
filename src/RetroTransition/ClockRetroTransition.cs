// <copyright file="ClockRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreAnimation;

namespace RetroTransition;

/// <summary>
/// Clock Retro Transition.
/// </summary>
public class ClockRetroTransition : RetroTransition, IUIViewControllerAnimatedTransitioning
{
    /// <inheritdoc/>
    public override double DefaultDuration()
    {
        return 0.7;
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

        if (fromVC?.View == null || toVC?.View == null)
        {
            return;
        }

        var containerView = transitionContext.ContainerView;
        containerView.AddSubview(toVC.View);
        containerView.AddSubview(fromVC.View);

        var radius = 2 * Math.Sqrt(Math.Pow(fromVC.View.Bounds.Height / 2, 2) + Math.Pow(fromVC.View.Bounds.Width / 2, 2));
        var circleCenter = new CGPoint(radius, radius);

        Func<double, CGPath> circleFromToAngle = (endAngle) =>
        {
            var path = new UIBezierPath();
            path.MoveTo(circleCenter);
            path.AddLineTo(circleCenter);
            path.AddArc(circleCenter, (nfloat)radius, 0, (nfloat)endAngle, true);
            return path.CGPath!;
        };

        var shapeLayer = new CAShapeLayer
        {
            Bounds = new CGRect(0, 0, radius, radius),
            Position = new CGPoint(
                (fromVC.View.Frame.Width / 2) - (radius / 2),
                (fromVC.View.Frame.Height / 2) - (radius / 2)),
            Path = circleFromToAngle(2.0 * Math.PI),
        };

        fromVC.View.Layer.Mask = shapeLayer;

        Action cleanup = () =>
        {
            transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled);
            fromVC.View.Layer.Mask = null;
            fromVC.View.RemoveFromSuperview();
        };

        Action<CGPath, CGPath, Action> runAnimationToPathWithCompletion = (pathStart, pathEnd, completion) =>
        {
            var animation = new RetroBasicAnimation
            {
                KeyPath = "path",
                Duration = this.Duration / 4,
                From = NSObject.FromObject(pathStart),
                To = NSObject.FromObject(pathEnd),
                RemovedOnCompletion = false,
                FillMode = CAFillMode.Forwards,
                AutoReverses = false,
                OnFinish = completion,
            };

            shapeLayer.AddAnimation(animation, "path");
        };

        // Chain the animations
        runAnimationToPathWithCompletion(
            circleFromToAngle(Math.PI * 2.0),
            circleFromToAngle(Math.PI * 1.50001),
            () => runAnimationToPathWithCompletion(
                circleFromToAngle(Math.PI * 1.5),
                circleFromToAngle(Math.PI * 1.00001),
                () => runAnimationToPathWithCompletion(
                    circleFromToAngle(Math.PI * 1.0),
                    circleFromToAngle(Math.PI * 0.50001),
                    () => runAnimationToPathWithCompletion(
                        circleFromToAngle(Math.PI * 0.5),
                        circleFromToAngle(Math.PI * 0.0001),
                        cleanup))));
    }
}