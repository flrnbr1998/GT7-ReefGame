using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;

public class DiceScript : MonoBehaviour
{

    [SerializeField] private GameObject one;
    [SerializeField] private GameObject two;
    [SerializeField] private GameObject three;
    [SerializeField] private GameObject four;
    [SerializeField] private GameObject five;
    [SerializeField] private GameObject six;


    private Rigidbody rb;
    private bool hasLanded;
    private bool wasThrown;
    private bool isOnline;
    public Vector3 startPosition = new Vector3(0.0f, 0.0f, -0.0f); // Startposition des Würfels
    public Renderer cubeRenderer; // Renderer des Würfels, um die Farbe zu ändern
    private int diceValue;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = rb.GetComponent<Renderer>().transform.position;
        hasLanded = true;
        isOnline = false;
    }

    void Update()
    {
        if (isOnline) {
            if (rb.IsSleeping() && !hasLanded)
            {
                hasLanded = true;
                diceValue = getValue();
                ChangeCubeColor(diceValue); // Ändere die Farbe basierend auf der gewürfelten Zahl
                Debug.Log("Würfelergebnis: " + diceValue);
                wasThrown = true;
                GameManager.Instance.diceCount = diceValue;
            }
            else if (rb.GetComponent<Renderer>().transform.position.y > 5.0f)
            {
                hasLanded = false;
            }
        }
    }

    public void ResetWuerfel()
    {
        transform.position = startPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        hasLanded = true;
    }

    public void ResetDiceValue()
    {
        diceValue = 0;
        GameManager.Instance.diceCount = diceValue;
    }

    private void ChangeCubeColor(int diceValue)
    {
        Color color = GetColorForDiceValue(diceValue);
        cubeRenderer.material.color = color;
    }

    private Color GetColorForDiceValue(int diceValue)
    {
        switch (diceValue)
        {
            case 1:
                return Color.red;
            case 2:
                return Color.blue;
            case 3:
                return Color.green;
            case 4:
                return Color.yellow;
            case 5:
                return Color.cyan;
            case 6:
                return Color.magenta;
            default:
                return Color.white;
        }
    }
    public int getDiceValue()
    {
        wasThrown = false;
        hasLanded = true;
        getValue();
        return diceValue;
    }


    public bool GetWasThrown()
    {
        return wasThrown;
    }

    public void setWuerfelOffline()
    {
        isOnline = false;
    }

    public void setWuerfelOnline()
    {
        isOnline = true;
    }

    public void setInactive()
    {
        rb.GetComponent<Renderer>().enabled = false;
        setWuerfelOffline();
    }

    public void setActive()
    {
        rb.GetComponent<Renderer>().enabled = true;
        setWuerfelOnline();
    }

    private int getValue()
    {
        float one_h = one.GetComponent<Renderer>().transform.position.y;
        float two_h = two.GetComponent<Renderer>().transform.position.y;
        float three_h = three.GetComponent<Renderer>().transform.position.y;
        float four_h = four.GetComponent<Renderer>().transform.position.y;
        float five_h = five.GetComponent<Renderer>().transform.position.y;
        float six_h = six.GetComponent<Renderer>().transform.position.y;
        float max = one_h;
        int value = 1;
        if (max < two_h) {
            max = two_h;
            value = 2;
        }
        if (max < three_h)
        {
            max = three_h;
            value = 3;
        }
        if (max < four_h)
        {
            max = four_h;
            value = 4;
        }
        if(max < five_h)
        {
            max = five_h;
            value = 5;
        }
        if (max < six_h)
        {
            max = six_h;
            value = 6;
        }
        return value;
    }
}
