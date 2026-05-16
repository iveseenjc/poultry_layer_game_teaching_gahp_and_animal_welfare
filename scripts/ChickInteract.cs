using Godot;
using System;

public partial class ChickInteract : Area3D {
	[Export] public MeshInstance3D ChickMesh;
	[Export] public Label3D NameOverlay;
	[Export(PropertyHint.File, "*.gdshader")] public string OutlineShaderPath;
	public ChickStats ChickStats { get; private set; }
	private ShaderMaterial outlineMaterial;
	private static bool hasSelection;
	
	public override void _Ready() {
		NameOverlay.Show();
		InputRayPickable = true;
		Connect(SignalName.InputEvent, new Callable(this, MethodName.OnInputEvent));
	}

	public override void _UnhandledInput(InputEvent @event) {
		if (@event is InputEventScreenTouch screenTouch && screenTouch.Pressed) {
			hasSelection = false;
			ChickStats = null;
			// NameOverlay.Hide();
		}
	}

	private void OnInputEvent(Node camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, long shapeIdx) {
		if (@event is InputEventScreenTouch touch && touch.IsPressed()) {
			GetViewport().SetInputAsHandled();

			if (!hasSelection) {
				hasSelection = true;
				ChickStats = GetParent<ChickBehaviour>().Stats;
				// NameOverlay.Show();
				EventBus.Instance.EmitSignal(EventBus.SignalName.OpenChickStatsUI, this);
			}
		}
	}
}
