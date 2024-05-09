using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodMarker : MonoBehaviour
{
    public static List<FoodMarker> foodScripts = new List<FoodMarker>();
    public bool isEatten = false;
    private void Start()
    {
        foodScripts.Add(this);
    }
    private void OnDestroy()
    {
        foodScripts.Remove(this);
    }
}
