using UnityEngine.Tilemaps;
using UnityEngine;

//using letters as the names of the shapes in tetris, commonly known as the tetromino
public enum Tetromino
{
    I,
    O,
    T,
    J,
    L,
    S,
    Z
}

//store data for each & add a custom attribute so it can be displayed in the editor
[System.Serializable]
public struct TetrominoData
{
    //declare variable and select while tile to draw for selected tetromino
    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] cells;
}
