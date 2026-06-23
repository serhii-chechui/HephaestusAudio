using UnityEngine;
using UnityEngine.Audio;

namespace WTFGames.Hephaestus.AudioSystem
{
    [CreateAssetMenu(fileName = "AudioManagerConfig", menuName = "HephaestusMobile/Core/Audio/AudioManagerConfig")]
    public class AudioManagerConfig : ScriptableObject
    {
        public AudioLibrary audioLibrary;
        public AudioMixer audioMixer;

        [Header("Mixer Groups:")]
        [Tooltip("Name of the AudioMixerGroup used for music clips.")]
        public string musicMixerGroupName = "Music";
        [Tooltip("Name of the AudioMixerGroup used for sound clips.")]
        public string soundsMixerGroupName = "Sounds";

        [Header("Exposed Mixer Parameters:")]
        [Tooltip("Exposed AudioMixer parameter controlling music volume.")]
        public string musicVolumeParameter = "MusicVolume";
        [Tooltip("Exposed AudioMixer parameter controlling sounds volume.")]
        public string soundsVolumeParameter = "SoundsVolume";
    }
}