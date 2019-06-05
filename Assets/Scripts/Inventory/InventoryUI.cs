// Copyright (c) 2019 JensenJ
// NAME: InventoryUI
// PURPOSE: Manages Inventory UI

using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    Inventory inventory;
    InventorySlot[] slots;

    public void UpdateUI()
    {
        inventory = Inventory.instance;
        slots = transform.GetChild(1).GetChild(0).GetChild(0).GetComponentsInChildren<InventorySlot>();

        for (int i = 0; i < slots.Length; i++)
        {
            if(i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
