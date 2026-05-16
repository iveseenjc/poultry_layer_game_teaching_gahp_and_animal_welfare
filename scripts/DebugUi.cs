using Godot;
using Godot.Collections;

public partial class DebugUi : CanvasLayer {
	[Export] private Label touchPointsDisplay;
	[Export] private GameCamera gameCamera;

	public override void _Ready() {
		if (gameCamera != null) {
			gameCamera.SendTouchPoints += UpdateTouchPointDisplay;
			Log.Info("wassy");
		}
	}

	private void UpdateTouchPointDisplay(Dictionary touchPoints) {
		touchPointsDisplay.Text = touchPoints.ToString();
		Log.Info(touchPoints);
	}
}
