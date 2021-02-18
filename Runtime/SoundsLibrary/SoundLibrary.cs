using System.Collections.Generic;
using UnityEngine;

namespace HephaestusMobile.Audio.SoundsLibrary {
    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "HephaestusMobile/Core/Audio/SoundLibrary")]
    public class SoundLibrary : ScriptableObject {
        [HideInInspector] public List<SoundNamePair> soundsList = new List<SoundNamePair>();
    }
}