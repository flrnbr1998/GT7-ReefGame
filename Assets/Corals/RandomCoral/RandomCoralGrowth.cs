using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCoralGrowth : MonoBehaviour, ICombinable
{
    private Renderer[] renderers;
    private Vector3 initialPosition;
    private bool isGrowing;

    // Start is called before the first frame update
    void Start()
    {
        // Get all renderers in children
        transform.localScale = Vector3.zero;
        renderers = GetComponentsInChildren<Renderer>();
        initialPosition = transform.position;
        isGrowing = true;
    }

    void Update()
    {
        Vector3 newPosition = initialPosition + Vector3.up;
        transform.position = newPosition;
        transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        isGrowing = false;
        gameObject.isStatic = true;

    }

    public bool getIsGrowing()
    {
        return isGrowing;
    }

    // Coloring logic
    public void SetColor(float[] color)
    {
        Color newColor = new Color(color[0], color[1], color[2], color[3]);

        // Apply the new color to all renderers
        foreach (Renderer rend in renderers)
        {
            rend.material.color = newColor;
        }

    }
}
