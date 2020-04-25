using UnityEngine;

public enum CellType
{
    Obstacle = -2,
    Empty = -1,
    Start = 0,
    End = -1,
}

public class Level
{
    public CellType[,] Cells { get; private set; }
    public Vector2Int StartPoint { get; private set; }
    public Vector2Int EndPoint { get; private set; }

    public Level(int x, int y, Vector2Int start, Vector2Int end)
    {
        Cells = new CellType[x, y];
        StartPoint = start;
        EndPoint = end;
    }
}