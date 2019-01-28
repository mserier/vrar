using UnityEngine;

public class Interactable : MonoBehaviour {

    public float radius = 3f;

    bool isFocus = false;
    Transform player;

    public Transform interactionTransform;

    bool hasInteracted = false;

    public void Interact()
    {
        //this method is meant to be overwritten
        Debug.Log("INTERACTING with " + transform.name);
        chestOpen chestOpenScript = transform.GetComponentInChildren<chestOpen>();
        if (chestOpenScript != null)
        {
            chestOpenScript.Open();
        }
    }

    private void Update()
    {
        if (isFocus && !hasInteracted)
        {
            Debug.Log("Opening chest");
            float distance = Vector3.Distance(player.position, interactionTransform.position);
            if (distance <= radius)
            {
                Interact();
                hasInteracted = true;
                
            }
        }
    }

    public void OnFocused (Transform playerTransform)
    {
        
        isFocus = true;
        player = playerTransform;
        hasInteracted = false;
    }
    
    public void OnDeFocused()
    {
       
        isFocus = false;
        player = null;
        hasInteracted = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionTransform.position, radius);
    }
}
