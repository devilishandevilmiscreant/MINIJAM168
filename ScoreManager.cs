using Godot;
using System;

public partial class ScoreManager : Control
{
	private int points = 0;  // Variable to store the running points
	private Label pointsLabel;  // Reference to the label that displays the points

	public override void _Ready()
	{
		// Get the PointsLabel node (adjust the path if necessary)
		pointsLabel = GetNode<Label>("PointsLabel");

		// Initialize the score display
		UpdateScoreDisplay();
	}

	// Method to add points and update the display
	public void AddPoints(int value)
	{
		points += value;  // Add points to the running total
		UpdateScoreDisplay();  // Update the label to reflect the new total
	}

	// Method to update the score label
	private void UpdateScoreDisplay()
	{
		if (pointsLabel != null)
		{
			pointsLabel.Text = $"{points}";
		}
	}
}
