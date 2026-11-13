public interface IColorEffectService
{
    void TriggerBleach();
    void TriggerRecolor();
    void StopEffect();
    void Register(IColorable colorable);
}
