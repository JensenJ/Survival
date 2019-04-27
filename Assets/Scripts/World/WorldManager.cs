using System.Collections;
using System;
using UnityEngine;
using TMPro;

public class WorldManager : MonoBehaviour
{
    GameObject saveSelectionContent = null;
    [SerializeField] private GameObject WorldMenuWidgetPrefab = null;
    public string[] worlds;

    // Start is called before the first frame update
    void Start()
    {
        saveSelectionContent = transform.root.GetChild(1).GetChild(5).GetChild(1).GetChild(0).GetChild(0).gameObject;

        SaveData loadData = SaveSystem.LoadSaves();
        if (loadData != null)
        {
            worlds = new string[loadData.saveNames.Length];
            for (int i = 0; i < worlds.Length; i++)
            {
                worlds[i] = loadData.saveNames[i];
            }
            LoadWorlds(worlds);
        }
    }

    public void NewWorld()
    {
        GameObject widget = Instantiate(WorldMenuWidgetPrefab, transform.position, Quaternion.identity, saveSelectionContent.transform);
        // TODO: Warn user if name chosen could overwite an existing world.
        string name = "World " + UnityEngine.Random.Range(0, 10).ToString();
        widget.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        widget.gameObject.name = name;
        SaveWorlds();
    }

    public void SaveWorlds()
    {
        SortWorlds();
        SaveSystem.SaveSaves(this);
    }

    public void LoadWorlds(string[] worldNames)
    {
        for (int i = 0; i < worldNames.Length; i++)
        {
            GameObject widget = Instantiate(WorldMenuWidgetPrefab, transform.position, Quaternion.identity, saveSelectionContent.transform);
            string name = worldNames[i];
            widget.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
            widget.gameObject.name = name;
        }
        SaveWorlds();
    }

    //Sorts the worlds alphabetically in heirarchy and thus in the content panel as well.
    void SortWorlds()
    {
        GameObject[] transformWorlds = new GameObject[saveSelectionContent.transform.childCount];
        worlds = new string[transformWorlds.Length];

        for (int i = 0; i < saveSelectionContent.transform.childCount; i++)
        {
            transformWorlds[i] = saveSelectionContent.transform.GetChild(i).gameObject;
            worlds[i] = transformWorlds[i].name;
        }

        IComparer comparer = new WorldSorter();
        Array.Sort(transformWorlds, comparer);

        for (int i = 0; i < worlds.Length; i++)
        {
            saveSelectionContent.transform.GetChild(i).SetSiblingIndex(transformWorlds[i].transform.GetSiblingIndex());
            transformWorlds[i] = saveSelectionContent.transform.GetChild(i).gameObject;
            worlds[i] = transformWorlds[i].name;
        }
    }
}

//World order sorting
public class WorldSorter : IComparer
{
    int IComparer.Compare(System.Object x, System.Object y)
    {
        //Sort alphabetically
        return new CaseInsensitiveComparer().Compare(((GameObject)x).name, ((GameObject)y).name);
    }
}