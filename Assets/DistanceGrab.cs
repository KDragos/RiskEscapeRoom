using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceGrab : MonoBehaviour {

    [Tooltip("The line to display when the user wants to grab something.")]
    public LineRenderer line;

    [Tooltip("The colors to indicate if a user is pointing to an interactable object.")]
    public Color invalidColor;
    public Color validColor;

    [Tooltip("The distance of the line renderer.")]
    public float maxDistance;
    [Tooltip("How close can a grabbed object get to the controller?")]
    public float minDistance;

    [Tooltip("The layer all interactable items should be in.")]
    public LayerMask interactableLayer;

    public OVRInput.Button activationButton;
    public bool activateOnTouch = false;

    public OVRInput.Button grabButton;
    public bool grabOnTouch = false;

    private bool isGrabbing = false;
    private GameObject grabbedObject;

    private void Start()
    {
        line.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        if(GetActivationButtonDown()) {
            Debug.Log("Activation button down.");
            if(!line.gameObject.activeInHierarchy) {
                line.gameObject.SetActive(true);
            }
            line.SetPosition(0, gameObject.transform.position);
            // if line renderer hits
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, interactableLayer)) {
                //teleportLocation = hit.point;
                line.SetPosition(1, hit.point);
                //line.startColor = validColor;
                line.endColor = validColor;

                //teleportAimerObject.transform.position = new Vector3(teleportLocation.x, teleportLocation.y + yNudge, teleportLocation.z);
            } else {
                // TODO: We might also want to add a teleport check in here too.... 

                line.SetPosition(1, transform.forward * maxDistance + transform.position);
                //line.startColor = invalidColor;

                line.endColor = invalidColor;
                
            }


            if (GetGrabButton()) {
                Debug.Log("Grab button down.");
                // If a raycast hits a grabbable object in the layer mask, grab it.
            }
        } else {
            if (line.gameObject.activeInHierarchy) {
                line.gameObject.SetActive(false);
            }
        }
        if(isGrabbing) {
            // Track swipes. 
            // if swipe up, move object further away.
            // if swipe down, move object closer.
            // if swipe left, rotate item 90.
            // if swipe right, rotate item -90.
        }
        if(GetActivationButtonUp()) {
            if (line.gameObject.activeInHierarchy) {
                line.gameObject.SetActive(false);
            }
        }
		
	}
    
    private bool GetActivationButtonDown()
    {
        if(activateOnTouch) {
            return OVRInput.Get(ConvertToTouch(activationButton)) && !OVRInput.Get(activationButton);
        } else {
            return OVRInput.Get(activationButton);
        }
    }

    private bool GetActivationButtonUp()
    {
        if (activateOnTouch) {
            // Checks to see that we're only touching the button, not pressing it down. 
            //return OVRInput.Get(ConvertToTouch(activationButton)) && !OVRInput.Get(activationButton);
            return OVRInput.GetUp(ConvertToTouch(activationButton));
        } else {
            return OVRInput.GetUp(activationButton);
        }
    }

    private bool GetGrabButton()
    {
        if (grabOnTouch) {
            return OVRInput.Get(ConvertToTouch(grabButton));
        } else {
            return OVRInput.Get(grabButton);
        }
    }

    private OVRInput.Touch ConvertToTouch(OVRInput.Button activationButton)
    {
        switch (activationButton) {
            case OVRInput.Button.PrimaryIndexTrigger:
                return OVRInput.Touch.PrimaryIndexTrigger;
            case OVRInput.Button.PrimaryThumbstick:
                return OVRInput.Touch.PrimaryThumbstick;
            case OVRInput.Button.PrimaryTouchpad:
                return OVRInput.Touch.PrimaryTouchpad;
            case OVRInput.Button.SecondaryIndexTrigger:
                return OVRInput.Touch.SecondaryIndexTrigger;
            case OVRInput.Button.SecondaryThumbstick:
                return OVRInput.Touch.SecondaryThumbstick;
            case OVRInput.Button.SecondaryTouchpad:
                return OVRInput.Touch.SecondaryTouchpad;
            case OVRInput.Button.One:
                return OVRInput.Touch.One;
            case OVRInput.Button.Two:
                return OVRInput.Touch.Two;
            case OVRInput.Button.Three:
                return OVRInput.Touch.Three;
            case OVRInput.Button.Four:
                return OVRInput.Touch.Four;
            default:
                Debug.LogWarning("Couldn't find a matching mapped touch button. Returning any touch.");
                return OVRInput.Touch.Any;
        }
    }
}

public enum ButtonState
{
    Pressed,
    Released
}
