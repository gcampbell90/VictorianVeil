using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public InventoryController InventoryController { get; private set; }

    public List<BaseInventoryItem> inventoryItems;

    public TextMeshPro textMeshPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //Set up inventory controller
        InventoryController = new InventoryController(this);

        InventoryController.textMesh = Instantiate(textMeshPrefab);
    }

    private void OnDisable()
    {
        InventoryController.Dispose();
    }
}
