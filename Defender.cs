using Godot;
using System;

public partial class Defender : Sprite2D
{
	[Export] string BulletScene = "res://bullet.tscn";
	//The speed at which the defender moves in pixels/second
	[Export] public float Speed = 200;
	PackedScene bulletPackedScene;
	CoolDown canShoot = new(1);
	Vector2 movementDirection = Vector2.Zero;

	public override void _Ready() {
		bulletPackedScene = GD.Load<PackedScene>(BulletScene);
	}

	public override void _Process(double delta) {
		Position += new Vector2(movementDirection.X * Speed * (float)delta, 0);
		Position = Position.X > 900 ? new Vector2(900, Position.Y) : Position;
		Position = Position.X < 60 ? new Vector2(60, Position.Y) : Position;
	}
 
	public override void _Input(InputEvent @event) {
		if (Input.IsActionJustPressed("shoot"))
			if (canShoot.Ready) //have to specify here, as cooldown.ready resets when it returns a true
				spawnBullet();

		movementDirection = Vector2.Zero; //I declared a move_up and move_down but currently serve no purpose
		if (Input.IsActionPressed("move_left"))
			movementDirection.X += -1;
		if (Input.IsActionPressed("move_right"))
			movementDirection.X += 1;
	}

	void spawnBullet() {
		Bullet b = bulletPackedScene.Instantiate<Bullet>();
		AddSibling(b);
		b.GlobalPosition = GlobalPosition + Vector2.Up*32;
		b.MovementDirection = Vector2.Up;
		b.Invader = false;
	}
}
