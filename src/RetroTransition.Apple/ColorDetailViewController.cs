// <copyright file="ColorDetailViewController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace RetroTransition.Apple;

/// <summary>
/// Color Detail View Controller.
/// </summary>
public sealed class ColorDetailViewController : UIViewController
{
    private UIColor selectedColor;
    private UIButton? backButton;

    private RetroTransition transition;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorDetailViewController"/> class.
    /// </summary>
    /// <param name="color">The UIColor Background.</param>
    /// <param name="transition">The Transition.</param>
    public ColorDetailViewController(UIColor color, RetroTransition transition)
    {
        this.selectedColor = color;
        this.transition = transition;
    }

    /// <inheritdoc/>
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        this.View!.BackgroundColor = this.selectedColor;
        this.SetupBackButton();
    }

    private void SetupBackButton()
    {
        this.backButton = new UIButton(UIButtonType.System)
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
        };

        this.backButton.SetTitle("Back to Colors", UIControlState.Normal);
        this.backButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
        this.backButton.TouchUpInside += (sender, e) => this.NavigationController!.PopViewController(this.transition);

        this.View!.AddSubview(this.backButton);

        NSLayoutConstraint.ActivateConstraints(new[]
        {
            this.backButton.CenterXAnchor.ConstraintEqualTo(this.View.CenterXAnchor),
            this.backButton.CenterYAnchor.ConstraintEqualTo(this.View.CenterYAnchor),
        });
    }
}