using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    Board Board;
    private void StartGame()
    {
        //Spawn new piece when game starts
        Board.SpawnPiece();

    }
}
