// <copyright file="ColorGridViewController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace RetroTransition.TvOSApp;

/// <summary>
/// Color Grid View Controller.
/// </summary>
public sealed class ColorGridViewController : UIViewController
{
    private const string CellId = "ColorCell";
    private UICollectionView? collectionView;
    private List<UIColor>? colors;
    private List<RetroTransition> transitions;

    /// <inheritdoc/>
    public override void ViewDidLoad()
    {
        this.transitions =
        [
            new AngleLineRetroTransition(),
            new CircleRetroTransition(),
            new ClockRetroTransition(),
            new CollidingDiamondsRetroTransition(),
            new CrossFadeRetroTransition(),
            new FlipRetroTransition(),
            new ImageRepeatingRetroTransition(),
            new MultiCircleRetroTransition(),
            new MultiFlipRetroTransition(),
            new RectanglerRetroTransition(),
            new ReverseCircleRetroTransition(),
            new ReverseStarRevealRetroTransition(),
            new ShrinkingGrowingDiamondsRetroTransition(),
            new SplitFromCenterRetroTransition(),
            new StarRevealRetroTransition(),
            new StraightLineRetroTransition(),
            new SwingInRetroTransition(),
            new TiledFlipRetroTransition(),
        ];
        base.ViewDidLoad();
        this.Title = "Color Grid";

        this.InitializeColors();
        this.SetupCollectionView();
    }

    private void InitializeColors()
    {
        this.colors = new List<UIColor>
        {
            UIColor.Red,
            UIColor.Green,
            UIColor.Blue,
            UIColor.Yellow,
            UIColor.Purple,
            UIColor.Orange,
            UIColor.Brown,
            UIColor.Gray,
            UIColor.Cyan,
            UIColor.Magenta,
        };
    }
    
    private void SetupCollectionView()
    {
        var layout = new UICollectionViewFlowLayout
        {
            MinimumInteritemSpacing = 1,
            MinimumLineSpacing = 1,
            ItemSize = new CGSize((this.View!.Frame.Width / 3) - 1, (this.View!.Frame.Width / 3) - 1),
        };

        this.collectionView = new UICollectionView(this.View.Frame, layout)
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
        };

        this.collectionView.RegisterClassForCell(typeof(ColorCell), CellId);
        this.collectionView.DataSource = new ColorDataSource(this.colors!);
        this.collectionView.Delegate = new ColorDelegate(this);

        this.View.AddSubview(this.collectionView);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            this.collectionView.TopAnchor.ConstraintEqualTo(this.View!.TopAnchor),
            this.collectionView.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor),
            this.collectionView.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor),
            this.collectionView.BottomAnchor.ConstraintEqualTo(this.View.BottomAnchor),
        });
    }

    private class ColorDataSource : UICollectionViewDataSource
    {
        private List<UIColor> colors;

        public ColorDataSource(List<UIColor> colors)
        {
            this.colors = colors;
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.colors.Count;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (ColorCell)collectionView.DequeueReusableCell(CellId, indexPath);
            cell.SetColor(this.colors[indexPath.Row]);
            return cell;
        }
    }

    private class ColorDelegate : UICollectionViewDelegate
    {
        private ColorGridViewController parentViewController;

        public ColorDelegate(ColorGridViewController parentViewController)
        {
            this.parentViewController = parentViewController;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (ColorCell)collectionView.CellForItem(indexPath)!;
            var color = cell.ContentView.Subviews[0].BackgroundColor;

            // pick a transition at random
            var transition = this.parentViewController.transitions[new Random().Next(this.parentViewController.transitions.Count)];
            var detailVC = new ColorDetailViewController(color!, transition);
            this.parentViewController.NavigationController!.PushViewController(detailVC, transition);
        }
    }

    private sealed class ColorCell : UICollectionViewCell
    {
        private UIView colorView;

        [Export("initWithFrame:")]
        public ColorCell(CGRect frame)
            : base(frame)
        {
            this.colorView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            this.ContentView.AddSubview(this.colorView);

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                this.colorView.TopAnchor.ConstraintEqualTo(this.ContentView!.TopAnchor),
                this.colorView.LeadingAnchor.ConstraintEqualTo(this.ContentView.LeadingAnchor),
                this.colorView.TrailingAnchor.ConstraintEqualTo(this.ContentView.TrailingAnchor),
                this.colorView.BottomAnchor.ConstraintEqualTo(this.ContentView.BottomAnchor),
            });
        }

        public void SetColor(UIColor color)
        {
            this.colorView.BackgroundColor = color;
        }
    }
}