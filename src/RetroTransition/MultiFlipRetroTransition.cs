// <copyright file="MultiFlipRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreAnimation;

namespace RetroTransition;

/// <summary>
/// Multi Flip Retro Transition.
/// </summary>
public class MultiFlipRetroTransition : RetroTransition
{
    /// <summary>
    /// Gets or sets the step distance.
    /// </summary>
    public nfloat StepDistance { get; set; } = 0.333f;

    /// <summary>
    /// Gets or sets the animation step time.
    /// </summary>
    public double AnimationStepTime { get; set; } = 0.333;

    /// <summary>
    /// Default duration.
    /// </summary>
    /// <param name="transitionContext">The transition context.</param>
    /// <returns>Transition duration.</returns>
    [Export("transitionDuration:")]
    public new double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
    {
        return (1.0 / (double)this.StepDistance) * this.AnimationStepTime;
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
        containerView.AddSubview(toVC.View);

        var fromContainer = new UIView
        {
            Frame = fromVC.View.Bounds,
        };
        containerView.AddSubview(fromContainer);

        fromContainer.AddSubview(fromVC.View);

        this.FlipTo(transitionContext, fromVC.View, 1.0f - this.StepDistance);
    }

    private void FlipTo(
        IUIViewControllerContextTransitioning transitionContext,
        UIView view,
        nfloat scale)
    {
        var transform = view.Layer.Transform;
        view.Layer.AnchorPoint = new CGPoint(0.5f, 0.5f);

        transform = transform.Rotate((nfloat)Math.PI, 0.0f, 1.0f, 0.0f);
        transform = transform.Scale(scale, scale, 1.0f);

        var nextScale = scale - this.StepDistance;

        UIView.Animate(
            this.AnimationStepTime,
            0.0,
            UIViewAnimationOptions.CurveEaseInOut,
            () =>
            {
                view.Layer.Transform = transform;
            },
            () =>
            {
                if (nextScale > 0)
                {
                    this.FlipTo(transitionContext, view, nextScale);
                }
                else
                {
                    transitionContext.CompleteTransition(true);
                    view.Layer.Transform = CATransform3D.Identity;
                }
            });
    }
}