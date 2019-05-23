// Copyright (c) 2019 JensenJ
// NAME: Inventory
// PURPOSE: Manages which items are currently in player inventory

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory instance;

    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than one instance of inventory found.");
            return;
        }
        instance = this;
    }

    #endregion

    InventoryUI ui;

    //Attributes
    Attributes attributes;
    public float maxWeight = 100;
    public float numberOfSlots = 20;

    //Inventory item list
    public List<Item> items = new List<Item>();

    void Start()
    {
        //Get attribute component and update inventory
        attributes = GetComponent<Attributes>();
        ui = transform.root.GetChild(1).GetChild(6).GetComponent<InventoryUI>();
    }

    //Updates inventory data, such as weight and number of slots
    public void UpdateInventory()
    {
        //max weight calculation
        maxWeight = attributes.maxStaminaMeter / 100.0f * maxWeight; //Max weight value
        numberOfSlots = attributes.maxStaminaMeter / 100.0f * numberOfSlots; //Number of slots

        //Adds up all weight in inventory
        float currentWeight = 0;
        for (int i = 0; i < items.Count; i++)
        {
            currentWeight += items[i].weight;
        }

        //Checks if current weight exceeds weight limit
        if(currentWeight > maxWeight)
        {
            //Changes stamina level to prevent running
            attributes.ChangeStaminaLevel(-attributes.GetStaminaLevel(), false);
            attributes.bIsOverencumbered = true;
        }
        else
        {
            attributes.bIsOverencumbered = false;
        }

        //Updare inventory ui
        ui.UpdateUI();
    }

    //Loads inventory from save
    public void LoadInventory(string m_name, Sprite m_sprite, float m_weight, float m_value)
    {
        //Creates temporary item for adding to inventory, using paramaters for function
        Item tempItem = (Item)ScriptableObject.CreateInstance(typeof(Item));
        tempItem.name = m_name;
        tempItem.itemName = m_name;
        tempItem.icon = m_sprite;
        tempItem.weight = m_weight;
        tempItem.value = m_value;
        Add(tempItem);
    }

    //Add item
    public bool Add(Item item)
    {
        if(items.Count >= numberOfSlots)
        {
            Debug.Log("Not enough room");
            return false;
        }
        items.Add(item);
        UpdateInventory();
        return true;
    }

    //Remove item
    public void Remove(Item item, int ID)
    {
        if (items.Contains(item))
        {
            items.RemoveAt(ID);
        }
        UpdateInventory();
    }
}
