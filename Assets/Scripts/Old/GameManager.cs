using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public int diceCount;
    public string playerName;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
}
