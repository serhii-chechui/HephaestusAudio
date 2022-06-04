using HephaestusMobile.Audio.SoundsLibrary;
using UnityEngine;
using UnityEngine.Audio;

namespace HephaestusMobile.Audio.Manager {
    [CreateAssetMenu(fileName = "AudioManagerConfig", menuName = "HephaestusMobile/Core/Audio/AudioManagerConfig")]
    public class AudioManagerConfig : ScriptableObject {
        public SoundLibrary soundLibrary;
        public AudioMixer audioMixer;
        
        public float minSoundsVolume = -80f;
        public float maxSoundsVolume = 20f;
    }
}