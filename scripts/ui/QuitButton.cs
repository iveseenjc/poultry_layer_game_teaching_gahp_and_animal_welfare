using Godot;
using Godot.Collections;
using System;

public partial class QuitButton : Button
{
	[Export] private Array<Control> ThingsToHide = new(); 

	public override void _Pressed() {
		foreach (var thing in ThingsToHide) {
			thing.Hide();
		}

		QuitConfirmation.ShowWindow();
		Log.Info("Nigger is pressed");
	}
}
