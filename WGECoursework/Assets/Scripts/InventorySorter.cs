using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySorter : MonoBehaviour
{
    // Comparison function to use
    delegate int Comparer(InventoryItem a, InventoryItem b);

    public static List<InventoryItem> Sort(List<InventoryItem> inventory, SortInventoryBy by, SortInventoryOrder order)
    {
        // First create the comparer delegate depending on how we want to sort it
        Comparer comparer = null;
        if (by == SortInventoryBy.NAME)
        {
            if (order == SortInventoryOrder.ASCENDING) comparer = new Comparer(CompareByAZAsc);
            if (order == SortInventoryOrder.DESCENDING) comparer = new Comparer(CompareByAZDsc);
        }
        else if (by == SortInventoryBy.QUANTITY)
        {
            if (order == SortInventoryOrder.ASCENDING) comparer = new Comparer(CompareByQAsc);
            if (order == SortInventoryOrder.DESCENDING) comparer = new Comparer(CompareByQDsc);
        }

        return MergeSort(inventory, comparer);
    }

    static List<InventoryItem> MergeSort(List<InventoryItem> inventory, Comparer comparer)
    {
        List<InventoryItem> left = new List<InventoryItem>();
        List<InventoryItem> right = new List<InventoryItem>();
        List<InventoryItem> result = new List<InventoryItem>();

        // avoid infinite recursion
        if (inventory.Count <= 1) return inventory;

        // where to split the list
        int midpoint = inventory.Count / 2;

        // populate both sides
        for (int i = 0; i < inventory.Count; i++)
        {
            if (i < midpoint) left.Add(inventory[i]);
            else right.Add(inventory[i]);
        }

        // recursively sort both sides
        left = MergeSort(left, comparer);
        right = MergeSort(right, comparer);

        // merge both together
        result = Merge(left, right, comparer);
        return result;
    }

    static List<InventoryItem> Merge(List<InventoryItem> left, List<InventoryItem> right, Comparer comparer)
    {
        InventoryItem[] result = new InventoryItem[left.Count + right.Count];

        // indicies for either side
        int iLeft, iRight, iResult;
        iLeft = iRight = iResult = 0;

        // while either side still has elements
        while (iLeft < left.Count || iRight < right.Count)
        {
            // if both the sides have elements
            if (iLeft < left.Count && iRight < right.Count)
            {
                // compare both elements using the provided delegate
                InventoryItem a = left[iLeft];
                InventoryItem b = right[iRight];
                int comparison = comparer(a, b);

                // move the element from either the left or right side to the results
                if (comparison <= 0)
                {
                    result[iResult] = left[iLeft];
                    iLeft++;
                    iResult++;
                } else if (comparison > 0)
                {
                    result[iResult] = right[iRight];
                    iRight++;
                    iResult++;
                }
            } else if (iLeft < left.Count)
            {
                // if only the left side is left with elements, that's our completed merge
                result[iResult] = left[iLeft];
                iLeft++;
                iResult++;
            } else if (iRight < right.Count)
            {
                // if only the right side is left with elements, that's our completed merge
                result[iResult] = right[iRight];
                iRight++;
                iResult++;
            }
        }

        return new List<InventoryItem>(result);
    }

    static int CompareByAZAsc(InventoryItem a, InventoryItem b) { return string.Compare(b.type.ToString(), a.type.ToString()); }
    static int CompareByAZDsc(InventoryItem a, InventoryItem b) { return string.Compare(a.type.ToString(), b.type.ToString()); }
    static int CompareByQAsc(InventoryItem a, InventoryItem b)
    {
        if (a.count < b.count) return -1;
        else if (a.count > b.count) return 1;

        // they're equal
        return 0;
    }

    static int CompareByQDsc(InventoryItem a, InventoryItem b)
    {
        if (a.count < b.count) return 1;
        else if (a.count > b.count) return -1;

        // they're equal
        return 0;
    }
}
