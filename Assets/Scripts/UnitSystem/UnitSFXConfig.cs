using UnityEngine;
using Random = UnityEngine.Random;

namespace UnitSystem
{
    [CreateAssetMenu(menuName = "Audio/UnitSFX")]
    public class UnitSFXConfig : ScriptableObject
    {
        [SerializeField] private AudioClip[] walkingSounds;

        [SerializeField] private AudioClip[] hitClip;
        [SerializeField] private AudioClip dieClip;

        private int _walkClipIndex;
        
        public AudioClip GetWalkClip()
        {

            if (_walkClipIndex < walkingSounds.Length - 1)
            {
                _walkClipIndex++;
            }
            else
            {
                _walkClipIndex = 0;
            }
            
            Debug.Log("Sound Clip: " + walkingSounds[_walkClipIndex]);
            return walkingSounds[_walkClipIndex];
        }
    }
}