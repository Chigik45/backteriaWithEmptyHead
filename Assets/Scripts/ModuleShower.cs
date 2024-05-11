using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleShower : MonoBehaviour
{
    [SerializeField] GameObject textPref;
    [SerializeField] GameObject backGround;
    [SerializeField] GameObject circlePrefab;
    [SerializeField] GameObject info;
    const float mod = 60;
    public void Clear()
    {
        foreach (var ch in transform.GetComponentsInChildren<Transform>())
        {
            if (ch != transform)
                Destroy(ch.gameObject);
        }
    }
    public void RefreshCanvas(BacteriaScript bs)
    {
        Clear();
        Instantiate(backGround, new Vector3(0, 0, -0.5f), Quaternion.identity, transform);
        Text txt = Instantiate(info, new Vector3(350, 0, -0.5f), Quaternion.identity, transform).GetComponent<Text>();
        txt.text = "Generation: " + bs.generation + "\nFood:" + bs.foodLevel + "\nTTL:" + bs.timeToLiveActual;
        txt.text += "\nBitesSum: " + bs.stats.bitesSum + "\nFoodSum: " + bs.stats.foodSum + "\nPhotoSum: " + bs.stats.photoSum + 
            "\nSplits: " + bs.stats.splitCount;
        foreach (var el in bs.actions)
        {
            GameObject curr = Instantiate(textPref, new Vector3(el.x * mod, el.y * mod, -1), Quaternion.identity, transform);
            if (el.isReversed)
            {
                curr.GetComponent<Text>().color = Color.gray;
            }
            string str = "";
            switch (el.type)
            {
                case ActionType.move:
                    str = "m";
                    break;
                case ActionType.attack:
                    str = "a";
                    break;
                case ActionType.bazinga:
                    str = "bz";
                    break;
                case ActionType.eat:
                    str = "e";
                    break;
                case ActionType.makePoison:
                    str = "mp";
                    break;
                case ActionType.photosynt:
                    str = "ph";
                    break;
                case ActionType.rotateLeft:
                    str = "rL";
                    break;
                case ActionType.rotateRight:
                    str = "rR";
                    break;
                case ActionType.split:
                    str = "s";
                    break;
                case ActionType.tie:
                    str = "t";
                    break;
                case ActionType.absorbRot:
                    str = "aR";
                    break;
                case ActionType.hybernate:
                    str = "h";
                    break;
                case ActionType.pickMeat:
                    str = "PM";
                    break;
                default:
                    str = ((int)el.type).ToString();
                    break;
            }
            curr.GetComponent<Text>().text = str;
        }
        foreach (var el in bs.receptors)
        {
            GameObject curr = Instantiate(textPref, new Vector3(el.x * mod, el.y * mod, -1), Quaternion.identity, transform);
            GameObject circl = Instantiate(circlePrefab, new Vector3(el.x * mod, el.y * mod, -0.9f), Quaternion.identity, transform);
            circl.transform.localScale = new Vector3(el.sensivity * mod, el.sensivity * mod, 1);
            curr.GetComponent<Text>().text = el.type.ToString()[0].ToString();
            curr.GetComponent<Text>().color = Color.green;
            if (el.isDeactivator)
            {
                curr.GetComponent<Text>().color = Color.red;
            }
            string str = "";
            switch (el.type)
            {
                case ReceptorType.constant:
                    str = "c";
                    break;
                case ReceptorType.bacteriaNear:
                    str = "bN";
                    break;
                case ReceptorType.edgeNear:
                    str = "eN";
                    break;
                case ReceptorType.foodFullness:
                    str = "fF";
                    break;
                case ReceptorType.foodNear:
                    str = "fN";
                    break;
                case ReceptorType.leftRightRandom:
                    str = "LRr";
                    break;
                case ReceptorType.sideBacteria:
                    str = "B?";
                    break;
                case ReceptorType.sideFood:
                    str = "F?";
                    break;
                case ReceptorType.wait5:
                    str = "w5";
                    break;
                case ReceptorType.plantNear:
                    str = "pN";
                    break;
                case ReceptorType.meatNear:
                    str = "mN";
                    break;
                case ReceptorType.rotNear:
                    str = "rN";
                    break;
                default:
                    str = ((int)el.type).ToString();
                    break;
            }
            curr.GetComponent<Text>().text = str;
        }
    }
}
