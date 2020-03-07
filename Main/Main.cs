using Godot;
using System;

public class Main : Node2D
{
    private int Score;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
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

    }
}
