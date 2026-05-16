using System;
using Godot;

public partial class UIChickStats : Control {
	[Export] public Label ChickNameLabel;
	[Export] public Label ChickWeightLabel;
	[Export] public Label ChickStageLabel;
	[Export] public ProgressBar HungerBar;
	[Export] public ProgressBar ThirstBar;
	[Export] public TextureButton FeedButton;
	[Export] public TextureButton DrinkButton;
	[Export] public Button CloseButton;
	private ChickInteract trackedChick;
	
	public override void _Ready() {
		EventBus.Instance.OpenChickStatsUI += OnOpenFullStatsUi;
		CloseButton.Pressed += OnCloseButtonPressed;
		FeedButton.Pressed += OnFeedButtonPressed;
		DrinkButton.Pressed += DrinkButtonPressed;
		Hide();
	}

	private void OnFeedButtonPressed() {
		GD.Print("Chick is Fed");
		trackedChick.ChickStats.Feed(0.02f);
		HungerBar.Value = trackedChick.ChickStats.HungerLevel * 100f;
	}

	private void DrinkButtonPressed() {
		GD.Print("Chick is Watered");
		trackedChick.ChickStats.Water(0.01f);
		ThirstBar.Value = trackedChick.ChickStats.ThirstLevel * 100f;
	}

	private void OnCloseButtonPressed() {
		GetTree().Paused = false;
		Hide();
	}

	private void OnOpenFullStatsUi(ChickInteract chick) {
		GetTree().Paused = true;
		
		trackedChick = chick;
		var chickStats = chick.ChickStats;

		ChickNameLabel.Text = chickStats.Name;
		ChickWeightLabel.Text = chickStats.WeightGrams.ToString() + "kg";
		ChickStageLabel.Text = chickStats.Stage.ToString();
		HungerBar.Value = chickStats.HungerLevel * 100f;
		ThirstBar.Value = chickStats.ThirstLevel * 100f;
		Show();
	}
}