using Godot;
using System;

public class CoolDown
{
    public CoolDown() {
        this.time = 1f;
    }    
    public CoolDown(float time) {
        this.time = time;
    }

    float time = 1f;
    Timer timer = null;
    bool _ready = true;
    public bool Ready {
        get {
            if (_ready) {
                _ready = false;
                _countDown();
                return true;
            }
            return false;
        }
    }   
    void _countDown() {
        if (timer == null) {
            timer = new() {
                OneShot = true,
                Autostart = true,
                WaitTime = time
            };
            timer.Timeout += () => _ready = true;
            CoolDownHolder.Instance.AddChild(timer);
        }
        timer.Start();
    }
}
