using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public Vector2Int LevelSize = new Vector2Int(10, 10);

    [Range(0f, 2f)]
    public float ObstacleDensity = 0.5f;

    public bool RandomStart = false;
    public bool RandomEnd = false;
    public GameObject ObstaclePrefab;
    public GameObject StartPrefab;
    public GameObject FinishPrefab;
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;

    [Range(1, 10)]
    public int EnemiesCount = 2;

    private GameObject levelBase;
    private GameObject player;
    private Level level;

    public void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        Destroy(levelBase);
        Destroy(player);

        levelBase = new GameObject("Level");

        Vector2Int startPoint, endPoint;

        if (RandomStart)
        {
            startPoint = new Vector2Int(Random.Range(0, LevelSize.x - 1), 0);
        }
        else
        {
            startPoint = new Vector2Int(0, 0);
        }
        if (RandomEnd)
        {
            endPoint = new Vector2Int(Random.Range(0, LevelSize.x - 1), LevelSize.y - 1);
        }
        else
        {
            endPoint = new Vector2Int(LevelSize.x - 1, LevelSize.y - 1);
        }

        level = new Level(LevelSize.x, LevelSize.y, startPoint, endPoint);

        // Генерируем основание с точкой старта и финиша
        for (int i = 0; i < LevelSize.x; i++)
        {
            for (int j = 0; j < LevelSize.y; j++)
            {
                if (i == startPoint.x && j == startPoint.y)
                {
                    level.Cells[i, j] = CellType.Start;
                    SpawnObstacle(StartPrefab, new Vector3(i, 0, j));
                }
                else if (i == endPoint.x && j == endPoint.y)
                {
                    level.Cells[i, j] = CellType.End;
                    SpawnObstacle(FinishPrefab, new Vector3(i, 0, j));
                }
                else
                {
                    level.Cells[i, j] = CellType.Obstacle;
                    SpawnObstacle(ObstaclePrefab, new Vector3(i, 0, j));
                }
            }
        }

        GenerateWay(startPoint, endPoint);

        for (int i = 0; i < EnemiesCount; i++)
        {
            GenerateEnemy();
        }

        RemoveRandom();
        GenerateObstacles();

        player = Instantiate(PlayerPrefab, new Vector3(startPoint.x, 1.25f, startPoint.y), Quaternion.identity);
        player.GetComponent<Moving>().Level = level;
    }

    private void SpawnObstacle(GameObject prefab, Vector3 pos)
    {
        var obj = Instantiate(prefab, pos, Quaternion.identity);
        obj.transform.parent = levelBase.transform;
    }

    private Vector2Int[] GenerateWay(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        int x = start.x;
        int y = start.y;

        bool isGoRight;
        bool isGoLeft;

        // Удаляем 2 препятствия на точке старта
        level.Cells[x, y] = CellType.Empty;
        path.Add(new Vector2Int(x, y));

        if (x < end.x)
        {
            if (x + 1 <= level.Cells.GetLength(0))
            {
                level.Cells[x + 1, y] = CellType.Empty;
                path.Add(new Vector2Int(x + 1, y));
            }
        }
        else if (x > end.x)
        {
            if (x - 1 >= 0)
            {
                level.Cells[x - 1, y] = CellType.Empty;
                path.Add(new Vector2Int(x - 1, y));
            }
        }

        // Удаляем остальные препятствия
        while (x != end.x || y != end.y)
        {
            isGoRight = false;
            isGoLeft = false;

            if (x < end.x)
            {
                x++;
                isGoRight = true;
            }
            else if (x > end.x)
            {
                x--;
                isGoLeft = true;
            }

            if (y < end.y)
            {
                y++;
            }
            else if (y > end.y)
            {
                y--;
            }

            level.Cells[x, y] = CellType.Empty;
            path.Add(new Vector2Int(x, y));

            if (isGoRight && x + 1 < level.Cells.GetLength(0))
            {
                level.Cells[x + 1, y] = CellType.Empty;
                path.Add(new Vector2Int(x + 1, y));
            }

            if (isGoLeft && x - 1 >= 0)
            {
                level.Cells[x - 1, y] = CellType.Empty;
                path.Add(new Vector2Int(x - 1, y));
            }
        }

        return path.ToArray();
    }

    private void RemoveRandom()
    {
        int x, y;

        for (int i = 0; i < LevelSize.x * LevelSize.y * ObstacleDensity; i++)
        {
            x = Random.Range(0, LevelSize.x);
            y = Random.Range(0, LevelSize.y);

            level.Cells[x, y] = CellType.Empty;
        }
    }

    private void GenerateObstacles()
    {
        for (int i = 0; i < level.Cells.GetLength(0); i++)
        {
            for (int j = 0; j < level.Cells.GetLength(1); j++)
            {
                if (level.Cells[i, j] == CellType.Obstacle)
                    SpawnObstacle(ObstaclePrefab, new Vector3(i, 1, j));
            }
        }
    }

    private void GenerateEnemy()
    {
        Vector2Int start, end;

        // Генерируем точки патрулирования исключая совпадения между собой и стартовой точкой игрока
        do
        {
            start = new Vector2Int(Random.Range(0, LevelSize.x), Random.Range(0, LevelSize.y));
        } while (start == level.StartPoint);
        do
        {
            end = new Vector2Int(Random.Range(0, LevelSize.x), Random.Range(0, LevelSize.y));
        } while (end == start && end == level.StartPoint);

        var enemy = Instantiate(EnemyPrefab, new Vector3(start.x, 1.5f, start.y), Quaternion.identity);
        enemy.GetComponent<Moving>().Level = level;
        enemy.GetComponent<EnemyMoving>().PatrolPath = GenerateWay(start, end);

        GetComponent<GameManager>().Enemies.Add(enemy);
    }
}