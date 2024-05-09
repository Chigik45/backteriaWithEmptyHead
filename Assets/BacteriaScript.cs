using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BacteriaPreset
{
    rotating
}

public class BacteriaScript : MonoBehaviour
{
    [SerializeField] GameObject bacteriaPrefab;
    [SerializeField] GameObject foodPrefab;
    public bool isInitial = false;
    public BacteriaPreset preset = BacteriaPreset.rotating;
    public static List<BacteriaScript> bacteriaScripts = new List<BacteriaScript>();
    public List<Receptor> receptors = new List<Receptor>();
    public List<Action> actions = new List<Action>();
    public List<GameObject> tied = new List<GameObject>();
    public float foodLevel = 0;
    public float timeToLiveActual = 0;
    public float rotateSpeed = 150;
    public int generation = 0;
    public float poison = 0;
    public Stats stats = new Stats();

    public struct Stats
    {
        public int splitCount;
        public float foodSum;
        public float bitesSum;
        public float photoSum;
    }

    public const float mindBoxSize = 5;
    public const float hungerGlobalSpeed = 10;
    public const float foodMax = 200;
    public const float maxTTL = 60;
    private void Start()
    {
        rotateSpeed = 360;
        bacteriaPrefab = Resources.Load<GameObject>("bact");
        timeToLiveActual = maxTTL;
        if (isInitial)
        {
            GetComponent<SpriteRenderer>().color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            foodLevel = 100;
            switch (preset)
            {
                case BacteriaPreset.rotating:
                    receptors.Add(new Receptor(1, 2, ReceptorType.foodNear, 1, false));
                    receptors.Add(new Receptor(4, 2, ReceptorType.foodFullness, 1, false));
                    actions.Add(new Action(1.5f, 2, ActionType.eat, false));
                    actions.Add(new Action(3.6f, 2, ActionType.split, false));

                    receptors.Add(new Receptor(2, 4, ReceptorType.leftRightRandom, 1, false));
                    actions.Add(new Action(1.25f, 4, ActionType.move, false));
                    actions.Add(new Action(1.5f, 4, ActionType.rotateLeft, false));
                    actions.Add(new Action(2.75f, 4, ActionType.move, false));
                    actions.Add(new Action(2.5f, 4, ActionType.rotateRight, false));
                    break;
            }
        }
        bacteriaScripts.Add(this);
    }
    private void OnDestroy()
    {
        // Instantiate(foodPrefab, transform.position, Quaternion.identity); // starships! :D
        bacteriaScripts.Remove(this);
    }
    private void Update()
    {
        if (ThingSpawn.Pause)
            return;
        Step();
    }
    public void Step()
    {
        foodLevel -= Time.deltaTime * hungerGlobalSpeed;
        timeToLiveActual -= Time.deltaTime;
        if (poison >= 0)
            poison -= Time.deltaTime;
        if (foodLevel <= 0 || timeToLiveActual <= 0)
        {
            Destroy(gameObject);
            return;
        }
        foodLevel = Mathf.Clamp(foodLevel, 0, foodMax);
        HashSet<Action> affected = new HashSet<Action>();
        HashSet<Action> deactivated = new HashSet<Action>();
        foreach (var el in receptors)
        {
            foreach (var act in actions)
            {
                float distBefteenActAndRec = Vector2.Distance(new Vector2(el.x, el.y), new Vector2(act.x, act.y));
                switch (el.type)
                {
                    case ReceptorType.constant:
                        {
                            if (distBefteenActAndRec <= el.sensivity)
                            {
                                if (!el.isDeactivator)
                                    affected.Add(act);
                                else
                                    deactivated.Add(act);
                            }
                        }
                        break;
                    case ReceptorType.leftRightRandom:
                        {
                            if (distBefteenActAndRec <= el.sensivity)
                            {
                                float ran = Random.Range(0, 1f) + Mathf.Sin(Time.time);
                                if (ran > 0)
                                {
                                    if (el.x >= act.x)
                                    {
                                        if (!el.isDeactivator)
                                            affected.Add(act);
                                        else
                                            deactivated.Add(act);
                                    }
                                }
                                else
                                {
                                    if (el.x <= act.x)
                                    {
                                        if (!el.isDeactivator)
                                            affected.Add(act);
                                        else
                                            deactivated.Add(act);
                                    }
                                }
                            }
                        }
                        break;
                    case ReceptorType.bacteriaNear:
                        {
                            float eff = 0;
                            foreach (var bact in bacteriaScripts)
                            {
                                if (bact != this && Vector2.Distance(transform.position, bact.gameObject.transform.position) < 2 &&
                                    Mathf.Clamp(1 / Vector2.Distance(transform.position, bact.gameObject.transform.position), 0, 1) > eff)
                                {
                                    eff = Mathf.Clamp(1 / Vector2.Distance(transform.position, bact.gameObject.transform.position), 0, 1);
                                }
                            }
                            if (distBefteenActAndRec <= el.sensivity * eff)
                            {
                                if (!el.isDeactivator)
                                    affected.Add(act);
                                else
                                    deactivated.Add(act);
                            }
                        }
                    break;
                    case ReceptorType.edgeNear:
                        {
                            float eff = 0;
                            if (Mathf.Abs(transform.position.x) > 8 || transform.position.y > 4 || transform.position.y < -1.5f)
                            {
                                eff = 1;
                            }
                            if (distBefteenActAndRec <= el.sensivity * eff)
                            {
                                if (!el.isDeactivator)
                                    affected.Add(act);
                                else
                                    deactivated.Add(act);
                            }
                        }
                        break;

                    case ReceptorType.sideBacteria:
                        {
                            float eff = 0;
                            int whichSide = 0;
                            foreach (var bact in bacteriaScripts)
                            {
                                if (bact != this && Vector2.Distance(transform.position, bact.gameObject.transform.position) < 2 &&
                                    Mathf.Clamp(1 / Vector2.Distance(transform.position, bact.gameObject.transform.position), 0, 1) > eff)
                                {
                                    float angle = Vector2.SignedAngle(transform.up, bact.gameObject.transform.position - transform.position);
                                    whichSide = angle > 0 ? 0 : 1;
                                    eff = Mathf.Clamp(1 / Vector2.Distance(transform.position, bact.gameObject.transform.position), 0, 1);
                                }
                            }
                            if (distBefteenActAndRec <= el.sensivity * eff)
                            {
                                if (whichSide == 0)
                                {
                                    if (el.x >= act.x)
                                    {
                                        if (!el.isDeactivator)
                                            affected.Add(act);
                                        else
                                            deactivated.Add(act);
                                    }
                                }
                                else
                                {
                                    if (el.x <= act.x)
                                    {
                                        if (!el.isDeactivator)
                                            affected.Add(act);
                                        else
                                            deactivated.Add(act);
                                    }
                                }
                            }
                        }
                        break;
                    case ReceptorType.sideFood:
                        {
                            float eff = 0;
                            int whichSide = 0;
                            foreach (var foods in FoodMarker.foodScripts)
                            {
                                if (foods != this && Vector2.Distance(transform.position, foods.gameObject.transform.position) < 2 &&
                                    Mathf.Clamp(1 / Vector2.Distance(transform.position, foods.gameObject.transform.position), 0, 1) > eff)
                                {
                                    float angle = Vector2.SignedAngle(transform.up, foods.gameObject.transform.position - transform.position);
                                    whichSide = angle > 0 ? 0 : 1;
                                    eff = Mathf.Clamp(1 / Vector2.Distance(transform.position, foods.gameObject.transform.position), 0, 1);
                                }
                            }
                            if (distBefteenActAndRec <= el.sensivity * eff)
                            {
                                if (whichSide == 0)
                                {
                                    if (el.x >= act.x)
                                    {
                                        if (!el.isDeactivator)
                                            affected.Add(act);
                                        else
                                            deactivated.Add(act);
                                    }
                                }
                                else
                                {
                                    if (el.x <= act.x)
                                    {
                                        if (!el.isDeactivator)
                                            affected.Add(act);
                                        else
                                            deactivated.Add(act);
                                    }
                                }
                            }
                        }
                        break;
                    case ReceptorType.foodFullness:
                        {
                            float eff;
                            eff = Mathf.Clamp(foodLevel / foodMax, 0, 1);
                            if (distBefteenActAndRec <= el.sensivity * eff)
                            {
                                if (!el.isDeactivator)
                                    affected.Add(act);
                                else
                                    deactivated.Add(act);
                            }
                        }
                    break;
                    case ReceptorType.wait5:
                        {
                            if (distBefteenActAndRec <= el.sensivity)
                            {
                                if ((Time.time / 5) % 2 == 0)
                                {
                                    if (el.x >= act.x)
                                    {
                                        if (!el.isDeactivator)
                                            affected.Add(act);
                                        else
                                            deactivated.Add(act);
                                    }
                                }
                                else
                                {
                                    if (el.x <= act.x)
                                    {
                                        if (!el.isDeactivator)
                                            affected.Add(act);
                                        else
                                            deactivated.Add(act);
                                    }
                                }
                            }
                        }
                        break;
                    case ReceptorType.foodNear:
                        {
                            float eff = 0;
                            foreach (var food in FoodMarker.foodScripts)
                            {
                                if (Vector2.Distance(transform.position, food.gameObject.transform.position) < 2 &&
                                    Mathf.Clamp(1 / Vector2.Distance(transform.position, food.gameObject.transform.position), 0, 1) > eff)
                                {
                                    eff = Mathf.Clamp(1 / Vector2.Distance(transform.position, food.gameObject.transform.position), 0, 1);
                                }
                            }
                            if (distBefteenActAndRec <= el.sensivity * eff)
                            {
                                if (!el.isDeactivator)
                                    affected.Add(act);
                                else
                                    deactivated.Add(act);
                            }
                        }
                    break;
                    default:
                        Debug.Log("Unknown receptor type!" + el.type);
                    break;
                }
            }
        }

        foreach (var deac in deactivated)
        {
            affected.Remove(deac);
        }
        foreach (var aff in affected)
        {
            bool hasNegative = false;
            foreach (var neg in affected)
            {
                if (neg.type == aff.type && neg.isReversed)
                {
                    hasNegative = true;
                    break;
                }
            }
            if (!hasNegative)
            {
                switch (aff.type)
                {
                    case ActionType.move:
                        {
                            transform.position = transform.position + Quaternion.Euler(0f, 0f, 90f) * transform.right * Time.deltaTime * 3;
                        }
                        break;
                    case ActionType.eat:
                        {
                            foreach (var foodScr in FoodMarker.foodScripts)
                            {
                                if (Vector2.Distance(transform.position, foodScr.gameObject.transform.position) < 1 && !foodScr.isEatten)
                                {
                                    foodLevel = Mathf.Clamp(foodLevel + 20, 0, foodMax);
                                    foodScr.isEatten = true;
                                    stats.foodSum += foodLevel + 20;
                                    Destroy(foodScr.gameObject);
                                }
                            }
                        }
                        break;
                    case ActionType.attack:
                        {
                            foreach (var bactScript in bacteriaScripts)
                            {
                                if (bactScript != this && Vector2.Distance(transform.position, bactScript.gameObject.transform.position) < 1)
                                {
                                    if (bactScript.poison > 0)
                                    {
                                        timeToLiveActual = -10;
                                    }
                                    foodLevel = Mathf.Clamp(bactScript.foodLevel >= 20 ? (foodLevel + 20) : foodLevel + bactScript.foodLevel, 0, foodMax);
                                    stats.bitesSum += bactScript.foodLevel >= 20 ? 20 : bactScript.foodLevel;
                                    bactScript.foodLevel -= 20;
                                }
                            }
                        }
                        break;
                    case ActionType.bazinga:
                        {
                            foreach (var bactScript in bacteriaScripts)
                            {
                                if (bactScript != this && Vector2.Distance(transform.position, bactScript.gameObject.transform.position) < 1)
                                {
                                    bactScript.gameObject.transform.position += (bactScript.gameObject.transform.position - transform.position).normalized * Time.deltaTime * 10;
                                }
                            }
                        }
                        break;
                    case ActionType.makePoison:
                        {
                            poison += Time.deltaTime * 2;
                            // TTL--; ???
                        }
                        break;
                    case ActionType.tie:
                        {
                            foreach (var bactScript in bacteriaScripts)
                            {
                                if (bactScript != this && !tied.Contains(bactScript.gameObject)  &&
                                    Vector2.Distance(transform.position, bactScript.gameObject.transform.position) < 0.5f)
                                {
                                    tied.Add(bactScript.gameObject);
                                }
                            }
                        }
                        break;
                    case ActionType.photosynt:
                        {
                            bool ableToPhotosynt = true;
                            foreach (var bactScript in bacteriaScripts)
                            {
                                if (bactScript != this && Vector2.Distance(transform.position, bactScript.gameObject.transform.position) < 0.5f)
                                {
                                    ableToPhotosynt = false;
                                    break;
                                }
                            }
                            if (ableToPhotosynt)
                            {
                                foodLevel = Mathf.Clamp(foodLevel + Time.deltaTime * hungerGlobalSpeed * 1.5f, 0, foodMax);
                                stats.photoSum += Time.deltaTime * hungerGlobalSpeed * 1.5f;
                            }
                        }
                        break;
                    case ActionType.rotateLeft:
                        {
                            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
                        }
                        break;
                    case ActionType.rotateRight:
                        {
                            transform.Rotate(0, 0, -rotateSpeed * Time.deltaTime);
                        }
                        break;
                    case ActionType.split:
                        {
                            if (foodLevel > 20)
                            {
                                stats.splitCount += 1;
                                GameObject curr = Instantiate(bacteriaPrefab, new Vector3(transform.position.x, transform.position.y + 0.01f, 0), Quaternion.identity);
                                curr.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
                                BacteriaScript bacteriaScript = curr.GetComponent<BacteriaScript>();
                                bacteriaScript.generation = generation + 1;
                                bacteriaScript.foodLevel = foodLevel / 2f;
                                bacteriaScript.receptors.Clear();
                                bacteriaScript.actions.Clear();
                                foreach (var el in receptors)
                                {
                                    bacteriaScript.receptors.Add(new Receptor(el));
                                }
                                foreach (var el in actions)
                                {
                                    bacteriaScript.actions.Add(new Action(el));
                                }
                                foodLevel /= 2f;
                                while (Random.Range(0, 100f) < 20)
                                {
                                    curr.GetComponent<SpriteRenderer>().color = new Color(
                                        Mathf.Clamp(GetComponent<SpriteRenderer>().color.r + Random.Range(-0.15f, 0.15f), 0, 1),
                                        Mathf.Clamp(GetComponent<SpriteRenderer>().color.g + Random.Range(-0.15f, 0.15f), 0, 1), 
                                        Mathf.Clamp(GetComponent<SpriteRenderer>().color.b + Random.Range(-0.15f, 0.15f), 0, 1));
                                    int typeOfMut = Random.Range(0, 6);
                                    switch (typeOfMut)
                                    {
                                        case 0:
                                            bacteriaScript.receptors.Add(new Receptor(
                                            Random.Range(0, mindBoxSize), Random.Range(0, mindBoxSize),
                                            (ReceptorType)Random.Range((int)0, (int)ReceptorType.GetValues(typeof(ReceptorType)).Length),
                                            Random.Range(0, 1.5f), Random.Range(0, 1f) < 0.25f));
                                            break;
                                        case 1:
                                            bacteriaScript.actions.Add(new Action(
                                                Random.Range(0, mindBoxSize), Random.Range(0, mindBoxSize),
                                                (ActionType)Random.Range((int)0, (int)ActionType.GetValues(typeof(ActionType)).Length),
                                                Random.Range(0, 2) == 0));
                                            break;
                                        case 2:
                                            if (bacteriaScript.receptors.Count > 0)
                                                bacteriaScript.receptors.RemoveAt(Random.Range(0, bacteriaScript.receptors.Count));
                                            break;
                                        case 3:
                                            if (bacteriaScript.actions.Count > 0)
                                                bacteriaScript.actions.RemoveAt(Random.Range(0, bacteriaScript.actions.Count));
                                            break;
                                        case 4:
                                            {
                                                int index = Random.Range(0, bacteriaScript.receptors.Count);
                                                if (bacteriaScript.receptors.Count > 0)
                                                    bacteriaScript.receptors[index] = new Receptor(
                                                    bacteriaScript.receptors[index].x, bacteriaScript.receptors[index].y,
                                                    (ReceptorType)Random.Range((int)0, (int)ReceptorType.GetValues(typeof(ReceptorType)).Length),
                                                    bacteriaScript.receptors[index].sensivity, bacteriaScript.receptors[index].isDeactivator);
                                            }
                                            break;
                                        case 5:
                                            {
                                                int index = Random.Range(0, bacteriaScript.actions.Count);
                                                if (bacteriaScript.actions.Count > 0)
                                                    bacteriaScript.actions[index] = new Action(
                                                    bacteriaScript.actions[index].x, bacteriaScript.actions[index].y,
                                                    (ActionType)Random.Range((int)0, (int)ActionType.GetValues(typeof(ActionType)).Length),
                                                    bacteriaScript.actions[index].isReversed);
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        Debug.Log("Unknown action type!" + aff.type);
                        break;
                }
            }
        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -9, 9), Mathf.Clamp(transform.position.y, -2.5f, 5), 0);
        for (int i = 0; i < tied.Count; ++i)
        {
            if (tied[i] == null)
            {
                tied.RemoveAt(i--);
                continue;
            }
            if (Vector2.Distance(transform.position, tied[i].transform.position) > 0.5f)
            {
                tied[i].transform.position += (transform.position - tied[i].transform.position) + (transform.position - tied[i].transform.position).normalized * 0.5f;
            }
        }
    }

    void OnMouseDown()
    {
        if (Input.GetKey("mouse 0"))
        {
            FindObjectOfType<ModuleShower>().RefreshCanvas(this);
        }
    }
}

public enum ReceptorType
{
    constant, foodNear, foodFullness, bacteriaNear, leftRightRandom, sideFood, sideBacteria, edgeNear, wait5
}

public struct Receptor
{
    public float x;
    public float y;
    public ReceptorType type;
    public float sensivity;
    public bool isDeactivator;

    public Receptor(float x, float y, ReceptorType type, float sensivity, bool isDeactivator)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.sensivity = sensivity;
        this.isDeactivator = isDeactivator;
    }
    public Receptor(Receptor other)
    {
        this.x = other.x;
        this.y = other.y;
        this.type = other.type;
        this.sensivity = other.sensivity;
        this.isDeactivator = other.isDeactivator;
    }
}
public enum ActionType
{
    move, eat, split, rotateLeft, rotateRight, photosynt, attack,
    bazinga, tie, makePoison
}

public struct Action
{
    public float x;
    public float y;
    public ActionType type;
    public bool isReversed;


    public Action(float x, float y, ActionType type, bool isReversed)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.isReversed = isReversed;
    }
    public Action(Action other)
    {
        this.x = other.x;
        this.y = other.y;
        this.type = other.type;
        this.isReversed = other.isReversed;
    }
}