using Godot;

public abstract partial class UIAutoload<T> : Control where T : Control {
	private static T _instance;
	public static T Instance => _instance;

	public override void _EnterTree() {
		if (_instance == null) {
			_instance = this as T;
		}
		else {
			QueueFree();
		}
	}

	public static void ShowWindow() => Instance.Show();
	public static void HideWindow() => Instance.Hide();
}