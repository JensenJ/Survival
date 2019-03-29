// Copyright (c) 2019 JensenJ
// NAME: WaypointManager
// PURPOSE: Manages the waypoint system

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaypointManager : MonoBehaviour
{

    //Settings
    [SerializeField] [Range(0, 32)] int MaxWaypointAmount = 16;
    [SerializeField] private GameObject waypointPrefab = null;
    public GameObject waypointManagerPanel = null;
    public GameObject waypointEditorPanel = null;
    [SerializeField] private Waypoint[] waypoints;

    //Editor variables/references
    private TMP_InputField waypointEditName;
    private TMP_InputField waypointEditX;
    private TMP_InputField waypointEditY;
    private TMP_InputField waypointEditZ;
    private Slider waypointEditR;
    private Slider waypointEditG;
    private Slider waypointEditB;
    private Image waypointEditColour;
    private Toggle waypointEditEnabled;
    private Color waypointColour;
    int waypointToRemove = 0;
    bool bIsEditingWaypoint = false;

    //Other defaults used throughout class
    private GameObject waypointManagerContent = null;
    private Vector3 targetTransform = Vector3.zero;
    private PlayerController pc = null;
    private GameObject spawnedDrone = null;
    private Transform waypointList = null;

    // Sets all basic variables and default settings
    void Start()
    {
        //Sets array max length for waypoints
        waypoints = new Waypoint[MaxWaypointAmount];
        pc = transform.root.GetChild(3).GetComponent<PlayerController>();
        waypointManagerPanel = transform.root.GetChild(1).GetChild(1).gameObject;
        waypointManagerContent = waypointManagerPanel.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
        waypointList = transform.root.GetChild(2);
        //Waypoint Editor
        waypointEditorPanel = transform.root.GetChild(1).GetChild(2).gameObject;
        waypointEditName = waypointEditorPanel.transform.GetChild(1).GetComponent<TMP_InputField>();

        //Input fields and defaults
        waypointEditX = waypointEditorPanel.transform.GetChild(2).GetChild(1).GetComponent<TMP_InputField>();
        waypointEditY = waypointEditorPanel.transform.GetChild(2).GetChild(2).GetComponent<TMP_InputField>();
        waypointEditZ = waypointEditorPanel.transform.GetChild(2).GetChild(3).GetComponent<TMP_InputField>();

        waypointEditR = waypointEditorPanel.transform.GetChild(3).GetChild(1).GetComponent<Slider>();
        waypointEditG = waypointEditorPanel.transform.GetChild(3).GetChild(2).GetComponent<Slider>();
        waypointEditB = waypointEditorPanel.transform.GetChild(3).GetChild(3).GetComponent<Slider>();
        waypointEditColour = waypointEditorPanel.transform.GetChild(3).GetChild(4).GetComponent<Image>();

        waypointEditEnabled = waypointEditorPanel.transform.GetChild(4).GetComponent<Toggle>();

        waypointEditR.value = 1.0f;
        waypointEditG.value = 1.0f;
        waypointEditB.value = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Checks for drone, used for facing waypoint towards active player/drone
        if (pc.bHasDeployedDrone)
        {
            spawnedDrone = pc.spawnedDrone;
            if (spawnedDrone != null)
            {
                targetTransform = spawnedDrone.transform.position;
            }
        }
        else
        {
            spawnedDrone = null;
            targetTransform = pc.transform.position;
        }

        //Updates colour preview in editor
        waypointColour = new Color(waypointEditR.value, waypointEditG.value, waypointEditB.value);
        waypointEditColour.color = waypointColour;
    }

    //Adds waypoint with specified parameters
    void AddWaypoint(string m_name, Vector3 m_location, Color m_color, bool bIsEnabled)
    {
        //Keeps track of current iteration in foreach loop
        int iteration = 0;
        bool bHasFilled = false;
        //Loops through each waypoint space to check for next available one.
        foreach (Waypoint i in waypoints)
        {
            //Checks whether the space is free for a waypoint in menu
            if (i.bIsEnabled == false)
            {
                bHasFilled = true;
                //Sets variables for waypoint
                waypoints[iteration].index = iteration;
                waypoints[iteration].bIsEnabled = bIsEnabled;
                waypoints[iteration].color = m_color;
                waypoints[iteration].location = m_location;
                waypoints[iteration].name = m_name;
                //Spawns new waypoint in menu and in world
                GameObject go_waypoint = Instantiate(waypointPrefab, pc.transform.position, transform.rotation, transform.root.GetChild(2));
                WaypointUI waypoint = go_waypoint.GetComponent<WaypointUI>();
                waypoint.SetWaypointSettings(iteration, bIsEnabled, m_name, m_location, m_color);

                //Breaks out of loop
                break;
            }
            //Increases which element we are on.
            iteration++;
        }

        //Checks whether a space was actually able to be filled and prints out if not.
        if (bHasFilled == false)
        {
            print("All waypoint slots taken.");
        }
    }

    public void Rearrange(bool m_bIsDown, int m_WaypointID)
    {
        //Get index at waypoint id
        int index = transform.GetChild(m_WaypointID).GetSiblingIndex();
        WaypointUI waypoint = FindWaypointByIndex(m_WaypointID);
        //Null pointer exception check
        if (waypoint != null)
        {
            if (m_bIsDown)
            {
                //Move sibling index up one for ui and manager child
                waypointManagerContent.transform.GetChild(waypoint.transform.GetSiblingIndex()).SetSiblingIndex(waypoint.transform.GetSiblingIndex() + 1);
                waypoint.transform.SetSiblingIndex(waypoint.transform.GetSiblingIndex() + 1);
            }
            else
            {
                //Move sibling index down one for ui and manager child
                waypointManagerContent.transform.GetChild(waypoint.transform.GetSiblingIndex()).SetSiblingIndex(waypoint.transform.GetSiblingIndex() - 1);
                waypoint.transform.SetSiblingIndex(waypoint.transform.GetSiblingIndex() - 1);
            }
        }
    }

    //Attempts to find waypoint at specified index
    WaypointUI FindWaypointByIndex(int m_WaypointID)
    {
        
        //Iterate through every waypoint and check id and compare against given id
        for (int i = 0; i < waypointList.childCount; i++)
        {
            WaypointUI waypoint = waypointList.GetChild(i).GetComponent<WaypointUI>();
            if (waypoint.WaypointID == m_WaypointID)
            {
                //Return it if found
                return waypoint;
            }
        }
        //Print error if not found and return null
        Debug.LogError("FindWaypointByName: Waypoint ID not found.");
        return null;
    }

    //Clears data of waypoint at specified index and removes it from game world.
    public void RemoveWaypoint(int m_index)
    {
        waypoints[m_index].index = 0;
        waypoints[m_index].bIsEnabled = false;
        waypoints[m_index].name = "";
        waypoints[m_index].location = Vector3.zero;
        waypoints[m_index].color = Color.black;

        WaypointUI waypoint = FindWaypointByIndex(m_index);
        //Null pointer exception check
        if (waypoint != null)
        {
            waypoint.RemoveWaypoint();
        }
    }

    public void NewWaypoint()
    {
        //Setting defaults for new waypoint.
        //Random colour generated
        waypointEditR.value = Random.Range(0.0f, 1.0f);
        waypointEditG.value = Random.Range(0.0f, 1.0f);
        waypointEditB.value = Random.Range(0.0f, 1.0f);
        waypointEditName.text = "New Waypoint";
        //Use coordinates of current position
        if (pc.bHasDeployedDrone)
        {
            waypointEditX.text = Mathf.Round(pc.transform.GetChild(3).position.x).ToString();
            waypointEditY.text = Mathf.Round(pc.transform.GetChild(3).position.y).ToString();
            waypointEditZ.text = Mathf.Round(pc.transform.GetChild(3).position.z).ToString();
        }
        else
        {
            waypointEditX.text = Mathf.Round(pc.transform.position.x).ToString();
            waypointEditY.text = Mathf.Round(pc.transform.position.y).ToString();
            waypointEditZ.text = Mathf.Round(pc.transform.position.z).ToString();
        }

        //Enables editor, disables manager
        waypointEditorPanel.SetActive(true);
        waypointManagerPanel.SetActive(false);
    }

    public void EditWaypoint(int m_index)
    {
        bIsEditingWaypoint = true;

        if(FindWaypointByIndex(m_index) != null)
        {
            //Load data into editor
            waypointEditR.value = waypoints[m_index].color.r;
            waypointEditG.value = waypoints[m_index].color.g;
            waypointEditB.value = waypoints[m_index].color.b;
            waypointEditName.text = waypoints[m_index].name;
            waypointEditX.text = waypoints[m_index].location.x.ToString();
            waypointEditY.text = waypoints[m_index].location.y.ToString();
            waypointEditZ.text = waypoints[m_index].location.z.ToString();
            waypointEditEnabled.isOn = waypoints[m_index].bIsEnabled;
            waypointToRemove = m_index;
        }

        waypointEditorPanel.SetActive(true);
        waypointManagerPanel.SetActive(false);
    }

    public void SaveWaypoint()
    {
        //Get location from textbox in editor for coords
        Vector3 location = new Vector3(int.Parse(waypointEditX.text), int.Parse(waypointEditY.text), int.Parse(waypointEditZ.text));
        //Check whether they are editing the waypoint or whether it is a new one
        if (bIsEditingWaypoint)
        {
            //If so, remove the old one
            RemoveWaypoint(waypointToRemove);
            bIsEditingWaypoint = false;
        }
        //Then add the new one
        AddWaypoint(waypointEditName.text, location, waypointColour, waypointEditEnabled.isOn);
        
        //Disables editor, enables manager
        waypointEditorPanel.SetActive(false);
        waypointManagerPanel.SetActive(true);
    }

    //Closes waypoint manager and returns to normal game
    public void CloseWaypointManager()
    {
        waypointManagerPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        pc.Crosshair.gameObject.SetActive(true);
        pc.bCanMove = true;
        if (pc.bHasDeployedDrone)
        {
            pc.spawnedDrone.GetComponent<DroneController>().bCanMove = true;
        }
    }

    //Closes waypoint editor and reopens manager
    public void CloseWaypointEditor()
    {
        waypointEditorPanel.SetActive(false);
        waypointManagerPanel.SetActive(true);
    }
}

//Struct for waypoints and what they should contain
[System.Serializable]
public struct Waypoint
{
    public int index;
    public bool bIsEnabled;
    public string name;
    public Vector3 location;
    public Color color;
}