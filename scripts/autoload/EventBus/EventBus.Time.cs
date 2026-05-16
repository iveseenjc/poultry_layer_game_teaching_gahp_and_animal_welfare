using Godot;

public partial class EventBus {
	[Signal] public delegate void OnTickEventHandler(double tickRate, int tickCount);
	[Signal] public delegate void OnMinutePassedEventHandler(int minutes);
	[Signal] public delegate void OnHourPassedEventHandler(int hours);
	[Signal] public delegate void OnDayChangedEventHandler(int day);
	[Signal] public delegate void OnWeekChangedEventHandler(int week);
}