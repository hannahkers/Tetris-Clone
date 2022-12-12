using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private int score;
    //public TextMeshPro scoreText;
    public TMPro.TextMeshProUGUI scoreText;



    public void IncreaseScore()
    {
        score++;
        scoreText.text = ("Score: " + score);
    }

    public void GameOver()
    {
        //this.tilemap.ClearAllTiles();

        //UI game over

        SceneManager.LoadScene("Game Over");
    }

}
