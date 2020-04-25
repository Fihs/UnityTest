using System.Collections;
using UnityEngine;

public abstract class Moving : MonoBehaviour
{
    public Level Level;

    [Range(0.1f, 5f)]
    public float Speed = 1f;

    private bool isMoving;

    protected virtual IEnumerator Move(Vector3 destination, bool autoRotate = true)
    {
        isMoving = true;

        if (autoRotate)
            GetQuaternion(destination);

        while (transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Speed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
    }

    private void GetQuaternion(Vector3 destination)
    {
        if (destination.x > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
            return;
        }
        else if (destination.x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
            return;
        }
        else if (destination.z > transform.position.z)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            return;
        }
        else if (destination.z < transform.position.z)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            return;
        }
    }

    protected virtual bool CanMove(Vector3 destination)
    {
        if (isMoving)
            return false;

        if (Level.Cells.GetLength(0) - 1 < destination.x || 0 > destination.x || Level.Cells.GetLength(1) - 1 < destination.z || 0 > destination.z)
        {
            return false;
        }
        else if (Level.Cells[(int)destination.x, (int)destination.z] == CellType.Obstacle)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}