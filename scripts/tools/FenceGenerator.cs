using Godot;
using System;

[Tool]
public partial class FenceGenerator : Path3D {
	[Export] public PackedScene FenceMesh;
	[Export] public float Spacing = 2f;
	[Export] 
	public bool GenerateFence_b {
		get => false;
		set {
			if (value) {
				GenerateFence();
			}
		}
	}

	[Export]
	public bool ClearFences_b {
		get => false;
		set {
			if (value) {
				ClearExistingFences();
			}
		}
	}

	public void GenerateFence() {
		if (FenceMesh == null)
			return;

		if (Curve == null || Spacing <= 0.1f) 
			return;

		float pathLength = Curve.GetBakedLength();
		int count = Mathf.FloorToInt(pathLength / Spacing) + 1;

		for (int i = 0; i < count; i++) {
			var follower = new PathFollow3D();
			AddChild(follower);

			follower.Owner = Engine.IsEditorHint() ? GetTree().EditedSceneRoot : Owner;
			follower.Progress = i * Spacing;

			var fencePiece = FenceMesh.Instantiate<Node3D>();
			follower.AddChild(fencePiece);
			fencePiece.Owner = Engine.IsEditorHint() ? GetTree().EditedSceneRoot : Owner;
		}

		#if false
		if (Engine.IsEditorHint()) {
			var selectTion = EditorInterface.Singleton.GetSelection();
			selectTion.Clear();
			selectTion.AddNode(this);
			EditorInterface.Singleton.EditNode(this);
		}
		#endif
	}

	public void ClearExistingFences() {
		foreach (Node child in GetChildren()) {
			if (child is PathFollow3D) {
				child.QueueFree();
			}
		}
	}
}
