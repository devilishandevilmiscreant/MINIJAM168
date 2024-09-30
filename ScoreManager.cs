using Godot;
using System;

public partial class ScoreManager : Control
{
	private int points = 0;  // Variable to store the running points
	private Label pointsLabel;  // Reference to the label that displays the points
	private Label subtitleLabel;  // Reference to the Subtitle label

	public override void _Ready()
	{
		// Get the PointsLabel node (adjust the path if necessary)
		pointsLabel = GetNode<Label>("PointsLabel");

		// Get the SubtitleLabel node (adjust the path if necessary)
		subtitleLabel = GetNode<Label>("Subtitle");

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

		// Check if points exceed 100 and update the subtitle
		if (subtitleLabel != null && points > 100)
		{
			subtitleLabel.Text = "Press X";
		}
	}
}
