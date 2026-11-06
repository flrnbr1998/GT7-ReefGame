using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class RedComponentGrowth : MonoBehaviour, ICombinable
{
    public float growthDuration = 10f;
    private float startTime;
    private Renderer treeRenderer;
    private bool isGrowing = true;
    private Vector3 initialScale;

    void Start()
    {
        startTime = Time.time;
        treeRenderer = GetComponent<Renderer>();
        initialScale = new Vector3(7000, 10000, 12000);

        // Ensure the material is set to Transparent rendering mode
        treeRenderer.material.SetFloat("_Mode", 3); // 3 corresponds to the Transparent rendering mode
        treeRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        treeRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        treeRenderer.material.SetInt("_ZWrite", 0);
        treeRenderer.material.DisableKeyword("_ALPHATEST_ON");
        treeRenderer.material.EnableKeyword("_ALPHABLEND_ON");
        treeRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        treeRenderer.material.renderQueue = 3000;
    }

    void Update()
    {
        float elapsedTime = Time.time - startTime;
        float scaleFactor = Mathf.Clamp01(elapsedTime / growthDuration);
        transform.localScale = Vector3.Lerp(Vector3.zero, initialScale, scaleFactor);
        if (elapsedTime >= growthDuration && isGrowing)
        {
            isGrowing = false;
            gameObject.isStatic = true;
        }
    }

    public bool getIsGrowing()
    {
        return isGrowing;
    }

    public void SetColor(float[] color)
    {
        if (treeRenderer != null)
        {
            treeRenderer.material.color = new Color(color[0], color[1], color[2], color[3]);
        }
    }
}