using Godot;

public partial class UIBaseWindow : Control {
	[Export] public bool IsPopup = false;
	[Export] public Control FirstFocusedElement;

	public virtual void ShowWindow() {
		Visible = true;
		// Logic for focusing buttons for controller/keyboard support
		FirstFocusedElement?.GrabFocus();

		if (IsPopup)
			GetTree().Paused = true;
	}

	public virtual void HideWindow() {
		Visible = false;

		if (IsPopup)
			GetTree().Paused = false;
	}
}