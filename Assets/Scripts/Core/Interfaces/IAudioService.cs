using UnityEngine;

public interface IAudioService
{
    void SetMusicParameter(string parameterName, string value);
    void SetAmbienceParameter(string parameterName, float value);
    void PlayOneShot(string eventPath, Vector3 worldPosition);
    void PlayFact(int factIndex);
}
