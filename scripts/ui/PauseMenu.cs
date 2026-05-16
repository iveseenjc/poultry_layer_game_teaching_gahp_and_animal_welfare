using Godot;
using System;

public partial class PauseMenu : PanelContainer
{
	[Export] public Button ResumeButton;
	[Export] public Button ReturnToMenuButton;
	[Export] public Button QuitButton;

	public override void _Ready() {
		Hide();
	}
}
