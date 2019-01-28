using UnityEngine;
using Valve.VR;



public class teleportUpDown : MonoBehaviour
{

    public SteamVR_Action_Boolean teleportUp;
    public SteamVR_Action_Boolean teleportDown;

    float teleportFadeTime = 0.2f;
    float currentFadeTime = 0f;
    // Update is called once per frame
    void FixedUpdate()
    {
        
       if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            //var y = Input.GetAxis("vertical") * Time.deltaTime * 150.0f;
            transform.Translate(0, 1, 0);
            fadeThing();

        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            print("down :" + transform.position);
            //var y = Input.GetAxis("vertical") * Time.deltaTime * 150.0f;
            transform.Translate(0, -1, 0);
            fadeThing();
        }
    }

    private void fadeThing()
    {
        currentFadeTime = teleportFadeTime;
        SteamVR_Fade.Start(Color.clear, 0);
        SteamVR_Fade.Start(Color.black, 0.1f);
        Invoke("TeleportPlayer", currentFadeTime);
    }

    //-------------------------------------------------
    private void TeleportPlayer()
    {
        SteamVR_Fade.Start(Color.clear, currentFadeTime);

    }
}
