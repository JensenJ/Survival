// Copyright (c) 2019 JensenJ
// NAME: Interactable
// PURPOSE: Handles interactable objects

using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3.0f;

    //Meant to be overwritten by child methods
    public virtual void Interact()
    {
        Debug.Log("Interacting with " + transform.name);
    }
}
