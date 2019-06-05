// Copyright (c) 2019 JensenJ
// NAME: InventoryData
// PURPOSE: Holds inventory data for saving and loading

using UnityEngine;

[System.Serializable]
public class InventoryData
{
    //Item values
    public string[] itemName;
    public string[] spriteName;
    public float[] weight;
    public float[] value;

    public InventoryData(Inventory inventory)
    {
        //Init of arrays
        itemName = new string[inventory.items.Count];
        spriteName = new string[inventory.items.Count];
        weight = new float[inventory.items.Count];
        value = new float[inventory.items.Count];

        //Setting values in arrays
        for (int i = 0; i < inventory.items.Count; i++)
        {
            itemName[i] = inventory.items[i].itemName;
            spriteName[i] = inventory.items[i].icon.texture.name;
            weight[i] = inventory.items[i].weight;
            value[i] = inventory.items[i].value;
        }
    }
}
