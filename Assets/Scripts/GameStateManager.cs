using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameStateManager : MonoBehaviour
{
    public CanvasGroup gameOverCanvas;
    public Text gameOverText;

    // Start is called before the first frame update
    void Start()
    {
        gameOverCanvas.alpha = 0;

        Messenger.AddListener<PlayerController>(Events.OnGameOver, GameOver);
    }

    void GameOver(PlayerController controller)
    {
        var playerName = "Dimitri";

        if (controller.playerNumber == 2)
            playerName = "Viktor";

        gameOverText.text = $"{playerName} Wins!";
        gameOverCanvas.DOFade(1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        Messenger.RemoveListener<PlayerController>(Events.OnGameOver, GameOver);
    }
}
