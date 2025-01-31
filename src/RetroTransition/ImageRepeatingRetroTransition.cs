// <copyright file="ImageRepeatingRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreFoundation;

namespace RetroTransition;

/// <summary>
/// Image Repeating Retro Transition.
/// </summary>
public class ImageRepeatingRetroTransition : RetroTransition
{
    /// <summary>
    /// The image views.
    /// </summary>
    private List<UIImageView> imageViews = new List<UIImageView>();

    /// <summary>
    /// Gets or sets the image step percent.
    /// </summary>
    public nfloat ImageStepPercent { get; set; } = 0.05f;

    /// <summary>
    /// Gets or sets the image step time.
    /// </summary>
    public double ImageStepTime { get; set; } = 0.2;

    /// <summary>
    /// The transition duration.
    /// </summary>
    /// <param name="transitionContext">The transition context.</param>
    /// <returns>Time for transition duration.</returns>
    [Export("transitionDuration:")]
    public new double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
    {
        var numberOfImageViews = (int)(0.5 / this.ImageStepPercent);
        return this.ImageStepTime * numberOfImageViews * 2;
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

        var fromViewControllerImage = this.Snapshot(fromVC.View);
        if (fromViewControllerImage == null)
        {
            return;
        }

        var containerView = transitionContext.ContainerView;
        containerView.AddSubview(toVC.View);

        this.AddImageView(transitionContext, fromViewControllerImage, containerView.Bounds);
    }

    private void RemoveImageView(IUIViewControllerContextTransitioning transitionContext)
    {
        if (this.imageViews.Count > 0)
        {
            var imageView = this.imageViews[0];
            imageView.RemoveFromSuperview();
            this.imageViews.RemoveAt(0);

            if (this.imageViews.Count == 0)
            {
                transitionContext.CompleteTransition(true);
                return;
            }

            DispatchQueue.MainQueue.DispatchAfter(
                new DispatchTime(DispatchTime.Now, TimeSpan.FromSeconds(this.ImageStepTime)),
                () => this.RemoveImageView(transitionContext));
        }
    }

    private void AddImageView(
        IUIViewControllerContextTransitioning transitionContext,
        UIImage fromViewImage,
        CGRect imageViewRect)
    {
        var imageView = new UIImageView(imageViewRect)
        {
            ClipsToBounds = true,
            Image = fromViewImage,
        };
        transitionContext.ContainerView.AddSubview(imageView);
        this.imageViews.Add(imageView);

        var widthStep = transitionContext.ContainerView.Bounds.Width * this.ImageStepPercent;
        var heightStep = transitionContext.ContainerView.Bounds.Height * this.ImageStepPercent;

        if (imageViewRect.Width - (widthStep * 2) <= 0 ||
            imageViewRect.Height - (heightStep * 2) <= 0)
        {
            DispatchQueue.MainQueue.DispatchAfter(
                new DispatchTime(DispatchTime.Now, TimeSpan.FromSeconds(this.ImageStepTime)),
                () => this.RemoveImageView(transitionContext));
            return;
        }

        var nextImageViewRect = new CGRect(
            imageViewRect.X + widthStep,
            imageViewRect.Y + heightStep,
            imageViewRect.Width - (widthStep * 2),
            imageViewRect.Height - (heightStep * 2));

        DispatchQueue.MainQueue.DispatchAfter(
            new DispatchTime(DispatchTime.Now, TimeSpan.FromSeconds(this.ImageStepTime)),
            () => this.AddImageView(transitionContext, fromViewImage, nextImageViewRect));
    }
}