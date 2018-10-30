using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Valve.VR.InteractionSystem
{
    public class UIRayCastScript : MonoBehaviour {

    public Hand hand;

    public TileRenderer tileRenderer;
    public TileInteractor tileInteractor;

    public bool clicked;

    void Start()
    {
    }

        void Update()
        {
            //Debug.Log(hand.transform.position);

            if(clicked)
            {
                clicked = false;
                RaycastHit hit;

                if (Physics.Raycast(hand.gameObject.transform.position, hand.gameObject.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
                {
                    Button button = hit.transform.gameObject.GetComponent<Button>();
                    if (hit.transform.gameObject.name.Contains("Load Selected lvl Button"))
                    {
                        tileRenderer.loadLevelOnclick();
                    }
                    else if (hit.transform.gameObject.name.Contains("Return to lvl selection Button"))
                    {
                        tileRenderer.unloadLevelOnClick();
                    }
                    else if (hit.transform.gameObject.name.Contains("Save lvl Button"))
                    {
                        tileRenderer.saveLVLClick();
                    }
                    //Debug.DrawRay(hand.gameObject.transform.position, hand.gameObject.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                }
                else
                {
                    //Debug.DrawRay(hand.gameObject.transform.position, hand.gameObject.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                    //Debug.Log("Did not Hit");
                }
                tileInteractor.setBrushTickingT(true);
            }
            else
            {
                tileInteractor.setBrushTickingT(false);
            }

        }
    }
}
