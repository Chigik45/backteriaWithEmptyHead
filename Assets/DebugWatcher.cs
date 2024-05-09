using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugWatcher : MonoBehaviour
{
    float befWatch = 0;
    int maxGen = 0;
    int simOfMaxGen = 0;
    int restartCount = 0;
    Text txt;
    private void Start()
    {
        txt = GetComponent<Text>();
    }
    void Update()
    {
        befWatch -= Time.deltaTime;
        if (befWatch <= 0)
        {
            foreach (var el in BacteriaScript.bacteriaScripts)
            {
                if (el.generation > maxGen)
                {
                    maxGen = el.generation;
                    simOfMaxGen = restartCount;
                }
            }
            if (BacteriaScript.bacteriaScripts.Count == 0)
            {
                restartCount++;
                foreach (var el in FoodMarker.foodScripts)
                {
                    Destroy(el.gameObject);
                }
                FindObjectOfType<ThingSpawn>().StartNewGeneration();
            }
            befWatch = 10f;
            txt.text = "Max gen: " + maxGen + " of " + simOfMaxGen + "\nCurrent sim: " + restartCount;
        }
    }
}
