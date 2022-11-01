using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//store static data for game
public static class Data 
{
    public static readonly float cos = Mathf.Cos(Mathf.PI / 2f);
    public static readonly float sin = Mathf.Sin(Mathf.PI / 2f);
    public static readonly float[] RotationMatrix = new float[] {cos, sin, -sin, -cos};


    public static readonly Dictionary<Tetromino, Vector2Int[]> Cells = new Dictionary<Tetromino, Vector2Int[]>()
    {

    };

}
