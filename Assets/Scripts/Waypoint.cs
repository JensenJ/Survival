// Copyright (c) 2019 JensenJ
// NAME: FacePlayer
// PURPOSE: Make game objects face the camera/player

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Waypoint : MonoBehaviour
{
    //Marker settings
    private TextMeshProUGUI distanceMesh;
    private TextMeshProUGUI nameMesh;
    private GameObject backgroundPanel;
    private Image waypointColor;

    private Transform objectToFace = null;
    private Transform playerPos;
    private PlayerController pc = null;
    private float distanceFromPlayer = 10.0f;

    [SerializeField] public int MarkerID = 0;

    //Setup
    void Awake()
    {
        objectToFace = transform.root.GetChild(3).transform;
        playerPos = objectToFace;
        pc = objectToFace.GetComponent<PlayerController>();

        backgroundPanel = transform.GetChild(0).GetChild(0).gameObject;
        distanceMesh = backgroundPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        nameMesh = backgroundPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        waypointColor = backgroundPanel.transform.GetChild(0).GetComponent<Image>();
        
    }

    //Function that is called when other functions, e.g. player controller changes waypoint settings.
    public void SetWaypointSettings(int index, bool m_bIsEnabled, string m_name, Vector3 m_location, Color m_color)
    {
        MarkerID = index;
        backgroundPanel.SetActive(m_bIsEnabled);
        waypointColor.color = m_color;
        transform.position = m_location;
        nameMesh.text = m_name;
    }

    //Removes waypoint by destroying it.
    public void RemoveWaypoint()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (pc.bHasDeployedDrone)
        {
            objectToFace = playerPos.GetChild(3);
        }
        else
        {
            objectToFace = playerPos;
        }
        
        distanceFromPlayer = Mathf.Round(Vector3.Distance(objectToFace.position, transform.position));
        distanceMesh.text = distanceFromPlayer.ToString() + "m";

        if(objectToFace != null)
        {
            transform.LookAt(objectToFace);
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        else
        {
            Debug.LogError("Object to face is null");
        }
    }
}