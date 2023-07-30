using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    //Subscribe to a few events
    //Also invoke an event that broadcasts a turn advancement
    [SerializeField] private TurnEventsHandler turnEventsHandler;
    
    private int _turnNumber;
    // Start is called before the first frame update
    void Start()
    {
        turnEventsHandler.OnPlayerAction += AdvanceTurn;
    }

    private void AdvanceTurn()
    {
        turnEventsHandler.TurnAdvanced();
        //Debug.Log("Advanced Turn: " + _turnNumber);
        _turnNumber++;
    }
}
