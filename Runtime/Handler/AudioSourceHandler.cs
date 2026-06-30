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

        /// <summary>
        /// Monotonically increasing token, bumped on every <see cref="Play"/>.
        /// Callers can capture it to detect when a pooled handler has been
        /// reused for a different clip.
        /// </summary>
        public int PlayId { get; private set; }

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
            AudioClipKey = 0;
            StopPlayingCoroutine();

            OnClipPlay = null;
            OnClipEnded = null;
            OnClipStop = null;
        }

        public void Play(int clipKey, AudioClip clip, bool loop = false, float volume = 1f, float delay = 0f)
        {
            AudioClipKey = clipKey;
            PlayId++;
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

        /// <summary>
        /// Stops playback only if the handler still serves the given play token.
        /// Safe to call on a handler that has since been pooled and reused.
        /// </summary>
        public void Stop(int playId)
        {
            if (playId != PlayId) return;
            Stop();
        }

        /// <summary>
        /// True while this handler is still playing the clip identified by the
        /// given play token.
        /// </summary>
        public bool IsValid(int playId)
        {
            return playId == PlayId && audioSource.isPlaying;
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