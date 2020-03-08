using Godot;
using System;

public class Player : Area2D
{
    [Signal] public delegate void Dead();

    [Export] public int Speed = 250; // Pixels / second

    private bool bIsPlaying = false;

    private Vector2 SCREEN_SIZE;

    private Vector2 Velocity = new Vector2();

    private AnimatedSprite AnimSprite = new AnimatedSprite();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Some optimizations, we don't need to get input or play animations while the game doesn't properly start.
        SCREEN_SIZE = GetViewport().Size;
        GetParent().GetNode<HUD>("HUD").Connect("StartGame", this, "IsPlaying");
        Connect("Dead", this, "IsNotPlaying");
    }

    public void IsPlaying()
    {
        bIsPlaying = true;
    }

    public void IsNotPlaying()
    {
        bIsPlaying = false;
        AnimSprite.Stop();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if(bIsPlaying)
        {
            // Manage the animations accordingly to player's velocity.
            AnimSprite = GetNode<AnimatedSprite>("AnimatedSprite");

            if(Velocity.x != 0)
            {
                AnimSprite.Animation = "Right";
                AnimSprite.FlipH = Velocity.x < 0;
                AnimSprite.FlipV = false;
            }
            else
            if(Velocity.y != 0)
            {
                AnimSprite.Animation = "Up";
                AnimSprite.FlipV = Velocity.y > 0;
            }
            else
                AnimSprite.Animation = "Idle";

            AnimSprite.Play();
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if(bIsPlaying)
        {
            // Get user's input.
            Velocity.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
            Velocity.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");

            // Set player's position.
            Position += Velocity.Normalized() * Speed * delta;
            
            // Allow the player pass from one side to another.
            if(Position.x > SCREEN_SIZE.x)
                Position = new Vector2(0, Position.y);
            else
            if(Position.x < 0)
                Position = new Vector2(SCREEN_SIZE.x, Position.y);

            if(Position.y > SCREEN_SIZE.y)
                Position = new Vector2(Position.x, 0);
            else
            if(Position.y < 0)
                Position = new Vector2(Position.x, SCREEN_SIZE.y);
        }
    }

    public void BeginPlay()
    {
        AnimSprite.FlipV = false;
        GetNode<CollisionShape2D>("Collision").Disabled = false;
        ZIndex = 1; // Part of the trail glitch fix.
    }

    public void OnCollisionDetected(PhysicsBody2D body2D)
    {
        /* Bug fixed, the trail's particles no longer appear on the last place the player had died,
           whenever the game is restarted.
           This was the only solution that I came up with, I hoped there was another one more cleaner. */
        var Location = GetParent().GetNode<Position2D>("Position").Position;
        Position = Location;
        ZIndex = -1;

        EmitSignal("Dead");
        GetNode<CollisionShape2D>("Collision").SetDeferred("disabled", true);
    }
}
