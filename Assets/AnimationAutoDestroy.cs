using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationAutoDestroy : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        float time = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        Debug.Log("Explosion Time: " + time);
        Destroy(transform.root.gameObject, time);   
    }
}
