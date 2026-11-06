using UnityEngine;

// No-op audio service for PC builds without FMOD
public class StubAudioService : IAudioService
{
    public void SetMusicParameter(string parameterName, string value) { }
    public void SetAmbienceParameter(string parameterName, float value) { }
    public void PlayOneShot(string eventPath, Vector3 worldPosition) { }
    public void PlayFact(int factIndex) { }
}
