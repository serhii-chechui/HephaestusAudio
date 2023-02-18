using System;
using HephaestusMobile.Audio.Handler;
using UnityEngine;
using Zenject;

namespace HephaestusMobile.Audio.Manager {
    public class AudioManager : IInitializable, IDisposable, IAudioManager
    {
        [Inject]
        private AudioManagerConfig _audioManagerConfig;

        private AudioManagerHandler _audioManagerHandler;

        private AudioListener _audioListener;
        
        public void Initialize()
        {
            _audioManagerHandler = new GameObject("AudioManagerHandler").AddComponent<AudioManagerHandler>();
            _audioManagerHandler.Initialize(_audioManagerConfig);

            _audioListener = _audioManagerHandler.gameObject.AddComponent<AudioListener>();
        }

        public void Dispose()
        {
            if(_audioManagerHandler == null) return;
            _audioManagerHandler.Dispose();
        }

        /// <inheritdoc cref="IAudioManager"/>
        public AudioHandler PlayMusicClip(Enum audioClipKey, bool loopSound = true, bool allowMultiple = true, float volume = 0.5f, float delay = 0)
        {
            return _audioManagerHandler.PlayMusicClip(Convert.ToInt32(audioClipKey), loopSound, allowMultiple, volume, delay);
        }

        /// <inheritdoc cref="IAudioManager"/>
        public AudioHandler PlaySoundClip(Enum soundClipName, bool loopSound = false, bool allowMultiple = true, float volume = 0.5f, float delay = 0)
        {
            return _audioManagerHandler.PlaySoundClip(Convert.ToInt32(soundClipName), loopSound, allowMultiple, volume, delay);
        }

        /// <inheritdoc cref="IAudioManager"/>
        public void StopPlayingMusic(Enum soundClipName)
        {
            _audioManagerHandler.StopPlayingMusic(Convert.ToInt32(soundClipName));
        }

        /// <inheritdoc cref="IAudioManager"/>
        public void StopPlayingSound(Enum soundClipName)
        {
            _audioManagerHandler.StopPlayingSound(Convert.ToInt32(soundClipName));
        }

        /// <inheritdoc cref="IAudioManager"/>
        public float GetMusicVolume()
        {
            return _audioManagerHandler.GetMusicVolume();
        }

        /// <inheritdoc cref="IAudioManager"/>
        public float GetSoundsVolume()
        {
            return _audioManagerHandler.GetSoundsVolume();
        }

        /// <inheritdoc cref="IAudioManager"/>
        public void SetMusicVolume(float volume)
        {
            _audioManagerHandler.SetMusicVolume(volume);
        }

        /// <inheritdoc cref="IAudioManager"/>
        public void SetSoundsVolume(float volume)
        {
            _audioManagerHandler.SetSoundsVolume(volume);
        }

        /// <inheritdoc cref="IAudioManager"/>
        public bool IsMusicPlay(Enum soundClipName)
        {
            return _audioManagerHandler.IsMusicPlay(Convert.ToInt32(soundClipName));
        }

        /// <inheritdoc cref="IAudioManager"/>
        public bool IsSoundPlay(Enum soundClipName)
        {
            return _audioManagerHandler.IsSoundPlay(Convert.ToInt32(soundClipName));
        }
    }
}