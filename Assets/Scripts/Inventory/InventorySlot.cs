// Copyright (c) 2019 JensenJ
// NAME: InventorySlot
// PURPOSE: Manages inventory slot

using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    Inventory inventory;

    Item item;
    Image icon;
    Image delBtnIcon;

    Button delBtn;
    Button mainBtn;

    public int slotID = 0;

    void MainButtonPressed()
    {
        //Equip item into hand
    }

    void DelButtonPressed()
    {
        inventory.Remove(item, slotID);
        //TODO: Drop Item in World
    }

    void Start()
    {
        slotID = transform.GetSiblingIndex();
        inventory = Inventory.instance;
        icon = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        delBtnIcon = transform.GetChild(1).GetComponent<Image>();

        //Buttons
        mainBtn = transform.GetChild(0).GetComponent<Button>();
        delBtn = transform.GetChild(1).GetComponent<Button>();

        mainBtn.onClick.AddListener(MainButtonPressed);
        delBtn.onClick.AddListener(DelButtonPressed);
    }

    public void AddItem(Item m_Item)
    {
        item = m_Item;
        icon.sprite = item.icon;
        icon.enabled = true;
        delBtnIcon.enabled = true;
        delBtn.interactable = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        delBtnIcon.enabled = false;
        delBtn.interactable = false;
    }
}
