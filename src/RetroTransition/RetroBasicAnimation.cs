// <copyright file="RetroBasicAnimation.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreAnimation;

namespace RetroTransition
{
    /// <summary>
    /// Retro Basic Animation.
    /// </summary>
    public class RetroBasicAnimation : CABasicAnimation, ICAAnimationDelegate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetroBasicAnimation"/> class.
        /// </summary>
        public RetroBasicAnimation()
            : base()
        {
            this.Delegate = this;
        }

        /// <summary>
        /// Gets or sets the OnFinish action.
        /// </summary>
        public Action? OnFinish { get; set; }

        /// <summary>
        /// Animation did stop.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="finished">Finished.</param>
        [Export("animationDidStop:finished:")]
        public void AnimationDidStop(CAAnimation animation, bool finished)
        {
            this.OnFinish?.Invoke();
        }
    }
}