using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMenuUI : MonoBehaviour
{
    WorldManager wm;

    void Start()
    { 
        wm = transform.root.GetChild(5).GetComponent<WorldManager>();

        //Listeners for buttons.
        Button playBtn = transform.GetChild(2).GetComponent<Button>();
        playBtn.onClick.AddListener(PlayButtonPressed);

        Button delBtn = transform.GetChild(1).GetComponent<Button>();
        delBtn.onClick.AddListener(DeleteButtonPressed);
    }

    //Button Listener functions
    void DeleteButtonPressed()
    {
        Destroy(gameObject);
    }

    void PlayButtonPressed()
    {
        
    }
}
