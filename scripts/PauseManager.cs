using Godot;
using Godot.Collections;

public partial class PauseManager : Node {
	public Array<Node> ExecptionNodes { get; private set; } = [];
	public Array<ProcessModeEnum> BackupProcessModes { get; private set; } = [];	
	
	public static PauseManager Create(Node scene, Array<Node> exceptionNodes) {
		var instance = new PauseManager();
		instance.ExecptionNodes = exceptionNodes;

		foreach (Node node in exceptionNodes) {
			instance.BackupProcessModes.Add(node.ProcessMode);
			// node.ProcessMode = ProcessModeEnum.Always; // Not sure if I really need to put it
		}

		scene.CallDeferred(Node.MethodName.AddChild, instance);
		return instance;
	}
	
	public void Pause() {
		foreach (Node node in ExecptionNodes) {
			node.ProcessMode = ProcessModeEnum.Always;
		}

		GetTree().Paused = true;
	}

	public void UnPause() {
        for (int i = 0; i < ExecptionNodes.Count; i++) {
            ExecptionNodes[i].ProcessMode = BackupProcessModes[i];
		}

		GetTree().Paused = false;
	}
}
