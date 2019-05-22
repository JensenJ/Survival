// Copyright (c) 2019 JensenJ
// NAME: WorldManager
// PURPOSE: Manages the world data

using System.Collections;
using System;
using UnityEngine;
using TMPro;

public class WorldManager : MonoBehaviour
{
    GameObject saveSelectionContent = null;
    GameObject worldManagerPanel = null;
    GameObject worldCreatorPanel = null;

    TMP_InputField worldNameInput;
    TMP_InputField worldSeedInput;

    MenuManager mm;

    [SerializeField] private GameObject WorldMenuWidgetPrefab = null;
    public string[] worlds;
    bool isEditing;

    // Start is called before the first frame update
    void Start()
    {

        mm = GetComponent<MenuManager>();
        saveSelectionContent = transform.root.GetChild(1).GetChild(5).GetChild(1).GetChild(0).GetChild(0).gameObject;
        worldManagerPanel = transform.root.GetChild(1).GetChild(5).gameObject;
        worldCreatorPanel = transform.root.GetChild(1).GetChild(6).gameObject;

        worldNameInput = worldCreatorPanel.transform.GetChild(1).GetComponent<TMP_InputField>();
        worldSeedInput = worldCreatorPanel.transform.GetChild(2).GetComponent<TMP_InputField>();

        //Loads world list
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

    //Button reference
    public void NewWorld()
    {
        worldManagerPanel.SetActive(false);
        worldCreatorPanel.SetActive(true);
        worldNameInput.text = "";
        worldSeedInput.text = "";
        isEditing = true;
    }

    public void CancelWorldCreation()
    {
        worldCreatorPanel.SetActive(false);
        worldManagerPanel.SetActive(true);
        isEditing = false;
    }

    public void AddWorld()
    {
        worldCreatorPanel.SetActive(false);
        worldManagerPanel.SetActive(true);
        //Create new widget and set settings
        GameObject widget = Instantiate(WorldMenuWidgetPrefab, transform.position, Quaternion.identity, saveSelectionContent.transform);
        string name = worldNameInput.text;

        if(name == "")
        {
            name = "New World";
        }

        int seed;
        if(worldSeedInput.text != "")
        {
            seed = int.Parse(worldSeedInput.text);
        }
        else
        {
            seed = UnityEngine.Random.Range(-100000, 100000);
        }
        
        //Open world creator and assign name. Also assign world seed
        widget.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        widget.gameObject.name = name;
        SaveWorlds();
        isEditing = false;
        mm.LoadWorld(name, seed);
        
    }

    public void OpenWorldSelection()
    {
        if (!isEditing)
        {
            worldManagerPanel.SetActive(!worldManagerPanel.activeSelf);
        }
    }

    public void SaveWorlds()
    {
        //Saves worlds
        SortWorlds();
        SaveSystem.SaveSaves(this);
    }

    //Loads worlds and creates them
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