using System;
using InventorySystem;
using Items;
using SoundManagement;
using UnityEngine;

namespace Player
{
    public class ThrowableObject : MonoBehaviour
    {
        [SerializeField] private TurnEventsHandler _turnEventsHandler;
        [SerializeField] private SFXEventChannel _sfxEventChannel;
        [SerializeField] private Throwable _throwableConfig;

        [SerializeField] private AnimationCurve _travelVelocity;
        [SerializeField] private AnimationCurve _travelArc;
        [SerializeField] private SpriteRenderer _spriteBody;
        [SerializeField] private Transform _shadow;

        private ItemContainer _itemContainer;
        
        private bool _travelling;
        private Vector2 _origin;
        private Vector2 _target;
        private float _arcCurrent;
        private float _velocityCurrent;
        private float _velocity;
        
        private bool _timerCountdown;
        private float _timer;

        private int _turnTimer;
        //Check if it is player thrown
        //Countdown with timer or turns
        private void Update()
        {
            if (_travelling)
            {
                Travel();
            }
            //Fly here
            if (_timerCountdown)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    Explode();
                }
            }
        }

        private void Travel()
        {
            //Think about travel trajectory
            _velocityCurrent = Mathf.MoveTowards(_velocityCurrent, 1f, _throwableConfig.throwVelocity * Time.deltaTime);
            _velocity = _travelVelocity.Evaluate(_velocityCurrent);
            _arcCurrent = Mathf.MoveTowards(_arcCurrent, 1f, _velocity * Time.deltaTime);
            Vector2 travelPosition = Vector2.Lerp(_origin, _target, _arcCurrent);
            _shadow.position = travelPosition;
            _spriteBody.transform.position = new Vector2(travelPosition.x, .15f + travelPosition.y + _travelArc.Evaluate(_arcCurrent) * 3);

            if (_arcCurrent >= 1)
            {
                _travelling = false;
            }
        }
        
        private void TurnCountDown()
        {
            _turnTimer--;
            if (_turnTimer <= 0)
            {
                Explode();
            }
        }
        
        private void Explode()
        {
            //Check with sphere Overlap
            //Damage all IDamageables
            Collider2D[] hits = Physics2D.OverlapCircleAll(_shadow.position, _throwableConfig.radius);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    IDamageable hitUnit = hits[i].GetComponentInParent<IDamageable>();
                    hitUnit?.TakeDamage(_throwableConfig.damage);
                }
            }
            _itemContainer.ClearSlots();

            if (_itemContainer != null)
            {
                Destroy(_itemContainer.transform.parent.gameObject);
            }
            
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            _turnEventsHandler.OnTurnAdvanced -= TurnCountDown;
        }

        public void Initialize(ItemContainer itemContainer, Vector2 origin, Vector2 target, bool timerCountdown = true)
        {
            Throwable throwableConfig = itemContainer.GetItem() as Throwable;
            
            if (throwableConfig == null)
            {
                Debug.Log("You Threw Nothing");
                return;
            }
            _spriteBody = GetComponent<SpriteRenderer>();
            _itemContainer = itemContainer;
            
            _throwableConfig = throwableConfig;
            _spriteBody.sprite = throwableConfig.GetSprite();
            _timerCountdown = timerCountdown;
            _timer = throwableConfig.fuseTimer;
            _turnTimer = _throwableConfig.turnTimer;

            _origin = origin;
            _target = target;
            _travelling = true;
            
            if (!timerCountdown)
            {
                _turnEventsHandler.OnTurnAdvanced += TurnCountDown;
            }
        }
    }
}