using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    private TileGridObject _gridObject;
    [SerializeField] private TextMeshPro textElement;
    public void SetGridObject(TileGridObject gridObject)
    {
        _gridObject = gridObject;
    }

    private void Update()
    {
        textElement.text = _gridObject.ToString();
    }
}
