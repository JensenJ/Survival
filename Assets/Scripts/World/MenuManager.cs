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

    public void Singleplayer()
    {
        wm.OpenWorldSelection();
    }

    public void LoadWorld(string name, int seed)
    {
        WorldData.currentlyLoadedName = name;
        WorldData.currentSeed = seed;
        if(SaveSystem.LoadMap(name) == null)
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
