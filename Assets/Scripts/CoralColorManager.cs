using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoralColorManager : MonoBehaviour
{
    private static CoralColorManager instance;
    private List<IColorable> colorableObjects = new List<IColorable>();
    private static bool isBleachingOrRecoloring;

    public int secondsToWait = 20;

    // Singleton instance property
    public static CoralColorManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CoralColorManager>();
                if (instance == null)
                {
                    Debug.LogError("CoralColorManager is missing in the scene.");
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void SetisBleachingOrRecoloring(bool status)
    {
        isBleachingOrRecoloring = status;
    }

    public static void RegisterColorable(IColorable colorable)
    {
        if (!instance.colorableObjects.Contains(colorable))
        {
            instance.colorableObjects.Add(colorable);
        }
    }

    public void DeregisterColorable(IColorable colorable)
    {
        if (instance.colorableObjects.Contains(colorable))
        {
            instance.colorableObjects.Remove(colorable);
        }
    }

    public IEnumerator TriggerColorChange(bool badCondition)
    {
        Debug.Log(isBleachingOrRecoloring);
        while (isBleachingOrRecoloring)
        {
            StartCoroutine(TriggerColorChangeCoroutine(badCondition));
            yield return new WaitForSeconds(10);
        }
        
    }

    private IEnumerator TriggerColorChangeCoroutine(bool badCondition)
    {

        List<IColorable> colorablesCopy  = new List<IColorable>(colorableObjects);
        

        Color currentColor;
        Color originalColor;

        foreach (IColorable colorable in colorablesCopy)
        {
            currentColor = colorable.getCurrentColor();
            originalColor = colorable.getOriginalColor();
            bool isGrowing = colorable.getIsGrowing();
            if (badCondition && currentColor == originalColor && !isGrowing)
            {
                colorable?.Bleach();
            }
                
            else if (!badCondition && currentColor != originalColor && !isGrowing)
            {
                colorable?.Recolor();
            }
            yield return new WaitForSeconds(secondsToWait); //new WaitForSeconds(5)

        }
    }

}
