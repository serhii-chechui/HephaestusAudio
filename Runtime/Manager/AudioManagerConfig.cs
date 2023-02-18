using HephaestusMobile.Audio.SoundsLibrary;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace HephaestusMobile.Audio.Manager {
    [CreateAssetMenu(fileName = "AudioManagerConfig", menuName = "HephaestusMobile/Core/Audio/AudioManagerConfig")]
    public class AudioManagerConfig : ScriptableObject {
        public AudioLibrary audioLibrary;
        public AudioMixer audioMixer;
        
        public float minSoundsVolume = -80f;
        public float maxSoundsVolume = 20f;
    }
}