using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

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

        private readonly Stack<AudioSourceHandler> _musicPool = new Stack<AudioSourceHandler>();
        private readonly Stack<AudioSourceHandler> _soundsPool = new Stack<AudioSourceHandler>();

        [Inject]
        public void Construct(AudioManagerConfig audioManagerConfig)
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

            if (TryGetMixerGroup(audioManagerConfig.musicMixerGroupName, out var musicGroup))
            {
                _musicAudioMixerGroup = musicGroup;
            }

            if (TryGetMixerGroup(audioManagerConfig.soundsMixerGroupName, out var soundsGroup))
            {
                _soundsAudioMixerGroup = soundsGroup;
            }

            // Detach from the Zenject context so the object can survive scene loads.
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        public class Factory : PlaceholderFactory<AudioManagerHandler>
        {
        }

        public void Dispose()
        {
            StopAllHandlers(musicAudioHandlers);
            StopAllHandlers(soundsAudioHandlers);

            _musicPool.Clear();
            _soundsPool.Clear();

            if (this != null && gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        private static void StopAllHandlers(List<AudioSourceHandler> handlers)
        {
            if (handlers == null) return;

            foreach (var handler in handlers)
            {
                if (handler != null) handler.Dismiss();
            }

            handlers.Clear();
        }

        public AudioSourceHandler PlayMusicClip(int audioClipKey, bool loopSound = true, float volume = 0.5f, float delay = 0f)
        {
            ReclaimInactiveHandlers(musicAudioHandlers, _musicPool);

            return PlayClipInternal(
                audioClipKey: audioClipKey,
                targetHandlers: musicAudioHandlers,
                pool: _musicPool,
                mixerGroup: _musicAudioMixerGroup,
                _musicAudioHandler,
                loopSound: loopSound,
                allowMultiple: false,
                volume: volume,
                delay: delay,
                exclusive: true
            );
        }

        public AudioSourceHandler PlaySoundClip(int audioClipKey, bool loopSound = false, bool allowMultiple = true, float volume = 1f, float delay = 0f)
        {
            ReclaimInactiveHandlers(soundsAudioHandlers, _soundsPool);

            return PlayClipInternal(
                audioClipKey: audioClipKey,
                targetHandlers: soundsAudioHandlers,
                pool: _soundsPool,
                mixerGroup: _soundsAudioMixerGroup,
                _soundAudioHandler,
                loopSound: loopSound,
                allowMultiple: allowMultiple,
                volume: volume,
                delay: delay,
                exclusive: false
            );
        }

        private AudioSourceHandler PlayClipInternal(
            int audioClipKey,
            List<AudioSourceHandler> targetHandlers,
            Stack<AudioSourceHandler> pool,
            AudioMixerGroup mixerGroup,
            Transform parent,
            bool loopSound,
            bool allowMultiple,
            float volume,
            float delay,
            bool exclusive
        )
        {
            if (!TryGetClip(audioClipKey, out var clip)) return null;

            if (mixerGroup == null)
            {
                Debug.LogError($"[AudioManager] Mixer group is null for clip key {audioClipKey}.");
                return null;
            }

            if (exclusive)
            {
                StopOtherHandlers(targetHandlers, pool, audioClipKey);
            }

            AudioSourceHandler audioHandler = null;

            if (!allowMultiple)
            {
                audioHandler = targetHandlers.FirstOrDefault(x => x.AudioClipKey == audioClipKey);
            }

            if (audioHandler == null)
            {
                audioHandler = AcquireHandler(pool, clip.name, parent, mixerGroup);
                targetHandlers.Add(audioHandler);
            }

            audioHandler.Play(audioClipKey, clip, loopSound, volume, delay);
            return audioHandler;
        }

        private AudioSourceHandler AcquireHandler(Stack<AudioSourceHandler> pool, string objectName, Transform parent, AudioMixerGroup mixerGroup)
        {
            while (pool.Count > 0)
            {
                var pooled = pool.Pop();
                if (pooled == null) continue;

                pooled.gameObject.name = objectName;
                pooled.gameObject.SetActive(true);
                return pooled;
            }

            var audioHandler = new GameObject(objectName, typeof(AudioSourceHandler)).GetComponent<AudioSourceHandler>();
            audioHandler.transform.SetParent(parent);
            audioHandler.Initialize(mixerGroup);
            return audioHandler;
        }

        private void ReturnToPool(AudioSourceHandler handler, Stack<AudioSourceHandler> pool)
        {
            handler.Dismiss();
            handler.gameObject.SetActive(false);
            pool.Push(handler);
        }

        private void StopOtherHandlers(List<AudioSourceHandler> handlers, Stack<AudioSourceHandler> pool, int keepKey)
        {
            for (var i = handlers.Count - 1; i >= 0; i--)
            {
                var handler = handlers[i];

                if (handler == null)
                {
                    handlers.RemoveAt(i);
                    continue;
                }

                if (handler.AudioClipKey == keepKey) continue;

                handler.Stop();
                handlers.RemoveAt(i);
                ReturnToPool(handler, pool);
            }
        }

        private void ReclaimInactiveHandlers(List<AudioSourceHandler> handlers, Stack<AudioSourceHandler> pool)
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
                ReturnToPool(handler, pool);
            }
        }

        private void ReclaimAllInactiveHandlers()
        {
            ReclaimInactiveHandlers(musicAudioHandlers, _musicPool);
            ReclaimInactiveHandlers(soundsAudioHandlers, _soundsPool);
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
            return GetAudioMixerGroupVolume(_audioManagerConfig.musicVolumeParameter);
        }

        public float GetSoundsVolume()
        {
            return GetAudioMixerGroupVolume(_audioManagerConfig.soundsVolumeParameter);
        }

        public void SetMusicVolume(float volume)
        {
            SetAudioMixerGroupVolume(_audioManagerConfig.musicVolumeParameter, volume);
        }

        public void SetSoundsVolume(float volume)
        {
            SetAudioMixerGroupVolume(_audioManagerConfig.soundsVolumeParameter, volume);
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
            return musicAudioHandlers.Any(x => x != null && x.AudioClipKey == audioClipKey && x.IsPlaying);
        }

        public bool HasSoundAudioHandler(int audioClipKey)
        {
            return soundsAudioHandlers.Any(x => x != null && x.AudioClipKey == audioClipKey && x.IsPlaying);
        }

        private AudioSourceHandler GetAudioSourceByClipKey(List<AudioSourceHandler> audioSources, int audioClipKey)
        {
            return audioSources.FirstOrDefault(t => t != null && t.AudioClipKey == audioClipKey);
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