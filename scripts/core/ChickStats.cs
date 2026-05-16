using Godot;

public partial class ChickStats : Resource {
	public int Id { get; private set; }
	public string Name { get; set; }
	public bool IsAlive { get; private set; } = true;
	public float WeightGrams { get; private set; }
	public ChickStage Stage { get; private set; } = ChickStage.Chick;
	public bool IsMale { get; private set; }
	public float HungerLevel { get; set; } = 0f;   // 0=full, 1=starving
	public float ThirstLevel { get; set; } = 0f;
	public bool IsSick { get; private set; } = false;

	public ChickStats(int id) {
		Id = id;
		WeightGrams = 40f;
		// ~50% male at hatch; player culls males at pullet stage
		IsMale = id % 25 == 0;
	}

	public void UpdateDay(int dayOfLife, int stressLevel) {
		if (!IsAlive) return;

		WeightGrams = BioEngine.GetExpectedWeightGrams(dayOfLife);
		WeightGrams -= stressLevel * 5f; // stress reduces growth
		if (IsSick) WeightGrams -= 10f;

		// Stage transitions
		ChickStage newStage = dayOfLife <= 21 ? ChickStage.Chick :
							  dayOfLife <= 105 ? ChickStage.Pullet : ChickStage.AdultHen;
		Stage = newStage;

		HungerLevel = Godot.Mathf.Clamp(HungerLevel + 0.2f + stressLevel * 0.05f, 0f, 1f);
		ThirstLevel = Godot.Mathf.Clamp(ThirstLevel + 0.3f + stressLevel * 0.1f, 0f, 1f);
	}

	public void Feed(float amount) => HungerLevel = Godot.Mathf.Clamp(HungerLevel - amount, 0f, 1f);
	public void Water(float amount) => ThirstLevel = Godot.Mathf.Clamp(ThirstLevel - amount, 0f, 1f);
	public void SetSick(bool sick) => IsSick = sick;
	public void Kill(string cause) => IsAlive = false;
}