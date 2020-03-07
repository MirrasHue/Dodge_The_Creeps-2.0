using Godot;
using System;

public class Critter : RigidBody2D
{
    private String[] EnemyTypes = {"Fly", "Spikey", "Spikey_Orb"};

    private AnimatedSprite AnimSprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AnimSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        AnimSprite.Animation = EnemyTypes[new Random().Next(0, EnemyTypes.Length)];
        AnimSprite.Play();
    }

    public void OnNotVisible()
    {
        QueueFree();
    }

    public void OnStartGame()
    {
        QueueFree();
    }
}
