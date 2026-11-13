using UnityEngine;

public class TrashHighlight : MonoBehaviour, IHighlightable
{
    private Renderer[] _renderers;
    private MaterialPropertyBlock _mpb;
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
        _mpb = new MaterialPropertyBlock();
    }

    public void Highlight()
    {
        foreach (Renderer r in _renderers)
        {
            if (r == null) continue;
            r.GetPropertyBlock(_mpb);
            _mpb.SetColor(ColorId, new Color(1f, 1f, 0.3f));
            r.SetPropertyBlock(_mpb);
        }
    }

    public void Unhighlight()
    {
        var empty = new MaterialPropertyBlock();
        foreach (Renderer r in _renderers)
        {
            if (r == null) continue;
            r.SetPropertyBlock(empty);
        }
    }
}
