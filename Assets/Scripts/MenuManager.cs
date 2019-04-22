using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void Singleplayer()
    {
        SceneManager.LoadScene("Game");
    }
    public void Multiplayer()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
