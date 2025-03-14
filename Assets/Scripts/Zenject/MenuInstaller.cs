using Zenject;

public class MenuInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<OptionsProvider>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<DifficultyProvider>().AsSingle();
        Container.BindInterfacesAndSelfTo<ScoreProviderConfigLoader>().AsSingle();
        Container.BindInterfacesAndSelfTo<ScoreProvider>().AsSingle();
    }
}