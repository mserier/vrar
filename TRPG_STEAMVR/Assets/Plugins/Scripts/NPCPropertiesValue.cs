using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCPropertiesValue : MonoBehaviour {

    public GameObject PropsTitle;
    public GameObject PropsAmount;
    public int[] NPCValues;

    public string title;
    public int amount;
    // Use this for initialization

    private static NPCPropertiesValue _instance; 
    public static NPCPropertiesValue instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<NPCPropertiesValue>();
            }

            return _instance;
        }
    }


    //-------------------------------------------------
    void Awake()
    {
        _instance = this;

        GetProps();
    }

    void Start()
    {
        

        

        amount = NPCValues[0];

        


    }

    void Update()
    {
    }

    public void GetProps()
    {
        int[] NPCValues = new int[] { 100, 80, 43 };
    }

    public void Plus()
    {
        amount = amount + 1;
        PropsAmount.GetComponent<UnityEngine.UI.Text>().text = amount.ToString();
    }

    public void Min()
    {
        amount = amount - 1;
        PropsAmount.GetComponent<UnityEngine.UI.Text>().text = amount.ToString();
    }
}
