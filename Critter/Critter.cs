using Godot;
using System;

public class Critter : RigidBody2D
{
    private String[] EnemyTypes = {"Fly", "Spikey", "Spikey_Orb"};

    private AnimatedSprite AnimSprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Select random enemies and play their animation.
        AnimSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        AnimSprite.Animation = EnemyTypes[new Random().Next(0, EnemyTypes.Length)];
        AnimSprite.Play();
    }

    /* Only enable enemy collision when it enters the screen.
       This might work better with the new mechanic(player flip sides),
       at least reducing the possibilities of the player being hit at the borders
       when it's passing through them. */
    public void OnScreenEntered()
    {
        GetNode<CollisionShape2D>("Collision").Disabled = false;
    }

    // Receiver method which is called when the critter leaves the screen, so we can delete its instance.
    public void OnScreenExited()
    {
        QueueFree();
    }

    // For when we restart the game right after a game over and there are still creeps on the screen.
    public void OnStartGame()
    {
        QueueFree();
    }
}
