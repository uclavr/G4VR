using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public Transform head;
    public Transform camera;
    public float spawnDistance = 0;
    public GameObject menu;
    public GameObject panel;
    public GameObject info;
    public InputActionProperty showButton;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        menu.transform.position = head.position + new Vector3(0, 0.1f, 0.25f);
        //if (showButton.action.WasPressedThisFrame())
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            menu.SetActive(!menu.activeSelf);

        }
        //panel.transform.LookAt(camera.transform);
        //panel.transform.Rotate(0f, 180f, 0f, Space.Self);

        menu.transform.forward *= -1;
        //menu.transform.forward *= 1;
        menu.transform.LookAt(camera.transform);
        menu.transform.Rotate(0f, 180f, 0f, Space.Self);
        //info.transform.position = camera.position + new Vector3(0, 0f, 2f);
    }

    public void help()
    {
        info.SetActive(!info.activeSelf);

    }

    
}

