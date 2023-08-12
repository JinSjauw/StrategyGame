using UnityEngine;
using UnityEngine.Events;

public class FakeHeightObject : MonoBehaviour {

    public UnityEvent onGroundHitEvent;

    public Sprite stickSprite;
    
    public Transform parentObject;
    public Transform body;
    public Transform shadow;

    public float gravity = -20;
    public float velocity;
    public Vector2 direction;
    public float verticalVelocity;
    private float lastIntialVerticalVelocity;

    public bool isGrounded;

    void Update(){
        UpdatePosition();
        CheckGroundHit();   
    }
    
    public void Initialize(Vector2 direction, float verticalVelocity, float velocity = 1){

        Debug.Log("Splatter Spawned!");
        isGrounded = false;
        this.velocity = velocity;
        this.direction = direction;
        this.verticalVelocity = verticalVelocity;
        lastIntialVerticalVelocity = verticalVelocity;
    }

    void UpdatePosition(){

        if(!isGrounded){
            verticalVelocity += gravity * Time.deltaTime;
            body.position += new Vector3(0, verticalVelocity, 0) * (velocity * Time.deltaTime);
        }
        
        parentObject.position += (Vector3)direction * (velocity * Time.deltaTime);

        if (shadow != null)
        {
            shadow.position = parentObject.position;
        }
    }

    void CheckGroundHit(){

        if(body.position.y < parentObject.position.y && !isGrounded){

            body.position = parentObject.position;
            isGrounded = true;
            GroundHit();
        }

    }

    void GroundHit(){
        onGroundHitEvent.Invoke();
    }

    public void Stick(){
        direction = Vector2.zero;
        //Set body sprite to stick sprite
        body.parent = null;
        Destroy(gameObject);
    }

    public void Bounce(float divisionFactor){
        Initialize(direction, lastIntialVerticalVelocity / divisionFactor, 1);
    }
    
    public void SlowDownGroundVelocity(float divisionFactor){
        direction = direction / divisionFactor;
    }

    public void Destroy(float timeToDestruction){

        Destroy(gameObject, timeToDestruction);

    }



}