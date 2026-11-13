using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; private set; }

    //[field: Header("Player SFX")]
    //[field: SerializeField] public EventReference playerMovement { get; private set; }
    // __________
    [field: Header("Choral Fact")]
    [field: SerializeField] public EventReference choralFact { get; private set; }
    // __________

    [field: Header("Env SFX")]
    [field: SerializeField] public EventReference pickUpTrash { get; private set; }
    [field: SerializeField] public EventReference splash { get; private set; }
    [field: SerializeField]
    public EventReference saug
    { get; private set; }

    // TODO: FMOD-Pfade aus FMOD Studio eintragen (z.B. "event:/Ambience/Underwater")
public const string AmbiencePath   = "event:/Ambience/Underwater";   // TODO: verify
public const string MusicPath      = "event:/Music/Main";             // TODO: verify
public const string ChoralFactPath = "event:/SFX/ChoralFact";         // TODO: verify
public const string PickUpTrashPath = "event:/SFX/PickUpTrash";       // TODO: verify
public const string SplashPath     = "event:/SFX/Splash";             // TODO: verify
public const string SaugPath       = "event:/SFX/Saug";               // TODO: verify

public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }
        instance = this;
    }
}