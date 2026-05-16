using Godot;
using System;

public partial class TimeInfoHud : TextureRect
{
	[Export] public Label DayLabel;
	[Export] public Label TimeLabel;
	
	public override void _Ready() {
		EventBus.Instance.OnMinutePassed += OnMinutePassed;
		GameClock.Instance.SetClockAtive(true);
		UpdateDateTime();
	}

	private void OnMinutePassed(int minutes) {
		int hours = GameClock.Instance.Hours;
		int days = GameClock.Instance.Days;
		
		UpdateDateTime(days, hours, minutes);
	}

	public void UpdateDateTime() {
		int minutes = GameClock.Instance.Minutes;
		int hours = GameClock.Instance.Hours;
		int days = GameClock.Instance.Days;

		UpdateDateTime(days, hours, minutes);
	}

	public void UpdateDateTime(int day, int hour, int minute) {
		DayLabel.Text = $"Day {day}";
		TimeLabel.Text = $"{hour:D2}:{minute:D2}";
	}

	public override void _ExitTree() {
		EventBus.Instance.OnMinutePassed -= OnMinutePassed;
		GameClock.Instance.SetClockAtive(false);
	}
}
