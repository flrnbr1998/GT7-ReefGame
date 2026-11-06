using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class RedCoralGrowth : CoralGrowthBase
{
    public GameObject bubbleMaterial;

    public RedCoralGrowth()
    {
        isRedCoral = true;
        SetRGBArray(new float[] { 0.56f, 0.07f, 0.05f, 1 }, new float[] { 0.98f, 0.5f, 0.7f, 1f }, new float[] { 0.5f, 0.1f, 0.55f, 1f });
        rgbArray = GetRGBArray();
        iterations=3;
        minAngle = -45;
        maxAngle = 45;
        
    }

    protected override GameObject GetPrefab()
    {
        return bubbleMaterial;
    }

    protected override List<Rule> GetRulesList()
    {
        return new List<Rule>()
        {
            new Rule("F[+FX][&++X][^X][--X]", 0.35f),
            new Rule("F[+FX][-^FX][&X][++X]", 0.25f),
            new Rule("F[--X][+FX][^+X][&-X]", 0.40f)
        };
    }
}
