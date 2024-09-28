using Godot;
using System;

public partial class Invader : Sprite2D
{
    //The speed at which the defender moves in pixels/second
    [Export] public float Speed = 50;
    [Export] public int SpriteSize = 64;
    public int Margin = 16;
    public Vector2 MovementDirection = Vector2.Right;

    public override void _Process(double delta)
    {
        if (Position.X > 900 && MovementDirection.X > 0) {
            MovementDirection = Vector2.Left;
            Position = new Vector2(900, Position.Y + SpriteSize + Margin);
        } else if (Position.X < 60 && MovementDirection.X < 0) {
            MovementDirection = Vector2.Right;
            Position = new Vector2(60, Position.Y + SpriteSize + Margin);
        }
        Position += new Vector2(MovementDirection.X * Speed * (float)delta, 0);
    }
}
