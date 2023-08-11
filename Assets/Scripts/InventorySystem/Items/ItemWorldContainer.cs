using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InventorySystem.Items
{
    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public class ItemWorldContainer : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private ItemContainer _itemContainer;

        private Rigidbody2D _rigidbody2D;
        private Vector2 _targetDirection;
        private Vector2 _origin;
        private float _travelDistance;

        private bool _travelling;
        //Has an collider

        private void Update()
        {
            Travel();
        }

        private void Travel()
        {
            _travelDistance = Vector2.Distance(transform.position, _origin);
            if (_travelDistance > 1.3f && _travelling)
            {
                _rigidbody2D.velocity = Vector2.zero;
                _rigidbody2D.angularVelocity = 0;
                _travelling = false;
            }
        }
        
        public void Initialize(ItemContainer itemContainer, Vector2 origin)
        {
            //Init WorldContainer;
            _itemContainer = itemContainer;
            transform.position = origin;
            _origin = origin;
            itemContainer.transform.parent.gameObject.SetActive(false);
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.spriteSortPoint = SpriteSortPoint.Center;
            _spriteRenderer.sprite = itemContainer.GetItem().GetSprite();
            //Set colliderSize
            BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
            boxCollider2D.size = new Vector2(itemContainer.GetWidth() / 4f, itemContainer.GetHeight() / 4f);
            _targetDirection = Random.insideUnitCircle * 1f;
            Debug.Log($"TargetDir: {_targetDirection} : {origin}");
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _rigidbody2D.AddForce(_targetDirection.normalized * 1.1f, ForceMode2D.Impulse);
            _rigidbody2D.AddTorque(5 , ForceMode2D.Impulse);
            _travelling = true;
        }

        public ItemContainer GetItemContainer()
        {
            _itemContainer.transform.parent.gameObject.SetActive(true);
            return _itemContainer;
        }
    }
}