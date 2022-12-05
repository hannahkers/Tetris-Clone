using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Piece : MonoBehaviour
{
    //*wall kick data and methods made with help*

    //tetris is played with one piece at a time, make one piece and re-instantiate it over and over again throughout the game
    //declare properties

    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int position { get; private set; }

    public Vector3Int[] cells { get; private set; }

    public int rotationIndex { get; private set; }

    public float stepDelay = 1f;

    public float lockDelay = 0.5f;

    private float stepTime;
    private float lockTime;

    public void Initialize(Board board, Vector3Int position, TetrominoData data  )
    {
        //assign these 1:1
        this.board = board;
        this.position = position;
        this.data = data;
        this.stepTime = Time.time + stepDelay;
        this.lockTime = 0f;

        //set to 0 so it spawns in default state
        this.rotationIndex = 0;

        if (this.cells == null)
        {
            //if the array hasn't been initialed, then do that here
            
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            //cast to vector 3
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    //handle player input
    private void Update()
    {
        //first thing, clear the board
        board.Clear(this);

        //increase lock time
        lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //rotate counter clockwise
            Rotate(-1);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            //rotate clockwise
            Rotate(1);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            //move left
            Move(Vector2Int.left);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            //move right
            Move(Vector2Int.right);

        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            //softdrop piece down
            Move(Vector2Int.down);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        if (Time.time >= this.stepTime)
        {
            Step();
        }

        //reset after movements
        this.board.Set(this);
    }

    private void Step()
    {
        //set new time
        this.stepTime = Time.time + this.stepDelay;

        //
        Move(Vector2Int.down);
        if (this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }

    
    private void HardDrop()
    {
        //harddrop piece
        //while we move down, continue to move down until you no longer can
        while (Move(Vector2Int.down))
        {
            continue;
        }

        //add locking
        Lock();
    }

    private void Lock()
    {
        //lock piece in place and spawn a new piece onto the board
        board.Set(this);
        board.ClearCLines();
        board.SpawnPiece();
    }

    private bool Move(Vector2Int translation)
    {
        //player moves piece, how much they want to move the piece

        //calculate new position 
        Vector3Int newPos = this.position;
        newPos.x += translation.x;
        newPos.y += translation.y;

        //verify if position is valid
        bool valid = this.board.IsPositionValid(this, newPos);

        //if position is valid, set the new position
        if (valid)
        {
            this.position = newPos;
            //reset locktime when move is made
            this.lockTime = 0f;
        }


        return valid;
    }

    private void Rotate(int direction)
    {
        //store current rotation index
        int originalRotation = this.rotationIndex;

        //update roation index
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);

        //apply rotation matrix
        RotationMatrix(direction);

        //wallkick tests, reposition in bounds
        if (!TestWallKicks(this.rotationIndex, direction))
        {
            this.rotationIndex = originalRotation;
            RotationMatrix(-direction);
        }
    }

    private void RotationMatrix(int direction)
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];

            int x, y;

            //apply rotation matrix to all cells
            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // "I" and "O" are rotated from an offset center point
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
       //find which test to run bc "I" has a different test
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < this.data.wallKicks.GetLength(1); i++)
        {
            //look up transaltion in array
            Vector2Int translation = this.data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }
        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationIndex<0)
        {
            wallKickIndex--;
        }
        return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));
    }

    
    private int Wrap(int input, int min, int max)
    {
        //wrap function from Zigurous on youtube 

        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }
}
