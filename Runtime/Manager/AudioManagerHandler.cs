using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HephaestusMobile.Audio.Handler;
using HephaestusMobile.Audio.SoundsLibrary;
using UnityEngine;
using UnityEngine.Audio;

namespace HephaestusMobile.Audio.Manager
{
    public class AudioManagerHandler : MonoBehaviour
    {
        private AudioManagerConfig _audioManagerConfig;

        private AudioLibrary _audioLibrary;

        private AudioMixer _audioMixer;

        private AudioMixerGroup _musicAudioMixerGroup;
        private AudioMixerGroup _soundsAudioMixerGroup;

        [Header("Audio Sources:")]
        [SerializeField]
        private List<AudioSourceHandler> musicAudioHandlers;

        [SerializeField]
        private List<AudioSourceHandler> soundsAudioHandlers;

        private Coroutine _claningCoroutine;

        public void Initialize(AudioManagerConfig audioManagerConfig)
        {
            _audioManagerConfig = audioManagerConfig;

            if (musicAudioHandlers == null)
            {
                var musicAudioHandler = new GameObject("Music-Audio-Handler", typeof(AudioSourceHandler))
                    .GetComponent<AudioSourceHandler>();
                musicAudioHandler.transform.SetParent(transform);
                musicAudioHandlers = new List<AudioSourceHandler> { musicAudioHandler };
            }

            if (soundsAudioHandlers == null)
            {
                var soundAudioHandler = new GameObject("Sounds-Audio-Handler", typeof(AudioSourceHandler))
                    .GetComponent<AudioSourceHandler>();
                soundAudioHandler.transform.SetParent(transform);
                soundsAudioHandlers = new List<AudioSourceHandler> { soundAudioHandler };
            }

            if (_audioLibrary == null)
            {
                _audioLibrary = audioManagerConfig.audioLibrary;
            }

            if (_audioMixer == null)
            {
                _audioMixer = audioManagerConfig.audioMixer;
            }

            if (_audioMixer != null)
            {
                _musicAudioMixerGroup = _audioMixer.FindMatchingGroups("Music")[0];
                _soundsAudioMixerGroup = _audioMixer.FindMatchingGroups("Sounds")[0];

                foreach (var musicAudioSource in musicAudioHandlers)
                {
                    musicAudioSource.Initialize(_musicAudioMixerGroup);
                }

                foreach (var soundAudioSource in soundsAudioHandlers)
                {
                    soundAudioSource.Initialize(_soundsAudioMixerGroup);
                }
            }

            _claningCoroutine = StartCoroutine(CleanInactiveAudioSources());

            DontDestroyOnLoad(gameObject);
        }

        public void Dispose()
        {
            if (_claningCoroutine != null)
            {
                StopCoroutine(_claningCoroutine);
            }
        }

        public AudioSourceHandler PlayMusicClip(int soundClipKey, bool loopSound = true, bool allowMultiple = true,
            float volume = 0.5F, float delay = 0f)
        {
            var clip = _audioLibrary.audioPairsList.Find(x => x.key == soundClipKey).audioClip;

            var audioHandler = new GameObject("Music-Audio-Handler", typeof(AudioSourceHandler)).GetComponent<AudioSourceHandler>();
            audioHandler.transform.SetParent(transform);
            audioHandler.Initialize(_musicAudioMixerGroup);
            musicAudioHandlers.Add(audioHandler);

            audioHandler.Play(soundClipKey, clip, loopSound, volume, delay);

            return audioHandler;
        }

        public AudioSourceHandler PlaySoundClip(int audioClipKey, bool loopSound = false, bool allowMultiple = true,
            float volume = 1f, float delay = 0f)
        {
            var clip = _audioLibrary.audioPairsList.Find(x => x.key == audioClipKey).audioClip;

            if (!allowMultiple && IsSoundPlay(audioClipKey)) return null;

            var audioHandler = new GameObject("Sound-Audio-Handler", typeof(AudioSourceHandler)).GetComponent<AudioSourceHandler>();
            audioHandler.transform.SetParent(transform);
            audioHandler.Initialize(_soundsAudioMixerGroup);
            soundsAudioHandlers.Add(audioHandler);
            audioHandler.Play(audioClipKey, clip, loopSound, volume, delay);

            return audioHandler;
        }

        public void StopPlayingMusic(int soundClipName)
        {
            if (!IsMusicPlay(soundClipName)) return;
            var currentMusicAudioSource = GetAudioSourceByClipName(musicAudioHandlers, soundClipName);
            if (currentMusicAudioSource == null) return;
            currentMusicAudioSource.Stop();
        }

        public void StopPlayingSound(int audioClipKey)
        {
            if (!IsSoundPlay(audioClipKey)) return;
            var currentSoundAudioSource = GetAudioSourceByClipName(musicAudioHandlers, audioClipKey);
            if (currentSoundAudioSource == null) return;
            currentSoundAudioSource.Stop();
        }

        public float GetMusicVolume()
        {
            return GetAudioMixerGroupVolume("MusicVolume");
        }

        public float GetSoundsVolume()
        {
            return GetAudioMixerGroupVolume("SoundsVolume");
        }

        public void SetMusicVolume(float volume)
        {
            SetAudioMixerGroupVolume("MusicVolume", volume);
        }

        public void SetSoundsVolume(float volume)
        {
            SetAudioMixerGroupVolume("SoundsVolume", volume);
        }

        private void SetAudioMixerGroupVolume(string groupName, float volume)
        {
            var dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1)) * 20;
            _audioMixer.SetFloat(groupName, dB);
        }
        
        private float GetAudioMixerGroupVolume(string groupName)
        {
            float dB;
            return _audioMixer.GetFloat(groupName, out dB) ? Mathf.Pow(10, dB / 20) : 1f;
        }

        private AudioSourceHandler GetCurrentAudioSource(List<AudioSourceHandler> audioSources)
        {
            return audioSources.FirstOrDefault(t => t.IsPlaying);
        }

        private AudioSourceHandler GetAudioSourceByClipName(List<AudioSourceHandler> audioSources, int audioClipKey)
        {
            return audioSources.FirstOrDefault(t => t.AudioClipKey == audioClipKey);
        }

        IEnumerator CleanInactiveAudioSources()
        {
            yield return null;

            while (true)
            {
                yield return new WaitForSeconds(3f);

                for (var i = 0; i < musicAudioHandlers.Count; i++)
                {
                    var musicAudioHandler = musicAudioHandlers[i];
                    if (musicAudioHandler.IsPlaying) continue;
                    musicAudioHandler.Dismiss();
                    musicAudioHandlers.Remove(musicAudioHandler);
                    Destroy(musicAudioHandler.gameObject);
                }

                for (var i = 0; i < soundsAudioHandlers.Count; i++)
                {
                    var soundsAudioHandler = soundsAudioHandlers[i];
                    if (soundsAudioHandler.IsPlaying) continue;
                    soundsAudioHandler.Dismiss();
                    soundsAudioHandlers.Remove(soundsAudioHandler);
                    Destroy(soundsAudioHandler.gameObject);
                }
            }
        }

        public bool IsMusicPlay(int audioClipKey)
        {
            var audioHandler = musicAudioHandlers.Find(x => x.AudioClipKey == audioClipKey);
            return audioHandler != null;
        }

        public bool IsSoundPlay(int audioClipKey)
        {
            var audioHandler = soundsAudioHandlers.Find(x => x.AudioClipKey == audioClipKey);
            return audioHandler != null;
        }
    }
}