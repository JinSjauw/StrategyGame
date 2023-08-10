using UnityEngine;
using UnityEngine.Events;

namespace SoundManagement
{
    [CreateAssetMenu(menuName = "Audio/SFX Event Channel")]
    public class SFXEventChannel : ScriptableObject
    {
        public event UnityAction<AudioClip, Vector2> SFXRequest = delegate(AudioClip arg0, Vector2 vector2) {  };

        public void RequestSFX(AudioClip sfxConfig, Vector2 worldPosition)
        {
            SFXRequest.Invoke(sfxConfig, worldPosition);
        }
    }
}