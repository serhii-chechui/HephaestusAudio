using System;
using System.Collections.Generic;
using UnityEngine;

namespace WTFGames.Hephaestus.AudioSystem
{
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "HephaestusMobile/Core/Audio/AudioLibrary")]
    public class AudioLibrary : ScriptableObject
    {
        public AudioLibraryConstants audioLibraryConstants;

        [HideInInspector]
        public List<AudioNamePair> audioPairsList = new List<AudioNamePair>();

        public AudioClip GetAudioClipByKey(int audioClipKey)
        {
            return audioPairsList.Find(x => x.key == audioClipKey).audioClip;
        }
    }
}