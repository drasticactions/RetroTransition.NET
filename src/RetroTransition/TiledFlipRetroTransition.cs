// <copyright file="TiledFlipRetroTransition.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreFoundation;

namespace RetroTransition;

/// <summary>
/// Tiled Flip Retro Transition.
/// </summary>
public class TiledFlipRetroTransition : RetroTransition
{
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

        var snapshotToVc = this.Snapshot(toVC.View);
        var snapshotFromVc = this.Snapshot(fromVC.View);

        if (snapshotToVc == null || snapshotFromVc == null)
        {
            return;
        }

        var containerView = transitionContext.ContainerView;
        fromVC.View.RemoveFromSuperview();

        var parentView = new UIView
        {
            BackgroundColor = UIColor.Clear,
            Frame = fromVC.View.Frame,
        };
        containerView.AddSubview(parentView);

        Action cleanup = () =>
        {
            containerView.AddSubview(toVC.View);
            parentView.RemoveFromSuperview();
            transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled);
        };

        nfloat squareSizeWidth = fromVC.View.Bounds.Width / 5;
        nfloat squareSizeHeight = fromVC.View.Bounds.Height / 10;

        int numRows = 1 + (int)(toVC.View.Bounds.Width / squareSizeWidth);
        int numCols = 1 + (int)(toVC.View.Bounds.Height / squareSizeWidth);

        var random = new Random();
        for (int x = 0; x <= numRows; x++)
        {
            for (int y = 0; y <= numCols; y++)
            {
                var rect = new CGRect(
                    x * squareSizeWidth,
                    y * squareSizeHeight,
                    squareSizeWidth,
                    squareSizeHeight);

                var randomPercent = random.NextDouble();
                var delay = this.Duration * 0.5 * randomPercent;

                this.FlipSegment(snapshotToVc, snapshotFromVc, delay, rect, (nfloat)(this.Duration / 2), parentView);
            }
        }

        DispatchQueue.MainQueue.DispatchAfter(
            new DispatchTime(DispatchTime.Now, TimeSpan.FromSeconds(this.Duration)), cleanup);
    }

    private void FlipSegment(UIImage toViewImage, UIImage fromViewImage, double delay, CGRect rect, nfloat animationTime, UIView parentView)
    {
        // Get cropped CGImages
        var cgToImage = toViewImage.CGImage;
        var cgFromImage = fromViewImage.CGImage;

        if (cgToImage == null || cgFromImage == null)
        {
            return;
        }

        var toRect = new CGRect(
            toViewImage.CurrentScale * rect.X,
            toViewImage.CurrentScale * rect.Y,
            toViewImage.CurrentScale * rect.Width,
            toViewImage.CurrentScale * rect.Height);

        var fromRect = new CGRect(
            fromViewImage.CurrentScale * rect.X,
            fromViewImage.CurrentScale * rect.Y,
            fromViewImage.CurrentScale * rect.Width,
            fromViewImage.CurrentScale * rect.Height);

        var toImageRef = cgToImage.WithImageInRect(toRect);
        var fromImageRef = cgFromImage.WithImageInRect(fromRect);

        if (toImageRef == null || fromImageRef == null)
        {
            return;
        }

        var toImage = UIImage.FromImage(toImageRef);
        var toImageView = new UIImageView
        {
            ClipsToBounds = true,
            Frame = rect,
            Image = toImage,
        };

        var fromImage = UIImage.FromImage(fromImageRef);
        var fromImageView = new UIImageView
        {
            ClipsToBounds = true,
            Frame = rect,
            Image = fromImage,
        };

        var containerView = new UIView
        {
            Frame = fromImageView.Frame,
            BackgroundColor = UIColor.Clear,
        };

        fromImageView.Frame = new CGRect(CGPoint.Empty, fromImageView.Frame.Size);
        toImageView.Frame = new CGRect(CGPoint.Empty, toImageView.Frame.Size);

        containerView.AddSubview(fromImageView);
        parentView.AddSubview(containerView);

        var transitionOptions = UIViewAnimationOptions.TransitionFlipFromRight |
                              UIViewAnimationOptions.CurveEaseInOut;

        DispatchQueue.MainQueue.DispatchAfter(new DispatchTime(DispatchTime.Now, TimeSpan.FromSeconds(delay)), () =>
        {
            UIView.Transition(
                containerView,
                animationTime,
                transitionOptions,
                () =>
                {
                    containerView.AddSubview(toImageView);
                    fromImageView.RemoveFromSuperview();
                },
                () =>
                {
                });
        });
    }
}