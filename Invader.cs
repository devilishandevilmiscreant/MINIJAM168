using Godot;
using System;

public partial class Invader : Sprite2D
{
	[Export] string BulletScene = "res://bullet.tscn";
	//The speed at which the defender moves in pixels/second
	[Export] public float Speed = 50;
	[Export] public float Chance = 0.125f;
	[Export] public int SpriteSize = 64;
	public int Margin = 16;
	public Vector2 MovementDirection = Vector2.Right;
	PackedScene bulletPackedScene;
	CoolDown canShoot = new(6);
	RandomNumberGenerator rng = new();

	public override void _Ready() {
		bulletPackedScene = GD.Load<PackedScene>(BulletScene);
	}

	public override void _Process(double delta)
	{
		if (canShoot.Ready) { //every three seconds try to shoot
			if (rng.Randf() <= Chance)
				spawnBullet();
		}

		if (Position.X > 900 && MovementDirection.X > 0) {
			MovementDirection = Vector2.Left;
			Position = new Vector2(900, Position.Y + SpriteSize + Margin);
		} else if (Position.X < 60 && MovementDirection.X < 0) {
			MovementDirection = Vector2.Right;
			Position = new Vector2(60, Position.Y + SpriteSize + Margin);
		}
		Position += new Vector2(MovementDirection.X * Speed * (float)delta, 0);
	}

	void spawnBullet() {
		Bullet b = bulletPackedScene.Instantiate<Bullet>();
		AddSibling(b);
		b.GlobalPosition = GlobalPosition + Vector2.Down*SpriteSize;
		b.MovementDirection = Vector2.Down;
	}
}
