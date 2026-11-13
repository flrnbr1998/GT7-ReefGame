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
        builder.Register<PCInputService>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<ReefStateService>(Lifetime.Singleton).AsImplementedInterfaces();

        // MonoBehaviour services (must exist in scene)
        builder.RegisterComponentInHierarchy<AudioManager>().AsImplementedInterfaces();
        builder.RegisterComponentInHierarchy<CoralColorEffectService>().AsImplementedInterfaces();
        builder.RegisterComponentInHierarchy<TerrainSpawner>().AsImplementedInterfaces();
        builder.RegisterComponentInHierarchy<TrashSpawnerScript>().AsImplementedInterfaces();
        builder.RegisterComponentInHierarchy<PickupFeedbackService>().AsImplementedInterfaces();
        builder.RegisterComponentInHierarchy<AnimalSpawnService>().AsImplementedInterfaces();
        builder.RegisterComponentInHierarchy<HUDController>().AsImplementedInterfaces();

        // MonoBehaviours that receive injection
        builder.RegisterComponentInHierarchy<RiffGameLoop>();
        builder.RegisterComponentInHierarchy<PlayerController>();
        builder.RegisterComponentInHierarchy<PickupInputHandler>();
        builder.RegisterComponentInHierarchy<CoralPlantHandler>();
        builder.RegisterComponentInHierarchy<TrashHighlightHandler>();
        builder.RegisterComponentInHierarchy<GameSessionController>();
        builder.RegisterComponentInHierarchy<SharkMovement>();
        // TrashPickupHandler is runtime-instantiated — injected via container.InjectGameObject() in TrashSpawnerScript
    }
}
