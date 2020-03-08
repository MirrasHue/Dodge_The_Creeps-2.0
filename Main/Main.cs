using Godot;
using System;

public class Main : Node2D
{
    [Export] PackedScene Critter;

    [Export] int CritterMinVel = 250;
    [Export] int CritterMaxVel = 350;

    private int Score;

    private Random RandGenerator = new Random();

    public float FRandRange(float min, float max)
    {
        return (float) RandGenerator.NextDouble() * (max - min) + min;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var Music = GetNode<AudioStreamPlayer>("MainMusic");
        Music.VolumeDb = -25f;
        Music.Play();

        GetNode<AudioStreamPlayer>("DeathSound").VolumeDb = -20f;
    }

    public void NewGame()
    {
        Score = 0;

        var Hud = GetNode<HUD>("HUD");
        Hud.UpdateScore(Score);
        Hud.ShowMessage("Get Ready");

        Vector2 SpawnPos = GetNode<Position2D>("Position").Position;
        GetNode<Player>("Player").BeginPlay(SpawnPos);

        GetNode<Timer>("StartTimer").Start();
    }

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
        Score++;
        GetNode<HUD>("HUD").UpdateScore(Score);
    }

    public void OnCritterTimerTimeout()
    {
        var SpawnLocation = GetNode<PathFollow2D>("Path2D/PathFollow2D");
        SpawnLocation.Offset = RandGenerator.Next();

        var CritterInstance = Critter.Instance() as RigidBody2D;
        AddChild(CritterInstance);

        float Rotation = SpawnLocation.Rotation + Mathf.Pi / 2;

        CritterInstance.Position = SpawnLocation.Position;

        Rotation += FRandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        CritterInstance.Rotation = Rotation;

        CritterInstance.LinearVelocity = new Vector2(FRandRange(CritterMinVel, CritterMaxVel), 0).Rotated(Rotation);

        GetNode<HUD>("HUD").Connect("StartGame", CritterInstance, "OnStartGame");
    }
}
