using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// [Tool]
public partial class InvaderPopulater : Node2D
{
	public static InvaderPopulater Instance;
	[Export] string InvaderScene = "res://invader.tscn";
	[Export] int NumberOfInvaders = 44;
	[Export] int Margin = 16;
	[Export] Vector2 Offset = new(60, 60);//40 pix gap on either side
	public List<Invader> Invaders = new();

	public override void _Ready() {
		Instance = this;

		var invaderPackedScene = GD.Load<PackedScene>(InvaderScene);
		int invaderSize = 0;        
		int rows = 0;
		int widthOfRow = 0;

		for (int i = 0; i < NumberOfInvaders; i++) {
			if (i != 0 && i % widthOfRow == 0 && rows % 2 == 1) {
				GD.Print("Invador #" + i+ " omited");
				continue; //There are gaps that are filled during the first step that we have to avoid
			}
			
			Invader inv = invaderPackedScene.Instantiate<Invader>();
			AddChild(inv);
			Invaders.Add(inv);
			
			if (i == 0) {
				invaderSize = inv.SpriteSize; //set sprite size on the first invader
				widthOfRow = (960 - 2 * Margin) / (invaderSize + Margin);
			} 
			
			if (Offset.X - inv.MovementDirection.X*20f/(Margin/16f) + (i - widthOfRow * rows) * (invaderSize + Margin) > 900)
				rows++;

			inv.MovementDirection.X = 1 - (rows%2) * 2;//even rows = 1, odd rows = -1
			inv.DesiredPosition = new Vector2(Offset.X - inv.MovementDirection.X*20f/(Margin/16f) + (i - widthOfRow * rows) * (invaderSize + Margin), Offset.Y + rows * (invaderSize + Margin));
			inv.Margin = Margin;
			if (inv.CanShoot.Ready);//call ready to throw it away
		}
	}
	public void Remove(Invader inv) {
		Invaders.Remove(inv);
		if (Invaders.Count == 0)
			GameManager.Instance.Lose("You let the invasion fail.");
	}
}
