using Godot;
using System;

public class Player : Area2D
{
    [Signal]
    public delegate void Dead();

    [Export]
    public int Speed = 250;

    private bool bIsPlaying = true;

    private Vector2 SCREEN_SIZE;

    private Vector2 Velocity = new Vector2();

    private AnimatedSprite AnimSprite = new AnimatedSprite();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SCREEN_SIZE = GetViewport().Size;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
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

    public override void _PhysicsProcess(float delta)
    {
        if(bIsPlaying)
        {
            // Get user's input
            Velocity.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
            Velocity.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");

            // Set player's position
            Position += Velocity.Normalized() * Speed * delta;
            // Prevent the player going off-screen
            Position = new Vector2(Mathf.Clamp(Position.x, 0, SCREEN_SIZE.x),
                                   Mathf.Clamp(Position.y, 0, SCREEN_SIZE.y));
        }
    }

    public void OnCollisionDetected(PhysicsBody2D body2D)
    {
        Hide();
        EmitSignal("Dead");
        GetNode<CollisionShape2D>("Collision").SetDeferred("disabled", true);
    }
}
