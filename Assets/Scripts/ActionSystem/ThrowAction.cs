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
        private void Throw()
        {
            //Throw grenade to spot;
            
            //Lerp the grenade to the end spot
            Debug.Log("Item name: " + holderUnit.selectedItem.GetItem().name);
            //Instantiate Carrier MonoBehvaiour
            ItemContainer itemContainer = holderUnit.selectedItem;
            ThrowableObject throwable = Instantiate(throwablePrefab).GetComponent<ThrowableObject>();
            throwable.Initialize(itemContainer, holderUnit.transform.position, _target,false);
            //Inject GrenadeSO config 
            //Lerp object to target position
            _onComplete();
        }
        
        
        public override void SetAction(Vector2 target)
        {
            _target = target;
        }

        public override List<Vector2> GetPreview()
        {
            //Get preview List
            return new List<Vector2>();
        }

        public override void Execute()
        {
            Throw();
        }
    }
}