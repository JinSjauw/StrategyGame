using System.Collections;
using System.Collections.Generic;
using InventorySystem.Grid;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryHighlighter : MonoBehaviour
    {
        [SerializeField] private RectTransform _highlight;

        public void Show(bool state)
        {
            _highlight.gameObject.SetActive(state);
        }
        
        public void SetSize(ItemContainer itemContainer)
        {
            Vector2 size = new Vector2();
            size.x = itemContainer.GetWidth() * InventoryGrid.TileSizeWidth;
            size.y = itemContainer.GetHeight() * InventoryGrid.TileSizeHeight;
            _highlight.sizeDelta = size;
        }

        public void SetPosition(InventoryGrid targetGrid, ItemContainer targetContainer)
        {
            _highlight.SetParent(targetGrid.GetGridRect());
            _highlight.SetAsLastSibling();
            _highlight.localPosition = targetGrid.CalculateContainerPosition(targetContainer, targetContainer.GetGridposition());
        }

        public void SetPosition(InventoryGrid targetGrid, ItemContainer targetContainer, GridPosition gridPosition)
        {
            _highlight.SetParent(targetGrid.GetGridRect());
            _highlight.SetAsLastSibling();
            _highlight.localPosition = targetGrid.CalculateContainerPosition(targetContainer, gridPosition);
        }
    }
}
