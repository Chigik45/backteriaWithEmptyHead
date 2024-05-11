using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingSpawn : MonoBehaviour
{
    public static bool Pause = false;
    [SerializeField] GameObject foodPrefab;
    [SerializeField] GameObject bacteriaBrefab;
    float bef = 0;
    private void Start()
    {
        StartNewGeneration();
    }
    public void StartNewGeneration()
    {
        for (int i = 0; i < 5; ++i)
        {
            GameObject curr = Instantiate(bacteriaBrefab, new Vector3(Random.Range(-9f, 9), Random.Range(-2.5f, 5), 0), Quaternion.identity);
            curr.GetComponent<BacteriaScript>().isInitial = true;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            foreach (var el in BacteriaScript.bacteriaScripts)
            {
                el.timeToLiveActual = -10;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ThingSpawn.Pause = !ThingSpawn.Pause;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            FindObjectOfType<ModuleShower>().Clear();
        }
        if (ThingSpawn.Pause)
            return;
        bef -= Time.deltaTime;
        if (bef < 0)
        {
            bef = 0.1f;
            FoodMarker foodMarker = Instantiate(foodPrefab, new Vector3(Random.Range(-9f, 9), Random.Range(-2.5f, 5), 0), Quaternion.identity).GetComponent<FoodMarker>();
            foodMarker.ChangeSource(FoodSource.plant);
        }
    }
}
