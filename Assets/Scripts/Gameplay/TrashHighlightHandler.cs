using System.Collections.Generic;
using UnityEngine;

public class TrashHighlightHandler : MonoBehaviour
{
    [SerializeField] private float highlightRadius = 5f;

    private readonly HashSet<IHighlightable> _highlighted = new HashSet<IHighlightable>();

    private void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, highlightRadius);

        var current = new HashSet<IHighlightable>();
        foreach (Collider col in hits)
        {
            if (col.GetComponentInParent<TrashPickupHandler>() == null) continue;
            var h = col.GetComponentInParent<IHighlightable>();
            if (h != null) current.Add(h);
        }

        foreach (IHighlightable h in current)
            if (!_highlighted.Contains(h)) h.Highlight();

        foreach (IHighlightable h in _highlighted)
        {
            if (h is MonoBehaviour mb && mb == null) continue;
            if (!current.Contains(h)) h.Unhighlight();
        }

        _highlighted.Clear();
        foreach (IHighlightable h in current) _highlighted.Add(h);
    }

    private void OnDisable()
    {
        foreach (IHighlightable h in _highlighted)
        {
            if (h is MonoBehaviour mb && mb == null) continue;
            h.Unhighlight();
        }
        _highlighted.Clear();
    }
}
