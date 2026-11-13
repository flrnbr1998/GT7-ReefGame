using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class BubbleCoralGrowth : CoralGrowthBase
{
    public GameObject bubbleMaterial;

    public BubbleCoralGrowth()
    {
        SetRGBArray(new float[] { 0.36f, 0.7f, 0.98f, 0.75f }, new float[]  { 0.85f, 0.98f, 0.36f, 0.75f }, new float[] { 0.98f, 0.58f, 0.81f, 0.75f });
        rgbArray = GetRGBArray();
        iterations++;
        minAngle = 25;
        maxAngle = 75;
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
