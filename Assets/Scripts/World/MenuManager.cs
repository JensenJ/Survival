using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    WorldManager wm;
    PlayerController pc;

    void Start()
    {
        wm = GetComponent<WorldManager>();
        pc = transform.root.GetChild(3).GetComponent<PlayerController>();
    }

    public void Singleplayer(string worldName)
    {
        WorldData.currentlyLoadedName = worldName;
        if(SaveSystem.LoadMap(worldName) == null)
        {
            WorldData.isNewMap = true;
        }
        else
        {
            WorldData.isNewMap = false;
        }
        SceneManager.LoadScene("Game");
    }
    public void Multiplayer()
    {

    }

    public void MainMenu()
    {
        if(pc != null)
        {
            pc.SaveGame();
        }
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    void OnApplicationQuit()
    {
        if(wm != null)
        {
            wm.SaveWorlds();
        }

        if(pc != null)
        {
            pc.SaveGame();
        }
    }
}
