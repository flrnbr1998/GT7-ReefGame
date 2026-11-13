using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CoralGrowthBase : MonoBehaviour, ISpawnable, IColorable
{
    protected string axiom;
    protected string currentString;
    protected Stack<TransformInfo> transformStack;
    protected Vector3 position;
    protected static System.Random random;
    MeshRenderer meshRenderer;

    protected Light coralLight;
    protected Color originalLightColor;
    protected Color currentLightColor;
    public GameObject parent;
    private Color originalColor;
    private Color currentColor;

    protected int iterations;
    public float length;
    public float growthSpeed;
    protected float[] rgbArray;
    protected int minAngle;
    protected int maxAngle;
    protected bool isStaghornCoral;
    protected bool isTreeCoral;
    protected bool isRandomCoral;
    protected bool isRedCoral;
    private Vector3 lastTopPosition;
    private bool isGrowing;
    private bool hasAllComponents;

    private IColorEffectService _colorEffect;

    protected List<GameObject> childObjects;

    protected Dictionary<char, string> rules = new Dictionary<char, string>
    {
            {'X', "F"},
            {'F', "F" }
    };

    protected abstract List<Rule> GetRulesList();
    protected abstract GameObject GetPrefab();

    public CoralGrowthBase()
    {
        axiom = "X";
        random = new System.Random();
        growthSpeed = 1f;
        iterations = 3;
        isTreeCoral = false;
        isStaghornCoral = false;
        isRandomCoral = false;
        childObjects = new List<GameObject>();
    }

    public virtual void Start()
    {
        isGrowing = true;
        hasAllComponents = false;
        currentString = axiom;
        transformStack = new Stack<TransformInfo>();
        GetComponent<MeshFilter>().sharedMesh = new Mesh();


        if (TryGetComponent<Light>(out coralLight))
        {
            originalLightColor = coralLight.color;
            currentLightColor = originalLightColor;
        }

        StartCoroutine(GrowLSystem());
        StartCoroutine(CombineMeshesAfterGrowing());

    }

    IEnumerator CombineMeshesAfterGrowing()
    {
        bool run = true;
        while (run)
        {
           
            if (isGrowing && run && hasAllComponents)
            {
                MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
                CombineInstance[] combine = new CombineInstance[meshFilters.Length];

                Matrix4x4 parentTransform = transform.worldToLocalMatrix;

                isGrowing = getIsGrowing();
                if (!isGrowing)
                {
                    for (int i = 1; i < meshFilters.Length; i++)
                    {

                        combine[i].mesh = meshFilters[i].sharedMesh;
                        combine[i].transform = parentTransform * meshFilters[i].transform.localToWorldMatrix; // angepasst
                        meshFilters[i].gameObject.SetActive(false);
                    }

                    Mesh mesh = new Mesh();
                    mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                    mesh.CombineMeshes(combine, true, true); // angepasst
                    transform.GetComponent<MeshFilter>().sharedMesh = mesh;

                    MeshFilter rootMeshFilter = transform.GetComponent<MeshFilter>();
                    if (rootMeshFilter == null)
                    {
                        rootMeshFilter = gameObject.AddComponent<MeshFilter>();
                    }
                    rootMeshFilter.sharedMesh = mesh;

                    meshRenderer = transform.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        meshRenderer.material.color = originalColor;

                    }
                    else
                    {
                        Debug.LogWarning("No MeshRenderer found on the GameObject.");
                    }

                    transform.gameObject.SetActive(true);
                    _colorEffect?.Register(this);

                    run = false;
                }
            }

            // Wait for 10 seconds
            yield return new WaitForSeconds(10.0f);
        }
    }

    public void SetColorEffectService(IColorEffectService service) => _colorEffect = service;

    public void SetStartPosition(Vector3 startPosition)
    {
        position = startPosition;
        lastTopPosition = position;
    }

    public void SetParent(GameObject parentObject)
    {
        parent = parentObject;
    }

    protected IEnumerator GrowLSystem()
    {
        
        for (int i = 0; i < iterations; i++)
        {
            currentString = ApplyRules(currentString);
        }
        currentString += "E";

        if (isRandomCoral)
        {
            Debug.Log(currentString);
        }

        Vector3 direction = Vector3.up;

        if (isTreeCoral)
        {
            // Set initial position at the base of the tree
            position = transform.position;
            lastTopPosition = position; // Initially set lastTopPosition to the base position
        }

        foreach (char c in currentString)
        {
            switch (c)
            {
                case 'F':
                    if (isStaghornCoral)
                    {
                        direction = Quaternion.Euler(SetAngle(-90, 90), 0, SetAngle(-90, 90)) * direction;
                        isStaghornCoral = false;
                    }
                    else if(isRandomCoral)
                    {

                    }
                    GrowTree(ref position, direction);
                    yield return new WaitForSeconds(growthSpeed);
                    break;
                case 'X':
                    break;
                case '+':
                    direction = Quaternion.Euler(SetAngle(minAngle, maxAngle) * -1, 0, 0) * direction;
                    break;
                case '-':
                    direction = Quaternion.Euler(SetAngle(minAngle, maxAngle), 0, 0) * direction;
                    break;
                case '&':
                    direction = Quaternion.Euler(0, 0, SetAngle(minAngle, maxAngle)) * direction;
                    break;
                case '^':
                    direction = Quaternion.Euler(0, 0, SetAngle(minAngle, maxAngle) * -1) * direction;
                    break;
                case '[':
                    SaveTransformState(position, direction);
                    break;
                case ']':
                    RestoreTransformState(ref position, ref direction);
                    break;
                case 'E':
                    hasAllComponents = true;
                    break;
            }
        }
    }

    protected string ApplyRules(string input)
    {
        var result = new System.Text.StringBuilder();
        float totalProbability = 0;
        string ruleForX = "";

        if (!isRandomCoral)
        {
            foreach (var rule in GetRulesList())
            {
                totalProbability += rule.probability;
            }

            ruleForX = ChooseRule(totalProbability);
        }

        foreach (char c in input)
        {
            if (c == 'X')
            {
               if (isRandomCoral)
                {
                    result.Append(rules[c]);
                }
               else
                {
                    result.Append(ruleForX);
                }
                
            }
            else if (rules.ContainsKey(c))
            {
                result.Append(rules[c]);
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }

    protected void GrowTree(ref Vector3 position, Vector3 direction)
    {
        Vector3 newPosition;
        GameObject tree;

        if (isTreeCoral || isRandomCoral)
        {
            newPosition = lastTopPosition + direction * length * 0.1f;

            // Instantiate the object with a scale of zero
            tree = Instantiate(GetPrefab(), newPosition, Quaternion.identity);
            tree.transform.SetParent(transform);
            UpdateChildList();
            //tree.transform.localScale = Vector3.zero; // Set scale to zero initially
            tree.transform.position += direction;

            // Align cylinder's up direction with current direction
            tree.transform.up = direction;

            // Update lastTopPosition to the top of the newly spawned cylinder
            lastTopPosition = newPosition + direction;
        }
        else
        {
            newPosition = position + direction * length;

            // Instantiate the object with a scale of zero
            tree = Instantiate(GetPrefab(), position, Quaternion.LookRotation(direction));
            tree.transform.SetParent(transform);
            UpdateChildList();
            tree.transform.localScale = Vector3.zero; // Set scale to zero initially
        }

        if (tree != null)
        {
            StartCoroutine(SetTreeColorNextFrame(tree));
        }

        position = newPosition;
    }


    protected IEnumerator SetTreeColorNextFrame(GameObject tree)
    {
        {
            yield return null; // Wait for the next frame

            TreeGrowth treeGrowth = tree.GetComponent<TreeGrowth>();
            CylinderGrowth cylinderGrowth = tree.GetComponent<CylinderGrowth>();
            RandomCoralGrowth randomGrowth = tree.GetComponent<RandomCoralGrowth>();
            RedComponentGrowth redGrowth = tree.GetComponent<RedComponentGrowth>();

            if (treeGrowth != null)
            {
                treeGrowth.SetColor(rgbArray);
                currentColor = originalColor = new Color(rgbArray[0], rgbArray[1], rgbArray[2], rgbArray[3]);
            }
            else if (cylinderGrowth != null)
            {
                cylinderGrowth.SetColor(rgbArray);
                currentColor = originalColor = new Color(rgbArray[0], rgbArray[1], rgbArray[2]);
            }
            else if (randomGrowth != null)
            {
                randomGrowth.SetColor(rgbArray);
                currentColor = originalColor = new Color(rgbArray[0], rgbArray[1], rgbArray[2], rgbArray[3]);
            }
            else if (redGrowth != null)
            {
                redGrowth.SetColor(rgbArray);
                currentColor = originalColor = new Color(rgbArray[0], rgbArray[1], rgbArray[2], rgbArray[3]);
            }
        }
    }

    protected void UpdateChildList()
    {
        childObjects.Clear();

        foreach (Transform child in transform)
        {
            childObjects.Add(child.gameObject);
        }

        //Debug.Log("Total children collected: " + childObjects.Count);
    }

    public static float SetAngle(int minAngle, int maxAngle)
    {
        return (float)random.Next(minAngle, maxAngle);
    }

    private void SaveTransformState(Vector3 position, Vector3 direction)
    {
        transformStack.Push(new TransformInfo
            {
                position = position,
                direction = direction,
                lastTopPosition = lastTopPosition
            });        
    }

    private void RestoreTransformState(ref Vector3 position, ref Vector3 direction)
    {
        var ti = transformStack.Pop();
        position = ti.position;
        direction = ti.direction;
        lastTopPosition = ti.lastTopPosition;
    }

    private string ChooseRule(float totalProbability)
    {
        float randomPoint = (float)(random.NextDouble() * totalProbability);

        foreach (var rule in GetRulesList())
        {
            if (randomPoint < rule.probability)
            {
                return rule.ruleString;
            }
            randomPoint -= rule.probability;
        }
        return "F";
    }

    public virtual void Recolor()
    {
        if (this != null)
        {
            StartCoroutine(ColorTransitionLight(1f, currentLightColor, originalLightColor));
            StartCoroutine(ColorTransitionCoral(1f, currentColor, originalColor));
            currentLightColor = originalLightColor;
        }
    }

    public virtual void Bleach()
    {
        if (this != null)
        {
            StartCoroutine(ColorTransitionLight(1f, originalLightColor, Color.white));
            StartCoroutine(ColorTransitionCoral(1f, originalColor, Color.white));
            currentLightColor = Color.white;
        }
    }

    public Color getOriginalColor()
    {
        return originalLightColor;
    }

    public Color getCurrentColor()
    {
        return currentLightColor;
    }

    public bool getIsGrowing()
    {
        foreach (GameObject child in childObjects)
        {
            if (child != null)
            {
                ICombinable combinableChild = child.GetComponent<ICombinable>();
                if (combinableChild != null && combinableChild.getIsGrowing())
                {
                    return true;
                }
            }
        }
        return false;
    }

    private IEnumerator ColorTransitionLight(float duration, Color originalLightColor, Color targetColor)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            if (this == null)
            {
                break;
            }
            try
            {
                if (coralLight.color != null)
                {
                    coralLight.color = Color.Lerp(originalLightColor, targetColor, t);
                }
            }
            catch (Exception)
            {
            }
            yield return null;
        }

        if (coralLight.color != null)
        {
            coralLight.color = targetColor;
        }
        
    }

    private IEnumerator ColorTransitionCoral(float duration, Color originalColor, Color targetColor)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            meshRenderer.material.color = Color.Lerp(originalColor, targetColor, t);
            yield return null;
        }

        meshRenderer.material.color = targetColor;
    }

    protected void SetRGBArray(float[] commonColor, float[] uncommonColor, float[] rareColor)
    {

        int randomVal = random.Next(1, 101);

        if (randomVal >= 90 && randomVal < 99)
        {

            rgbArray = uncommonColor;

        }
        else if (randomVal >= 99)
        {
            rgbArray = rareColor;
        }
        else
        {
            rgbArray = commonColor;

        }
    }

    protected float[] GetRGBArray() { return rgbArray; }

    protected class Rule
    {
        public string ruleString { get; private set; }
        public float probability { get; private set; }

        public Rule(string ruleString, float probability)
        {
            this.ruleString = ruleString;
            this.probability = probability;
        }
    }

    protected struct TransformInfo
    {
        public Vector3 position;
        public Vector3 direction;
        public Vector3 lastTopPosition;
    }
}
