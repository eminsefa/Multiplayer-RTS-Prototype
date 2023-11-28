using Fusion;
using Zenject;

namespace Project.Scripts.Managers
{
    public class ProjectMonoInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<LevelManager>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<InputManager>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<UIManager>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<NetworkRunner>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<CameraManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        }
    }
}