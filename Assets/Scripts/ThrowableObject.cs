using Items;
using SoundManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class ThrowableObject : MonoBehaviour
    {
        [SerializeField] private TurnEventsHandler _turnEventsHandler;
        [SerializeField] private SFXEventChannel _sfxEventChannel;
        [SerializeField] private Throwable _throwableConfig;

        [SerializeField] private AudioClip _impactSound;
        [SerializeField] private AudioClip _explosionSound;
        [SerializeField] private AudioClip _throwSound;
        
        [SerializeField] private AnimationCurve _travelVelocity;
        [SerializeField] private AnimationCurve _travelArc;
        [SerializeField] private SpriteRenderer _spriteBody;
        [SerializeField] private Transform _shadow;
        [SerializeField] private Transform _explosionAnimation;

        [SerializeField] private Transform _worldUI;
        [SerializeField] private Image _fuseBar;

        private DebrisDispenser _debrisDispenser;
        //private ItemContainer _itemContainer;
        
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
                _fuseBar.fillAmount += Time.deltaTime / _throwableConfig.fuseTimer;
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
            Vector2 arcPosition = new Vector2(travelPosition.x, .15f + travelPosition.y + _travelArc.Evaluate(_arcCurrent) * 3);
            _shadow.position = travelPosition;
            _spriteBody.transform.position = arcPosition;
            _worldUI.position = arcPosition;
            if (_arcCurrent >= 1)
            {
                _sfxEventChannel.RequestSFX(_impactSound, _target);
                _travelling = false;
            }
        }
        
        private void TurnCountDown()
        {
            _fuseBar.fillAmount += 1f / _throwableConfig.turnTimer;
            _turnTimer--;
            if (_turnTimer <= 0)
            {
                Explode();
            }
        }
        
        private void Explode()
        {
            _spriteBody.sprite = null;
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

            /*if (_itemContainer != null)
            {
                _itemContainer.ClearSlots();
                Destroy(_itemContainer.transform.parent.gameObject);
            }*/
            _debrisDispenser.DispenseDebris();
            _sfxEventChannel.RequestSFX(_explosionSound, _target, .7f);
            _explosionAnimation.transform.position = new Vector3(_target.x, _target.y + 0.8f, 0);
            _explosionAnimation.gameObject.SetActive(true);
            _turnEventsHandler.OnTurnAdvanced -= TurnCountDown;
            enabled = false;
        }

        private void OnDestroy()
        {
            _turnEventsHandler.OnTurnAdvanced -= TurnCountDown;
        }

        public void Initialize(BaseItem throwableConfig, Vector2 origin, Vector2 target, bool timerCountdown = true)
        {
            //Throwable throwableConfig = itemContainer.GetItem() as Throwable;
            _throwableConfig = throwableConfig as Throwable;
            
            if (_throwableConfig == null)
            {
                Debug.Log("You Threw Nothing");
                return;
            }
            _debrisDispenser = GetComponentInChildren<DebrisDispenser>();
            _sfxEventChannel.RequestSFX(_throwSound, origin);

            _spriteBody.sprite = throwableConfig.GetSprite();
            _timerCountdown = timerCountdown;
            _timer = _throwableConfig.fuseTimer;
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