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

        private Coroutine _playingAudioCoroutine;

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
            StopPlayingCoroutine();
        }

        public void Play(int clipKey, AudioClip clip, bool loop = false, float volume = 1f, float delay = 0f)
        {
            AudioClipKey = clipKey;
            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.volume = volume;

            if (delay > 0f)
                audioSource.PlayDelayed(delay);
            else
                audioSource.Play();

            OnClipPlay?.Invoke(AudioClipKey);

            StopPlayingCoroutine();
            _playingAudioCoroutine = StartCoroutine(WaitUntilClipEnd_Co());
        }

        public void Stop()
        {
            audioSource.Stop();
            audioSource.clip = null;

            OnClipEnded?.Invoke(AudioClipKey);
            OnClipStop?.Invoke(AudioClipKey);

            AudioClipKey = 0;

            StopPlayingCoroutine();
        }

        private void StopPlayingCoroutine()
        {
            if (_playingAudioCoroutine == null) return;
            StopCoroutine(_playingAudioCoroutine);
            _playingAudioCoroutine = null;
        }

        private IEnumerator WaitUntilClipEnd_Co()
        {
            while (audioSource.isPlaying)
            {
                yield return null;
            }

            OnClipEnded?.Invoke(AudioClipKey);
            _playingAudioCoroutine = null;
        }
    }
}