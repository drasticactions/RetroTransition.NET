// <copyright file="ColorGridViewController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace RetroTransition.Apple;

/// <summary>
/// Color Grid View Controller.
/// </summary>
public sealed class ColorGridViewController : UIViewController
{
    private const string CellId = "ColorCell";
    private UIPickerView? pickerView;
    private TransitionPickerDataModel? pickerDataModel;
    private UICollectionView? collectionView;
    private List<UIColor>? colors;

    /// <summary>
    /// Gets the selected transition.
    /// </summary>
    public RetroTransition SelectedTransition => this.pickerDataModel!.SelectedTransition ?? new CircleRetroTransition();

    /// <inheritdoc/>
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        this.Title = "Color Grid";
        this.View!.BackgroundColor = UIColor.White;

        this.InitializeColors();
        this.SetupPickerView();
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

    private void SetupPickerView()
    {
        this.pickerView = new UIPickerView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
        };

        this.View!.AddSubview(this.pickerView);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            this.pickerView.TopAnchor.ConstraintEqualTo(this.View.SafeAreaLayoutGuide.TopAnchor),
            this.pickerView.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor),
            this.pickerView.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor),
            this.pickerView.HeightAnchor.ConstraintEqualTo(150),
        });

        this.pickerView.Model = this.pickerDataModel = new TransitionPickerDataModel();
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
            BackgroundColor = UIColor.White,
            TranslatesAutoresizingMaskIntoConstraints = false,
        };

        this.collectionView.RegisterClassForCell(typeof(ColorCell), CellId);
        this.collectionView.DataSource = new ColorDataSource(this.colors!);
        this.collectionView.Delegate = new ColorDelegate(this);

        this.View.AddSubview(this.collectionView);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            this.collectionView.TopAnchor.ConstraintEqualTo(this.pickerView!.BottomAnchor),
            this.collectionView.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor),
            this.collectionView.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor),
            this.collectionView.BottomAnchor.ConstraintEqualTo(this.View.BottomAnchor),
        });
    }

    private class TransitionPickerDataModel : UIPickerViewModel
    {
        private List<RetroTransition> transitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionPickerDataModel"/> class.
        /// </summary>
        public TransitionPickerDataModel()
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

            this.SelectedTransition = this.transitions[0];
        }

        public RetroTransition? SelectedTransition { get; private set; }

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return this.transitions.Count;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            return this.transitions[(int)row].GetType().Name;
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
           this.SelectedTransition = this.transitions[(int)row];
        }
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
            var transition = this.parentViewController.SelectedTransition;
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