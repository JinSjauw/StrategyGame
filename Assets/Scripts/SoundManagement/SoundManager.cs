using UnityEngine;

namespace SoundManagement
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        //SFXEventChannel
        [SerializeField] private SFXEventChannel _sfxEventChannel;
        //MusicEventChannel

        [SerializeField] private float cameraZDistance;
        
        private AudioSource _musicSource;

        private void Awake()
        {
            //Subscribe to events
            _sfxEventChannel.SFXRequest += PlaySFX;
        }

        private void PlaySFX(AudioClip audioClip, Vector2 worldPosition)
        {
            AudioSource.PlayClipAtPoint(audioClip, new Vector3(worldPosition.x, worldPosition.y, cameraZDistance), 1f);
        }
    }
}

