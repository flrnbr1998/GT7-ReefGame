using UnityEngine;
using System.Collections;

public class CylinderGrowth : MonoBehaviour, ICombinable
{
    public float growthDuration = 10f;
    private float startTime;
    private Renderer treeRenderer;
    private bool isGrowing = true;

    private Renderer[] renderers;
    private Vector3 initialScale;
    private Vector3 initialPosition;

    void Start()
    {
        startTime = Time.time;
        treeRenderer = GetComponent<Renderer>();

        // Record the initial scale and position of the object
        renderers = GetComponentsInChildren<Renderer>();
        initialScale = new Vector3 (3, 5, 3);
        initialPosition = transform.position;

        // Start with scale set to zero
        transform.localScale = Vector3.zero;

        // Initialize colors
        // originalColor = treeRenderer.material.color;
    }

    // Grow correctly
    void Update()
    {
        if (isGrowing)
        {
            float elapsedTime = Time.time - startTime;
            float scaleFactor = Mathf.Clamp01(elapsedTime / growthDuration);

            transform.localScale = Vector3.Lerp(Vector3.zero, initialScale, scaleFactor);


            if (scaleFactor >= 1)
            {
                isGrowing = false;
                gameObject.isStatic = true;
            }
        }
    }

    public bool getIsGrowing()
    {
        return isGrowing;
    }

    public void SetColor(float[] color)
    {
        Color newColor = new Color(color[0], color[1], color[2]);

        // Apply the new color to all renderers
        foreach (Renderer rend in renderers)
        {
            rend.material.color = newColor;
        }

    }
}
