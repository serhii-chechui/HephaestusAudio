using HephaestusMobile.Audio.Manager;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Hephaestus.Audio.Samples
{
    public class AudioManagerTestHelper : MonoBehaviour
    {
        [Inject]
        private IAudioManager _audioManager;

        [SerializeField]
        private Slider _musicVolumeSlider;

        [SerializeField]
        private Slider _soundsVolumeSlider;

        [SerializeField]
        private Button _musicButton;

        [SerializeField]
        private Button _soundsButton;

        private void Start()
        {
            _musicVolumeSlider.onValueChanged.AddListener(volume => { _audioManager.SetMusicVolume(volume); });

            _musicVolumeSlider.value = 0.5f;

            _soundsVolumeSlider.onValueChanged.AddListener(volume => { _audioManager.SetSoundsVolume(volume); });
            _soundsVolumeSlider.value = 0.5f;

            _musicButton.onClick.AddListener(() =>
            {
                if (_audioManager.IsMusicPlay(AudioLibraryConstants.MUSIC))
                {
                    _audioManager.StopPlayingMusic(AudioLibraryConstants.MUSIC);
                }
                else
                {
                    _audioManager.PlayMusicClip(AudioLibraryConstants.MUSIC, true);
                }
            });

            _soundsButton.onClick.AddListener(() =>
            {
                _audioManager.PlaySoundClip(AudioLibraryConstants.GEM_SOUND, false, false);
            });
        }
    }
}