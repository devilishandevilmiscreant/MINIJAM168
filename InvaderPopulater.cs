using Godot;
using System;
using System.Collections.Generic;

public partial class InvaderPopulater : Node2D
{
	public static InvaderPopulater Instance;
	[Export] string InvaderScene = "res://invader.tscn";
	[Export] int NumberOfInvaders = 44;
	[Export] int Margin = 16;
	[Export] Vector2 Offset = new(60, 60); // Starting position for invaders
	public List<Invader> Invaders = new();

	private PackedScene invaderPackedScene;
	private int invaderSize = 0;
	private int rows = 0;
	private int widthOfRow = 0;
	private HashSet<Vector2> occupiedPositions = new(); // Track occupied positions

	public override void _Ready()
	{
		Instance = this;
		invaderPackedScene = GD.Load<PackedScene>(InvaderScene);
		SpawnInvaders();
	}

	// Method to spawn all invaders initially
	public void SpawnInvaders()
	{
		for (int i = 0; i < NumberOfInvaders; i++)
		{
			if (i != 0 && i % widthOfRow == 0 && rows % 2 == 1)
			{
				GD.Print("Invader #" + i + " omitted");
				continue; // Avoid gaps that are filled during the first step
			}

			SpawnSingleInvader(i);
		}
	}

	// Method to spawn a single invader
	public void SpawnSingleInvader(int i)
	{
		Invader inv = invaderPackedScene.Instantiate<Invader>();
		AddChild(inv);
		Invaders.Add(inv);

		if (i == 0)
		{
			invaderSize = inv.SpriteSize; // Set sprite size on the first invader
			widthOfRow = (960 - 2 * Margin) / (invaderSize + Margin);
		}

		if (Offset.X - inv.MovementDirection.X * 20f / (Margin / 16f) + (i - widthOfRow * rows) * (invaderSize + Margin) > 900)
			rows++;

		inv.MovementDirection.X = 1 - (rows % 2) * 2; // Even rows = 1, odd rows = -1
		inv.DesiredPosition = new Vector2(
			Offset.X - inv.MovementDirection.X * 20f / (Margin / 16f) + (i - widthOfRow * rows) * (invaderSize + Margin),
			Offset.Y + rows * (invaderSize + Margin)
		);
		inv.Margin = Margin;

		// Add the position to the set of occupied positions
		occupiedPositions.Add(inv.DesiredPosition);

		inv.CanShoot.Throw(); //Stop the initial barrage as they all spawn in
	}

	public void Remove(Invader inv)
	{
		Invaders.Remove(inv);
		if (Invaders.Count == 0)
			GameManager.Instance.Lose("You let the invasion fail.");

		// Remove the invader's position from the set of occupied positions
		occupiedPositions.Remove(inv.DesiredPosition);
	}

	// Listen for input
	public override void _Input(InputEvent @event)
	{
		// Check if the x key is pressed
		if (Input.IsActionJustPressed("ui_invasion"))
		{
			SpawnNewInvader();
		}
	}

	// Method to spawn a new invader when the button is pressed
	public void SpawnNewInvader()
	{
		Invader inv = invaderPackedScene.Instantiate<Invader>();
		AddChild(inv);
		Invaders.Add(inv);

		// Find a random, empty spot in the top half of the screen
		Vector2 newPosition = GetRandomEmptyPosition();
		inv.DesiredPosition = newPosition;
		inv.MovementDirection.X = GD.RandRange(0, 2) > 1 ? -1 : 1; // Randomize left or right movement
		inv.Margin = Margin;

		// Add the position to the set of occupied positions
		occupiedPositions.Add(newPosition);
	}

	// Function to find a random empty position in the top half of the screen
	private Vector2 GetRandomEmptyPosition()
	{
		float screenWidth = 960; 
		float screenHeight = 540; 
		float x, y;
		Vector2 position;

		// Keep generating random positions until we find an empty spot
		do
		{
			x = (float)GD.RandRange(Margin, screenWidth - Margin);
			y = (float)GD.RandRange(Margin, screenHeight - Margin);
			position = new Vector2(x, y);
		} while (occupiedPositions.Contains(position));

		return position;
	}
}
