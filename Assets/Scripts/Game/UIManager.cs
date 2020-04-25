using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text EndGameText;
    public Text EndGameButtonText;
    public Animator ButtonsAnimator;
    public Animator BackgroundAnimator;

    public void ExitToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void ShowFailureDialog()
    {
        EndGameButtonText.text = "Restart";
        EndGameText.text = "You Lose!";
        End();
    }

    public void ShowSuccessDialog()
    {
        EndGameButtonText.text = "Continue";
        EndGameText.text = "You Win!";
        End();
    }

    public void Start()
    {
        ButtonsAnimator.Play("ScaleDown");
        BackgroundAnimator.Play("Hide");
    }

    public void End()
    {
        ButtonsAnimator.Play("ScaleUp");
        BackgroundAnimator.Play("Show");
    }

    public void Restart()
    {
        EndGameButtonText.text = "Restart";
        Start();
    }
}