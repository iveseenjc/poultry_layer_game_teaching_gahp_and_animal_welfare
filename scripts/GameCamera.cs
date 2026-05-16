using Godot;
using Godot.Collections;
using System.Linq;

public partial class GameCamera : Node3D {
	[Export] private float panSpeed = 0.03f;
	[Export] private Vector2 zoomRange = new(5f, 15f);
	[Export] private float zoomDampener = 0.5f;
	[Export] private float minimumYLevel = 0f;
	[Export] private bool canPan = true;
	[Export] private bool canZoom = true;
	[Export] public Area3D boundingArea;
	private Camera3D camera;
	private readonly Dictionary<int, Vector2> touchPoints = [];
	private float startDistance = 0.0f;
	private float startZoom = 0.0f;

	// Track the previous centroid to calculate relative movement during 2-finger pan
	private Vector2 lastCentroid = Vector2.Zero;

	private int debug_touchPointHash = 0;

	[Signal] public delegate void SendTouchPointsEventHandler(Dictionary touchPoints);

	public override void _Ready() {
		camera = GetNode<Camera3D>("Camera3D");
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventScreenTouch touch) {
			HandleScreenTouch(touch);
		}
		else if (@event is InputEventScreenDrag drag) {
			HandleScreenDrag(drag);
		}
	}

	public void HandleScreenTouch(InputEventScreenTouch @event) {
		if (@event.IsPressed()) {
			touchPoints[@event.Index] = @event.Position;
		}
		else {
			touchPoints.Remove(@event.Index);
		}

		if (touchPoints.Count == 2) {
			var points = touchPoints.Values.ToArray();
			startDistance = points[0].DistanceTo(points[1]);
			startZoom = camera.Size;
			// Initialize the centroid so the first drag frame doesn't "jump"
			lastCentroid = (points[0] + points[1]) * 0.5f;
		}
		else if (touchPoints.Count < 2) {
			startDistance = 0.0f;
		}
	}

	public void HandleScreenDrag(InputEventScreenDrag @event) {
		touchPoints[@event.Index] = @event.Position;

		if (touchPoints.Count == 1 && canPan) {
			// Single finger pan uses the direct event relative delta
			CalculateAndApplyPan(@event.Relative);
		}
		else if (touchPoints.Count == 2) {
			var points = touchPoints.Values.ToArray();
			Vector2 currentCentroid = (points[0] + points[1]) * 0.5f;
			float currentDistance = points[0].DistanceTo(points[1]);

			// 1. Handle Simultaneous Pan
			if (canPan) {
				Vector2 dragDelta = currentCentroid - lastCentroid;
				CalculateAndApplyPan(dragDelta);
				lastCentroid = currentCentroid;
			}

			// 2. Optimized, Smooth Touch Zoom Control
			// Setting the threshold to 10.0f ensures fingers don't jitter on microscopic pixel ranges
			if (canZoom && Mathf.Abs(startDistance) > 10.0f) {
				// Calculate the raw pixel distance differential between where fingers started vs where they are now
				float distanceDelta = currentDistance - startDistance;

				// Apply the dampener to linearize speed (Screen dimensions can scale this cleanly)
				// Higher zoomDampener = faster zoom, Lower = smoother/slower zoom
				float zoomAdjustment = distanceDelta * zoomDampener * 0.01f;

				// Subtract from startZoom so moving fingers APART (positive delta) DECREASES size (Zooms IN)
				float targetSize = startZoom - zoomAdjustment;

				// Clamp safely inside your bounds
				camera.Size = Mathf.Clamp(targetSize, zoomRange.X, zoomRange.Y);
			}
		}
	}
	
	private void CalculateAndApplyPan(Vector2 dragDelta) {
		Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
		Vector2 normalizedDelta = dragDelta / viewportSize;

		// We use camera rotation and size to calculate world-space movement
		float pitchRad = Mathf.Abs(camera.Rotation.X);
		float worldWidth = camera.Size * GetAspectRatio();

		// Adjust vertical movement based on camera tilt (Orthographic projection logic)
		float worldHeight = camera.Size / Mathf.Sin(pitchRad);

		float moveX = normalizedDelta.X * worldWidth;
		float moveY = normalizedDelta.Y * worldHeight;

		ApplyPan(moveX, moveY);
	}

	private void ApplyPan(float panX, float panY) {
		var cameraYaw = Basis.FromEuler(Vector3.Up * camera.GlobalRotation.Y);
		Position -= cameraYaw * new Vector3(panX, 0, panY);

		if (boundingArea != null) {
			var area = boundingArea.GetChild<CollisionShape3D>(0);

			if (area.Shape is BoxShape3D box) {
				var bounds = box.Size;
				// Clamping position based on the bounding area provided
				Position = new Vector3(
					Mathf.Clamp(Position.X, -bounds.X * 0.5f, bounds.X * 0.5f),
					Position.Y,
					Mathf.Clamp(Position.Z, -bounds.Z * 0.5f, bounds.Z * 0.5f)
				);
			}
		}
	}

	public float GetAspectRatio() {
		Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
		return viewportSize.X / viewportSize.Y;
	}
}