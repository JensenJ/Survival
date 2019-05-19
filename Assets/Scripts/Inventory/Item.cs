// Copyright (c) 2019 JensenJ
// NAME: Item
// PURPOSE: 

using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;
    public int weight;
    public int value;
}
