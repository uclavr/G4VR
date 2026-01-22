using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class options_manager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject parent;
    public Slider slider;
    public GameObject current_gobj;
    public void showobj(GameObject obj)
    {

        foreach (Transform child in parent.transform)
        {
            // Set each child to inactive
            child.gameObject.SetActive(false);
        }
        obj.SetActive(true);
        current_gobj = obj;
        GDMLPhysVolParser.SetOpacity(obj, slider.value);

    }

    public void SetOpacity()
    {
        GDMLPhysVolParser.SetOpacity(current_gobj, slider.value);
    }

    public void loadsim()
    {
        SceneManager.LoadScene("NaI");
    }

    public void loadgeo()
    {
        SceneManager.LoadScene("Geometries");
    }
}
