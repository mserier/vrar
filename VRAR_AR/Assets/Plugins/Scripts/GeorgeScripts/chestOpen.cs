using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class chestOpen : MonoBehaviour {

   
    public AudioClip clip;

    // private void OnMouseDown()
    // {
        // GetComponent<Rigidbody>().angularVelocity = new Vector3(.75f, 0, 0);
        // StartCoroutine(stopOpening());
        // AudioSource.PlayClipAtPoint(clip, new Vector3(0.75f, 0, 0));
        // Debug.Log("started");
    // }
    
    public void Open()
    {
        GetComponent<Rigidbody>().angularVelocity = new Vector3(.75f, 0, 0);
        StartCoroutine(stopOpening());
        AudioSource.PlayClipAtPoint(clip, new Vector3(0.75f, 0, 0));
        Debug.Log("started");
    }

    IEnumerator stopOpening()
    {
        yield return new WaitForSeconds(1.1f);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
         
    }
    
}
