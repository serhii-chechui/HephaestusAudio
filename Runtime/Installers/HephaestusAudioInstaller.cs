using Zenject;

namespace WTFGames.Hephaestus.AudioSystem
{
    public class HephaestusAudioInstaller : Installer<HephaestusAudioInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<AudioManager>().AsSingle();
        }
    }
}