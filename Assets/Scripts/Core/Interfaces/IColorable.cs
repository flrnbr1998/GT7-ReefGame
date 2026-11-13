using UnityEngine;

public interface IColorable
{
    void Bleach();
    void Recolor();
    Color getCurrentColor();
    Color getOriginalColor();
    bool getIsGrowing();
}
