using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LSystemGrowth : CoralGrowthBase
{
    public GameObject branchPrefab;
    public Vector3 scale = new Vector3(1f, 1f, 1f);

    public LSystemGrowth()
    {
        rules = new Dictionary<char, string>
        {
            {'X', "F"},
            {'F', "FFF+&[+F-FF-F&]-[-F+&FF+F^]" }
        };
        SetRGBArray(new float[] { 0.36f, 0.7f, 0.98f, 0.75f }, new float[] {0.85f, 0.98f, 0.36f, 0.75f }, new float[] { 0.98f, 0.58f, 0.81f, 0.75f });
        rgbArray = GetRGBArray();
        minAngle = -25;
        maxAngle = 25;
        isRandomCoral = true;
    }

    protected override List<Rule> GetRulesList()
    {
        return new List<Rule>()
        {
            new Rule("FFF+&[+F-FF-F&]-[-F+&FF+F^]", 1f)
        };
    }

    protected override GameObject GetPrefab()
    {
        return branchPrefab;
    }
    }
