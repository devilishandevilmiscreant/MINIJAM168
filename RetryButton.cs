using Godot;
using System;

public partial class RetryButton : Button
{
    public void _on_pressed() {
        GameManager.Instance.Start();
    }
}
