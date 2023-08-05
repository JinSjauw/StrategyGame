using System.Collections;
using System.Collections.Generic;
using CustomInput;
using InventorySystem.Grid;
using UnityEngine;

public class InventoryController : MonoBehaviour
{

    [SerializeField] private InventoryGrid selectedInventoryGrid;

    private InputReader _inputReader;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void OnMouseClickStart(object sender, ClickEventArgs e)
    {
        if (selectedInventoryGrid == null)
        {
            Debug.Log(" NO INVENTORY GRID");
            return;
        }
        Debug.Log(selectedInventoryGrid.GetGridPosition(e.m_Target));
        
        Debug.Log(selectedInventoryGrid.GetInventorySlot(selectedInventoryGrid.GetGridPosition(e.m_Target)).m_GridPosition);
    }

    private void OnMouseClickEnd(object sender, ClickEventArgs e)
    {
        if (selectedInventoryGrid == null)
        {
            Debug.Log(" NO INVENTORY GRID");
            return;
        }
        
        Debug.Log(selectedInventoryGrid.GetGridPosition(e.m_Target));
    }

    #region Public

    public void Initialize(InputReader inputReader)
    {
        _inputReader = inputReader;
        
        _inputReader.InventoryClickStartEvent += OnMouseClickStart;
        _inputReader.InventoryClickEndEvent += OnMouseClickEnd;
    }

    public void SetInventory(InventoryGrid inventoryGrid)
    {
        selectedInventoryGrid = inventoryGrid;
    }

    public void ClearInventory()
    {
        selectedInventoryGrid = null;
    }
    
    #endregion
    
   
    
}
