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

    public float touchSensitivity = 0.2f;
    [Tooltip("The distance of each step the grabbed object will move forward or backward during swipes.")]
    public float distanceStep = 0.1f;
    [Tooltip("The rotation step of the grabbed object.")]
    public int rotateStep = 10;

    private bool isGrabbing = false;
    private GameObject grabbedObject;
    private Rigidbody grabbedRb;
    private float currentDistance;

    private void Start()
    {
        line.gameObject.SetActive(false);
    }

    void FixedUpdate () {
        if(GetActivationButtonDown()) {
            if(!line.gameObject.activeInHierarchy) {
                line.gameObject.SetActive(true);
            }
            line.SetPosition(0, gameObject.transform.position);
            RaycastHit hit;
            // TODO: We might also want to add a teleport check in here too.... 
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, interactableLayer)) {
                // We've hit an object that can be interacted with or grabbed.
                line.SetPosition(1, hit.point);
                line.endColor = validColor;

                if (GetGrabButtonDown()) {
                    if(grabbedObject != hit.collider.gameObject) {
                        grabbedObject = hit.collider.gameObject;
                        grabbedRb = grabbedObject.GetComponent<Rigidbody>();
                        if(grabbedRb) {
                            grabbedRb.isKinematic = true;
                        }
                        isGrabbing = true;
                        currentDistance = hit.distance;
                    }
                }
                if (GetGrabButtonUp()) {
                    DropObject();
                }

            } else {
                // We've hit something else.
                line.SetPosition(1, transform.forward * maxDistance + transform.position);
                line.endColor = invalidColor;
            }

        } else {
            if (line.gameObject.activeInHierarchy) {
                line.gameObject.SetActive(false);
            }
        }
        if(!isGrabbing) {
            if (grabbedRb) {
                grabbedRb.isKinematic = false;
                grabbedRb = null;
            }
            grabbedObject = null;
        } else {
            if (!grabbedRb) {
                Debug.LogWarning("Attach a rigidbody to the grabbable object.");
            } else {
                Vector2 primaryTouchpad = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);

                var newPos = gameObject.transform.position + (gameObject.transform.forward * currentDistance);
                grabbedRb.MovePosition(newPos);
                
                // Handle rotation changes.
                if (primaryTouchpad.x > touchSensitivity) {
                    Quaternion newRot = Quaternion.AngleAxis(rotateStep, Vector3.up) * grabbedRb.rotation;
                    grabbedRb.MoveRotation(newRot);
                } else if (primaryTouchpad.x < -touchSensitivity) {
                    Quaternion newRot = Quaternion.AngleAxis(-rotateStep, Vector3.up) * grabbedRb.rotation;
                    grabbedRb.MoveRotation(newRot);
                }

                // Handle distance changes.
                if (primaryTouchpad.y > touchSensitivity && currentDistance <= maxDistance) {
                    currentDistance += distanceStep;
                } else if (primaryTouchpad.y < -touchSensitivity && currentDistance >= minDistance) {
                    currentDistance -= distanceStep;
                }
            }
        }
        if (!GetGrabButtonDown() && isGrabbing) {
            DropObject();
        }
        if (GetActivationButtonUp()) {
            if (line.gameObject.activeInHierarchy) {
                line.gameObject.SetActive(false);
            }
        }
		
	}

    private void DropObject()
    {
        isGrabbing = false;
        if (grabbedRb) {
            grabbedRb.isKinematic = false;
            grabbedRb = null;
        }
        grabbedObject = null;
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

    private bool GetGrabButtonDown()
    {
        if (grabOnTouch) {
            return OVRInput.Get(ConvertToTouch(grabButton));
        } else {
            return OVRInput.Get(grabButton);
        }
    }

    private bool GetGrabButtonUp()
    {
        if (grabOnTouch) {
            return OVRInput.GetUp(ConvertToTouch(grabButton));
        } else {
            return OVRInput.GetUp(grabButton);
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
