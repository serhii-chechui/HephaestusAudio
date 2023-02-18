using System.Collections.Generic;
using Hephaestus.Audio.SoundsLibrary;
using UnityEngine;

namespace HephaestusMobile.Audio.SoundsLibrary {
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "HephaestusMobile/Core/Audio/AudioLibrary")]
    public class AudioLibrary : ScriptableObject
    {
        public AudioLibraryConstants audioLibraryConstants;
        
        [HideInInspector]
        public List<AudioNamePair> audioPairsList = new List<AudioNamePair>();
    }
}