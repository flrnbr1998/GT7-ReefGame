using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCoralGrowth : CoralGrowthBase
{
    public GameObject branchObject;

    public TreeCoralGrowth()
    {
        isTreeCoral = true;
        iterations = 3;
        SetRGBArray(new float[] { 0.98f, 0.4f, 0.31f }, new float[] { 1f, 0.7f, 0.3f  }, new float[] {  0.31f, 0.98f, 0.82f });
        rgbArray = GetRGBArray();
        minAngle = 5;
        maxAngle = 45;
        
    }

    protected override GameObject GetPrefab()
    {
        return branchObject;
    }

    // List of rules with probabilities for randomness
    protected override List<Rule> GetRulesList()
    {
        return new List<Rule> {
            new Rule("F[+FX][-^FX][&X][++X]", 0.5f),
            new Rule("F[+FX][-^FX][&X][++F][--X][^^X][&&X]", 0.5f)
        };
    }

   
}