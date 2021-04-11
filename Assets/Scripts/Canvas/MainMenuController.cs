using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController menu;
    
    public Text title, desc, controls, scoreGame, scoreDeath;
    public GameObject blackBG, gameOver, lmb;

    public float waitBeforeCanPlay = 2f;
    private float _elapsed;

    private bool _hasPlayerDied, _waitingForInput = true;
    
    private void Awake()
    {
        if (!menu)
            menu = this;
        else
            Destroy(this);
    }

    private void Update()
    {
        if (!_waitingForInput) return;
        
        if(_elapsed <= waitBeforeCanPlay)
            _elapsed += Time.deltaTime;
        else
        {
            if (_hasPlayerDied)
            {
                lmb.SetActive(true);
                if (Input.GetButtonDown("Fire1"))
                {
                    RestartGame();
                }
            }
            else
            {
                lmb.SetActive(true);
                if (Input.GetButtonDown("Fire1"))
                {
                    HideDescription();
                    GameController.game.StartGameplay();
                    _waitingForInput = false;
                }
            }
        }
    }

    public void HideDescription()
    {
        blackBG.SetActive(false);
        desc.enabled = false;
        controls.enabled = true;
        lmb.SetActive(false);
    }

    public void UpdateScore()
    {
        if (GameController.score > 1)
            scoreGame.text = GameController.score + " Days of Summer";
    }

    public void ShowGameOver()
    {
        title.enabled = false;
        controls.enabled = false;
        scoreGame.enabled = false;
        blackBG.SetActive(true);
        gameOver.SetActive(true);
        if(GameController.score > 1)
            scoreDeath.text = "You lasted just " + GameController.score + " days of summer.";

        _elapsed = 0f;
        _hasPlayerDied = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
