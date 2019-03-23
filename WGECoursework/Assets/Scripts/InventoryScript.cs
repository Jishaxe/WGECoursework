using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InventoryItem
{
    public int count;
    public Block type;
}

public enum SortInventoryBy { NAME, QUANTITY }
public enum SortInventoryOrder { DESCENDING, ASCENDING }

public class InventoryScript : MonoBehaviour
{
    List<InventoryItem> inventory = new List<InventoryItem>();
    HotbarScript hotbar;

    public void Start()
    {
        DroppedCubeScript.OnDroppedCubePickup += AddBlock;
        PlayerScript.OnBlockPlacement += OnBlockPlacement;
        HotbarScript.OnSortInventory += OnSortInventory;

        hotbar = GetComponent<HotbarScript>();
    }
    
    void OnSortInventory(SortInventoryBy by, SortInventoryOrder order)
    {
        inventory = InventorySorter.Sort(inventory, by, order);

        hotbar.UpdateInventory(inventory);
    }

    public void AddBlock(Block type)
    {
        bool alreadyExists = false;
        // First check if we already have an entry for this blocktype
        foreach (InventoryItem item in inventory)
        {
            if (item.type == type)
            {
                // If we do, increment it and return
                item.count++;
                alreadyExists = true;
            }
        }

        if (!alreadyExists)
        {
            // We have only reached here if there isn't already an entry for the block so add the entry
            InventoryItem newItem = new InventoryItem();
            newItem.count = 1;
            newItem.type = type;
            inventory.Add(newItem);
        }

        hotbar.UpdateInventory(inventory);
    }

    public int GetBlockCount(Block type)
    {
        foreach (InventoryItem item in inventory)
        {
            if (item.type == type) return item.count;
        }

        return 0;
    }

    void OnBlockPlacement(int blockType, Vector3 position)
    {
        RemoveBlock((Block)blockType - 1);
    }

    public void RemoveBlock(Block type)
    {
        InventoryItem toRemove = null;

        // First check if we already have an entry for this blocktype
        foreach (InventoryItem item in inventory)
        {
            if (item.type == type)
            {
                // If we do, decrement it
                item.count--;

                // If we've hit zero on this item remove it from the list alltogether
                if (item.count == 0) toRemove = item;
            }
        }
        
        if (toRemove != null) inventory.Remove(toRemove);

        hotbar.UpdateInventory(inventory);
    }
}
