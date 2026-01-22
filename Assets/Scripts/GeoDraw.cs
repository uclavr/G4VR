using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using UnityEngine;
//using UnityEngine.ProBuilder;
//using UnityEngine.ProBuilder.MeshOperations;
//using UnityEngine.ProBuilder.Shapes;
using Parabox.CSG;
using System;
//using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.IO;
using UnityEngine.Rendering;


public class GeoDraw : MonoBehaviour
{
    // Start is called before the first frame update
    static Quaternion rhs_transform = Quaternion.Euler(0, 0, 0);
    public static void GeoManager(List<(string PhysVolName, string VolRef, List<string> Position, List<string> Rotation, string SolidRef, string Material)> physvols)
    {
        /*foreach (var physvol in physvols)
        {
            string gdmlFilePath = @"C:\Users\uclav\Documents\B's Sandbox\G4VR\Assets\lhcbvelo.gdml";
            XDocument gdmlDoc = XDocument.Load(gdmlFilePath);
            var solid = gdmlDoc.Descendants("solids")
                   .Elements()
                   .FirstOrDefault(s => s.Attribute("name")?.Value == physvol.SolidRef);

            string solidType = solid?.Name.LocalName ?? "UnknownSolidType";
            Debug.Log("Attempting Draw for " + physvol.VolRef);
            GeoDraw.GeoDrawer(gdmlDoc, physvol.SolidRef, physvol.Position, physvol.Rotation, solidType, physvol.SolidRef);
        }*/
        //Debug.Log("Total physvols:" + physvols.Count);
        //string gdmlFilePath = @"C:\Users\uclav\Documents\B's Sandbox\G4VR\Assets\lhcbvelo.gdml";
        string path = @"C:\Users\uclav\Documents\B's Sandbox\G4VR\Assets\lhcbvelo";

        /* // OLD PARSER CODE
        XDocument gdmlDoc = XDocument.Load(gdmlFilePath);
        ProcessPrimitives(gdmlFilePath);
        foreach (var physvol in physvols) // this loop assigns positions/rotations to primitives. 
        {
            GameObject obj = GameObject.Find(physvol.SolidRef);

            if (obj != null)
            {
                Vector3 position = new Vector3(float.Parse(physvol.Position[0]),float.Parse(physvol.Position[1]),float.Parse(physvol.Position[2]) );
                Quaternion rotation = Quaternion.Euler(float.Parse(physvol.Rotation[0]),float.Parse(physvol.Rotation[1]),float.Parse(physvol.Rotation[2]));
                obj.transform.localPosition = position;
                obj.transform.localRotation = rotation * rhs_transform;
                //Debug.Log($"Updated {physvol.SolidRef} position to {position} and rotation to {rotation}");
            }
            else
            {
                var solid = gdmlDoc.Descendants("solids").Elements().FirstOrDefault(s => s.Attribute("name")?.Value == physvol.SolidRef);
                string solidType = solid?.Name.LocalName ?? "UnknownSolidType";
                if (solidType == "subtraction" || solidType == "union")
                    continue; 
                Debug.LogWarning($"PRIMITIVE:GameObject with name {physvol.SolidRef} not found in scene.");
            }
            
        }
        ProcessBooleans(gdmlFilePath);
        foreach (var physvol in physvols) // this loop assigns positions/rotations to primitives. 
        {
            GameObject obj = GameObject.Find(physvol.SolidRef);

            if (obj != null)
            {
                Vector3 position = new Vector3(float.Parse(physvol.Position[0]), float.Parse(physvol.Position[1]), float.Parse(physvol.Position[2]));
                Quaternion rotation = Quaternion.Euler(float.Parse(physvol.Rotation[0]), float.Parse(physvol.Rotation[1]), float.Parse(physvol.Rotation[2]));
                obj.transform.localPosition = position;
                obj.transform.localRotation = rotation * rhs_transform;
                //Debug.Log($"Updated {physvol.SolidRef} position to {position} and rotation to {rotation}");
            }
            else
            {
                Debug.LogWarning($"BOOLEAN:GameObject with name {physvol.SolidRef} not found in scene.");
            }

        }*/  // OLD PARSER CODE

        string[] objs = Directory.GetFiles(path,"*.obj");
        Debug.Log($"objs count:{objs.Length}");
        List<GameObject> imp_objects = new List<GameObject>();

        foreach (string file in objs) // instantiate objects 
        {
            Debug.Log($"Current file: {file}");
            string name = Path.GetFileNameWithoutExtension(file);
            Mesh mesh = LoadObj(file);
            GameObject obj = new GameObject("LoadedObj", typeof(MeshFilter), typeof(MeshRenderer));
            obj.GetComponent<MeshFilter>().mesh = mesh;
            obj.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
            obj.name = name;
            imp_objects.Add(obj);
            //Instantiate(obj);

        }

        foreach (var physvol in physvols) // this loop assigns positions/rotations to primitives. 
        {
            GameObject obj = GameObject.Find(physvol.VolRef);

            if (obj != null)
            {
                Vector3 position = new Vector3(float.Parse(physvol.Position[0]), float.Parse(physvol.Position[1]), float.Parse(physvol.Position[2]));
                Quaternion rotation = Quaternion.Euler(float.Parse(physvol.Rotation[0]), float.Parse(physvol.Rotation[1]), float.Parse(physvol.Rotation[2]));
                obj.transform.localPosition = position;
                obj.transform.localRotation = rotation * rhs_transform;
                //Debug.Log($"Updated {physvol.SolidRef} position to {position} and rotation to {rotation}");
            }
            else
            {
                Debug.LogWarning($"PRIMITIVE:GameObject with name {physvol.SolidRef} not found in scene.");
            }

        }


    }

