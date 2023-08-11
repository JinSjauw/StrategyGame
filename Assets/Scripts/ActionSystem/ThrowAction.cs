using System.Collections.Generic;
using InventorySystem;
using Items;
using Player;
using UnityEngine;

namespace ActionSystem
{
    [CreateAssetMenu(menuName = "Actions/ThrowAction")]
    public class ThrowAction : BaseAction
    {
        [SerializeField] private Transform throwablePrefab;
        private Vector2 _target;
        private List<TileGridObject> _circleArea;

        private void Throw()
        {
            StopPreview();
            
            //Throw grenade to spot;
            ItemContainer itemContainer = holderUnit.selectedItem;
            ThrowableObject throwable = Instantiate(throwablePrefab).GetComponent<ThrowableObject>();
            throwable.Initialize(itemContainer, holderUnit.transform.position, _target,false);
            _onComplete();
        }
        
        
        public override void SetAction(Vector2 target)
        {
            Vector2 origin = holderUnit.transform.position;
            float distance = Vector2.Distance(origin, target);
            Debug.Log($"Distance: {distance}");
            if (distance > holderUnit.unitData.detectionRadius / 2)
            {
                //Set it to the closest
                Vector2 validTarget = target - origin;
                _target = origin + Vector2.ClampMagnitude(validTarget, holderUnit.unitData.detectionRadius / 2);
                Debug.Log($"Target: {target}");
                return;
            }
            
            _target = target;
        }

        public override void Preview()
        {
            //Get preview List
            _circleArea = holderUnit.levelGrid.GetTilesInCircle(holderUnit.transform.position, holderUnit.unitData.detectionRadius / 2);
            for (int i = 0; i < _circleArea.Count; i++)
            {
                _circleArea[i].m_TileVisual.TurnHighlightOn();
            }
        }

        public override void StopPreview()
        {
            for (int i = 0; i < _circleArea.Count; i++)
            {
                _circleArea[i].m_TileVisual.TurnHighlightOff();
            }
        }

        public override void Execute()
        {
            Throw();
        }
    }
}