using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

public class TestItem : MonoBehaviour
{
    [SerializeField] private BaseItem item;
    // Start is called before the first frame update
    void Start()
    {
        item.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
