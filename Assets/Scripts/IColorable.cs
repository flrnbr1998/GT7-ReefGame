using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IColorable
{
    void Bleach();
    void Recolor();
    Color getCurrentColor();
    Color getOriginalColor();
    bool getIsGrowing();
}
