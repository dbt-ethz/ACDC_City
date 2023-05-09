using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;


public class PlotSubdivide_A : MolaMonoBehaviour
{
    public float dX = 117;
    public float dY = 228;
    public float dZ = 100;

    public float roadwidth = 5;


    public override void UpdateGeometry()
    {
        MolaMesh plot = MeshFactory.CreateQuad(dX, dY);

        plot = MeshSubdivision.SubdivideMeshGrid(plot, 5, 5);
        plot = MeshSubdivision.SubdivideMeshOffset(plot, roadwidth * - 0.5f);

        MolaMesh plots = plot.CopySubMeshByModulo(4, 5);
        MolaMesh street = plot.CopySubMeshByModulo(4, 5,true);

        List<MolaMesh> meshes = new List<MolaMesh> { street, plots };
        List<string> materials = new List<string> { "White", "Green" };

        AddLODMeshes(meshes, materials);
        AddLODsToObject();

        for (int i = 0; i < plots.FacesCount(); i++)
        {
            GameObject buildingObject = new GameObject();
            buildingObject.transform.parent = this.transform;
            Vec3 center = plots.FaceCenter(i); // need to create unity Vector3 from mola Vec3
            buildingObject.transform.localPosition = new Vector3(center.x, center.z, center.y);

            var buildingscript = buildingObject.AddComponent<Building_A>();
            buildingscript.dX = plots.FaceEdgeLength(i, 0) * 0.8f;
            buildingscript.dY = plots.FaceEdgeLength(i, 1) * 0.8f;
            buildingscript.dZ = Random.Range(5, dZ);
        }

    }

}
