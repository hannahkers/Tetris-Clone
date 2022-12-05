using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    //define and array of tetromino data 
    public TetrominoData[] tetrominoes;

    //references
    public Tilemap tilemap {get; private set;}
    public Piece activePiece { get; private set;}
    public Vector3Int spawnPos;
    public Vector2Int boardSize = new Vector2Int(10,20); //standard tetris board size

    public RectInt Bounds //needs position and size
    {
        get
        {
            //postition of the bounds needs to be the corner of the board
            Vector2Int position = new Vector2Int(-this.boardSize.x/2, -this.boardSize.y /2);
            return new RectInt(position, this.boardSize);
        }
    }


    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        //loop through and call the initialize data
        for (int i = 0; i < this.tetrominoes.Length; i++)
        {
            this.tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        //Spawn new piece when game starts
        SpawnPiece();

    }

    public void SpawnPiece()
    {
        //pick random element from tetrominoes
        int random = Random.Range(0,this.tetrominoes.Length);
        TetrominoData data = this.tetrominoes[random];

        //initialize piece with data
        this.activePiece.Initialize(this, this.spawnPos, data);

        //only set if valid
        if (IsPositionValid(this.activePiece, this.spawnPos))
        {
            //set piece
            Set(this.activePiece);
        }
        else
        {
            GameOver();
        }
        
    }

    private void GameOver()
    {
        this.tilemap.ClearAllTiles();

        //UI game over
    }

    public void Set(Piece piece)
    {
        //loop through every cell on the piece to set onto tilemap
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePos, piece.data.tile);
        }
    }

    //clear piece
    public void Clear(Piece piece)
    {
        //loop through every cell on the piece to set onto tilemap
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePos, null);
        }
    }

    //check if the position of the piece is valid and can be placed
    public bool IsPositionValid(Piece piece, Vector3Int position)
    {
        //is it out of bounds or is there another tile occupying the space?

        RectInt bounds = this.Bounds;
        
        for (int i = 0; i < piece.cells.Length; i++)
        {
            //tile position
            Vector3Int tilePos = piece.cells[i] + position;

            //testing if out of bounds
            if (!bounds.Contains((Vector2Int)tilePos))
            {
                return false;
            }

            //testing if a tile is already in that position
            if (this.tilemap.HasTile(tilePos))
            {
                return false;
            }
        }
        //if it loops through and is never false, return true
        return true;
    }

    public void ClearCLines()
    {
        //clear line when it is filled and shift pieces down

        RectInt bounds = this.Bounds;
        int row = bounds.yMin;

        while (row< bounds.yMax)
        {
            if (IsLineFull(row))
            {
                //if the row is full, clear and move pieces down
                LineClear(row);
            }
            else
            {
                row++;
            }
        }
    }

    public bool IsLineFull(int row)
    {
        //check to see if the row isfull
        RectInt bounds = this.Bounds;

        for (int collumn = bounds.xMin; collumn < bounds.xMax; collumn++)
        {
            Vector3Int position = new Vector3Int(collumn, row, 0);

            if (!this.tilemap.HasTile(position))
            {
                return false;
            }
        }
        return true;
    }

    private void LineClear(int row)
    {
        //clear tiles from position
        RectInt bounds = this.Bounds;

        for (int collumn = bounds.xMin; collumn < bounds.xMax; collumn++)
        {
            Vector3Int position = new Vector3Int(collumn, row, 0);
            this.tilemap.SetTile(position, null);
        }

        //shift row down
        while (row < bounds.yMax)
        {
            for (int collumn = bounds.xMin; collumn < bounds.xMax; collumn++)
            {
                //get the row above and set the position 
                Vector3Int position = new Vector3Int(collumn, row + 1, 0);
                TileBase above = this.tilemap.GetTile(position);

                position = new Vector3Int(collumn, row, 0);
                this.tilemap.SetTile(position, above);
            }

            row++;
        }
    }
}
