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
        
        private SoundLibrary _soundLibrary;
        
        private AudioMixer _audioMixer;

        private AudioMixerGroup _musicAudioMixerGroup;
        private AudioMixerGroup _soundsAudioMixerGroup;

        [Header("Audio Sources:")]
        [SerializeField] private List<AudioHandler> musicAudioHandlers;
        [SerializeField] private List<AudioHandler> soundsAudioHandlers;

        private Coroutine _claningCoroutine;
        public void Initialize(AudioManagerConfig audioManagerConfig)
        {
            _audioManagerConfig = audioManagerConfig;
            
            if (musicAudioHandlers == null) {
                var musicAudioHandler = new GameObject("Music-Audio-Handler", typeof(AudioHandler)).GetComponent<AudioHandler>();
                musicAudioHandler.transform.SetParent(transform);
                musicAudioHandlers = new List<AudioHandler> {musicAudioHandler};
            }

            if (soundsAudioHandlers == null) {
                var soundAudioHandler = new GameObject("Sounds-Audio-Handler", typeof(AudioHandler)).GetComponent<AudioHandler>();
                soundAudioHandler.transform.SetParent(transform);
                soundsAudioHandlers = new List<AudioHandler> {soundAudioHandler};
            }

            if (_soundLibrary == null) {
                _soundLibrary = audioManagerConfig.soundLibrary;
            }

            if (_audioMixer == null) {
                _audioMixer = audioManagerConfig.audioMixer;
            }

            if (_audioMixer != null) {

                _musicAudioMixerGroup = _audioMixer.FindMatchingGroups("Music")[0];
                _soundsAudioMixerGroup = _audioMixer.FindMatchingGroups("Sounds")[0];
                
                foreach (var musicAudioSource in musicAudioHandlers) {
                    musicAudioSource.Initialize(_musicAudioMixerGroup);
                }

                foreach (var soundAudioSource in soundsAudioHandlers) {
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
        
        public AudioHandler PlayMusicClip(string soundClipName, bool loopSound = true, bool allowMultiple = true, float volume = 0.5F, float delay = 0f) {
            
            var clip = _soundLibrary.soundsList.Find(x => x.soundName == soundClipName).sound;
            
            var audioHandler = new GameObject("Music-Audio-Handler", typeof(AudioHandler)).GetComponent<AudioHandler>();
            audioHandler.transform.SetParent(transform);
            audioHandler.Initialize(_musicAudioMixerGroup);
            musicAudioHandlers.Add(audioHandler);

            audioHandler.Play(soundClipName, clip, loopSound, volume, delay);

            return audioHandler;
        }

        public AudioHandler PlaySoundClip(string soundClipName, bool loopSound = false, bool allowMultiple = true, float volume = 1f, float delay = 0f) {
            var clip = _soundLibrary.soundsList.Find(x => x.soundName == soundClipName).sound;

            if (!allowMultiple && IsSoundPlay(soundClipName)) return null;
            
            var audioHandler = new GameObject("Sound-Audio-Handler", typeof(AudioHandler)).GetComponent<AudioHandler>();
            audioHandler.transform.SetParent(transform);
            audioHandler.Initialize(_soundsAudioMixerGroup);
            soundsAudioHandlers.Add(audioHandler);
            audioHandler.Play(soundClipName, clip, loopSound, volume, delay);
            
            return audioHandler;
        }

        public void StopPlayingMusic(string soundClipName) {
            if(!IsMusicPlay(soundClipName)) return;
            var currentMusicAudioSource = GetAudioSourceByClipName(musicAudioHandlers, soundClipName);
            if(currentMusicAudioSource == null) return;
            currentMusicAudioSource.Stop();
        }

        public void StopPlayingSound(string soundClipName) {
            if(!IsSoundPlay(soundClipName)) return;
            var currentSoundAudioSource = GetAudioSourceByClipName(musicAudioHandlers, soundClipName);
            if(currentSoundAudioSource == null) return;
            currentSoundAudioSource.Stop();
        }

        public float GetMusicVolume() {
            _audioMixer.GetFloat("MusicVolume", out var musicGroupVolume);
            return Mathf.InverseLerp(_audioManagerConfig.minSoundsVolume, _audioManagerConfig.maxSoundsVolume, musicGroupVolume);
        }

        public float GetSoundsVolume() {
            _audioMixer.GetFloat("SoundsVolume", out var soundsGroupVolume);
            return Mathf.InverseLerp(_audioManagerConfig.minSoundsVolume, _audioManagerConfig.maxSoundsVolume, soundsGroupVolume);
        }

        public void SetMusicVolume(float volume) {
            _audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * _audioManagerConfig.maxSoundsVolume);
        }

        public void SetSoundsVolume(float volume) {
            _audioMixer.SetFloat("SoundsVolume", Mathf.Log10(volume) * _audioManagerConfig.maxSoundsVolume);
        }

        private AudioHandler GetCurrentAudioSource(List<AudioHandler> audioSources) {
            return audioSources.FirstOrDefault(t => t.IsPlaying);
        }
        
        private AudioHandler GetAudioSourceByClipName(List<AudioHandler> audioSources, string clipName) {
            return audioSources.FirstOrDefault(t => t.AudioClipName == clipName);
        }

        IEnumerator CleanInactiveAudioSources() {
            
            yield return null;

            while (true) {
                yield return new WaitForSeconds(3f);
            
                for (var i = 0; i < musicAudioHandlers.Count; i++) {
                    var musicAudioHandler = musicAudioHandlers[i];
                    if (musicAudioHandler.IsPlaying) continue;
                    musicAudioHandler.Dismiss();
                    musicAudioHandlers.Remove(musicAudioHandler);
                    Destroy(musicAudioHandler.gameObject);
                }
            
                for (var i = 0; i < soundsAudioHandlers.Count; i++) {
                    var soundsAudioHandler = soundsAudioHandlers[i];
                    if (soundsAudioHandler.IsPlaying) continue;
                    soundsAudioHandler.Dismiss();
                    soundsAudioHandlers.Remove(soundsAudioHandler);
                    Destroy(soundsAudioHandler.gameObject);
                }
            }
        }

        public bool IsMusicPlay(string soundClipName)
        {
            var audioHandler = musicAudioHandlers.Find(x => x.AudioClipName == soundClipName);
            return audioHandler != null;
        }

        public bool IsSoundPlay(string soundClipName)
        {
            var audioHandler = soundsAudioHandlers.Find(x => x.AudioClipName == soundClipName);
            return audioHandler != null;
        }
    }
}