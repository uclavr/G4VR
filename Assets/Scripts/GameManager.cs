using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string URL;
    public string customscene = "DEFAULT";
    public static GameManager instance;
    public static int number_of_runs = 0; 
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            GameObject Geometry = new GameObject("Scene");
            //Geometry.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            DontDestroyOnLoad(Geometry);
        }
        else {Destroy(gameObject); }
    }

   
}
