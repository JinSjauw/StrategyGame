using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSortingLayer : MonoBehaviour
{
    [SerializeField] private string sortingLayer;
    [SerializeField] private int sortingOrder;
    private MeshRenderer _renderer;
    
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        SetLayer();
    }

    void SetLayer()
    {
        _renderer.sortingLayerName = sortingLayer;
        _renderer.sortingOrder = sortingOrder;
    }
}
