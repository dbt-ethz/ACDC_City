using Mola;
using System.Collections.Generic;
using UnityEngine;

public enum SubdivMethod
{
    None,
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

public enum FilterMethod
{
    None,
    NormalX,
    NormalY,
    NormalZ,
    Modulo,
    Dimension,
    BoundingBox,
    EdgeLength
}

public class SubdivPlayground : MolaMonoBehaviour
{

    public SubdivMethod subdiv = new();
    public FilterMethod filter = new();

    [Header("Subdiv Parameters")]
    [Range(0.1f, 0.5f)]
    public float param1 = 0.2f;
    [Range(0, 4)]
    public int param2 = 1;
    public bool cap = true;

    [Header("Filter Parameters")]
    [Range(0, 1)]
    public float param3 = 0.1f;
    [Range(0, 2)]
    public float param4 = 1;
    [Range(0, 5)]
    public int index = 0;

    private Material[] filteredMats;

    private void Start()
    {
        InitMesh();
        UpdateGeometry();
    }

    public override void UpdateGeometry()
    {
        InitMat();

        MolaMesh mesh = MeshFactory.CreateBox(0, 0, 0, 1, 1, 1);

        switch (subdiv)
        {
            case SubdivMethod.Extrude:
                mesh = MeshSubdivision.SubdivideMeshExtrude(mesh, param1, cap); break;
            case SubdivMethod.ExtrudeToPointCenter:
                mesh = MeshSubdivision.SubdivideMeshExtrudeToPointCenter(mesh, param1); break;
            case SubdivMethod.ExtrudeTapered:
                mesh = MeshSubdivision.SubdivideMeshExtrudeTapered(mesh, param1, param1, cap); break;
            case SubdivMethod.Grid:
                mesh = MeshSubdivision.SubdivideMeshGrid(mesh, param2, param2); break;
            case SubdivMethod.SplitGridAbs:
                mesh = MeshSubdivision.SubdivideMeshSplitGridAbs(mesh, param1, param1); break;
            case SubdivMethod.SplitRelative:
                mesh = MeshSubdivision.SubdivideMeshSplitRelative(mesh, param2, param1, param1, param1, param1); break;
            case SubdivMethod.LinearSplitBorder:
                mesh = MeshSubdivision.SubdivideMeshLinearSplitBorder(mesh, param1, param1, param2); break;
            case SubdivMethod.LinearSplitQuad:
                mesh = MeshSubdivision.SubdivideMeshLinearSplitQuad(mesh, param1, param1, param2); break;
            case SubdivMethod.SplitRoof:
                mesh = MeshSubdivision.SubdivideMeshSplitRoof(mesh, param1); break;
            case SubdivMethod.SplitFrame:
                mesh = MeshSubdivision.SubdivideMeshSplitFrame(mesh, param1); break;
            case SubdivMethod.Offset:
                mesh = MeshSubdivision.SubdivideMeshOffset(mesh, -param1); break;
            case SubdivMethod.OffsetPerEdge:
                mesh = MeshSubdivision.SubdivideMeshOffsetPerEdge(mesh, new float[] { -0.25f * param1, -0.5f * param1, -param1, -1.5f * param1 }); break;
            case SubdivMethod.CatmullClark:
                mesh.UpdateTopology();
                mesh = MeshSubdivision.SubdivideMeshCatmullClark(mesh);
                mesh.SetColorToAllVertices();
                mesh.SeparateVertices(); break;
            default: mesh.SeparateVertices(); break;
        }


        MolaMesh filteredMesh = new();

        switch (filter)
        {
            case FilterMethod.NormalX:
                filteredMesh = mesh.CopySubMeshByNormalX(param3, param4);
                mesh = mesh.CopySubMeshByNormalX(param3, param4, false, true); break;
            case FilterMethod.NormalY:
                filteredMesh = mesh.CopySubMeshByNormalY(param3, param4);
                mesh = mesh.CopySubMeshByNormalY(param3, param4, false, true); break;
            case FilterMethod.NormalZ:
                filteredMesh = mesh.CopySubMeshByNormalZ(param3, param4);
                mesh = mesh.CopySubMeshByNormalZ(param3, param4, false, true); break;
            case FilterMethod.Modulo:
                filteredMesh = mesh.CopySubMeshByModulo(index, 6);
                mesh = mesh.CopySubMeshByModulo(index, 6, true); break;
            case FilterMethod.Dimension:
                filteredMesh = mesh.CopySubMeshByDimension(param3, param4, param3, param4, param3, param4);
                mesh = mesh.CopySubMeshByDimension(param3, param4, param3, param4, param3, param4, true); break;
            case FilterMethod.BoundingBox:
                filteredMesh = mesh.CopySubMeshByBoundingBox(param3, param3, param3, param4, param4, param4);
                mesh = mesh.CopySubMeshByBoundingBox(param3, param3, param3, param4, param4, param4, true); break;
            case FilterMethod.EdgeLength:
                filteredMesh = mesh.CopySubMeshByEdgeLength(param3, param4, index);
                mesh = mesh.CopySubMeshByEdgeLength(param3, param4, index, true); break;
            default: break;
        }

        if (filter == FilterMethod.None)
        {
            FillUnitySubMesh(SplitFaces(mesh));
            ColorSubMeshRandom();
        }
        else
        {
            FillUnitySubMesh(new List<MolaMesh>() { mesh, filteredMesh });
            ColorSubMesh(filteredMats);
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

    private void InitMat()
    {
        filteredMats ??= new Material[]
        {
            Resources.Load("Gray", typeof(Material)) as Material,
            Resources.Load("Green", typeof(Material)) as Material
        };
    }
}
