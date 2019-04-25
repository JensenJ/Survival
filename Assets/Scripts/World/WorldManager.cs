using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class WorldManager : MonoBehaviour
{
    GameObject saveSelectionContent = null;
    [SerializeField] private GameObject WorldMenuWidgetPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        saveSelectionContent = transform.root.GetChild(1).GetChild(5).GetChild(1).GetChild(0).GetChild(0).gameObject;
    }

    public void NewWorld()
    {
        GameObject widget = Instantiate(WorldMenuWidgetPrefab, transform.position, Quaternion.identity, saveSelectionContent.transform);
        // TODO: Warn user if name chosen could overwite an existing world.
        string name = "World " + UnityEngine.Random.Range(0, 1).ToString();
        widget.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        widget.gameObject.name = name;
        SortWorlds();
    }

    //Sorts the worlds alphabetically in heirarchy and thus in the content panel as well.
    void SortWorlds()
    {
        GameObject[] worlds = new GameObject[saveSelectionContent.transform.childCount];
        for (int i = 0; i < saveSelectionContent.transform.childCount; i++)
        {
            worlds[i] = saveSelectionContent.transform.GetChild(i).gameObject;
        }

        IComparer comparer = new WorldSorter();
        Array.Sort(worlds, comparer);

        for (int i = 0; i < worlds.Length; i++)
        {
            saveSelectionContent.transform.GetChild(i).SetSiblingIndex(worlds[i].transform.GetSiblingIndex());
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