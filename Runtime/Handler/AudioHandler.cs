using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace HephaestusMobile.Audio.Handler
{
    public class AudioHandler : MonoBehaviour
    {
        public event Action OnClipPlay;
        public event Action OnClipEnded;
        public event Action OnClipStop;

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

        public void Play(int clipKey, AudioClip clip, bool loop = false, float volume = 1f, float delay = 0f)
        {
            AudioClipKey = clipKey;
            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.volume = volume;
            audioSource.Play((ulong) delay);
            OnClipPlay?.Invoke();
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
            OnClipEnded?.Invoke();
            OnClipStop?.Invoke();
            if (_playingAudionCoroutine == null) return;
            StopCoroutine(WaitUntilClipEnd_Co());
            _playingAudionCoroutine = null;
        }

        public void Dismiss()
        {
            audioSource.Stop();
            audioSource.clip = null;
            if (_playingAudionCoroutine == null) return;
            StopCoroutine(WaitUntilClipEnd_Co());
            _playingAudionCoroutine = null;
        }

        IEnumerator WaitUntilClipEnd_Co()
        {
            yield return new WaitUntil(() => !audioSource.isPlaying);
            OnClipEnded?.Invoke();
            _playingAudionCoroutine = null;
        }
    }
}