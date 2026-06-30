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
            var pair = audioPairsList.Find(x => x.key == audioClipKey);

            if (pair != null) return pair.audioClip;

            Debug.LogError($"Audio clip with key {audioClipKey} was not found.");
            return null;
        }
    }
}