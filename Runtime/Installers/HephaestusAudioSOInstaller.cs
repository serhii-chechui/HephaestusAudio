using HephaestusMobile.Audio.Manager;
using UnityEngine;
using Zenject;

namespace HephaestusMobile.Audio.Installers
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