using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodMarker : MonoBehaviour
{
    public static List<FoodMarker> foodScripts = new List<FoodMarker>();
    public bool isEatten = false;
    public float nutrients = 20f;
    public FoodSource source = FoodSource.plant;
    float timeLived = 0;
    bool getRotten = false;
    private void Start()
    {
        foodScripts.Add(this);
    }
    private void OnDestroy()
    {
        foodScripts.Remove(this);
    }

    private void Update()
    {
        timeLived += Time.deltaTime;
        if (!getRotten && timeLived > 60)
        {
            getRotten = true;
            ChangeSource(FoodSource.rot);
        }
        if (timeLived > 300)
        {
            Destroy(gameObject);
        }
    }
    public void ChangeSource(FoodSource newSource)
    {
        source = newSource;
        switch (source)
        {
            case FoodSource.plant:
                GetComponent<SpriteRenderer>().color = Color.green;
                break;
            case FoodSource.meat:
                GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case FoodSource.rot:
                GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            default:
                GetComponent<SpriteRenderer>().color = Color.white;
                break;
        }
    }
}

public enum FoodSource
{
    plant, meat, rot
}