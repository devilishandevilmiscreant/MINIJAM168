using Godot;
using System;

public partial class Bullet : Sprite2D
{
	[Export] public float Speed = 500;
	[Export] public int Interval = 16;//960/2^3/8
	public Vector2 MovementDirection = Vector2.Zero;
	public Vector2 DesiredPosition;
	public bool Invader = true;

	// Audio players for laser and explosion sounds
	[Export] private AudioStreamPlayer2D laserSoundPlayer;
	[Export] private AudioStreamPlayer2D explosionSoundPlayer;

	public override void _Ready()
	{
		DesiredPosition = Position;
		// Play the laser sound when the bullet is spawned
		if (laserSoundPlayer != null)
		{
			laserSoundPlayer.Play();
		}
	}

	public override void _Process(double delta)
	{
		DesiredPosition += MovementDirection * Speed * (float)delta;
		base._Process(delta);
		if (DesiredPosition.Y > 960 || DesiredPosition.Y < 0)
			QueueFree();

		Position = new Vector2(
			Mathf.Round(DesiredPosition.X / Interval),
			Mathf.Round(DesiredPosition.Y / Interval)
		);
		Position *= Interval;
	}

	public void _on_area_2d_area_entered(Area2D area)
	{
		Node parent = area.GetParent();

		// Disable friendly fire
		if (parent is Invader && Invader) return;
		if (parent is Defender && !Invader) return;
		if (!(parent is Invader || parent is Defender)) return;

		// Destroy both the bullet and the collided object
		parent.QueueFree();

		// Play explosion sound before destroying the bullet
		if (explosionSoundPlayer != null)
		{
			// Duplicate the explosion sound player to allow it to continue playing after the bullet is destroyed
			var detachedExplosionSound = (AudioStreamPlayer2D)explosionSoundPlayer.Duplicate();
			detachedExplosionSound.Stream = explosionSoundPlayer.Stream;
			detachedExplosionSound.GlobalPosition = GlobalPosition; // Position the sound at the bullet's location

			// Add the detached sound to the parent scene
			GetParent().AddChild(detachedExplosionSound);
			detachedExplosionSound.Play();

			// Connect the finished signal to queue_free (use a callable instead of a string)
			detachedExplosionSound.Connect("finished", new Callable(detachedExplosionSound, "queue_free"));
		}

		// Destroy the bullet immediately after
		QueueFree();
	}
}
