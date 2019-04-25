using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void Singleplayer(string worldName)
    {
        WorldData.currentlyLoadedName = worldName;
        SceneManager.LoadScene("Game");
    }
    public void Multiplayer()
    {

    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
