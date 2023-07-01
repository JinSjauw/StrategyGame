using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionState
{
   Started = 0,
   Performing = 1,
   Completed = 2,
}

public interface IAction
{
   //Gets called in the Update Method
   ActionState Execute();
}
