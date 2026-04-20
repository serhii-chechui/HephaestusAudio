using UnityEngine;
using Zenject;

namespace WTFGames.Hephaestus.AudioSystem
{
    [CreateAssetMenu(fileName = "HephaestusSOInstaller", menuName = "HephaestusMobile/Core/Audio/HephaestusSOInstaller")]
    public class HephaestusAudioSOInstaller : ScriptableObjectInstaller<HephaestusAudioSOInstaller>
    {
        [SerializeField]
        private AudioManagerConfig _audioManagerConfig;

        public override void InstallBindings()
        {
            Container.BindInstance(_audioManagerConfig);
        }
    }
}