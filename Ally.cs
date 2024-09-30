using Godot;
using System;

public partial class Ally : Sprite2D //I remeber godot having weird inheritance so I've avoided inhereting a root from defender
{
	[Export] string BulletScene = "res://bullet.tscn";
	//The speed at which the defender moves in pixels/second
	[Export] public float Speed = 200;
	[Export] public int Interval = 16;//960/2^3/8
    [Export] Bunker[] bunkers;
	PackedScene bulletPackedScene;
	CoolDown canShoot = new(1);
    CoolDown canThink = new(0.25f);//only think one fourth of the time //just like me fr
	Vector2 movementDirection = Vector2.Right;
	Vector2 desiredPosition;

	public override void _Ready() {
		desiredPosition = Position;
		bulletPackedScene = GD.Load<PackedScene>(BulletScene);
	}

	public override void _Process(double delta) {
        if (!GameManager.Instance.TimePasses) return;

        if (canThink.Ready) {//AI
            int closestBunkerX = -960;//get closest bunker
            for (int i = 0; i < bunkers.Length; i++) {
                if (!bunkers[i].Defendable()) continue;

                if (Mathf.Abs(Position.X - bunkers[i].Position.X) < Mathf.Abs(Position.X - closestBunkerX))
                    closestBunkerX = (int)bunkers[i].Position.X;
            }

            movementDirection.X = closestBunkerX - Position.X;//move to closest bunker
            movementDirection.X = Mathf.Clamp(movementDirection.X, -1, 1);

            //if can shoot move away from closest bunker
            if (canShoot.Peak()) {//can shoot but does not start timer
                movementDirection.X = -movementDirection.X;
                if (desiredPosition.X > 900 - 64 && movementDirection.X > 0)//if edge bunkers
                    movementDirection.X = -movementDirection.X;
                if (desiredPosition.X < 60 + 64 && movementDirection.X < 0)
                    movementDirection.X = -movementDirection.X;

                if (Mathf.Abs(Position.X - closestBunkerX) > 64)
                    if (canShoot.Ready)//we already known it is ready but we need to start the timer
                        spawnBullet();
            }
        }

        //locomotion
		if (desiredPosition.X > 900 && movementDirection.X > 0) {
			movementDirection = Vector2.Left;
			desiredPosition = new Vector2(900, desiredPosition.Y);
		} else if (desiredPosition.X < 60 && movementDirection.X < 0) {
			movementDirection = Vector2.Right;
			desiredPosition = new Vector2(60, desiredPosition.Y);
		}
        desiredPosition += new Vector2(movementDirection.X * Speed * (float)delta, 0);

		Position = new Vector2(
			Mathf.Round(desiredPosition.X / Interval),
			Mathf.Round(desiredPosition.Y / Interval)
		);
		Position *= Interval;
	}
 
	void spawnBullet() {
		Bullet b = bulletPackedScene.Instantiate<Bullet>();
		AddSibling(b);
		b.DesiredPosition = GlobalPosition + Vector2.Up*32;
		b.MovementDirection = Vector2.Up;
		b.Invader = false;
	}
}
