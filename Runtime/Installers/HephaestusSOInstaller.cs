using HephaestusMobile.Audio.Manager;
using UnityEngine;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = "HephaestusSOInstaller", menuName = "HephaestusMobile/Core/Audio/HephaestusSOInstaller")]
    public class HephaestusSOInstaller : ScriptableObjectInstaller<HephaestusSOInstaller>
    {
        [SerializeField]
        private AudioManagerConfig _audioManagerConfig;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_audioManagerConfig);
        }
    }
}