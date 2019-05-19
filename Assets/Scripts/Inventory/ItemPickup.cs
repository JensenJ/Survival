// Copyright (c) 2019 JensenJ
// NAME: ItemPickup
// PURPOSE: Class for managing pickups

using UnityEngine;

public class ItemPickup : Interactable
{
    //Item data
    public Item item;

    public override void Interact()
    {
        //base.Interact();

        Pickup();
    }

    //Picks up item and adds to inventory
    void Pickup()
    {
        Inventory.instance.Add(item);
        Destroy(gameObject);
    }
}
