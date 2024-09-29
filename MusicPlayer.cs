using Godot;
using System;

public partial class MusicPlayer : AudioStreamPlayer2D
{
	public override void _Ready()
	{
		var bgMusic = GD.Load<AudioStream>("res://assets/sounds/mus_background.mp3");
		Stream = bgMusic;
		Play();
	}

	public override void _Process(double delta)
	{
	}
}
