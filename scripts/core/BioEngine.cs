using Godot;

/// <summary>
/// Computes HDP, FCR, flock uniformity, mortality.
/// Called every in-game day by FlockManager.
/// </summary>
public static class BioEngine {
	// Hen-Day Production: eggs laid / hens alive
	public static float CalculateHDP(int eggsLaid, int hensAlive) {
		if (hensAlive <= 0) return 0f;
		return (float)eggsLaid / hensAlive * 100f;
	}

	// Feed Conversion Ratio: feed consumed (kg) / eggs produced (kg)
	// Standard egg weight ~60g
	public static float CalculateFCR(float feedConsumedKg, int eggsProduced) {
		float eggWeightKg = eggsProduced * 0.06f;
		if (eggWeightKg <= 0) return 99f; // bad score
		return feedConsumedKg / eggWeightKg;
	}

	// Flock uniformity: % of birds within ±10% of mean weight
	public static float CalculateUniformity(float[] weights) {
		if (weights.Length == 0) return 0f;
		float mean = 0f;
		foreach (var w in weights) mean += w;
		mean /= weights.Length;

		float low = mean * 0.9f;
		float high = mean * 1.1f;
		int inRange = 0;
		foreach (var w in weights)
			if (w >= low && w <= high) inRange++;

		return (float)inRange / weights.Length * 100f;
	}

	// THI = Dry Bulb Temp °C + 0.36 * Dew Point °C + 41.2
	// Or simplified: THI = Temp - (0.55 - 0.0055 * RH) * (Temp - 14.5)
	public static float CalculateTHI(float tempC, float relativeHumidity) {
		return tempC - (0.55f - 0.0055f * relativeHumidity) * (tempC - 14.5f);
	}

	// Returns stress level 0-3: 0=None, 1=Mild, 2=Moderate, 3=Severe
	public static int GetHeatStressLevel(float thi) {
		if (thi < 72f) return 0;
		if (thi < 78f) return 1;
		if (thi < 84f) return 2;
		return 3;
	}

	// Daily mortality probability based on stress and disease
	public static float CalculateMortalityRate(int stressLevel, bool hasDisease, float baseRate = 0.003f) {
		float rate = baseRate;
		rate += stressLevel * 0.005f;
		if (hasDisease) rate += 0.02f;
		return Mathf.Clamp(rate, 0f, 1f);
	}

	// Feed requirement per bird per day (grams) by stage
	public static float GetDailyFeedRequirementGrams(ChickStage stage) {
		return stage switch {
			ChickStage.Chick => 15f,   // Day 1-21
			ChickStage.Pullet => 60f,   // Week 4-15
			ChickStage.AdultHen => 115f, // Week 16+
			_ => 15f
		};
	}

	// Water requirement: roughly 2x feed weight
	public static float GetDailyWaterRequirementMl(ChickStage stage)
		=> GetDailyFeedRequirementGrams(stage) * 2f;

	// Expected weight by day
	public static float GetExpectedWeightGrams(int dayOfLife) {
		// Lohmann Brown growth curve approximation
		if (dayOfLife <= 21) return 40f + dayOfLife * 4f;    // Brooding: ~40-124g
		if (dayOfLife <= 105) return 124f + (dayOfLife - 21) * 15f; // Growing
		return 1800f; // Adult plateau ~1.8kg
	}
}