using Godot;
using System;

public class HUD : CanvasLayer
{
    [Signal]
    public delegate void StartGame();

    public void ShowMessage(String text)
    {
        var Message = GetNode<Label>("MessageLabel");
        Message.Text = text;
        Message.Show();
        
        GetNode<Timer>("MessageTimer").Start();
    }

    /* Launch a separate thread to keep track of whether the MessageTimer reaches 0 or not,
       thus each message is shown for the expected period of time. */
    async public void ShowGameOver()
    {
        ShowMessage("Game Over");

        /* This statement kinda says: Don't proceed until the MessageTimer reaches 0.
           So it basically waits for the "timeout" signal be emitted from the specified timer. */
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
