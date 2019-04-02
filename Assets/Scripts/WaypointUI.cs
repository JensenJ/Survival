// Copyright (c) 2019 JensenJ
// NAME: Waypoint
// PURPOSE: Responsible for waypoint menu and in game UI

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaypointUI : MonoBehaviour
{
    //Waypoint widget settings (Game World)
    private TextMeshProUGUI gameDistanceMesh;
    private TextMeshProUGUI gameNameMesh;
    private GameObject gameBackgroundPanel;
    private Image gameWaypointColor;

    //Waypoint manager settings
    private GameObject managerContentPanel;
    private TextMeshProUGUI managerLocationMesh;
    private TextMeshProUGUI managerNameMesh;
    private Image managerWaypointColor;
    private GameObject waypointMenuWidget = null;

    //Other useful variables
    private Transform objectToFace = null;
    private Transform playerPos;
    private PlayerController pc = null;
    private float distanceFromPlayer = 10.0f;
    WaypointManager wp;

    //Settings for individual waypoints
    [SerializeField] private GameObject waypointMenuWidgetPrefab = null;
    [SerializeField] [Range(0.01f, 20f)] private float gameWidgetScale = 2.0f;
    public int WaypointID = 0;

    //Button Listener functions
    void DeleteButtonPressed()
    {
        wp.RemoveWaypoint(WaypointID);
    }

    void EditButtonPressed()
    {
        wp.EditWaypoint(WaypointID);
    }

    void DownButtonPressed()
    {
        wp.Rearrange(true, WaypointID);
    }

    void UpButtonPressed()
    {
        wp.Rearrange(false, WaypointID);
    }
    
    //Setup
    void Awake()
    {
        objectToFace = transform.root.GetChild(3).transform;
        playerPos = objectToFace;
        pc = objectToFace.GetComponent<PlayerController>();

        //Where each ui element is in relation to the hierarchy.
        gameBackgroundPanel = transform.GetChild(0).GetChild(0).gameObject;
        gameDistanceMesh = gameBackgroundPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        gameNameMesh = gameBackgroundPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        gameWaypointColor = gameBackgroundPanel.transform.GetChild(0).GetComponent<Image>();

        managerContentPanel = transform.root.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(0).gameObject;
        wp = transform.parent.GetComponent<WaypointManager>();
    }

    //Function that is called when other functions, e.g. waypoint manager changes waypoint settings.
    public void SetWaypointSettings(int index, bool m_bIsEnabled, string m_name, Vector3 m_location, Color m_color)
    {
        //Waypoint settings
        WaypointID = index;
        gameBackgroundPanel.SetActive(m_bIsEnabled);
        gameWaypointColor.color = m_color;
        transform.position = m_location;
        gameNameMesh.text = m_name;

        //Instantiate waypoint menu widget
        waypointMenuWidget = Instantiate(waypointMenuWidgetPrefab, m_location, Quaternion.identity, managerContentPanel.transform);
        //Color
        managerWaypointColor = waypointMenuWidget.transform.GetChild(0).GetComponent<Image>();
        managerWaypointColor.color = m_color;

        //Name
        managerNameMesh = waypointMenuWidget.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        if (m_bIsEnabled)
        {
            managerNameMesh.text = m_name + "(enabled)";
        }
        else
        {
            managerNameMesh.text = m_name + "(disabled)";
        }

        //Location
        managerLocationMesh = waypointMenuWidget.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        managerLocationMesh.text = Mathf.Round(m_location.x).ToString() + ", " + Mathf.Round(m_location.y).ToString() + ", " + Mathf.Round(m_location.z).ToString();

        //Listeners for buttons.
        Button editBtn = waypointMenuWidget.transform.GetChild(3).GetComponent<Button>();
        editBtn.onClick.AddListener(EditButtonPressed);

        Button delBtn = waypointMenuWidget.transform.GetChild(4).GetComponent<Button>();
        delBtn.onClick.AddListener(DeleteButtonPressed);

        Button downBtn = waypointMenuWidget.transform.GetChild(5).GetComponent<Button>();
        downBtn.onClick.AddListener(DownButtonPressed);

        Button upBtn = waypointMenuWidget.transform.GetChild(6).GetComponent<Button>();
        upBtn.onClick.AddListener(UpButtonPressed);
    }

    //Removes waypoint by destroying it.
    public void RemoveWaypoint()
    {
        Destroy(gameObject);
        if(waypointMenuWidget != null)
        {
            Destroy(waypointMenuWidget);
        }
    }

    // Waypoint game widget, facing objects and scaling based on distance
    void Update()
    {
        //Get object to face
        if (pc.bHasDeployedDrone)
        {
            objectToFace = playerPos.GetChild(3);
        }
        else
        {
            objectToFace = playerPos;
        }
        
        //Calculate distance and use this to scale ui objects as they get further away.
        distanceFromPlayer = Vector3.Distance(objectToFace.position, transform.position);
        gameDistanceMesh.text = Mathf.Round(distanceFromPlayer).ToString() + "m";
        transform.localScale = new Vector3(distanceFromPlayer / gameWidgetScale, distanceFromPlayer / gameWidgetScale, 1);

        //Null pointer exception prevention
        if(objectToFace != null)
        {
            transform.LookAt(objectToFace);
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        else
        {
            //Log error otherwise
            Debug.LogError("WaypointUI: Object to face is null");
        }
    }
}