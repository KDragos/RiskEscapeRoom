using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealHidden : MonoBehaviour {
    public OVRInput.Button lightActivationButton;

    private Light lightComponent;
    private MeshRenderer previousHiddenRenderer;
    private Vector3 oldLightPos;
    private Vector3 oldLightDir;

	// Use this for initialization
	void Start () {
        lightComponent = GetComponent<Light>();
        if(lightComponent) {
            lightComponent.enabled = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if(lightComponent) {
            if (OVRInput.Get(lightActivationButton)) {
                lightComponent.enabled = true;
                RaycastHit hit;
                Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, lightComponent.range);

                if (hit.collider.gameObject.CompareTag("Hidden")) {
                    var rend = hit.collider.gameObject.GetComponent<MeshRenderer>();
                    if (rend != null) {
                        ResetPreviousRenderer();
                        // Save the updated renderer data.
                        previousHiddenRenderer = rend;
                        oldLightPos = rend.material.GetVector("_LightPos");
                        oldLightDir = rend.material.GetVector("_LightDir");

                        // Show the hidden stuff based on light position.
                        rend.material.SetVector("_LightPos", transform.position);
                        rend.material.SetVector("_LightDir", transform.TransformDirection(Vector3.forward));
                    }
                } else {
                    ResetPreviousRenderer();
                }
            } else {
                lightComponent.enabled = false;
                ResetPreviousRenderer();
            }
        }
	}

    private void ResetPreviousRenderer()
    {
        if (previousHiddenRenderer != null) {
            previousHiddenRenderer.material.SetVector("_LightPos", oldLightPos);
            previousHiddenRenderer.material.SetVector("_LightDir", oldLightDir);
            previousHiddenRenderer = null;
        }
    }
}
