using Godot;
using System;

public partial class Bunker : Sprite2D
{
	private const int VOXEL_SIZE = 32;//128/4
	private int[] voxelGrid = {//stored as 1d array so we can paste it into the shader //glsl doesn't support 2d arrays
		1, 1, 1, 1,
		1, 1, 1, 1,
		1, 1, 1, 1,
		1, 1, 1, 1
	};
	
	[Export] private AudioStreamPlayer2D impactSoundPlayer;

	public void _on_area_2d_area_entered(Area2D area) {
		if (area.GetParent() is Bullet b) {
			Vector2 impactPoint = area.GlobalPosition;
			impactPoint -= GlobalPosition - Vector2.One*64;
			int gridX = (int)(impactPoint.X / VOXEL_SIZE);
			int gridY = (int)(impactPoint.Y / VOXEL_SIZE);
			gridX = Mathf.Clamp(gridX, 0, 3);
			gridY = Mathf.Clamp(gridY, 0, 3);

			while (gridY <= 3 && gridY >= 0) {
				if (voxelGrid[gridX + gridY*4] == 1) {
					voxelGrid[gridX + gridY*4] = 0;  // Mark voxel as destroyed
					UpdateBunkerSprite();
					
					if (impactSoundPlayer != null)
					{
						impactSoundPlayer.Play();
					}
					
					area.GetParent().QueueFree();  // Remove the bullet
					break;
				} else {
					gridY += (int)b.MovementDirection.Y;
				}
			}

		}
	}
	void UpdateBunkerSprite() {
		((ShaderMaterial)Material).SetShaderParameter("text", voxelGrid);
	}
}
