using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawnable
{
    void SetParent(GameObject parent);
    void SetStartPosition(Vector3 position);

}
