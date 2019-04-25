using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMenuUI : MonoBehaviour
{
    MenuManager mm;

    void Start()
    { 
        mm = transform.root.GetChild(5).GetComponent<MenuManager>();

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
        mm.Singleplayer(gameObject.name);
    }
}
