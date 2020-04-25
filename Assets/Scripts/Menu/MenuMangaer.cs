using UnityEngine;

public class MenuMangaer : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            AppQuit();
        }
    }

    public void AppQuit()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}