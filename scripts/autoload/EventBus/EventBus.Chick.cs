using Godot;

public partial class EventBus {
	[Signal] public delegate void ChickHungryEventHandler(int chickId);
	[Signal] public delegate void ChickThirstyEventHandler(int chickId);
	[Signal] public delegate void ChickSickEventHandler(int chickId, string disease);
	[Signal] public delegate void ChickDiedEventHandler(int chickId, string cause);
	[Signal] public delegate void ChickGrowthStageChangedEventHandler(int chickId, ChickStage stage);

	[Signal] public delegate void OpenChickStatsUIEventHandler(ChickInteract chickInteract);
}