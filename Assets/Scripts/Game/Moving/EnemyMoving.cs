using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyMoving : Moving
{
    /// <summary>
    /// Путь патрулирования
    /// </summary>
    [HideInInspector]
    public Vector2Int[] PatrolPath;

    public bool IsHunting
    {
        get => isHunting;
        set
        {
            isHunting = value;

            if (value)
            {
                StopAllCoroutines();
                StartCoroutine(MoveToPlayer());
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(StartPatrolling());
            }

            gameObject.transform.GetChild(0).gameObject.SetActive(value);
            gameObject.GetComponent<Renderer>().material.color = value ? HuntColor : NormalColor;
        }
    }

    /// <summary>
    /// Должен ли враг преследовать игрока до последнего
    /// </summary>
    public bool IsChase = true;

    /// <summary>
    /// Может ли враг видеть через препятствия
    /// </summary>
    public bool WallHack = false;

    /// <summary>
    /// Цвет врага во время патрулирования
    /// </summary>
    public Color NormalColor = Color.yellow;

    /// <summary>
    /// Цвет врага во время преследования
    /// </summary>
    public Color HuntColor = Color.red;

    private bool isHunting;
    private bool isEnclosed;

    private void Start()
    {
        IsHunting = IsHunting;
    }

    private IEnumerator StartPatrolling()
    {
        yield return StartCoroutine(ReturnToPatrol());

        while (true)
        {
            for (int i = 1; i < PatrolPath.Length; i++)
            {
                yield return StartCoroutine(Move(new Vector3(PatrolPath[i].x, transform.position.y, PatrolPath[i].y)));
            }

            for (int i = PatrolPath.Length - 1; i >= 0; i--)
            {
                yield return StartCoroutine(Move(new Vector3(PatrolPath[i].x, transform.position.y, PatrolPath[i].y)));
            }
        }
    }

    private IEnumerator ReturnToPatrol()
    {
        // находим путь к начальной точке маршрута патрулирования
        var path = FindPath(PatrolPath[0]);

        for (int i = path.Length - 1; i >= 0; i--)
        {
            yield return StartCoroutine(Move(new Vector3(path[i].x, transform.position.y, path[i].y)));
        }
    }

    private Vector2Int[] FindPath(Vector2Int destination)
    {
        var start = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        // Создаём промежуточную матрицу для поиска
        CellType[,] tmpMap = new CellType[Level.Cells.GetLength(0), Level.Cells.GetLength(1)];

        for (int i = 0; i < Level.Cells.GetLength(0); i++)
        {
            for (int j = 0; j < Level.Cells.GetLength(1); j++)
            {
                if (Level.Cells[i, j] == CellType.Obstacle)
                {
                    tmpMap[i, j] = CellType.Obstacle;
                }
                else if (i == destination.x && j == destination.y)
                {
                    tmpMap[i, j] = CellType.End;
                }
                else if (i == start.x && j == start.y)
                {
                    tmpMap[i, j] = CellType.Start;
                }
                else
                {
                    tmpMap[i, j] = CellType.Empty;
                }
            }
        }

        // Поиск пути
        Lee(tmpMap, start.x, start.y);

        // Восстановление пути
        List<Vector2Int> path = new List<Vector2Int>();

        path.Add(new Vector2Int(destination.x, destination.y));

        while (destination != start)
        {
            var cell = GetNeighbors(destination.x, destination.y).FirstOrDefault(item => tmpMap[item.x, item.y] != CellType.Obstacle && tmpMap[item.x, item.y] < tmpMap[destination.x, destination.y]);

            if (cell != default)
            {
                path.Add(new Vector2Int(cell.x, cell.y));

                destination = cell;
            }
            else
            {
                // Если ни одна из соседних клеток не подходит значит объект в замкнутом пространстве
                isEnclosed = true;
                break;
            }
        }

        return path.ToArray();
    }

    /// <summary>
    /// Метод возвращает соседние клетки
    /// </summary>
    private IEnumerable<Vector2Int> GetNeighbors(int x, int y)
    {
        if (x < 0 || x >= Level.Cells.GetLength(0) || y < 0 || y >= Level.Cells.GetLength(1))
            yield break;

        if (x < Level.Cells.GetLength(0) - 1)
        {
            yield return new Vector2Int(x + 1, y);
        }
        if (y < Level.Cells.GetLength(1) - 1)
        {
            yield return new Vector2Int(x, y + 1);
        }
        if (x > 0)
        {
            yield return new Vector2Int(x - 1, y);
        }
        if (y > 0)
        {
            yield return new Vector2Int(x, y - 1);
        }
    }

    /// <summary>
    /// Поиск пути алгоритмом Ли
    /// </summary>
    private void Lee(CellType[,] A, int x, int y)
    {
        if (x + 1 < A.GetLength(0) && A[x + 1, y] != CellType.Obstacle && (A[x + 1, y] == CellType.Empty || A[x + 1, y] > A[x, y] + 1))
        {
            A[x + 1, y] = A[x, y] + 1;
            Lee(A, x + 1, y);
        }

        if (x - 1 >= 0 && A[x - 1, y] != CellType.Obstacle && (A[x - 1, y] == CellType.Empty || A[x - 1, y] > A[x, y] + 1))
        {
            A[x - 1, y] = A[x, y] + 1;
            Lee(A, x - 1, y);
        }

        if (y + 1 < A.GetLength(1) && A[x, y + 1] != CellType.Obstacle && (A[x, y + 1] == CellType.Empty || A[x, y + 1] > A[x, y] + 1))
        {
            A[x, y + 1] = A[x, y] + 1;
            Lee(A, x, y + 1);
        }

        if (y - 1 >= 0 && A[x, y - 1] != CellType.Obstacle && (A[x, y - 1] == CellType.Empty || A[x, y - 1] > A[x, y] + 1))
        {
            A[x, y - 1] = A[x, y] + 1;
            Lee(A, x, y - 1);
        }
    }

    private IEnumerator MoveToPlayer()
    {
        do
        {
            var path = FindPath(new Vector2Int((int)GameObject.FindWithTag("Player").transform.position.x, (int)GameObject.FindWithTag("Player").transform.position.z));

            if (path.Length > 1)
                for (int i = path.Length - 2; i >= 0; i--)
                {
                    yield return StartCoroutine(Move(new Vector3(path[i].x, transform.position.y, path[i].y)));
                }
            else
            {
                break;
            }
        } while (IsChase && !isEnclosed);

        IsHunting = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            IsHunting = false;
            Destroy(other.gameObject);
            GameObject.FindWithTag("Manager").GetComponent<GameManager>().Lose();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isEnclosed && !IsHunting && other.gameObject.tag == "Player")
        {
            if (WallHack)
            {
                IsHunting = true;
                return;
            }
            else
            {
                Ray ray = new Ray(transform.position, other.transform.position - transform.position);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        IsHunting = true;
                    }
                }
            }
        }
    }
}