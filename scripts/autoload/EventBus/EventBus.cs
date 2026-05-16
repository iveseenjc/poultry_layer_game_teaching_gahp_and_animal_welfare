using Godot;

public partial class EventBus : Autoload<EventBus> {
	[Signal] public delegate void OnPausedEventHandler();
	[Signal] public delegate void OnResumedEventHandler();
}