using Godot;
using System;

public partial class Bullet : Sprite2D
{
    [Export] public float Speed = 500;
    public Vector2 MovementDirection = Vector2.Zero;
    public bool Invader = true;
    public override void _Process(double delta)
    {
        Position += MovementDirection * Speed * (float)delta;
        base._Process(delta);
        if (Position.Y > 960 || Position.Y < 0)
            QueueFree();
    }
    public void _on_area_2d_area_entered(Area2D area) {
        Node parent = area.GetParent();
        if (parent is Invader) {//disable friendly fire
            if (Invader) return;
        } else if (parent is Defender){
            if (!Invader) return;
        } else return;

        parent.QueueFree();//destroy subject
        QueueFree();//destroy bullet
    }
}
