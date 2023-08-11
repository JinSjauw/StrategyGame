using UnityEngine;
using UnityEngine.Events;

namespace SoundManagement
{
    [CreateAssetMenu(menuName = "Audio/SFX Event Channel")]
    public class SFXEventChannel : ScriptableObject
    {
        public event UnityAction<AudioClip, Vector2, float> SFXRequest = delegate(AudioClip arg0, Vector2 vector2, float volume) {  };

        public void RequestSFX(AudioClip sfxConfig, Vector2 worldPosition, float volume = 1f)
        {
            SFXRequest.Invoke(sfxConfig, worldPosition, volume);
        }
    }
}