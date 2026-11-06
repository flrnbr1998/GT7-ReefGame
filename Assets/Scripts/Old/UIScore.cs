using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIScore : MonoBehaviour  
{
    private TextMeshProUGUI textMeshProUGUI;
    private TextMeshProUGUI textMeshProPlayerName;
    // Start is called before the first frame update
    void Start()
    {
        textMeshProUGUI = gameObject.GetComponent<TextMeshProUGUI>();
        textMeshProPlayerName = gameObject.GetComponent<TextMeshProUGUI>();    
    }

    // Update is called once per frame
    void Update()
    {
        textMeshProUGUI.text = GameManager.Instance.diceCount.ToString();
    }
}
