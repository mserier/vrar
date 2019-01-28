using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    public Interactable focus;

    Camera cam;
    public LayerMask movementMask;
    PlayerMotor motor;

    // Use this for initialization
    void Start () {
        cam = Camera.main;
        motor = GetComponent<PlayerMotor>();
		
	}

	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, movementMask)) 
            {
                //move to were we hit
                motor.MoveToPoint(hit.point);
                //stop focusing any objects
                RemoveFocus();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                //check if we hit an interactable
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                //if we did set it as our focus
                if (interactable != null)
                {
                    SetFocus(interactable);
                }
            }
        }
        
    }
    void SetFocus (Interactable newFocus)
    {
        if (newFocus != focus)
        {
            if (focus != null)
                focus.OnDeFocused();

            focus = newFocus;
            motor.FollowTarget(newFocus);

        }

        newFocus.OnFocused(transform);
        
    }
    
    void RemoveFocus()
    {
        if (focus != null)
            focus.OnDeFocused();

        focus = null;
        motor.StopFollowingTarget();
    }
}
