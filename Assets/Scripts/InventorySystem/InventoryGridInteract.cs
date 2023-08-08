using UnityEngine;
using UnityEngine.EventSystems;

namespace InventorySystem.Grid
{
    public class InventoryGridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private InventoryController _inventoryController;
        private InventoryGrid _inventoryGrid;

        private void Awake()
        {
            _inventoryController = FindObjectOfType<InventoryController>();
            _inventoryGrid = GetComponent<InventoryGrid>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _inventoryController.SetInventory(_inventoryGrid);
            //Debug.Log("Entered New Inventory: " + gameObject.name);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("Exited Old Inventory: " + gameObject.name);
            //_inventoryController.ClearInventory();
        }
    }
}

