using Godot;
using System;

public partial class ReturnToMenuButton : Button
{
	[Export(PropertyHint.File, "*.tscn")] public string  SceneToGo;

	public override void _Pressed() {
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile(SceneToGo);
	}

}
