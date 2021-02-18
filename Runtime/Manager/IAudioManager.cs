using HephaestusMobile.Audio.Handler;

namespace HephaestusMobile.Audio.Manager {
    public interface IAudioManager {
        void Initialize(AudioManagerConfig audioManagerConfig);
        AudioHandler PlayMusicClip(string soundClipName, bool loopSound = false, float volume = 0.5f, float delay = 0f);
        AudioHandler PlaySoundClip(string soundClipName, bool loopSound = false, float volume = 0.5f, float delay = 0f);
        void StopPlayingMusic();
        void StopPlayingSound();
        float GetMusicVolume();
        float GetSoundsVolume();
        void SetMusicVolume(float volume);
        void SetSoundsVolume(float volume);
    }
}