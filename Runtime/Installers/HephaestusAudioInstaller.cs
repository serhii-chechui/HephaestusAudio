using HephaestusMobile.Audio.Manager;
using Zenject;

namespace HephaestusMobile.Audio.Installers
{
    public class HephaestusAudioInstaller : Installer<HephaestusAudioInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<AudioManager>().AsSingle();
        }
    }
}