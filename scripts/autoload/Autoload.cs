using Godot;

public abstract partial class Autoload<T> : Node where T : Node {
	protected static T _instance;

	public static T Instance {
		get {
			return _instance;
		}
	}

	public override void _EnterTree() {
		if (_instance == null) {
			_instance = this as T;

			// Helpful debug: if the cast fails, instance will still be null
			if (_instance == null) {
				GD.PrintErr($"Autoload Error: {Name} could not be cast to {typeof(T).Name}");
			}
		}
		else {
			QueueFree();
		}
	}
}