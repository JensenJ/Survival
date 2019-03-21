// Copyright (c) 2019 JensenJ
// NAME: WaypointManager
// PURPOSE: Manages the waypoint system

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaypointManager : MonoBehaviour
{

    [SerializeField] int MaxWaypointAmount = 16;
    [SerializeField] private GameObject waypointPrefab = null;
    [SerializeField] public GameObject waypointManagerPanel = null;
    [SerializeField] private PlayerController pc = null;
    //[SerializeField] private DroneController dc = null;
    [SerializeField] private Vector3 targetTransform = Vector3.zero;

    //Waypoint editor
    [SerializeField] public GameObject waypointEditorPanel = null;
    [SerializeField] private TMP_InputField waypointEditName;
    [SerializeField] private TMP_InputField waypointEditX;
    [SerializeField] private TMP_InputField waypointEditY;
    [SerializeField] private TMP_InputField waypointEditZ;
    [SerializeField] private Slider waypointEditR;
    [SerializeField] private Slider waypointEditG;
    [SerializeField] private Slider waypointEditB;
    [SerializeField] private Image waypointEditColour;
    [SerializeField] private Toggle waypointEditEnabled;
    [SerializeField] private Color waypointColour;

    int waypointToRemove = 0;
    bool bIsEditingWaypoint = false;
    bool bIsWaypointEnabled = true;

    private GameObject spawnedDrone = null;

    [SerializeField] private Waypoint[] waypoints;

    // Start is called before the first frame update
    void Start()
    {
        //Sets array max length for waypoints
        waypoints = new Waypoint[MaxWaypointAmount];
        pc = transform.root.GetChild(3).GetComponent<PlayerController>();
        waypointManagerPanel = transform.root.GetChild(1).GetChild(1).gameObject;
        waypointEditorPanel = transform.root.GetChild(1).GetChild(2).gameObject;

        waypointEditName = waypointEditorPanel.transform.GetChild(1).GetComponent<TMP_InputField>();

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
            //Checks whether the space is free for a waypoint
            if (i.bIsEnabled == false)
            {
                bHasFilled = true;
                //Sets variables for waypoint
                waypoints[iteration].index = iteration;
                waypoints[iteration].bIsEnabled = bIsEnabled;
                waypoints[iteration].color = m_color;
                waypoints[iteration].location = m_location;
                waypoints[iteration].name = m_name;
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
            if (marker.MarkerID == index)
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
            if (marker.MarkerID == index)
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

    public void CloseWaypointManager()
    {
        waypointManagerPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        pc.bCanMove = true;
        if (pc.bHasDeployedDrone)
        {
            pc.spawnedDrone.GetComponent<DroneController>().bCanMove = true;
        }
    }

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