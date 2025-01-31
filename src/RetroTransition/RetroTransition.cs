// <copyright file="RetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace RetroTransition
{
    /// <summary>
    /// Retro Transition.
    /// </summary>
    public class RetroTransition : NSObject, IUIViewControllerAnimatedTransitioning
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetroTransition"/> class.
        /// </summary>
        /// <param name="duration">The duration to override. Otherwise uses default.</param>
        public RetroTransition(double? duration = null)
        {
            this.Duration = duration ?? this.DefaultDuration();
        }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        public double Duration { get; set; } = 0.33;

        /// <summary>
        /// Default duration.
        /// </summary>
        /// <returns>Double.</returns>
        public virtual double DefaultDuration()
        {
            return 0.33;
        }

        /// <summary>
        /// Transition duration.
        /// </summary>
        /// <param name="transitionContext">The transition context.</param>
        /// <returns>Transition time.</returns>
        [Export("transitionDuration:")]
        public double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
        {
            return this.Duration;
        }

        /// <summary>
        /// Animate the transition.
        /// </summary>
        /// <param name="transitionContext">The transition context.</param>
        [Export("animateTransition:")]
        public void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled);
        }

        /// <summary>
        /// Rectangle moved in.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="magnitude">The magnitude.</param>
        /// <returns>CGRect.</returns>
        internal static CGRect RectMovedIn(CGRect rect, nfloat magnitude)
        {
            return new CGRect(
                rect.X + magnitude,
                rect.Y + magnitude,
                rect.Width - (magnitude * 2),
                rect.Height - (magnitude * 2));
        }

        /// <summary>
        /// Snapshot a view.
        /// </summary>
        /// <param name="view">The snapshot view.</param>
        /// <returns>UIImage.</returns>
        internal UIImage Snapshot(UIView view)
        {
            UIImage image;

            if (OperatingSystem.IsIOSVersionAtLeast(17, 0) || OperatingSystem.IsTvOSVersionAtLeast(17, 0))
            {
                UIGraphicsImageRenderer renderer = new UIGraphicsImageRenderer(view.Bounds.Size);
                image = renderer.CreateImage((context) =>
                {
                    view.Layer.RenderInContext(context.CGContext);
                });
            }
            else
            {
                UIGraphics.BeginImageContextWithOptions(view.Bounds.Size, true, UIScreen.MainScreen.Scale);
                view.Layer.RenderInContext(UIGraphics.GetCurrentContext());
                image = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
            }

            return image;
        }
    }
}