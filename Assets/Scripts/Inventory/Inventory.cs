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

    //Delegate for UI
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    //Attributes
    Attributes attributes;
    public float maxWeight = 100;

    //Inventory item list
    public List<Item> items = new List<Item>();

    void Start()
    {
        //Get attribute component and update inventory
        attributes = GetComponent<Attributes>();
        UpdateInventory();
    }

    //Updates inventory data, such as weight
    public void UpdateInventory()
    {
        //max weight calculation
        maxWeight = attributes.maxStaminaMeter / 100.0f * maxWeight; //Max weight value

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
            print("Over inventory weight limit");
            attributes.ChangeStaminaLevel(-attributes.GetStaminaLevel(), false);
            attributes.bIsOverencumbered = true;
        }
        else
        {
            attributes.bIsOverencumbered = false;
        }

        //Call delegate/callback
        if(onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }

    //Loads inventory from save
    public void LoadInventory(string m_name, Sprite m_sprite, float m_weight, float m_value)
    {
        Item tempItem = new Item
        {
            name = m_name,
            itemName = m_name,
            icon = m_sprite,
            weight = m_weight,
            value = m_value
        };

        Add(tempItem);
    }

    //Add item
    public void Add(Item item)
    {
        items.Add(item);
        UpdateInventory();
    }

    //Remove item
    public void Remove(Item item)
    {
        items.Remove(item);
        UpdateInventory();
    }
}
