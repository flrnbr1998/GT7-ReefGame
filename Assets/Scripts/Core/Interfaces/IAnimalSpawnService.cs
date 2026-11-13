using System;

public interface IAnimalSpawnService
{
    event Action<AnimalType> OnAnimalSpawned;
    event Action<AnimalType> OnAnimalDespawned;
}