    public static Mesh LoadObj(string path)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        foreach (var line in File.ReadLines(path))
        {
            var parts = line.Split(' ');

            if (line.StartsWith("v "))  
            {
                Vector3 vertex = new Vector3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                vertices.Add(vertex);
            }
            else if (line.StartsWith("f "))  
            {
                int index1 = int.Parse(parts[1].Split('/')[0]) - 1;
                int index2 = int.Parse(parts[2].Split('/')[0]) - 1;
                int index3 = int.Parse(parts[3].Split('/')[0]) - 1;
                triangles.Add(index1);
                triangles.Add(index2);
                triangles.Add(index3);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    public static void ProcessPrimitives(string gdmlFilePath)
    {
        XDocument gdmlDoc = XDocument.Load(gdmlFilePath);
        var solids = gdmlDoc.Descendants("solids").Elements();

        foreach (var solid in solids)
        {
            string solidName = solid.Attribute("name")?.Value ?? "UnknownSolid";
            string solidType = solid.Name.LocalName;
            if (solidName.Contains("World")|| solidName.Contains("Envelope") || solidName.Contains("world") || solidName.Contains("envelope"))
            {
                Debug.Log($"Skipping solid '{solidName}' as it contains 'World'.");
                continue;
            }
            //Debug.Log($"Processing solid: {solidName} of type {solidType}");
            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            XElement posElement = solid.Element("position");
            XElement rotElement = solid.Element("rotation");

            if (posElement != null)
            {
                float x = float.Parse(posElement.Attribute("x")?.Value ?? "0");
                float y = float.Parse(posElement.Attribute("y")?.Value ?? "0");
                float z = float.Parse(posElement.Attribute("z")?.Value ?? "0");
                position = new Vector3(x, y, z);
            }

            if (rotElement != null)
            {
                float rx = float.Parse(rotElement.Attribute("x")?.Value ?? "0");
                float ry = float.Parse(rotElement.Attribute("y")?.Value ?? "0");
                float rz = float.Parse(rotElement.Attribute("z")?.Value ?? "0");
                rotation = Quaternion.Euler(rx, ry, rz);
            }
            GameObject obj = null;
            switch (solidType)
            {
                case "box":
                    double x = double.Parse(solid.Attribute("x")?.Value ?? "0");
                    double y = double.Parse(solid.Attribute("y")?.Value ?? "0");
                    double z = double.Parse(solid.Attribute("z")?.Value ?? "0");
                    obj = CreateBoxMesh(solidName, (float)x, (float)y, (float)z);
                    obj.transform.localPosition = position;
                    obj.transform.localRotation = rotation * rhs_transform;
                    break;

                case "cone":
                    double rmin1 = double.Parse(solid.Attribute("rmin1")?.Value ?? "0");
                    double rmax1 = double.Parse(solid.Attribute("rmax1")?.Value ?? "0");
                    double rmin2 = double.Parse(solid.Attribute("rmin2")?.Value ?? "0");
                    double rmax2 = double.Parse(solid.Attribute("rmax2")?.Value ?? "0");
                    double zCone = double.Parse(solid.Attribute("z")?.Value ?? "0");
                    double startphi_cone = double.Parse(solid.Attribute("startphi")?.Value ?? "0");
                    double deltaphi_cone = double.Parse(solid.Attribute("deltaphi")?.Value ?? "0");
                    obj = CreateConeSegmentMesh(solidName, (float)rmin1, (float)rmax1, (float)rmin2, (float)rmax2, (float)zCone, (float)startphi_cone, (float)deltaphi_cone);
                    obj.transform.localPosition = position;
                    obj.transform.localRotation = rotation * rhs_transform; break;

                case "tube":
                    double rmin = double.Parse(solid.Attribute("rmin")?.Value ?? "0");
                    double rmax = double.Parse(solid.Attribute("rmax")?.Value ?? "0");
                    double zTube = double.Parse(solid.Attribute("z")?.Value ?? "0");
                    double startphi_tube = double.Parse(solid.Attribute("startphi")?.Value ?? "0");
                    double deltaphi_tube = double.Parse(solid.Attribute("deltaphi")?.Value ?? "0");
                    obj = CreateTubeSegmentMesh(solidName, (float)rmin, (float)rmax, (float)zTube, (float)startphi_tube, (float)deltaphi_tube);
                    obj.transform.localPosition = position;
                    obj.transform.localRotation = rotation * rhs_transform; break;

                case "ellipsoid":
                    double ax = double.Parse(solid.Attribute("ax")?.Value ?? "0");
                    double by = double.Parse(solid.Attribute("by")?.Value ?? "0");
                    double cz = double.Parse(solid.Attribute("cz")?.Value ?? "0");
                    obj = CreateEllipsoidMesh(solidName, (float)ax, (float)by, (float)cz);
                    obj.transform.localPosition = position;
                    obj.transform.localRotation = rotation * rhs_transform; break;

                case "sphere":
                    double rminSphere = double.Parse(solid.Attribute("rmin")?.Value ?? "0");
                    double rmaxSphere = double.Parse(solid.Attribute("rmax")?.Value ?? "0");
                    double startphi_sphere = double.Parse(solid.Attribute("startphi")?.Value ?? "0");
                    double deltaphi_sphere = double.Parse(solid.Attribute("deltaphi")?.Value ?? "0");
                    double starttheta_sphere = double.Parse(solid.Attribute("starttheta")?.Value ?? "0");
                    double deltatheta_sphere = double.Parse(solid.Attribute("deltatheta")?.Value ?? "0");
                    obj = CreateSphereSegmentMesh(solidName, (float)rminSphere, (float)rmaxSphere, (float)startphi_sphere, (float)deltaphi_sphere, (float)starttheta_sphere, (float)deltatheta_sphere);
                    obj.transform.localPosition = position;
                    obj.transform.localRotation = rotation * rhs_transform; break;

                case "trd":
                    double x1 = double.Parse(solid.Attribute("x1")?.Value ?? "0");
                    double x2 = double.Parse(solid.Attribute("x2")?.Value ?? "0");
                    double y1 = double.Parse(solid.Attribute("y1")?.Value ?? "0");
                    double y2 = double.Parse(solid.Attribute("y2")?.Value ?? "0");
                    double z_trd = double.Parse(solid.Attribute("z")?.Value ?? "0");
                    obj = CreateTrdMesh(solidName, (float)x1, (float)x2, (float)y1, (float)y2, (float)z_trd);
                    obj.transform.localPosition = position;
                    obj.transform.localRotation = rotation * rhs_transform; break;
                case "trap":
                    {
                        double xone = double.Parse(solid.Attribute("x1")?.Value ?? "0");
                        double xtwo = double.Parse(solid.Attribute("x2")?.Value ?? "0");
                        double xthree = double.Parse(solid.Attribute("x3")?.Value ?? "0");
                        double xfour = double.Parse(solid.Attribute("x4")?.Value ?? "0");
                        double yone = double.Parse(solid.Attribute("y1")?.Value ?? "0");
                        double ytwo = double.Parse(solid.Attribute("y2")?.Value ?? "0");
                        double z_trap = double.Parse(solid.Attribute("z")?.Value ?? "0");
                        double theta = double.Parse(solid.Attribute("theta")?.Value ?? "0");
                        double phi = double.Parse(solid.Attribute("phi")?.Value ?? "0");
                        double alpha1 = double.Parse(solid.Attribute("alpha1")?.Value ?? "0");
                        double alpha2 = double.Parse(solid.Attribute("alpha2")?.Value ?? "0");

                        obj = CreateTrapezoidMesh(solidName, (float)xone, (float)xtwo, (float)xthree, (float)xfour, (float)yone, (float)ytwo, (float)z_trap, (float)theta, (float)phi, (float)alpha1, (float)alpha2);
                        obj.transform.localPosition = position;
                        obj.transform.localRotation = rotation * rhs_transform;
                    }
                    break;
                case "polycone":
                    double startPhi_pc = double.Parse(solid.Attribute("startphi")?.Value ?? "0");
                    double deltaPhi_pc = double.Parse(solid.Attribute("deltaphi")?.Value ?? "0");

                    List<(float rmin, float rmax, float z)> zplanes = new List<(float, float, float)>();
                    foreach (var zplane in solid.Elements("zplane"))
                    {
                        float rmin_pc = float.Parse(zplane.Attribute("rmin")?.Value ?? "0");
                        float rmax_pc = float.Parse(zplane.Attribute("rmax")?.Value ?? "0");
                        float z_pc = float.Parse(zplane.Attribute("z")?.Value ?? "0");
                        zplanes.Add((rmin_pc, rmax_pc, z_pc));
                    }
                    Debug.Log("GENERATING POLYCONES");
                    obj = GeneratePolyconeMesh(solidName, (float)startPhi_pc, (float)deltaPhi_pc, zplanes);
                    obj.transform.localPosition = position;
                    obj.transform.localRotation = rotation * rhs_transform;
                    break;

                case "subtraction":
                    /*string subresultname = solid.Attribute("name").Value ?? "Error";
                    string sub_first = solid.Element("first").Attribute("ref").Value ?? "Error";
                    string sub_second = solid.Element("second").Attribute("ref").Value ?? "Error";
                    try
                    {
                        obj = PerformSubtraction(obj,sub_first, new List<string>() { position.x.ToString(), position.y.ToString(), position.z.ToString() },
                        new List<string>() { rotation.eulerAngles.x.ToString(), rotation.eulerAngles.y.ToString(), rotation.eulerAngles.z.ToString() }, sub_second, subresultname);
                        obj.transform.localPosition = position;
                        obj.transform.localRotation = rotation;
                    }
                    catch
                    {
                        Debug.LogWarning($"Error Processing {solidName}");
                        continue;
                    }
                    break;*/
                    break;

                case "union":
                    /*string unresultname = solid.Attribute("name").Value ?? "Error";
                    string un_first = solid.Element("first").Attribute("ref").Value ?? "Error";
                    string un_second = solid.Element("second").Attribute("ref").Value ?? "Error";
                    obj = PerformUnion(obj, un_first, new List<string>() { position.x.ToString(), position.y.ToString(), position.z.ToString() },
                        new List<string>() { rotation.eulerAngles.x.ToString(), rotation.eulerAngles.y.ToString(), rotation.eulerAngles.z.ToString() }, un_second, unresultname);
                    obj.transform.localPosition = position;
                    obj.transform.localRotation = rotation;
                    try
                    {
                        
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Error Processing {solidName}: {e.Message}");
                        continue;
                    }
                    break;*/
                    break;

                default:
                    Debug.LogWarning($"SolidType '{solidType}' is not supported.");
                    break;
            }

            // If a valid GameObject was created, set its name
            if (obj != null)
            {
                obj.name = solidName;
            }
            else
            {
                //Debug.LogWarning($"GO null; Cannot process {solidName}");
                //break;
            }
        }

        //Debug.Log("Finished processing all solids.");
    }
    
    public static void ProcessBooleans(string path)
    {
        Debug.Log("--------------COMMENCING BOOLEANS--------------");
        XDocument gdmlDoc = XDocument.Load(path);
        var solids = gdmlDoc.Descendants("solids").Elements();

        foreach (var solid in solids)
        {
            string solidName = solid.Attribute("name")?.Value ?? "UnknownSolid";
            string solidType = solid.Name.LocalName;
            if (solidName.Contains("World") || solidName.Contains("Envelope"))
            {
                Debug.Log($"Skipping solid '{solidName}' as it contains 'World'.");
                continue;
            }
            Debug.Log($"Processing solid: {solidName} of type {solidType}");
            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            XElement posElement = solid.Element("position");
            XElement rotElement = solid.Element("rotation");

            if (posElement != null)
            {
                float x = float.Parse(posElement.Attribute("x")?.Value ?? "0");
                float y = float.Parse(posElement.Attribute("y")?.Value ?? "0");
                float z = float.Parse(posElement.Attribute("z")?.Value ?? "0");
                position = new Vector3(x, y, z);
            }

            if (rotElement != null)
            {
                float rx = float.Parse(rotElement.Attribute("x")?.Value ?? "0");
                float ry = float.Parse(rotElement.Attribute("y")?.Value ?? "0");
                float rz = float.Parse(rotElement.Attribute("z")?.Value ?? "0");
                rotation = Quaternion.Euler(rx, ry, rz);
            }
            GameObject obj = null;
            switch (solidType)
            {
                case "subtraction":

                    string subresultname = solid.Attribute("name").Value ?? "Error";
                    string sub_first = solid.Element("first").Attribute("ref").Value ?? "Error";
                    string sub_second = solid.Element("second").Attribute("ref").Value ?? "Error";
                    Debug.Log($"Performing Subtraction for {subresultname}; subtracting {sub_second} from {sub_first}");
                    obj = PerformSubtraction(obj,sub_first, new List<string>() { position.x.ToString(), position.y.ToString(), position.z.ToString() },
                        new List<string>() { rotation.eulerAngles.x.ToString(), rotation.eulerAngles.y.ToString(), rotation.eulerAngles.z.ToString() }, sub_second, subresultname);
                     if(obj!=null)   {obj.transform.localPosition = position;
                        obj.transform.localRotation = rotation * rhs_transform;
                        obj.name = subresultname;
                    }
                    Debug.Log($"Performing Subtraction for {subresultname}");
                    break;

                case "union":
                    string unresultname = solid.Attribute("name").Value ?? "Error";
                    string un_first = solid.Element("first").Attribute("ref").Value ?? "Error";
                    string un_second = solid.Element("second").Attribute("ref").Value ?? "Error";
                    Debug.Log($"Performing Union for {unresultname}; unifying {un_first} and {un_second}");
                    obj = PerformUnion(obj, un_first, new List<string>() { position.x.ToString(), position.y.ToString(), position.z.ToString() },
                                            new List<string>() { rotation.eulerAngles.x.ToString(), rotation.eulerAngles.y.ToString(), rotation.eulerAngles.z.ToString() }, un_second, unresultname);
                       if(obj!=null){ obj.transform.localPosition = position;
                        obj.transform.localRotation = rotation * rhs_transform;
                        obj.name = unresultname;
                    }
                    break;

                default:
                    //Debug.LogWarning($"SolidType '{solidType}' is not supported.");
                    break;
            }

            // If a valid GameObject was created, set its name
            if (obj != null)
            {
                obj.name = solidName;
            }
            else
            {
                //Debug.LogWarning($"GO null; Cannot process {solidName}");
                //break;
            }
        }

        Debug.Log("Finished processing all solids.");
    }


    private static void GeoDrawer(XDocument gdmlDoc, string name, List<string> pos, List<string> rot, string solidtype, string solidref)
    {
        var solid = gdmlDoc.Descendants("solids")
                           .Elements()
                           .FirstOrDefault(s => s.Attribute("name")?.Value == solidref);

        if (solid == null)
        {
            Debug.LogError($"SolidRef '{solidref}' not found for SolidType '{solidtype}'.");
            return;
        }

        Vector3 position = new Vector3(float.Parse(pos[0]), float.Parse(pos[2]), float.Parse(pos[1]));
        Quaternion rotation = Quaternion.Euler(float.Parse(rot[0]), float.Parse(rot[1]), float.Parse(rot[2]));

        switch (solidtype)
        {
            case "box":
                double x = double.Parse(solid.Attribute("x")?.Value ?? "0");
                double y = double.Parse(solid.Attribute("y")?.Value ?? "0");
                double z = double.Parse(solid.Attribute("z")?.Value ?? "0");
                GameObject obj = CreateBoxMesh(name, (float)x, (float)y, (float)z);
                obj.transform.localPosition = position;
                obj.transform.localRotation = rotation;
                break;

            case "cone":
                double rmin1 = double.Parse(solid.Attribute("rmin1")?.Value ?? "0");
                double rmax1 = double.Parse(solid.Attribute("rmax1")?.Value ?? "0");
                double rmin2 = double.Parse(solid.Attribute("rmin2")?.Value ?? "0");
                double rmax2 = double.Parse(solid.Attribute("rmax2")?.Value ?? "0");
                double zCone = double.Parse(solid.Attribute("z")?.Value ?? "0");
                double startphi_cone = double.Parse(solid.Attribute("startphi")?.Value ?? "0");
                double deltaphi_cone = double.Parse(solid.Attribute("deltaphi")?.Value ?? "0");
                GameObject obj1 = CreateConeSegmentMesh(name, (float)rmin1, (float)rmax1, (float)rmin2, (float)rmax2, (float)zCone, (float)startphi_cone, (float)deltaphi_cone);
                obj1.transform.localPosition = position;
                obj1.transform.localRotation = rotation; break;

            case "tube":
                double rmin = double.Parse(solid.Attribute("rmin")?.Value ?? "0");
                double rmax = double.Parse(solid.Attribute("rmax")?.Value ?? "0");
                double zTube = double.Parse(solid.Attribute("z")?.Value ?? "0");
                double startphi_tube = double.Parse(solid.Attribute("startphi")?.Value ?? "0");
                double deltaphi_tube = double.Parse(solid.Attribute("deltaphi")?.Value ?? "0");
                GameObject obj2 = CreateTubeSegmentMesh(name, (float)rmin, (float)rmax, (float)zTube, (float)startphi_tube, (float)deltaphi_tube);
                obj2.transform.localPosition = position;
                obj2.transform.localRotation = rotation; break;

            case "ellipsoid":
                double ax = double.Parse(solid.Attribute("ax")?.Value ?? "0");
                double by = double.Parse(solid.Attribute("by")?.Value ?? "0");
                double cz = double.Parse(solid.Attribute("cz")?.Value ?? "0");
                GameObject obj3 = CreateEllipsoidMesh(name, (float)ax, (float)by, (float)cz);
                obj3.transform.localPosition = position;
                obj3.transform.localRotation = rotation; break;

            case "sphere":
                double rminSphere = double.Parse(solid.Attribute("rmin")?.Value ?? "0");
                double rmaxSphere = double.Parse(solid.Attribute("rmax")?.Value ?? "0");
                double startphi_sphere = double.Parse(solid.Attribute("startphi")?.Value ?? "0");
                double deltaphi_sphere = double.Parse(solid.Attribute("deltaphi")?.Value ?? "0");
                double starttheta_sphere = double.Parse(solid.Attribute("starttheta")?.Value ?? "0");
                double deltatheta_sphere = double.Parse(solid.Attribute("deltatheta")?.Value ?? "0");
                GameObject obj4 = CreateSphereSegmentMesh(name, (float)rminSphere, (float)rmaxSphere, (float)startphi_sphere, (float)deltaphi_sphere, (float)starttheta_sphere, (float)deltatheta_sphere);
                obj4.transform.localPosition = position;
                obj4.transform.localRotation = rotation; break;

            case "trd":
                double x1 = double.Parse(solid.Attribute("x1")?.Value ?? "0");
                double x2 = double.Parse(solid.Attribute("x2")?.Value ?? "0");
                double y1 = double.Parse(solid.Attribute("y1")?.Value ?? "0");
                double y2 = double.Parse(solid.Attribute("y2")?.Value ?? "0");
                double z_trap = double.Parse(solid.Attribute("z")?.Value ?? "0");
                GameObject obj5 = CreateTrdMesh(name, (float)x1, (float)x2, (float)y1, (float)y2, (float)z_trap);
                obj5.transform.localPosition = position;
                obj5.transform.localRotation = rotation; break;

            case "subtraction":
                string subresultname = solid.Attribute("name").Value ?? "Error";
                string sub_first = solid.Element("first").Attribute("ref").Value ?? "Error";
                string sub_second = solid.Element("second").Attribute("ref").Value ?? "Error";
                GameObject obj10 = null;
                PerformSubtraction(obj10,sub_first, pos, rot, sub_second, subresultname);
                break;

            case "union":
                string unresultname = solid.Attribute("name").Value ?? "Error";
                string un_first = solid.Element("first").Attribute("ref").Value ?? "Error";
                string un_second = solid.Element("second").Attribute("ref").Value ?? "Error";
                GameObject obj11 = null;
                PerformUnion(obj11,un_first, pos, rot, un_second, unresultname);
                break;

            default:
                Debug.LogWarning($"SolidType '{solidtype}' is not supported.");
                break;
        }
    }

    private static GameObject CreateTrapezoidMesh(string name, float x1, float x2, float x3, float x4, float y1, float y2, float z, float theta, float phi, float alpha1, float alpha2)
    {
        GameObject trapezoid = new GameObject(name);
        MeshFilter meshFilter = trapezoid.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = trapezoid.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[8];

        vertices[0] = new Vector3(-x1 / 2, -y1 / 2, -z / 2);
        vertices[1] = new Vector3(x1 / 2, -y1 / 2, -z / 2);
        vertices[2] = new Vector3(-x2 / 2, y1 / 2, -z / 2);
        vertices[3] = new Vector3(x2 / 2, y1 / 2, -z / 2);

        vertices[4] = new Vector3(-x3 / 2, -y2 / 2, z / 2);
        vertices[5] = new Vector3(x3 / 2, -y2 / 2, z / 2);
        vertices[6] = new Vector3(-x4 / 2, y2 / 2, z / 2);
        vertices[7] = new Vector3(x4 / 2, y2 / 2, z / 2);

        // Define triangles
        int[] triangles = new int[]
        {
        // Bottom face (-Z)
        0, 1, 2,
        1, 3, 2,

        // Top face (+Z)
        4, 6, 5,
        5, 6, 7,

        // Front face (Y = -y)
        0, 4, 1,
        1, 4, 5,

        // Back face (Y = +y)
        2, 3, 6,
        3, 7, 6,

        // Left face (X = -x)
        0, 2, 4,
        2, 6, 4,

        // Right face (X = +x)
        1, 5, 3,
        3, 5, 7
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshRenderer.material = new Material(Shader.Find("Standard"));

        return trapezoid;
    }

    private static GameObject PerformSubtraction(GameObject obj, string name, List<string> pos, List<string> rot, string solidref, string resultname)
{
    GameObject first = GameObject.Find(name);  
    GameObject second = GameObject.Find(solidref);
        var composite = new GameObject();
        if (first == null || second == null)
    {
        Debug.LogError($"Either the minuend '{name}' or the subtrahend '{solidref}' could not be found.");
        return null;
    }
        Model result;
        try
        {
            result = CSG.Subtract(second, first);
            Debug.Log($"Subtraction successful for {resultname}");
        }
        catch (Exception e)
        {

            Debug.LogError($"Error Processing {resultname}: {e.Message}");
            result = null;
            return composite;
        }
        // Create a gameObject to render the result
        composite.AddComponent<MeshFilter>().sharedMesh = result.mesh;
        composite.AddComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();
        composite.name = resultname;
        return composite;
        //GameObject.Destroy(second);
    }


private static GameObject PerformUnion(GameObject obj,string name, List<string> pos, List<string> rot, string solidref, string resultname)
    {
        GameObject objectA = GameObject.Find(solidref); 
        GameObject objectB = GameObject.Find(name);
        var composite = new GameObject();
        if (objectA == null || objectB == null)
        {
            Debug.LogError($"Either '{name}' or '{solidref}' could not be found for union.");
            return composite;
        }
        Model result;
        try
        {
            result = CSG.Subtract(objectA, objectB);
            Debug.Log($"Union successful for {resultname}");

        } catch (Exception e) {
        
            Debug.LogError($"Error Processing {resultname}: {e.Message}");
            result = null;
            return composite;
        }
        composite.AddComponent<MeshFilter>().sharedMesh = result.mesh;
        composite.AddComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();
        composite.name = resultname;
        return composite;
        
        //GameObject.Destroy(objectA); // I am choosing to destory it, but this may or may not be necessary - have to check
        //GameObject.Destroy(objectB);

        


    }


    private static GameObject CreateBoxMesh(string name, float width, float height, float depth)
    {
        GameObject box = new GameObject(name);
        MeshFilter meshFilter = box.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = box.AddComponent<MeshRenderer>();
        Material defaultMaterial = new Material(Shader.Find("Standard")); // Unity's Standard Shader
        defaultMaterial.color = Color.gray; // Optional: Set a default color
        meshRenderer.material = defaultMaterial;
        Mesh mesh = new Mesh();
        mesh.vertices = new[]
        {
        new Vector3(-width / 2, -height / 2, -depth / 2),
        new Vector3(width / 2, -height / 2, -depth / 2),
        new Vector3(width / 2, height / 2, -depth / 2),
        new Vector3(-width / 2, height / 2, -depth / 2),
        new Vector3(-width / 2, -height / 2, depth / 2),
        new Vector3(width / 2, -height / 2, depth / 2),
        new Vector3(width / 2, height / 2, depth / 2),
        new Vector3(-width / 2, height / 2, depth / 2),
    };

        mesh.triangles = new[]
        {
        0, 2, 1, 0, 3, 2, // Front
        4, 5, 6, 4, 6, 7, // Back
        0, 1, 5, 0, 5, 4, // Bottom
        2, 3, 7, 2, 7, 6, // Top
        0, 4, 7, 0, 7, 3, // Left
        1, 2, 6, 1, 6, 5  // Right
    };

        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

        return box;
    }


    private static GameObject CreateConeSegmentMesh(string name, float rmin1, float rmax1, float rmin2, float rmax2, float z, float startphi, float deltaphi, int segments = 36)
    {
        GameObject coneSegment = new GameObject(name);
        MeshFilter meshFilter = coneSegment.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = coneSegment.AddComponent<MeshRenderer>();
        Material defaultMaterial = new Material(Shader.Find("Standard")); // Unity's Standard Shader
        defaultMaterial.color = Color.gray; // Optional: Set a default color
        meshRenderer.material = defaultMaterial;
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float angleStep = deltaphi / segments;
        float startRad = Mathf.Deg2Rad * startphi;
        float endRad = Mathf.Deg2Rad * (startphi + deltaphi);

        // Create vertices
        for (int i = 0; i <= segments; i++)
        {
            float angle = startRad + i * Mathf.Deg2Rad * angleStep;

            float xOuterBase = Mathf.Cos(angle) * rmax1;
            float zOuterBase = Mathf.Sin(angle) * rmax1;

            float xInnerBase = Mathf.Cos(angle) * rmin1;
            float zInnerBase = Mathf.Sin(angle) * rmin1;

            float xOuterTop = Mathf.Cos(angle) * rmax2;
            float zOuterTop = Mathf.Sin(angle) * rmax2;

            float xInnerTop = Mathf.Cos(angle) * rmin2;
            float zInnerTop = Mathf.Sin(angle) * rmin2;

            // Bottom ring
            vertices.Add(new Vector3(xOuterBase, -z / 2, zOuterBase)); // Outer base
            vertices.Add(new Vector3(xInnerBase, -z / 2, zInnerBase)); // Inner base

            // Top ring
            vertices.Add(new Vector3(xOuterTop, z / 2, zOuterTop)); // Outer top
            vertices.Add(new Vector3(xInnerTop, z / 2, zInnerTop)); // Inner top
        }

        // Generate triangles
        for (int i = 0; i < segments; i++)
        {
            int baseIndex = i * 4;

            // Outer wall
            triangles.Add(baseIndex);
            triangles.Add(baseIndex + 4);
            triangles.Add(baseIndex + 2);

            triangles.Add(baseIndex + 2);
            triangles.Add(baseIndex + 4);
            triangles.Add(baseIndex + 6);

            // Inner wall
            triangles.Add(baseIndex + 1);
            triangles.Add(baseIndex + 3);
            triangles.Add(baseIndex + 5);

            triangles.Add(baseIndex + 5);
            triangles.Add(baseIndex + 3);
            triangles.Add(baseIndex + 7);

            // Bottom ring
            triangles.Add(baseIndex);
            triangles.Add(baseIndex + 1);
            triangles.Add(baseIndex + 4);

            triangles.Add(baseIndex + 4);
            triangles.Add(baseIndex + 1);
            triangles.Add(baseIndex + 5);

            // Top ring
            triangles.Add(baseIndex + 2);
            triangles.Add(baseIndex + 6);
            triangles.Add(baseIndex + 3);

            triangles.Add(baseIndex + 3);
            triangles.Add(baseIndex + 6);
            triangles.Add(baseIndex + 7);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

        return coneSegment;
    }

    private static GameObject CreateTubeSegmentMesh(string name, float rmin, float rmax, float z, float startphi, float deltaphi, int segments = 36)
    {


        GameObject tubeSegment = new GameObject(name);
        MeshFilter meshFilter = tubeSegment.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = tubeSegment.AddComponent<MeshRenderer>();
        Material defaultMaterial = new Material(Shader.Find("Standard")); // Unity's Standard Shader
        defaultMaterial.color = Color.red; // Optional: Set a default color
        meshRenderer.material = defaultMaterial;
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float angleStep = deltaphi / segments;
        float startRad = Mathf.Deg2Rad * startphi;

        // Generate vertices
        for (int i = 0; i <= segments; i++)
        {
            float angle = startRad + i * Mathf.Deg2Rad * angleStep;

            float xOuter = Mathf.Cos(angle) * rmax;
            float zOuter = Mathf.Sin(angle) * rmax;

            float xInner = Mathf.Cos(angle) * rmin;
            float zInner = Mathf.Sin(angle) * rmin;

            // Bottom ring
            /*
            vertices.Add(new Vector3(xOuter, -z / 2, zOuter)); // Outer bottom
            vertices.Add(new Vector3(xInner, -z / 2, zInner)); // Inner bottom

            // Top ring
            vertices.Add(new Vector3(xOuter, z / 2, zOuter)); // Outer top
            vertices.Add(new Vector3(xInner, z / 2, zInner)); // Inner top*/

            vertices.Add(new Vector3(xOuter, zOuter, -z/2)); // Outer bottom
            vertices.Add(new Vector3(xInner, zInner, -z/2)); // Inner bottom

            // Top ring
            vertices.Add(new Vector3(xOuter, zOuter, z/2)); // Outer top
            vertices.Add(new Vector3(xInner, zInner, z/2)); 
        }

        // Generate triangles
        for (int i = 0; i < segments; i++)
        {
            int baseIndex = i * 4;

            // Outer wall
            triangles.Add(baseIndex);
            triangles.Add(baseIndex + 4);
            triangles.Add(baseIndex + 2);

            triangles.Add(baseIndex + 2);
            triangles.Add(baseIndex + 4);
            triangles.Add(baseIndex + 6);

            // Inner wall
            triangles.Add(baseIndex + 1);
            triangles.Add(baseIndex + 3);
            triangles.Add(baseIndex + 5);

            triangles.Add(baseIndex + 5);
            triangles.Add(baseIndex + 3);
            triangles.Add(baseIndex + 7);

            // Bottom ring
            triangles.Add(baseIndex);
            triangles.Add(baseIndex + 1);
            triangles.Add(baseIndex + 4);

            triangles.Add(baseIndex + 4);
            triangles.Add(baseIndex + 1);
            triangles.Add(baseIndex + 5);

            // Top ring
            triangles.Add(baseIndex + 2);
            triangles.Add(baseIndex + 6);
            triangles.Add(baseIndex + 3);

            triangles.Add(baseIndex + 3);
            triangles.Add(baseIndex + 6);
            triangles.Add(baseIndex + 7);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

        return tubeSegment;
    }


    private static GameObject CreateEllipsoidMesh(string name, float radiusX, float radiusY, float radiusZ)
    {
        GameObject ellipsoid = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ellipsoid.name = name;
        ellipsoid.transform.localScale = new Vector3(radiusX * 2, radiusY * 2, radiusZ * 2);
        return ellipsoid;
    }

    private static GameObject CreateSphereSegmentMesh(string name, float rmin, float rmax, float startphi, float deltaphi, float starttheta, float deltatheta, int segmentsPhi = 36, int segmentsTheta = 18)
    {
        GameObject sphereSegment = new GameObject(name);
        MeshFilter meshFilter = sphereSegment.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = sphereSegment.AddComponent<MeshRenderer>();
        Material defaultMaterial = new Material(Shader.Find("Standard")); // Unity's Standard Shader
        defaultMaterial.color = Color.gray; // Optional: Set a default color
        meshRenderer.material = defaultMaterial;

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float startPhiRad = Mathf.Deg2Rad * startphi;
        float deltaPhiRad = Mathf.Deg2Rad * deltaphi;
        float startThetaRad = Mathf.Deg2Rad * starttheta;
        float deltaThetaRad = Mathf.Deg2Rad * deltatheta;

        // Generate vertices
        for (int t = 0; t <= segmentsTheta; t++)
        {
            float theta = startThetaRad + t * deltaThetaRad / segmentsTheta;
            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);

            for (int p = 0; p <= segmentsPhi; p++)
            {
                float phi = startPhiRad + p * deltaPhiRad / segmentsPhi;
                float sinPhi = Mathf.Sin(phi);
                float cosPhi = Mathf.Cos(phi);

                // Outer radius
                float xOuter = rmax * sinTheta * cosPhi;
                float yOuter = rmax * cosTheta;
                float zOuter = rmax * sinTheta * sinPhi;
                vertices.Add(new Vector3(xOuter, yOuter, zOuter));

                // Inner radius
                if (rmin > 0.0f)
                {
                    float xInner = rmin * sinTheta * cosPhi;
                    float yInner = rmin * cosTheta;
                    float zInner = rmin * sinTheta * sinPhi;
                    vertices.Add(new Vector3(xInner, yInner, zInner));
                }
            }
        }

        // Generate triangles
        int stride = rmin > 0.0f ? 2 : 1; // Determine if inner surface exists
        for (int t = 0; t < segmentsTheta; t++)
        {
            for (int p = 0; p < segmentsPhi; p++)
            {
                int current = t * (segmentsPhi + 1) * stride + p * stride;
                int next = current + (segmentsPhi + 1) * stride;

                // Outer surface
                triangles.Add(current);
                triangles.Add(current + stride);
                triangles.Add(next);

                triangles.Add(next);
                triangles.Add(current + stride);
                triangles.Add(next + stride);

                if (rmin > 0.0f) // Inner surface
                {
                    triangles.Add(current + 1);
                    triangles.Add(next + 1);
                    triangles.Add(current + stride + 1);

                    triangles.Add(next + 1);
                    triangles.Add(next + stride + 1);
                    triangles.Add(current + stride + 1);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

        return sphereSegment;
    }

    private static GameObject CreateTrdMesh(string name, float dx1, float dx2, float dy1, float dy2, float dz)
    {
        GameObject trapezoid = new GameObject(name);
        MeshFilter meshFilter = trapezoid.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = trapezoid.AddComponent<MeshRenderer>();
        Material defaultMaterial = new Material(Shader.Find("Standard")); // Unity's Standard Shader
        defaultMaterial.color = Color.red; // Optional: Set a default color
        meshRenderer.material = defaultMaterial;
        Mesh mesh = new Mesh();

        // Define vertices for trapezoid
        var vertices = new[]
        {
        new Vector3(-dx1, -dy1, -dz), new Vector3(dx1, -dy1, -dz),   // Bottom face
        new Vector3(dx2, dy2, dz), new Vector3(-dx2, dy2, dz),      // Top face
    };

        // Define triangles for faces
        var triangles = new[]
        {
        0, 1, 2, 2, 3, 0, // Bottom face
    };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

        return trapezoid;
    }

    private static GameObject GeneratePolyconeMesh(string name, float startPhi, float deltaPhi, List<(float rmin, float rmax, float z)> zplanes, int segments = 32)
    {
        GameObject polycone = new GameObject(name);
        MeshFilter meshFilter = polycone.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = polycone.AddComponent<MeshRenderer>();
        Material defaultMaterial = new Material(Shader.Find("Standard")); 
        defaultMaterial.color = Color.red; 
        meshRenderer.material = defaultMaterial;
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float startAngle = startPhi;
        float endAngle = startPhi + deltaPhi;
        float angleStep = deltaPhi / segments;

        foreach (var zplane in zplanes)
        {
            float z = -zplane.z;

            for (int i = 0; i <= segments; i++)
            {
                float angle = startAngle + i * angleStep;
                float xOuter = zplane.rmax * Mathf.Cos(angle);
                float yOuter = zplane.rmax * Mathf.Sin(angle);
                float xInner = zplane.rmin * Mathf.Cos(angle);
                float yInner = zplane.rmin * Mathf.Sin(angle);

                vertices.Add(new Vector3(xOuter, yOuter, z)); 
                vertices.Add(new Vector3(xInner, yInner, z)); 
            }
        }

        int ringVertexCount = (segments + 1) * 2;

        for (int i = 0; i < zplanes.Count - 1; i++)
        {
            int ringOffset1 = i * ringVertexCount;
            int ringOffset2 = (i + 1) * ringVertexCount;

            for (int j = 0; j < segments; j++)
            {
                int i1 = ringOffset1 + j * 2;
                int i2 = ringOffset1 + j * 2 + 1;
                int i3 = ringOffset2 + j * 2;
                int i4 = ringOffset2 + j * 2 + 1;

                // Outer surface
                triangles.Add(i1);
                triangles.Add(i3);
                triangles.Add(i3 + 2);

                triangles.Add(i1);
                triangles.Add(i3 + 2);
                triangles.Add(i1 + 2);

                // Inner surface
                triangles.Add(i2);
                triangles.Add(i4 + 2);
                triangles.Add(i4);

                triangles.Add(i2);
                triangles.Add(i2 + 2);
                triangles.Add(i4 + 2);
            }
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        return polycone;
    }

}

