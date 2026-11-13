using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class StaghornCoralGrowth : CoralGrowthBase
{
    public GameObject treePrefab;
    private List<GameObject> branchObjects = new List<GameObject>();

    public StaghornCoralGrowth()
    {
        SetRGBArray(new float[] { 0.79f, 0.44f, 0.01f, 1f}, new float[] { 0.76f, 0.05f, 0.09f, 1f  }, new float[] {0.47f, 0.06f, 0.89f, 1f});
        rgbArray = GetRGBArray();
        minAngle = 25;
        maxAngle = 75;
        isStaghornCoral = true;
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

    protected override GameObject GetPrefab()
    {
        return treePrefab;
    }
}
