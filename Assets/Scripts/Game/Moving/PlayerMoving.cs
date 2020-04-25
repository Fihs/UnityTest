using System.Collections;
using UnityEngine;

public class PlayerMoving : Moving
{
    private GameManager EnemyManager;

    private void Start()
    {
        EnemyManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && CanMove(transform.position + Vector3.forward))
        {
            StartCoroutine(Move(transform.position + Vector3.forward, false));
        }
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && CanMove(transform.position + Vector3.back))
        {
            StartCoroutine(Move(transform.position + Vector3.back, false));
        }
        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && CanMove(transform.position + Vector3.right))
        {
            StartCoroutine(Move(transform.position + Vector3.right, false));
        }
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && CanMove(transform.position + Vector3.left))
        {
            StartCoroutine(Move(transform.position + Vector3.left, false));
        }
    }

    protected override IEnumerator Move(Vector3 vector, bool autoRotate = true)
    {
        yield return base.Move(vector, autoRotate);
        EnemyManager.NoiseIncrease();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Finish")
        {
            GameObject.FindWithTag("Manager").GetComponent<GameManager>().Win();
        }
    }
}