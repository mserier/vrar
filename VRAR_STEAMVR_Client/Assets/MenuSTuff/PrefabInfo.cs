using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabInfo : MonoBehaviour {

    public Text name;
    public GameObject image;


	// Use this for initialization
	void Start () {
       
	}


    public void UpdateInfo(string text, Sprite image)
    {
        name.text = text;
        this.image.GetComponent<SpriteRenderer>().sprite = image;
    }

    
	
	// Update is called once per frame
	void Update () {
		
	}
}
