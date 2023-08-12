using UnityEngine;
using GD.MinMaxSlider;

public class DebrisDispenser : MonoBehaviour
{
    public GameObject debrisParticle;
    public GameObject debrisMark;

    public Color debrisParticleColor;
    public Color debrisMarkColor;

    [MinMaxSlider(0,300)]
    public Vector2 debrisGroundVelocityRange;
    [MinMaxSlider(0,300)]
    public Vector2 debrisVerticalVelocityRange;

    [MinMaxSlider(.5f,32)]
    public Vector2 debrisParticleSizeRange;
    [MinMaxSlider(1,64)]
    public Vector2 debrisMarkSizeRange;

    [MinMaxSlider(0,30)]
    public Vector2Int debrisAmountRange;

    public int debrisDispenseDegrees;

    public Vector2 moveDirection;
    public Vector2 lastPosition;
    private Vector2 debrisDirection = new Vector2();

    void Update(){
        moveDirection = ((Vector2)transform.position - lastPosition).normalized;
    }

    void LateUpdate(){
        lastPosition = transform.position;
    }

    public void DispenseDebris(Vector2 direction = new Vector2()){

        int debrisAmount = Random.Range(debrisAmountRange.x, debrisAmountRange.y);

        if(debrisMark != null){
                GameObject instantiatedDebrisMark = Instantiate(debrisMark, transform.position, Quaternion.identity);
                instantiatedDebrisMark.GetComponent<SpriteRenderer>().color = debrisMarkColor;
                instantiatedDebrisMark.transform.localScale = Vector3.one * Random.Range(debrisMarkSizeRange.x, debrisMarkSizeRange.y);
        }

        
        for(int i = 0; i < debrisAmount; i++){

            FakeHeightObject instantiatedDebrisParticle = Instantiate(debrisParticle, transform.position, Quaternion.identity).GetComponent<FakeHeightObject>();
            instantiatedDebrisParticle.transform.localScale = Vector3.one * Random.Range(debrisParticleSizeRange.x, debrisParticleSizeRange.y);
            instantiatedDebrisParticle.body.GetComponent<SpriteRenderer>().color = debrisParticleColor;

            Vector2 directionToDispense;
            if (direction == Vector2.zero)
            {
                directionToDispense = moveDirection;
            }
            else
            {
                directionToDispense = direction;
            }
            
            float randomizedDirectionAngle = Mathf.Atan2(directionToDispense.y, directionToDispense.x) + (Random.Range(-debrisDispenseDegrees / 2, debrisDispenseDegrees / 2) * Mathf.Deg2Rad);
            debrisDirection.x = Mathf.Cos(randomizedDirectionAngle);
            debrisDirection.y = Mathf.Sin(randomizedDirectionAngle);
            debrisDirection.Normalize();

            instantiatedDebrisParticle.Initialize(debrisDirection * Random.Range(debrisGroundVelocityRange.x, debrisGroundVelocityRange.y),
             Random.Range(debrisVerticalVelocityRange.x, debrisVerticalVelocityRange.y));

        }
    }
}
