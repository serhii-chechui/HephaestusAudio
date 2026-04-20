using UnityEngine;
using UnityEngine.Audio;

namespace WTFGames.Hephaestus.AudioSystem
{
    [CreateAssetMenu(fileName = "AudioManagerConfig", menuName = "HephaestusMobile/Core/Audio/AudioManagerConfig")]
    public class AudioManagerConfig : ScriptableObject
    {
        public AudioLibrary audioLibrary;
        public AudioMixer audioMixer;
    }
}