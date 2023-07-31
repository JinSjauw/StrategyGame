using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementAnimation", menuName = "Animations/Movement")]
public class MovementAnimation : ScriptableObject
{
    [SerializeField] private AnimationCurve movementCurve;
    [SerializeField] private AnimationCurve bounceCurve;
    [SerializeField] private AnimationCurve rotationCurve;
    
    public AnimationCurve MovementCurve { get => movementCurve; }
    
    public void PlayMoveAnimation(float evaluator, SpriteRenderer animTarget)
    {
        if (evaluator < 0.1f)
        {
            evaluator = 0;
        }
        //Bounce
        animTarget.transform.parent.localPosition = new Vector2(0, bounceCurve.Evaluate(evaluator));
        //Wobble
        animTarget.transform.parent.localRotation = Quaternion.Euler(0,0,rotationCurve.Evaluate(evaluator) * 5f);
    }
}
