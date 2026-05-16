using Godot;
using System;

public partial class PauseButton : TextureButton {	
	[Export] public Control PauseMenu;
	[Export] public Button ResumeButton;
	
	public override void _Ready() {
		if (ResumeButton != null) {
			ResumeButton.Pressed += OnResumePressed;
		}
	}

	

	private void OnResumePressed() {
		PauseMenu.Hide();
		GetTree().Paused = false;
	}

	public override void _Pressed() {
		PauseMenu.Show();
		GetTree().Paused = true;
	}
}
