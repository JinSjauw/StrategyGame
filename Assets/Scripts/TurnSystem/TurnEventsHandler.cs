using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Player Event Handler")]
public class TurnEventsHandler : ScriptableObject
{
    public event UnityAction OnPlayerAction = delegate {  };
    public event UnityAction OnTurnAdvanced = delegate {  };
    
    public void PlayerActed()
    {
        OnPlayerAction.Invoke();
    }

    public void TurnAdvanced()
    {
        OnTurnAdvanced.Invoke();
    }
}
