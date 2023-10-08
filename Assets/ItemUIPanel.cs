using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class ItemUIPanel : MonoBehaviour
{
    //Debug
    [SerializeField] int rows = 10;  // predefined number of cells in the X direction
    [SerializeField] int cols = 10;  // predefined number of cells in the Y direction

    Dictionary<int, ItemSlot> gridPositions = new Dictionary<int, ItemSlot>();

    private void Awake()
    {
        SetGridPositions();
        var follow = gameObject.AddComponent<LazyFollow>();
        follow.targetOffset = new Vector3(0.5f, 0.2f, 1f);
    }

    public void SetGridPositions()
    {
        Vector3 quadSize = transform.localScale;

        // Determine the cell size based on the smaller side of the quad and the greater of rows/cols
        float cellSize = Mathf.Min(quadSize.x / cols, quadSize.y / rows);

        // Calculate the total width and height of the grid to correctly position cells.
        float totalWidth = cols * cellSize;
        float totalHeight = rows * cellSize;

        // Start position is adjusted based on the total grid width and height.
        Vector3 startPosition = transform.position - new Vector3(totalWidth * 0.5f, totalHeight * 0.5f, 0);

        int counter = 0;
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                var position = startPosition + new Vector3(i * cellSize + cellSize / 2, j * cellSize + cellSize / 2, 0);

                ItemSlot itemSlot = new ItemSlot()
                {
                    Index = counter,
                    IsOccupied = false,
                    Position = transform.InverseTransformPoint(position)
                };

                gridPositions.Add(counter, itemSlot);
                counter++;
            }
        }
    }

    internal ItemSlot GetItemSlot()
    {
        ItemSlot m_itemSlot = null;
        foreach (var item in gridPositions)
        {
            if (!item.Value.IsOccupied)
            {
                item.Value.IsOccupied = true;
                m_itemSlot = item.Value;
                break;
            }
        }
        Debug.Log($"Giving Item slot {m_itemSlot.Index}");
        return m_itemSlot;
    }

    internal void Remove(InventoryUIItem item)
    {
        gridPositions[item.UISlotIndex].IsOccupied = false;
        Debug.Log($"Removing item {item.UISlotIndex}");
    }

    private void OnDrawGizmos()
    {
        Vector3 quadSize = transform.localScale;

        // Determine the cell size based on the smaller side of the quad and the greater of rows/cols
        float cellSize = Mathf.Min(quadSize.x / cols, quadSize.y / rows);

        // Calculate the total width and height of the grid to correctly position cells.
        float totalWidth = cols * cellSize;
        float totalHeight = rows * cellSize;

        // Start position is adjusted based on the total grid width and height.
        Vector3 startPosition = transform.position - new Vector3(totalWidth * 0.5f, totalHeight * 0.5f, 0);

        int counter = 0;
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                counter++;
                var pos = startPosition + new Vector3(i * cellSize + cellSize / 2, j * cellSize + cellSize / 2, 0);
                Gizmos.DrawWireCube(pos, new Vector3(cellSize, cellSize, 0.01f));
                Handles.Label(pos, $"{counter} Inventory Slot");
            }
        }

        foreach (var slot in gridPositions)
        {
            Gizmos.DrawWireCube(transform.TransformPoint(slot.Value.Position), new Vector3(cellSize, cellSize, 0.01f));
        }
    }
}

public class ItemSlot
{
    public int Index { get; set; }
    public Vector3 Position { get; set; }
    public bool IsOccupied { get; set; }
}

