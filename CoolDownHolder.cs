using Godot;
using System;

public partial class CoolDownHolder : Node
{
	public static CoolDownHolder Instance;
	public override void _EnterTree() {
		Instance = this;
	}
}
