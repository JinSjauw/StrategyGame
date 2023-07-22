using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float velocity;

    //Have a bullet config SO so it can have different characteristics when Object Pooling by reassinging bullet SO!!!
    
    private Vector3 _direction;
    // Start is called before the first frame update
    
    // Update is called once per frame
    private void Update()
    {
        transform.position += _direction * velocity * Time.deltaTime;
    }

    private void DetectCollision()
    {
        //Raycast between last and current position
        //Check for hit in between.
        //Pass Action delegate for bullet SO to decide when its enough.
        //Run Bullet SO Detect(), Check for bool return;
    }
    
    public void Fire(Vector3 direction)
    {
        transform.up = direction;
        _direction = direction;
    }
}
