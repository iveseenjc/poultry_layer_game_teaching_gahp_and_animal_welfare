using Godot;
using System;

public partial class GameClock : Autoload<GameClock> {
	[Export] public double TickRate = 0.1; // 10 ticks per second
	[Export] public int TicksPerMinute = 30;

	private double timer = 0;
	private int tickCount = 0;
	public int Minutes { get; private set; } = 0;
	public int Hours { get; private set; } = 6;
	public int Days { get; private set; } = 1;
	public int Weeks { get; private set; } = 0;
	private bool clockTicking = false;

	public override void _Ready() {
		ProcessMode = ProcessModeEnum.Pausable;
	}

	public override void _Process(double delta) {
		if (!clockTicking)
			return;
		
		timer += delta;

		while (timer >= TickRate) {
			timer -= TickRate;
			tickCount++;

			ProcessGameTime();
			EventBus.Instance.EmitSignal(EventBus.SignalName.OnTick, TickRate, tickCount);
		}
	}

	public void SetClockAtive(bool setActive) {
		Log.Info("Game Clock Started!");
		clockTicking = setActive;
	}

	private void ProcessGameTime() {
		if (tickCount % TicksPerMinute == 0) {
			Minutes++;

			if (Minutes >= 60) {
				Minutes = 0;
				Hours++;
				EventBus.Instance.EmitSignal(EventBus.SignalName.OnHourPassed, Hours);
			}

			if (Hours >= 24) {
				Hours = 0;
				Days++;
				EventBus.Instance.EmitSignal(EventBus.SignalName.OnDayChanged, Days);
			}

			EventBus.Instance.EmitSignal(EventBus.SignalName.OnMinutePassed, Minutes);
		}
	}

}