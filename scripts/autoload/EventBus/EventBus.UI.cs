using Godot;

public partial class EventBus {
	[Signal] public delegate void UITriggerRequestedEventHandler(string trigger);
}