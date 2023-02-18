using System.Collections.Generic;
using UnityEngine;

namespace Hephaestus.Audio.SoundsLibrary
{
    [CreateAssetMenu(fileName = "AudioLibraryConstants", menuName = "HephaestusMobile/Core/Audio/AudioLibraryConstants", order = 1)]
    public class AudioLibraryConstants : ScriptableObject
    {
        [HideInInspector]
        public string enumsPath;
        
        [HideInInspector]
        public List<string> soundMapKeys;
    }
}