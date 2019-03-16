using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarScript : MonoBehaviour
{
    public delegate void HotbarEvent(int selected);
    public static event HotbarEvent OnHotbarSelectionChanged;

    public int currentlySelected = 0;
    public GameObject selector;

    Vector2 selectorInitialPosition;
    public float selectorWidth;

    public InventoryItemScript item1;
    public InventoryItemScript item2;
    public InventoryItemScript item3;
    public InventoryItemScript item4;

    // Start is called before the first frame update
    void Start()
    {
        selectorInitialPosition = selector.GetComponent<RectTransform>().anchoredPosition;
    }

    public void UpdateInventory(List<InventoryItem> items)
    {
        // Clear all the items
        item1.UpdateItem(Block.DIRT, 0);
        item2.UpdateItem(Block.DIRT, 0);
        item3.UpdateItem(Block.DIRT, 0);
        item4.UpdateItem(Block.DIRT, 0);

        try
        {
            item1.UpdateItem(items[0].type, items[0].count);
            item2.UpdateItem(items[1].type, items[1].count);
            item3.UpdateItem(items[2].type, items[2].count);
            item4.UpdateItem(items[3].type, items[3].count);
        } catch (ArgumentOutOfRangeException) { }

        SendHotbarSelectionChangedEvent();
    }

    // Update is called once per frame
    void Update()
    {
        // Move the selector according to the currently selected int
        selector.GetComponent<RectTransform>().anchoredPosition = new Vector2(selectorInitialPosition.x + (selectorWidth * currentlySelected), selectorInitialPosition.y);

        // Keycode controls
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeSelection(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeSelection(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeSelection(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeSelection(3);

        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel < 0) SelectNext();
        if (scrollWheel > 0) SelectPrevious();
    }

    void SendHotbarSelectionChangedEvent()
    {
        int sel = 0;

        switch (currentlySelected)
        {
            case 0:
                if (item1.amount > 0) sel = (int)(item1.blockType) + 1;
                break;
            case 1:
                if (item2.amount > 0) sel = (int)(item2.blockType) + 1;
                break;
            case 2:
                if (item3.amount > 0) sel = (int)(item3.blockType) + 1;
                break;
            case 3:
               if (item4.amount > 0) sel = (int)(item4.blockType) + 1;
               break;
        }

        OnHotbarSelectionChanged?.Invoke(sel);
    }

    public void SelectNext()
    {
        currentlySelected++;
        if (currentlySelected > 3) currentlySelected = 0;
        SendHotbarSelectionChangedEvent();
    }

    public void SelectPrevious()
    {
        currentlySelected--;
        if (currentlySelected < 0) currentlySelected = 3;
        SendHotbarSelectionChangedEvent();
    }

    public void ChangeSelection(int newSelection)
    {
        currentlySelected = newSelection;
        SendHotbarSelectionChangedEvent();
    }
}
