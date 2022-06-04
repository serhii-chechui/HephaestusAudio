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
        
        public void Initialize()
        {
            _audioManagerHandler = new GameObject("AudioManagerHandler").AddComponent<AudioManagerHandler>();
            _audioManagerHandler.Initialize(_audioManagerConfig);
        }

        public void Dispose()
        {
            if(_audioManagerHandler == null) return;
            _audioManagerHandler.Dispose();
        }

        /// <inheritdoc cref="IAudioManager"/>
        public AudioHandler PlayMusicClip(string soundClipName, bool loopSound = true, bool allowMultiple = true, float volume = 0.5f, float delay = 0)
        {
            return _audioManagerHandler.PlayMusicClip(soundClipName, loopSound, allowMultiple, volume, delay);
        }

        /// <inheritdoc cref="IAudioManager"/>
        public AudioHandler PlaySoundClip(string soundClipName, bool loopSound = false, bool allowMultiple = true, float volume = 0.5f, float delay = 0)
        {
            return _audioManagerHandler.PlaySoundClip(soundClipName, loopSound, allowMultiple, volume, delay);
        }

        /// <inheritdoc cref="IAudioManager"/>
        public void StopPlayingMusic(string soundClipName)
        {
            _audioManagerHandler.StopPlayingMusic(soundClipName);
        }

        /// <inheritdoc cref="IAudioManager"/>
        public void StopPlayingSound(string soundClipName)
        {
            _audioManagerHandler.StopPlayingSound(soundClipName);
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
        public bool IsMusicPlay(string soundClipName)
        {
            return _audioManagerHandler.IsMusicPlay(soundClipName);
        }

        /// <inheritdoc cref="IAudioManager"/>
        public bool IsSoundPlay(string soundClipName)
        {
            return _audioManagerHandler.IsSoundPlay(soundClipName);
        }
    }
}