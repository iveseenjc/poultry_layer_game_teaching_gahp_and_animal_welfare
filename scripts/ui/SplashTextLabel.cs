using Godot;
using System;

public partial class SplashTextLabel : Label {
	[Export(PropertyHint.File, "*.json")] public string JsonPath;

	public override void _Ready() {
		// Critical for the bounce effect to stay centered
		Resized += () => PivotOffset = Size / 2;
		PivotOffset = Size / 2;

		LoadRandomSplash();
		AnimateSplash();
	}

	private void LoadRandomSplash() {
		if (string.IsNullOrEmpty(JsonPath) || !FileAccess.FileExists(JsonPath)) return;

		using var file = FileAccess.Open(JsonPath, FileAccess.ModeFlags.Read);
		var json = new Json();
		if (json.Parse(file.GetAsText()) == Error.Ok) {
			var splashes = json.Data.AsGodotDictionary()["splashes"].AsGodotArray();
			Text = splashes[(int)(GD.Randi() % splashes.Count)].AsString();
		}
	}

	private void AnimateSplash() {
		Tween tween = CreateTween().SetLoops();
		tween.SetParallel(false);
		tween.TweenProperty(this, "scale", new Vector2(1.1f, 1.1f), 0.5f).SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(this, "scale", new Vector2(1.0f, 1.0f), 0.5f).SetTrans(Tween.TransitionType.Sine);
	}
}