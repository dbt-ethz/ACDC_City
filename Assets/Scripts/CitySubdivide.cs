using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;
using System.Linq;
using UnityEditor;
using TMPro;

public class CitySubdivide : MolaMonoBehaviour
{
    public float cityDimX = 1000;
    public float cityDimY = 600;
    public float cityDimZ = 200;
    [Range(2, 50)]
    public int roadWidth = 20;
    [Range(1, 5)]
    public int iteration = 4;
    [Range(0, 10)]
    public int seed = 0;

    public override void UpdateGeometry()
    {
        ClearChildrenImmediate();

        MolaMesh city = MeshFactory.CreateQuad(cityDimX, cityDimY);

        for (int i = 0; i < iteration; i++)
        {
            city = MeshSubdivision.SubdivideMeshSplitRelative(city, 0, 0.4f, 0.4f, 0.6f, 0.6f);
        }

        city = MeshSubdivision.SubdivideMeshOffset(city, -roadWidth * 0.5f);
        
        MolaMesh plots = city.CopySubMeshByModulo(4, 5);
        MolaMesh street = city.CopySubMeshByModulo(4, 5, true);


        List<MolaMesh> meshes = new List<MolaMesh> { plots, street };
        List<string> materials = new List<string> { "Green", "Grey" };

        AddLODMeshes(meshes, materials);
        AddLODsToObject();

        //Mesh unityMesh = new Mesh();
        //FillUnityMesh(unityMesh, city, true);
        //var savePath = "Assets/" + "cityMesh" + ".asset";
        //AssetDatabase.CreateAsset(unityMesh, savePath);
        //Debug.Log("mesh saved");

        InstantiateBlocks(plots);
    }
    private void InstantiateBlocks(MolaMesh blocks)
    {

        string groupName = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        List<int> parks = new List<int> { 0, 18, 23, 31};
        int groupIndex = 0;
        Debug.Log(blocks.FacesCount());
        for (int i = 0; i < blocks.FacesCount(); i++)
        {
            int n = i / (groupName.Length + parks.Count - 1);
            GameObject blockGO = new GameObject();
            if (parks.Contains(i))
            {
                int index = parks.FindIndex(a => a == i);
                blockGO.name = "Park" + index;
            }
            else
            {
                if(n == 0) blockGO.name = "Plot" + groupName[groupIndex];
                else blockGO.name = "Plot" + groupName[groupIndex% groupName.Length] + n;

                groupIndex++;
            }

            blockGO.transform.parent = transform;
            MolaMesh blockMesh = blocks.CopySubMesh(i);
            
            var text = blockGO.AddComponent<TextMeshPro>();
            text.rectTransform.Rotate(new Vector3(90, 0, 0), Space.World);
            Vec3 center = blockMesh.FaceCenter(0);
            text.rectTransform.position = new Vector3(center.x, 1, center.y);
            text.rectTransform.localScale = new Vector3(5, 5, 5);

            float a = (int)blockMesh.FaceEdgeLength(0, 0);
            float b = (int)blockMesh.FaceEdgeLength(0, 1);

            text.text = $"{blockGO.name}\n{a} * {b}";
        }

    }
}
