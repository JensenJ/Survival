using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    WorldManager wm;
    SaveManager sm;

    void Start()
    {
        wm = GetComponent<WorldManager>();
        sm = GetComponent<SaveManager>();
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
        if(sm != null)
        {
            sm.SaveGame();
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

        if(sm != null)
        {
            sm.SaveGame();
        }
    }
}
