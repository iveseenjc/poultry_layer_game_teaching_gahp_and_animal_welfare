using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class FlockManager : Node {
	[Export] public PackedScene Prefab;
	[Export] public Area3D SpawnArea;
	[Export] public int InitialFlockSize = 50;
	[Export(PropertyHint.File, "*.json")] public string RandomNamesJsonPath;

	public int AliveCount { get; private set; }

	private List<ChickStats> flockStats = new();
	private List<ChickBehaviour> flockEntities = new();

	public override void _Ready() {
		var indexFrequencyTable = new Array<int>();

		for (int i = 0; i < InitialFlockSize; i++) {
			Vector3 randomPoint = RandomPointInSpawnArea();
			var chick = Spawn(randomPoint);
			var stats = new ChickStats(i);

			stats.Name = $"Chick " + (i + 1);

			if (!string.IsNullOrEmpty(RandomNamesJsonPath) && FileAccess.FileExists(RandomNamesJsonPath)) {
				using var file = FileAccess.Open(RandomNamesJsonPath, FileAccess.ModeFlags.Read);
				var json = new Json();
				if (json.Parse(file.GetAsText()) == Error.Ok) {
					var names = json.Data.AsGodotDictionary()["chick_names"].AsGodotArray();

					if (names.Count > 0) {
						int randomIndex = GD.RandRange(0, names.Count - 1);
						while (indexFrequencyTable.Contains(randomIndex) && indexFrequencyTable.Count < InitialFlockSize) {
							randomIndex = GD.RandRange(0, names.Count - 1);
						}

						stats.Name = names[randomIndex].AsString();
					}
				}
			}

			chick.SetStats(stats);
			chick.SetWanderArea(SpawnArea);

			flockStats.Add(chick.Stats);
			flockEntities.Add(chick);
		}

		AliveCount = InitialFlockSize;
	}

	public override void _PhysicsProcess(double delta) {
		flockEntities.ForEach(c => c.ProcessMovement(delta));
	}

	public ChickBehaviour Spawn(Vector3 position) {
		var instance = Prefab.Instantiate<ChickBehaviour>();
		instance.Position = position;
		AddChild(instance);

		return instance;
	}

	public Vector3 RandomPointInSpawnArea() {
		var area = SpawnArea.GetChild<CollisionShape3D>(0);

		if (area.Shape is BoxShape3D boxArea) {
			Vector3 center = SpawnArea.GlobalPosition;
			float x = (float) GD.RandRange(-boxArea.Size.X, boxArea.Size.X) * 0.5f;
			float y = 0f;
			float z = (float) GD.RandRange(-boxArea.Size.X, boxArea.Size.X) * 0.5f;

			return center + new Vector3(x, y, z);
		}

		return Vector3.Zero;
	}
}