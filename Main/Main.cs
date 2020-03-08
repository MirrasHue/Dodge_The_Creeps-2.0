using Godot;
using System;

public class Main : Node2D
{
    /* Allow to choose the Critter scene in order to instance it from code.
       It is useful in this scenario, where a bunch of creeps will be randomly spawned at runtime.
       Need to be set(Critter.tscn) in the editor, otherwise it will not work. */
    [Export] PackedScene Critter;

    // Pixels / second
    [Export] int CritterMinSpeed = 250;
    [Export] int CritterMaxSpeed = 350;

    private int Score;

    private Random RandGenerator = new Random();

    // Returns a random number in a range [min : max)
    public float FRandRange(float min, float max)
    {
        return (float) RandGenerator.NextDouble() * (max - min) + min;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Set the sounds' volume and play the main music
        var Music = GetNode<AudioStreamPlayer>("MainMusic");
        Music.VolumeDb = -25f;
        Music.Play();

        GetNode<AudioStreamPlayer>("DeathSound").VolumeDb = -20f;

        /* Spawn the player for the first time at the middle of screen and "hide" it.
           Next respawns will be handled when the player collide with an enemy,
           due to a glitch that was happening. */
        Vector2 SpawnPos = GetNode<Position2D>("Position").Position;
        var Player = GetNode<Player>("Player");
        Player.Position = SpawnPos;
        Player.ZIndex = -1;
    }

    // Triggered when StartGame signal is emitted from HUD, due to StartButton being pressed.
    public void NewGame()
    {
        Score = 0;

        var Hud = GetNode<HUD>("HUD");
        Hud.UpdateScore(Score);
        Hud.ShowMessage("Get Ready");

        GetNode<Player>("Player").BeginPlay();

        GetNode<Timer>("StartTimer").Start();
    }

    // Triggered when a Dead signal is sent, i.e. when the player collides.
    public void GameOver()
    {
        GetNode<HUD>("HUD").ShowGameOver();
        GetNode<AudioStreamPlayer>("DeathSound").Play();

        GetNode<Timer>("ScoreTimer").Stop();
        GetNode<Timer>("CritterTimer").Stop();
    }

    public void OnStartTimerTimeout()
    {
        GetNode<Timer>("ScoreTimer").Start();
        GetNode<Timer>("CritterTimer").Start();
    }

    public void OnScoreTimerTimeout()
    {
        GetNode<HUD>("HUD").UpdateScore(++Score);
    }

    public void OnCritterTimerTimeout()
    {
        // Create a Critter instance and add it to the scene.
        var CritterInstance = Critter.Instance() as RigidBody2D;
        AddChild(CritterInstance);

        // Choose a random location along the Path2D.
        var SpawnLocation = GetNode<PathFollow2D>("Path2D/SpawnLocation");
        SpawnLocation.Offset = RandGenerator.Next();

        // Set the Critter's position to the randomized location.
        CritterInstance.Position = SpawnLocation.Position;

        // This rotation will be perpendicular to the SpawnLocation's rotation, so it will always point inside the viewport.
        float Rotation = SpawnLocation.Rotation + Mathf.Pi / 2;

        // Now that the rotation faces the viewport, we add to it a random angle to the right (-45° : 0) or left (0 : 45°] 
        Rotation += FRandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        CritterInstance.Rotation = Rotation;

        // Set a random speed in the X axis from which the critter is rotated to.
        CritterInstance.LinearVelocity = new Vector2(FRandRange(CritterMinSpeed, CritterMaxSpeed), 0).Rotated(Rotation);

        // If there are still any Critter when the game is started, this will get rid of them
        GetNode<HUD>("HUD").Connect("StartGame", CritterInstance, "OnStartGame");
    }
}
