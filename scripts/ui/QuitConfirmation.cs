using Godot;
using System;

public partial class QuitConfirmation : UIAutoload<QuitConfirmation> {
	[Export] public Button YesButton;
	[Export] public Button NoButton;

	public override void _Ready() {
		// Always use null checks for Exports to be safe!
		if (YesButton != null) YesButton.Pressed += OnYesPressed;
		else YesButton = GetNode<Button>("%YesButton");
		if (NoButton != null) NoButton.Pressed += OnNoPressed;
		else NoButton = GetNode<Button>("%NoButton");

		Hide();
	}

	public void OnNoPressed() {
		GetTree().Paused = false;
		Hide();
		Log.Info("No Pressed");
	}
	public void OnYesPressed() {
		GetTree().Quit();
		Log.Info("Yes Pressed");
	}
}
