using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameStateManager : MonoBehaviour
{
    public CanvasGroup gameOverCanvas;
    public Text gameOverText;
    public PlayerController player1;
    public PlayerController player2;

    // Start is called before the first frame update
    void Start()
    {
        gameOverCanvas.alpha = 0;

        Messenger.AddListener<PlayerController>(Events.OnGameOver, GameOver);
        Messenger.AddListener<Timer>(Events.OnTimeout, Timeout);
    }

    void GameOver(PlayerController controller)
    {
        var playerName = "Dimitri";

        if (controller.playerNumber == 2)
            playerName = "Viktor";

        gameOverText.text = $"{playerName} Wins!";
        gameOverCanvas.DOFade(1f, 1f);
    }

    void Draw(PlayerController controller)
    {
        gameOverText.text = "Draw!";
        gameOverCanvas.DOFade(1f, 1f);
    }

    void Timeout(Timer timer)
    {
        PlayerController winner;
        PlayerController loser;

        var health1 = player1.HealthPercentage();
        var health2 = player2.HealthPercentage();

        if (Math.Abs(health1 - health2) < .1)
        {
            // Draw
            Messenger.Broadcast<PlayerController>(Events.OnDraw, player1);
            Draw(player1);
            return;
        }

        if (health1 > health2)
        {
            winner = player1;
            loser = player2;
        } else
        {
            winner = player2;
            loser = player1;
        }

        Messenger.Broadcast<PlayerController>(Events.OnGameOver, winner);
        Messenger.Broadcast<PlayerController>(Events.OnStunned, loser);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener<PlayerController>(Events.OnGameOver, GameOver);
        Messenger.RemoveListener<Timer>(Events.OnTimeout, Timeout);
    }
}
