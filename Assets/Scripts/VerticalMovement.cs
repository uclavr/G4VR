using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VerticalMovement : MonoBehaviour
{
    // Start is called before the first frame update

    public float proximityRadius = 3f;
    public float normalSpeed = 35f;
    public float reducedSpeed = 5f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, proximityRadius);

        bool isNearAnyObject = false;
        foreach (var hit in hits)
        {
            if (hit.gameObject != this.gameObject)
            {
                isNearAnyObject = true;
                break;
            }
        }

        float currentSpeed = isNearAnyObject ? reducedSpeed : normalSpeed;
        float ascendSpeed = isNearAnyObject ? 0.1f : 0.8f;

        transform.GetComponent<ActionBasedContinuousMoveProvider>().moveSpeed = currentSpeed;
        OVRInput.Update();
        if (OVRInput.Get(OVRInput.Button.Two))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + ascendSpeed, transform.position.z);
        }

        if (OVRInput.Get(OVRInput.Button.One))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - ascendSpeed, transform.position.z);
        }

        

    }
}
