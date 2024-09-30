using Godot;
using System;

public partial class GameManager : Node
{
    public static GameManager Instance;
	public Viewport Root;
    public Node CurrentScene;
    public bool TimePasses = true;
    public override void _Ready() {
		CurrentScene = Root.GetChild(Root.GetChildCount() - 1);
    }
    public override void _EnterTree() {
		Instance = this;
		Root = GetTree().Root;
    }

    public void Win(string reason) {
        TimePasses = false;
        StartTimer(1, () => {
    		SwapScene("res://win_scene.tscn");
            GetNode<Label>("../Win/Reason").Text = reason;
            TimePasses = true;
        });
    }
    public void Lose(string reason) {
        TimePasses = false;
        StartTimer(1, () => {
    		SwapScene("res://lose_scene.tscn");
            GetNode<Label>("../Lose/Reason").Text = reason;
            TimePasses = true;
        });
    }

	public void StartTimer(float time, Action action) {
		Timer timer = new(){
			OneShot = true,
			Autostart = true,
			WaitTime = time,
		};
		AddChild(timer);
		timer.Timeout += action;
	}
    public void QuitApplication() {
		GetTree().Quit();
	}
	public void LoadScene(string scene) {
		var pack = GD.Load<PackedScene>(scene);
		var instance = pack.Instantiate();
		GetTree().Root.AddChild(instance);
		CurrentScene = instance;
	}
	public void DeleteScene() {
		Root.GetChild(Root.GetChildCount() - 1).QueueFree();
		CurrentScene = null;
	}
	public void SwapScene(string scene, float time) {
		StartTimer(time, () => {SwapScene(scene);});
	}
	public void SwapScene(string scene) {
		DeleteScene();
		LoadScene(scene);
	}
}
