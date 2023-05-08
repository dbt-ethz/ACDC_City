using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;

public class PlotSubdivide_Y : MolaMonoBehaviour
{
    [Range(10, 300)]
    public float dX = 206;
    [Range(10, 300)]
    public float dY = 120;
    [Range(10, 300)]
    public float dZ = 100;
    [Range(6, 50)]
    public float streetwidth = 10;
    [Range(1, 20)]
    public int nX = 2;
    [Range(1, 20)]
    public int nY = 3;
    override public void UpdateGeometry()
    {
        // clear previously generated plots and buildings
        ClearChildrenImmediate();

        // subdivide plot into smaller plots and street
        MolaMesh plot = MeshFactory.CreateQuad(dX, dY);
        plot = MeshSubdivision.SubdivideMeshGrid(plot, nX, nY);
        plot = MeshSubdivision.SubdivideMeshOffset(plot, streetwidth * -0.5f); // note that this is a different way to do offset

        MolaMesh plots = plot.CopySubMeshByModulo(4, 5); // get every 5th face
        MolaMesh street = plot.CopySubMeshByModulo(4, 5, true); // get the rest faces

        List<MolaMesh> meshes = new List<MolaMesh> { plots, street }; // mola meshes to be visualized
        List<string> materials = new List<string> { "Green", "Grey" }; // material name of mateirals in Resources folder

        AddLODMeshes(meshes, materials);
        AddLODsToObject();

        // for each face of plots, create a new game object, attach your building script
        for (int i = 0; i < plots.FacesCount(); i++)
        {
            GameObject buildingObject = new GameObject();
            buildingObject.transform.parent = this.transform;
            buildingObject.transform.localPosition = new Vector3(0, 0, 0); // if the plot is not at origin, need to reset child local position.

            var buildingscript = buildingObject.AddComponent<Building_F>();
            buildingscript.startMesh = plots.CopySubMesh(i);
            buildingscript.dZ = Random.Range(10, dZ);
        }
    }
}

