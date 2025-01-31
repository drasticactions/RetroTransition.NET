// <copyright file="SwingInRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace RetroTransition;

/// <summary>
/// Swing In Retro Transition.
/// </summary>
public class SwingInRetroTransition : RetroTransition
{
    /// <summary>
    /// Initial direction.
    /// </summary>
    public enum InitialDirection
    {
        /// <summary>
        /// Left.
        /// </summary>
        Left,

        /// <summary>
        /// Right.
        /// </summary>
        Right,
    }

    /// <summary>
    /// Gets or sets the direction.
    /// </summary>
    public InitialDirection Direction { get; set; } = InitialDirection.Left;

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

        var containerView = transitionContext.ContainerView;
        containerView.AddSubview(fromVC.View);

        var toContainerView = new UIView
        {
            BackgroundColor = UIColor.Clear,
            Frame = toVC.View.Bounds,
        };

        // Set initial position based on direction
        toContainerView.Frame = new CGRect(
            this.Direction == InitialDirection.Left ?
                -fromVC.View.Bounds.Width :
                fromVC.View.Bounds.Width * 2,
            0,
            toContainerView.Frame.Width,
            toContainerView.Frame.Height);

        toContainerView.AddSubview(toVC.View);
        containerView.AddSubview(toContainerView);

        // Set initial scale transform
        toVC.View.Transform = CGAffineTransform.MakeScale(0.1f, 0.1f);

        // Animate scale
        UIView.Animate(
            this.Duration,
            0.0,
            UIViewAnimationOptions.CurveEaseOut,
            () =>
            {
                toVC.View.Transform = CGAffineTransform.MakeIdentity();
            },
            () =>
            {
                transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled);
            });

        // Animate position with spring effect
        UIView.AnimateNotify(
            this.Duration,
            0.0,
            0.6f,  // damping ratio
            0.1f,  // initial spring velocity
            UIViewAnimationOptions.CurveEaseInOut,
            () =>
            {
                toContainerView.Frame = fromVC.View.Bounds;
            },
            null);
    }
}