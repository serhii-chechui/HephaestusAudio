using HephaestusMobile.Audio.Handler;

namespace HephaestusMobile.Audio.Manager {
    public interface IAudioManager {
        /// <summary>
        /// Play audio clip as music.
        /// </summary>
        /// <param name="soundClipName">Related sounds key from the config.</param>
        /// <param name="loopSound">Is sound should be play as loop.</param>
        /// <param name="allowMultiple">Allow to play multiple instances of the sound.</param>
        /// <param name="volume">Volume of the sound.</param>
        /// <param name="delay">Delay, before play the sound.</param>
        /// <returns></returns>
        AudioHandler PlayMusicClip(string soundClipName, bool loopSound = false, bool allowMultiple = true, float volume = 0.5f, float delay = 0f);
        
        /// <summary>
        /// Play audio clip as sound.
        /// </summary>
        /// <param name="soundClipName">Related sounds key from the config.</param>
        /// <param name="loopSound">Is sound should be play as loop.</param>
        /// <param name="allowMultiple">Allow to play multiple instances of the sound.</param>
        /// <param name="volume">Volume of the sound.</param>
        /// <param name="delay">Delay, before play the sound.</param>
        /// <returns></returns>
        AudioHandler PlaySoundClip(string soundClipName, bool loopSound = false, bool allowMultiple = true, float volume = 0.5f, float delay = 0f);
        
        /// <summary>
        /// Stops playing sound.
        /// </summary>
        /// <param name="soundClipName">Related sounds key from the config.</param>
        void StopPlayingMusic(string soundClipName);
        
        /// <summary>
        /// Stops playing sound.
        /// </summary>
        /// <param name="soundClipName">Related sounds key from the config.</param>
        void StopPlayingSound(string soundClipName);
        
        /// <summary>
        /// Get the volume of the AudioMixer group related to the music clips.
        /// </summary>
        /// <returns></returns>
        float GetMusicVolume();
        
        /// <summary>
        /// Get the volume of the AudioMixer group related to the sound clips.
        /// </summary>
        /// <returns></returns>
        float GetSoundsVolume();
        
        /// <summary>
        /// Set the volume of the AudioMixer group related to the music clips.
        /// </summary>
        /// <param name="volume">Music volume.</param>
        void SetMusicVolume(float volume);
        
        /// <summary>
        /// Set the volume of the AudioMixer group related to the sound clips.
        /// </summary>
        /// <param name="volume">Sounds volume.</param>
        void SetSoundsVolume(float volume);
        
        /// <summary>
        /// Check if there is already exists AudioHandler related to the specific sound key.
        /// </summary>
        /// <param name="soundClipName">Related sounds key from the config.</param>
        /// <returns></returns>
        bool IsMusicPlay(string soundClipName);
        
        /// <summary>
        /// Check if there is already exists AudioHandler related to the specific sound key.
        /// </summary>
        /// <param name="soundClipName">Related sounds key from the config.</param>
        /// <returns></returns>
        bool IsSoundPlay(string soundClipName);
    }
}