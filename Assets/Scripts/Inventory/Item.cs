// Copyright (c) 2019 JensenJ
// NAME: Item
// PURPOSE: Item base class

using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    //All item data
    public string itemName = "New Item";
    public Sprite icon = null;
    public float weight = 0;
    public float value = 0;
}
