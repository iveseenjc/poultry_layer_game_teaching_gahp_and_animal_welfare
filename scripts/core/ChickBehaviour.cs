using System;
using System.Data.Common;
using Godot;

public partial class ChickBehaviour : CharacterBody3D {
	[Export] public int ChickId = 0;
	[Export] public float WanderRadius = 3f;
	[Export] public float MoveSpeed = 1.2f;
	[Export] public ChickStats Stats { get; private set; }
	[Export] public Label3D NameOverlay { get; private set; }
	[Export] public Area3D WanderArea { get; private set; }

	private Vector3 targetPosition;
	private float wanderTimer = 0f;
	private const float wander_interval = 3f;

	// private AnimationPlayer _anim;
	private EventBus _bus;

	public override void _Ready() {
		// _anim = GetNode<AnimationPlayer>("AnimationPlayer");
		EventBus.Instance.ChickHungry += OnHungry;
		EventBus.Instance.ChickSick += OnSick;
		EventBus.Instance.ChickGrowthStageChanged += OnStageChanged;
		EventBus.Instance.OnTick += OnTick;
		PickNewWanderTarget();
	}

	private void OnTick(double tickRate, int tickCount) {
		if (tickCount % 15 == 0) {
			Stats.HungerLevel += 0.01f;
			Stats.ThirstLevel += 0.01f;
		}
	}

	public override void _ExitTree() {
		EventBus.Instance.ChickHungry -= OnHungry;
		EventBus.Instance.ChickSick -= OnSick;
		EventBus.Instance.ChickGrowthStageChanged -= OnStageChanged;
	}

	public override void _PhysicsProcess(double delta) {

	}

	public void ProcessMovement(double delta) {
		wanderTimer -= (float) delta;
		if (wanderTimer <= 0f) {
			PickNewWanderTarget();
			LookAt(targetPosition);
			wanderTimer = wander_interval + GD.Randf() * 2f;
		}

		Vector3 direction = (targetPosition - GlobalPosition).Normalized();
		direction.Y = 0;
		Velocity = direction * MoveSpeed;

		MoveAndSlide();

		// if (Velocity.Length() > 0.1f)
		// 	_anim?.Play("walk");
		// else
		// 	_anim?.Play("idle");
	}

	public ChickBehaviour SetStats(ChickStats stats) {
		Stats = stats;
		ChickId = stats.Id;

		if (NameOverlay != null)
			NameOverlay.Text = stats.Name;

		return this;
	}

	public void SetWanderArea(Area3D wanderArea) {
		WanderArea = wanderArea;
	}

	private void PickNewWanderTarget() {
		if (WanderArea != null) {
			var area = WanderArea.GetChild<CollisionShape3D>(0);

			if (area.Shape is BoxShape3D boxArea) {
				Vector3 center = WanderArea.GlobalPosition;
				float x = (float)GD.RandRange(-boxArea.Size.X, boxArea.Size.X) * 0.5f;
				float y = 0f;
				float z = (float)GD.RandRange(-boxArea.Size.X, boxArea.Size.X) * 0.5f;
				targetPosition = center + new Vector3(x, y, z);
				return;
			}
		}

		float angle = GD.Randf() * Mathf.Tau;
		float dist = GD.Randf() * WanderRadius;
		targetPosition = GlobalPosition + new Vector3(
			Mathf.Cos(angle) * dist, 0,
			Mathf.Sin(angle) * dist
		);
	}

	private void OnHungry(int id) {
		if (id != ChickId) return;
		// _anim?.Play("distress");
		// Trigger "chick cries" audio
		GetNode<AudioStreamPlayer3D>("AudioPlayer")?.Play();
	}

	private void OnSick(int id, string disease) {
		if (id != ChickId) return;
		// Show visual sickness indicator
		GetNode<MeshInstance3D>("SickIndicator").Visible = true;
	}

	private void OnStageChanged(int id, ChickStage stage) {
		if (id != ChickId) return;
		// Swap 3D model mesh based on stage
		string meshPath = stage switch {
			ChickStage.Chick => "res://assets/meshes/chick.mesh",
			ChickStage.Pullet => "res://assets/meshes/pullet.mesh",
			ChickStage.AdultHen => "res://assets/meshes/hen.mesh",
			_ => "res://assets/meshes/chick.mesh"
		};
		GetNode<MeshInstance3D>("Mesh").Mesh = GD.Load<Mesh>(meshPath);
	}
}