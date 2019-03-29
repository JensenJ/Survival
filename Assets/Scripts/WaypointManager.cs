// Copyright (c) 2019 JensenJ
// NAME: WaypointManager
// PURPOSE: Manages the waypoint system

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaypointManager : MonoBehaviour
{

    [SerializeField] [Range(0, 32)] int MaxWaypointAmount = 16;
    [SerializeField] private GameObject waypointPrefab = null;
    public GameObject waypointManagerPanel = null;
    public GameObject waypointEditorPanel = null;
    [SerializeField] private Waypoint[] waypoints;
    private GameObject waypointManagerContent = null;
    private Vector3 targetTransform = Vector3.zero;
    private PlayerController pc = null;
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

    private GameObject spawnedDrone = null;


    // Sets all basic variables and default settings
    void Start()
    {
        //Sets array max length for waypoints
        waypoints = new Waypoint[MaxWaypointAmount];
        pc = transform.root.GetChild(3).GetComponent<PlayerController>();
        waypointManagerPanel = transform.root.GetChild(1).GetChild(1).gameObject;
        waypointManagerContent = waypointManagerPanel.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
        //Waypoint Editor
        waypointEditorPanel = transform.root.GetChild(1).GetChild(2).gameObject;
        waypointEditName = waypointEditorPanel.transform.GetChild(1).GetComponent<TMP_InputField>();

        //Input fields
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
            if (!(spawnedDrone == null))
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
        //Keeps track of current iteration in for each loop
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
            print("All marker slots taken.");
        }
    }

    public void Rearrange(bool m_bIsDown, int m_WaypointID)
    {
        int index = transform.GetChild(m_WaypointID).GetSiblingIndex();

        if (m_bIsDown)
        {

            Transform markerList = transform.root.GetChild(2);

            for (int i = 0; i < markerList.childCount; i++)
            {
                WaypointUI marker = markerList.GetChild(i).GetComponent<WaypointUI>();
                if (marker.WaypointID == m_WaypointID)
                {                    
                    waypointManagerContent.transform.GetChild(marker.transform.GetSiblingIndex()).SetSiblingIndex(marker.transform.GetSiblingIndex() + 1);
                    marker.transform.SetSiblingIndex(marker.transform.GetSiblingIndex() + 1);
                    break;
                }
            }
        }
        else
        {
            Transform markerList = transform.root.GetChild(2);

            for (int i = 0; i < markerList.childCount; i++)
            {
                WaypointUI marker = markerList.GetChild(i).GetComponent<WaypointUI>();
                if (marker.WaypointID == m_WaypointID)
                {
                    waypointManagerContent.transform.GetChild(marker.transform.GetSiblingIndex()).SetSiblingIndex(marker.transform.GetSiblingIndex() - 1);
                    marker.transform.SetSiblingIndex(marker.transform.GetSiblingIndex() - 1);
                    break;
                }
            }
        }
    }

    //Clears data of waypoint at specified index.
    public void RemoveWaypoint(int index)
    {
        waypoints[index].index = 0;
        waypoints[index].bIsEnabled = false;
        waypoints[index].name = "";
        waypoints[index].location = Vector3.zero;
        waypoints[index].color = Color.black;
        Transform markerList = transform.root.GetChild(2);

        for (int i = 0; i < markerList.childCount; i++)
        {
            WaypointUI marker = markerList.GetChild(i).GetComponent<WaypointUI>();
            if (marker.WaypointID == index)
            {
                marker.RemoveWaypoint();
                break;
            }
        }
    }

    public void NewWaypoint()
    {
        //Setting defaults for new waypoint.
        waypointEditR.value = Random.Range(0.0f, 1.0f);
        waypointEditG.value = Random.Range(0.0f, 1.0f);
        waypointEditB.value = Random.Range(0.0f, 1.0f);
        waypointEditName.text = "New Waypoint";
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

        waypointEditorPanel.SetActive(true);
        waypointManagerPanel.SetActive(false);
    }

    public void EditWaypoint(int index)
    {
        bIsEditingWaypoint = true;
        Transform markerList = transform.root.GetChild(2);
        for (int i = 0; i < markerList.childCount; i++)
        {
            WaypointUI marker = markerList.GetChild(i).GetComponent<WaypointUI>();
            if (marker.WaypointID == index)
            {
                //Load data into editor
                waypointEditR.value = waypoints[index].color.r;
                waypointEditG.value = waypoints[index].color.g;
                waypointEditB.value = waypoints[index].color.b;
                waypointEditName.text = waypoints[index].name;
                waypointEditX.text = waypoints[index].location.x.ToString();
                waypointEditY.text = waypoints[index].location.y.ToString();
                waypointEditZ.text = waypoints[index].location.z.ToString();
                waypointEditEnabled.isOn = waypoints[index].bIsEnabled;
                waypointToRemove = index;
                break;
            }
        }

        waypointEditorPanel.SetActive(true);
        waypointManagerPanel.SetActive(false);
    }

    public void SaveWaypoint()
    {
        // TODO make sure user enters valid data.
        Vector3 location = new Vector3(int.Parse(waypointEditX.text), int.Parse(waypointEditY.text), int.Parse(waypointEditZ.text));
        if (bIsEditingWaypoint)
        {
            RemoveWaypoint(waypointToRemove);
            bIsEditingWaypoint = false;
        }
        AddWaypoint(waypointEditName.text, location, waypointColour, waypointEditEnabled.isOn);
        
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