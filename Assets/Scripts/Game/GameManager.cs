using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int NoiseMaxValue = 10;
    public int NoiseIncrementAmount = 3;
    public int NoiseDecrementAmount = 1;
    public float NoiseDecreaseDelay = 0.5f;
    public bool NoiseDecreaseAfterMax = false;
    public UnityEngine.UI.Image NoiseBar;

    [HideInInspector]
    public List<GameObject> Enemies;

    private Coroutine decreaseCoroutine;

    public int Noise
    {
        get => noise;
        private set
        {
            if (value > NoiseMaxValue)
            {
                if (!NoiseDecreaseAfterMax)
                {
                    StopCoroutine(decreaseCoroutine);
                    CoroutineStarted = false;
                }

                noise = NoiseMaxValue;

                foreach (var item in Enemies)
                {
                    item.GetComponent<EnemyMoving>().IsHunting = true;
                }
            }
            else if (value < 0)
            {
                noise = 0;
            }
            else
            {
                noise = value;
            }

            NoiseBar.rectTransform.localScale = new Vector3(1, noise * 0.1f, 1);
            NoiseBar.material.SetFloat("_Middle", 1 - noise * 0.1f);
        }
    }

    private int noise;
    private bool CoroutineStarted;

    public void NoiseIncrease()
    {
        Noise += NoiseIncrementAmount;
        if (!CoroutineStarted && (Noise != NoiseMaxValue || NoiseDecreaseAfterMax))
        {
            decreaseCoroutine = StartCoroutine(NoiseDecrease());
        }
    }

    private IEnumerator NoiseDecrease()
    {
        CoroutineStarted = true;

        while (Noise > 0)
        {
            yield return new WaitForSeconds(NoiseDecreaseDelay);
            Noise -= NoiseDecrementAmount;
        }

        CoroutineStarted = false;
    }

    private void End()
    {
        foreach (var item in Enemies)
        {
            Destroy(item);
        }

        Enemies.Clear();

        StopAllCoroutines();

        CoroutineStarted = false;
        Noise = 0;
    }

    public void Lose()
    {
        End();
        GetComponent<UIManager>().ShowFailureDialog();
    }

    public void Win()
    {
        End();
        GetComponent<UIManager>().ShowSuccessDialog();
    }

    public void Restart()
    {
        End();
        GetComponent<UIManager>().Restart();
        GetComponent<Generator>().Start();
    }
}
