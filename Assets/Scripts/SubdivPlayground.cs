using Mola;
using System.Collections.Generic;
using UnityEngine;

public enum SubdivMethod
{
    Extrude,
    ExtrudeToPointCenter,
    ExtrudeTapered,
    Grid,
    SplitGridAbs,
    SplitRelative,
    LinearSplitBorder,
    LinearSplitQuad,
    SplitRoof,
    SplitFrame,
    Offset,
    OffsetPerEdge,
    CatmullClark
}

public class SubdivPlayground : MolaMonoBehaviour
{

    public SubdivMethod subdiv = new();

    [Header("Parameters")]
    [Range(0, 1)]
    public float parameter1 = 0.5f;
    [Range(0, 1)]
    public float parameter2 = 0.2f;

    public bool capTop = true;

    private void Start()
    {
        InitMesh();
    }

    public override void UpdateGeometry()
    {
        MolaMesh mesh = MeshFactory.CreateBox(0, 0, 0, 1, 1, 1);

        switch (subdiv)
        {
            case SubdivMethod.Extrude:
                mesh = MeshSubdivision.SubdivideMeshExtrude(mesh, parameter1, true); break;
            case SubdivMethod.ExtrudeToPointCenter:
                mesh = MeshSubdivision.SubdivideMeshExtrudeToPointCenter(mesh, parameter1); break;
            case SubdivMethod.ExtrudeTapered:
                mesh = MeshSubdivision.SubdivideMeshExtrudeTapered(mesh, parameter1, parameter2, capTop); break;
            case SubdivMethod.Grid:
                mesh = MeshSubdivision.SubdivideMeshGrid(mesh, 3, 2); break;
            case SubdivMethod.SplitGridAbs:
                mesh = MeshSubdivision.SubdivideMeshSplitGridAbs(mesh, parameter1, parameter2); break;
            case SubdivMethod.SplitRelative:
                mesh = MeshSubdivision.SubdivideMeshSplitRelative(mesh, 0, 0.8f, 0.9f, 0.4f, 0.5f); break;
            case SubdivMethod.LinearSplitBorder:
                mesh = MeshSubdivision.SubdivideMeshLinearSplitBorder(mesh, parameter1, parameter2, 0); break;
            case SubdivMethod.LinearSplitQuad:
                mesh = MeshSubdivision.SubdivideMeshLinearSplitQuad(mesh, parameter1, 0); break;
            case SubdivMethod.SplitRoof:
                mesh = MeshSubdivision.SubdivideMeshSplitRoof(mesh, parameter1); break;
            case SubdivMethod.SplitFrame:
                mesh = MeshSubdivision.SubdivideMeshSplitFrame(mesh, parameter2); break;
            case SubdivMethod.Offset:
                mesh = MeshSubdivision.SubdivideMeshOffset(mesh, parameter2 * -1); break;
            case SubdivMethod.OffsetPerEdge:
                mesh = MeshSubdivision.SubdivideMeshOffsetPerEdge(mesh, new float[] { -0.1f, -0.2f, -0.4f, -0.6f }); break;
            case SubdivMethod.CatmullClark:
                mesh.UpdateTopology();
                mesh = MeshSubdivision.SubdivideMeshCatmullClark(mesh);
                mesh.SeparateVertices(); break;
        }

        if (subdiv == SubdivMethod.CatmullClark)
        {
            FillUnityMesh(mesh);
            ColorMeshRandom();
        }
        else
        {
            FillUnitySubMesh(SplitFaces(mesh));
            ColorSubMeshRandom();
        }
    }

    private List<MolaMesh> SplitFaces(MolaMesh m)
    {
        List<MolaMesh> splitedMeshes = new();
        for (int i = 0; i < m.Faces.Count; i++)
        {
            splitedMeshes.Add(m.CopySubMesh(i));
        }
        return splitedMeshes;
    }
}
