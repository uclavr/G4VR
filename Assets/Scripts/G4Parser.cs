using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.XR.CoreUtils.Capabilities;
using UnityEditor;
using UnityEngine;
//using USD.NET;
//using USD.NET.Unity;
//using UnityEditor.Experimental.AssetImporters;
//using static UnityEditor.PackageManager.UI.Sample;
//using Unity.Formats.USD;


public class GDMLPhysVolParser : MonoBehaviour
{
    public static List<(string PhysVolName, string VolRef, List<string> Position, List<string> Rotation, string SolidRef, string Material)> physVolData =
        new List<(string, string, List<string>, List<string>, string, string)>();
    [SerializeField] string name;
    [SerializeField] float geo_scale;
    [SerializeField] public static float scale;

    public static GameObject obj;

    public void Awake()
    {
        var obj = new GameObject("test_online");
        scale = geo_scale;

        // commented out in order to test USD imports
        obj = GameObject.Find($"{name}_scene");

        SetOpacity(obj, 0.05f);
        SetScale(obj);
        Debug.LogWarning($"World Position of child 3 {obj.transform.GetChild(3).transform.position}");
        
        //GenerateSegmentedCylinder(1f, 3f, 0.4f, 32);
        

    }

    public static void SetOpacity(GameObject parent, float opacity)
    {
        foreach (Transform child in parent.transform)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                foreach (Material mat in renderer.materials)
                {
                    Color color = mat.color;
                    color.a = opacity;
                    mat.color = color;
                    mat.SetFloat("_Mode", 2); 
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;

                    //Color emissiveColor = new Color(1f, 1f, 1f); 
                    //float emissiveIntensity = 1f;
                    //mat.SetColor("_EmissionColor", emissiveColor * emissiveIntensity);
                    //mat.EnableKeyword("_EMISSION");
                }
            }
        }
    }

    public static void SetScale(GameObject parent)
    {
        parent.transform.localScale = new Vector3(scale, scale, scale);
    }

    private static void ParseGDML(string filePath)
    {
        try
        {
            XDocument gdmlDoc = XDocument.Load(filePath);
            ExtractPhysVolData(gdmlDoc);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse GDML file: {ex.Message}");
        }
    }

    private static void ExtractPhysVolData(XDocument gdmlDoc)
    {
        var volumeData = gdmlDoc.Descendants("volume")
                                .ToDictionary(
                                    vol => vol.Attribute("name")?.Value ?? "UnnamedVolume",
                                    vol => new
                                    {
                                        SolidRef = vol.Element("solidref")?.Attribute("ref")?.Value ?? "UnknownSolid",
                                        Material = vol.Element("materialref")?.Attribute("ref")?.Value ?? "UnknownMaterial"
                                    });

        var defines = gdmlDoc.Descendants("define")
                             .Elements("position")
                             .ToDictionary(
                                 pos => pos.Attribute("name")?.Value ?? "Undefined",
                                 pos => new List<string>
                                 {
                                 pos.Attribute("x")?.Value ?? "0",
                                 pos.Attribute("y")?.Value ?? "0",
                                 pos.Attribute("z")?.Value ?? "0"
                                 }
                             );

        var rotationDefines = gdmlDoc.Descendants("define")
                                     .Elements("rotation")
                                     .ToDictionary(
                                         rot => rot.Attribute("name")?.Value ?? "Undefined",
                                         rot => new List<string>
                                         {
                                         rot.Attribute("x")?.Value ?? "0",
                                         rot.Attribute("y")?.Value ?? "0",
                                         rot.Attribute("z")?.Value ?? "0"
                                         }
                                     );

        Debug.Log($"Parsed {defines.Keys.Count} positions and {rotationDefines.Keys.Count} rotations from defines.");

        var structure = gdmlDoc.Descendants("structure");
        foreach (var volume in structure.Elements("volume"))
        {
            foreach (var physVol in volume.Elements("physvol"))
            {
                string physVolName = physVol.Attribute("name")?.Value ?? "UnnamedPhysVol";
                string volRef = physVol.Element("volumeref")?.Attribute("ref")?.Value ?? "UnnamedVolRef";

                string solidRef = volumeData.ContainsKey(volRef) ? volumeData[volRef].SolidRef : "UnknownSolid";
                string material = volumeData.ContainsKey(volRef) ? volumeData[volRef].Material : "UnknownMaterial";

                List<string> position = null;
                var positionElem = physVol.Element("position");
                string positionRef = physVol.Element("positionref")?.Attribute("ref")?.Value;

                if (!string.IsNullOrEmpty(positionRef) && defines.ContainsKey(positionRef))
                {
                    position = defines[positionRef];
                }
                else if (positionElem != null)
                {
                    position = new List<string>
                    {
                        positionElem.Attribute("x")?.Value ?? "0",
                        positionElem.Attribute("y")?.Value ?? "0",
                        positionElem.Attribute("z")?.Value ?? "0"
                    };
                }

                if (position == null)
                {
                    Debug.LogWarning($"Skipping physvol '{physVolName}' due to missing position.");
                    position = new List<string> { "0", "0", "0" };//continue;
                }

                var rotation = new List<string> { "0", "0", "0" }; 
                var rotationElem = physVol.Element("rotation");
                string rotationRef = physVol.Element("rotationref")?.Attribute("ref")?.Value;

                if (!string.IsNullOrEmpty(rotationRef) && rotationDefines.ContainsKey(rotationRef))
                {
                    rotation = rotationDefines[rotationRef];
                }
                else if (rotationElem != null)
                {
                    rotation = new List<string>
                    {
                        rotationElem.Attribute("x")?.Value ?? "0",
                        rotationElem.Attribute("y")?.Value ?? "0",
                        rotationElem.Attribute("z")?.Value ?? "0"
                    };
                }

                physVolData.Add((physVolName, volRef, position, rotation, solidRef, material));
            }
        }

    }

}
