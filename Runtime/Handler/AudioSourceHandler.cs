using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace WTFGames.Hephaestus.AudioSystem
{
    public class AudioSourceHandler : MonoBehaviour
    {
        public event Action<int> OnClipPlay;
        public event Action<int> OnClipEnded;
        public event Action<int> OnClipStop;

        private Coroutine _playingAudionCoroutine;

        public bool IsPlaying => audioSource.isPlaying;

        public int AudioClipKey { get; private set; }

        [SerializeField]
        private AudioSource audioSource;

        public void Initialize(AudioMixerGroup audioMixerGroup)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSource.volume = 1f;
        }
        
        public void Dismiss()
        {
            audioSource.Stop();
            audioSource.clip = null;
            if (_playingAudionCoroutine == null) return;
            StopCoroutine(WaitUntilClipEnd_Co());
            _playingAudionCoroutine = null;
        }

        public void Play(int clipKey, AudioClip clip, bool loop = false, float volume = 1f, float delay = 0f)
        {
            AudioClipKey = clipKey;
            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.volume = volume;
            audioSource.Play((ulong)delay);
            OnClipPlay?.Invoke(AudioClipKey);
            if (_playingAudionCoroutine != null)
            {
                StopCoroutine(WaitUntilClipEnd_Co());
                _playingAudionCoroutine = null;
            }
            else
            {
                _playingAudionCoroutine = StartCoroutine(WaitUntilClipEnd_Co());
            }
        }

        public void Stop()
        {
            AudioClipKey = 0;
            audioSource.Stop();
            audioSource.clip = null;
            OnClipEnded?.Invoke(AudioClipKey);
            OnClipStop?.Invoke(AudioClipKey);
            if (_playingAudionCoroutine == null) return;
            StopCoroutine(WaitUntilClipEnd_Co());
            _playingAudionCoroutine = null;
        }

        private IEnumerator WaitUntilClipEnd_Co()
        {
            while (audioSource.isPlaying)
            {
                yield return null;
            }
            
            OnClipEnded?.Invoke(AudioClipKey);
        }
    }
}