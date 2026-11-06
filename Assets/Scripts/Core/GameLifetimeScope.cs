using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private WellnessConfig wellnessConfig;

    protected override void Configure(IContainerBuilder builder)
    {
        // Config
        builder.RegisterInstance(wellnessConfig);

        // Pure C# services
        builder.Register<WellnessService>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<StubAudioService>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<PCInputService>(Lifetime.Singleton).AsImplementedInterfaces();

        // MonoBehaviour services (must exist in scene)
        builder.RegisterComponentInHierarchy<CoralColorEffectService>().AsImplementedInterfaces();
    }
}
