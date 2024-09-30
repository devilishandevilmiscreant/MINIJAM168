using Godot;
using System;

public partial class Invader : Sprite2D
{
	[Export] string BulletScene = "res://bullet.tscn";
	//The speed at which the defender moves in pixels/second
	[Export] public float Speed = 50;
	//The minimum distance this sprite can move
	[Export] public int Interval = 16;//960/2^3/8
	[Export] public float Chance = 0.125f;
	[Export] public int SpriteSize = 64;
	public CoolDown CanShoot;
	public int Margin = 16;
	public Vector2 MovementDirection = Vector2.Right;
	public Vector2 DesiredPosition;
	PackedScene bulletPackedScene;
	RandomNumberGenerator rng = new();

	public override void _Ready() {
		CanShoot = new(6 * (0.5f + rng.Randf()/2f));
		DesiredPosition = Position;
		bulletPackedScene = GD.Load<PackedScene>(BulletScene);
	}

	public override void _Process(double delta)
	{
		if (!GameManager.Instance.TimePasses) return;

		if (CanShoot.Ready) { //every three seconds try to shoot
			if (rng.Randf() <= Chance)
				spawnBullet();
		}

		if (DesiredPosition.X > 900 && MovementDirection.X > 0) {
			MovementDirection = Vector2.Left;
			DesiredPosition = new Vector2(900, DesiredPosition.Y + SpriteSize + Margin);
		} else if (DesiredPosition.X < 60 && MovementDirection.X < 0) {
			MovementDirection = Vector2.Right;
			DesiredPosition = new Vector2(60, DesiredPosition.Y + SpriteSize + Margin);
		}
		DesiredPosition += new Vector2(MovementDirection.X * Speed * (float)delta, 0);

		Position = new Vector2(
			Mathf.Round(DesiredPosition.X / Interval),
			Mathf.Round(DesiredPosition.Y / Interval)
		);
		Position *= Interval;
	}

	void spawnBullet() {
		Bullet b = bulletPackedScene.Instantiate<Bullet>();
		AddSibling(b);
		b.DesiredPosition = GlobalPosition + Vector2.Down*SpriteSize;
		b.MovementDirection = Vector2.Down;
	}
}
