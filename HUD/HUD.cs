using Godot;
using System;

public class HUD : CanvasLayer
{
    [Signal]
    public delegate void StartGame();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void ShowMessage(String text)
    {
        var Message = GetNode<Label>("MessageLabel");
        Message.Text = text;
        Message.Show();
        
        GetNode<Timer>("MessageTimer").Start();
    }

    async public void ShowGameOver()
    {
        ShowMessage("Game Over");

        await ToSignal(GetNode<Timer>("MessageTimer"), "timeout");

        var Message = GetNode<Label>("MessageLabel");
        Message.Text = "Dodge\nthe\nCreeps";
        Message.Show();

        GetNode<Button>("StartButton").Show();
    }

    public void OnStartButtonPressed()
    {
        GetNode<Button>("StartButton").Hide();
        EmitSignal("StartGame");
    }

    public void UpdateScore(int score)
    {
        GetNode<Label>("ScoreLabel").Text = score.ToString();
    }

    public void OnMessageTimerTimeout()
    {
        GetNode<Label>("MessageLabel").Hide();
    }
}
