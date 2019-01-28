using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour {

	private Vector2 fingerDown;
	private Vector2 fingerUp;
	public bool detectSwipeAfterRelease;
	GamePlayManagerAR myGamePlayManager;

	public float SWIPE_THRESHOLD = 20f;

    void Start()
    {
        myGamePlayManager = GamePlayManagerAR.instance;
    }

    // Update is called once per frame
    void Update () {

		// Detects the swipe
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
				fingerUp = touch.position;
				fingerDown = touch.position;
			}

			// Detects Swipe while finger is moving
			if (touch.phase == TouchPhase.Moved) {
				if (!detectSwipeAfterRelease) {
					fingerDown = touch.position;
					checkSwipe ();
				}
			}

			//Detects swpie after finger is released
			if (touch.phase == TouchPhase.Ended) {
				fingerDown = touch.position;
				checkSwipe ();
			}
		}
	}
		// Check the direction of the swipe

	void checkSwipe(){
		// check if it is a Vertical swipe
		if(verticalMove() > SWIPE_THRESHOLD && verticalMove() > horizontalValMove()){
			Debug.Log("It is a Vertical Swipe");
			if(fingerDown.y - fingerUp.y > 0){ // Swipe Up
				OnSwipeUp();
				Debug.Log("SwipeUp 1");
			} else if (fingerDown.y - fingerUp.y < 0){ //Swipe Down
				OnSwipeDown();
			}
			fingerUp = fingerDown;
		}

		// check if it is a Horizontal swipe
		else if (horizontalValMove() > SWIPE_THRESHOLD && horizontalValMove() > verticalMove()){
			Debug.Log("It is a Horizontal Swipe");
			if(fingerDown.x - fingerUp.x > 0){ // Right Swipe
				OnSwipeRight();
			}
			else if (fingerDown.x - fingerUp.x < 0){ // Left Swipe
				OnSwipeLeft();
			}
			fingerUp = fingerDown;
		}

		else{
			// No Swipe!
			Debug.Log ("No Swipe");
		}
			
	} // End of Void Check Swipe()

	float verticalMove(){
		return Mathf.Abs (fingerDown.y - fingerUp.y);
	}

	float horizontalValMove(){
		return Mathf.Abs (fingerDown.x - fingerUp.x);
	}

	// Now it's time for the Callback Functions!

	void OnSwipeUp(){
        myGamePlayManager.showGUI(true);
        Debug.Log("Menu open");
    }

	void OnSwipeDown(){
        myGamePlayManager.showGUI(false);
        Debug.Log("Menu gesloten");
    }

	void OnSwipeLeft(){

    }

	void OnSwipeRight(){

    }
}