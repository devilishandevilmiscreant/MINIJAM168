using Godot;
using System;

public partial class MusicPlayer : AudioStreamPlayer2D
{
	private float pitchIncreaseAmount = .1f;  // How much to increase the pitch each loop

	public override void _Ready()
	{
		var bgMusic = GD.Load<AudioStream>("res://assets/sounds/mus_background.mp3");
		Stream = bgMusic;

		// Play the music when the scene starts
		Play();

		// Connect the "finished" signal using a Callable object
		Connect("finished", new Callable(this, nameof(OnMusicFinished)));
	}

	// Method to handle what happens when the music finishes playing
	private void OnMusicFinished()
	{
		// Increase the pitch scale to speed up the track
		PitchScale += pitchIncreaseAmount;

		// Restart the music
		Play();
	}

	public override void _Process(double delta)
	{
	}
}
