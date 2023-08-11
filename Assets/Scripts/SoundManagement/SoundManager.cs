using UnityEngine;

namespace SoundManagement
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        //Have a pool that manages the sound//You add/restart sounds instead of playing them at the same time
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

        private void PlaySFX(AudioClip audioClip, Vector2 worldPosition, float volumeMax)
        {
            //AudioSource.PlayClipAtPoint(audioClip, new Vector3(worldPosition.x, worldPosition.y, cameraZDistance), volume);
            GameObject oneShotAudio = new GameObject("Custom One Shot Audio");
            gameObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, cameraZDistance);
            AudioSource audioSource = (AudioSource)oneShotAudio.AddComponent(typeof(AudioSource));
            //audioSource.clip = audioClip;
            audioSource.spatialBlend = 0f;
            audioSource.volume = Random.Range(volumeMax - 0.2f, volumeMax);
            audioSource.pitch = Random.Range(.8f, 1.2f);
            audioSource.PlayOneShot(audioClip);
            Destroy(oneShotAudio, audioClip.length + .2f);
        }
        
        
    }
}

