using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace WTFGames.Hephaestus.AudioSystem
{
    public class AudioManagerHandler : MonoBehaviour
    {
        private AudioManagerConfig _audioManagerConfig;

        private AudioLibrary _audioLibrary;

        private AudioMixer _audioMixer;

        private AudioMixerGroup _musicAudioMixerGroup;
        private AudioMixerGroup _soundsAudioMixerGroup;

        [Header("Audio Sources:")]
        private Transform _musicAudioHandler;
        [SerializeField]
        private List<AudioSourceHandler> musicAudioHandlers;
        
        private Transform _soundAudioHandler;
        [SerializeField]
        private List<AudioSourceHandler> soundsAudioHandlers;

        public void Initialize(AudioManagerConfig audioManagerConfig)
        {
            _audioManagerConfig = audioManagerConfig;

            if (_musicAudioHandler == null)
            {
                _musicAudioHandler = new GameObject("Music-Audio-Handler").transform;
                _musicAudioHandler.transform.SetParent(transform);
            }

            musicAudioHandlers ??= new List<AudioSourceHandler>();
            
            if (_soundAudioHandler == null)
            {
                _soundAudioHandler = new GameObject("Sounds-Audio-Handler").transform;
                _soundAudioHandler.transform.SetParent(transform);
            }
            
            soundsAudioHandlers ??= new List<AudioSourceHandler>();

            if (_audioLibrary == null)
            {
                _audioLibrary = audioManagerConfig.audioLibrary;
            }

            if (_audioMixer == null)
            {
                _audioMixer = audioManagerConfig.audioMixer;
            }

            if (TryGetMixerGroup("Music", out var musicGroup))
            {
                _musicAudioMixerGroup = musicGroup;
            }

            if (TryGetMixerGroup("Sounds", out var soundsGroup))
            {
                _soundsAudioMixerGroup = soundsGroup;
            }

            DontDestroyOnLoad(gameObject);
        }

        public void Dispose()
        {
            
        }

        public AudioSourceHandler PlayMusicClip(int audioClipKey, bool loopSound = true, float volume = 0.5f, float delay = 0f)
        {
            CleanupInactiveHandlers(musicAudioHandlers);
            
            return PlayClipInternal(
                audioClipKey: audioClipKey,
                targetHandlers: musicAudioHandlers,
                mixerGroup: _musicAudioMixerGroup,
                _musicAudioHandler,
                loopSound: loopSound,
                allowMultiple: false,
                volume: volume,
                delay: delay
            );
        }

        public AudioSourceHandler PlaySoundClip(int audioClipKey, bool loopSound = false, bool allowMultiple = true, float volume = 1f, float delay = 0f)
        {
            CleanupInactiveHandlers(soundsAudioHandlers);
            
            return PlayClipInternal(
                audioClipKey: audioClipKey,
                targetHandlers: soundsAudioHandlers,
                mixerGroup: _soundsAudioMixerGroup,
                _soundAudioHandler,
                loopSound: loopSound,
                allowMultiple: allowMultiple,
                volume: volume,
                delay: delay
            );
        }
        
        private AudioSourceHandler PlayClipInternal(
            int audioClipKey,
            List<AudioSourceHandler> targetHandlers,
            AudioMixerGroup mixerGroup,
            Transform parent,
            bool loopSound,
            bool allowMultiple,
            float volume,
            float delay
        )
        {
            if (!TryGetClip(audioClipKey, out var clip)) return null;

            if (mixerGroup == null)
            {
                Debug.LogError($"[AudioManager] Mixer group is null for clip key {audioClipKey}.");
                return null;
            }

            AudioSourceHandler audioHandler = null;

            if (!allowMultiple)
            {
                audioHandler = targetHandlers.FirstOrDefault(x => x.AudioClipKey == audioClipKey);
            }

            if (audioHandler == null)
            {
                audioHandler = CreateAudioHandler(clip.name, parent, mixerGroup);
                targetHandlers.Add(audioHandler);
            }
            
            audioHandler.Play(audioClipKey, clip, loopSound, volume, delay);
            return audioHandler;
        }

        private AudioSourceHandler CreateAudioHandler(string objectName, Transform parent, AudioMixerGroup mixerGroup)
        {
            var audioHandler = new GameObject(objectName, typeof(AudioSourceHandler)).GetComponent<AudioSourceHandler>();
            audioHandler.transform.SetParent(parent);
            audioHandler.Initialize(mixerGroup);
            return audioHandler;
        }

        private void CleanupInactiveHandlers(List<AudioSourceHandler> handlers)
        {
            for (var i = handlers.Count - 1; i >= 0; i--)
            {
                var handler = handlers[i];

                if (handler == null)
                {
                    handlers.RemoveAt(i);
                    continue;
                }

                if (handler.IsPlaying) continue;
                handlers.RemoveAt(i);
                Destroy(handler.gameObject);
            }
        }
        
        private void CleanupAllInactiveHandlers()
        {
            CleanupInactiveHandlers(musicAudioHandlers);
            CleanupInactiveHandlers(soundsAudioHandlers);
        }
        
        public void StopPlayingMusic(int audioClipKey)
        {
            var currentMusicAudioSource = GetAudioSourceByClipKey(musicAudioHandlers, audioClipKey);
            if (currentMusicAudioSource == null) return;
            currentMusicAudioSource.Stop();
        }

        public void StopPlayingSound(int audioClipKey)
        {
            var currentSoundAudioSource = GetAudioSourceByClipKey(soundsAudioHandlers, audioClipKey);
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
            return _audioMixer.GetFloat(groupName, out var dB) ? Mathf.Pow(10, dB / 20) : 1f;
        }

        public bool HasMusicAudioHandler(int audioClipKey)
        {
            return musicAudioHandlers.Any(x => x.AudioClipKey == audioClipKey);
        }

        public bool HasSoundAudioHandler(int audioClipKey)
        {
            return soundsAudioHandlers.Any(x => x.AudioClipKey == audioClipKey);
        }

        private AudioSourceHandler GetAudioSourceByClipKey(List<AudioSourceHandler> audioSources, int audioClipKey)
        {
            return audioSources.FirstOrDefault(t => t.AudioClipKey == audioClipKey);
        }
        
        private bool TryGetClip(int key, out AudioClip clip)
        {
            clip = _audioLibrary?.audioPairsList?.FirstOrDefault(x => x.key == key)?.audioClip;
            if (clip != null) return true;

            Debug.LogError($"Audio clip with key {key} was not found.");
            return false;
        }
        
        private bool TryGetMixerGroup(string groupName, out AudioMixerGroup mixerGroup)
        {
            mixerGroup = null;

            if (_audioMixer == null)
            {
                Debug.LogError("[AudioManager] AudioMixer is not assigned.");
                return false;
            }

            var groups = _audioMixer.FindMatchingGroups(groupName);
            mixerGroup = groups.FirstOrDefault();

            if (mixerGroup != null)
                return true;

            Debug.LogError($"[AudioManager] Mixer group '{groupName}' was not found in mixer '{_audioMixer.name}'.");
            return false;
        }
    }
}