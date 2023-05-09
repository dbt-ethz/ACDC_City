using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;
using System.Linq;

public class Building_A : MolaMonoBehaviour
{

    [Range(3, 40)]
    public float dX = 10;
    public float dY = 10;
    public float dZ = 10;
    [Range(3, 40)]
    public int seg = 10;
    [Range(0, 10)]
    public float extrudeHeight = 3;
    [Range(0, 10)]
    public float extrudeHeightroof = 3;
    [Range(0, 20)]
    public int division = 3;
    [Range(0, 1)]
    public float mask = 0.5f;
    [Range(0, 20)]
    public int gridNum = 3;
    [Range(0, 10)]
    public float extrudetapered = 2;
    [Range(0, 1)]
    public float tapered = 0.2f;
    [Range(0, 1)]
    public float extrude = 3;
    [Range(0, 1)]
    public float taperedwindow = 0.2f;

    public MolaMesh startMesh;

    public override void UpdateGeometry()
    {
        if (startMesh == null)
        {
            startMesh = MeshFactory.CreateCircle(0, 0, 0, dX*0.5f, seg, null);
        }
        MolaMesh floor = startMesh;

        //LOD1
        MolaMesh wall = new MolaMesh();
        MolaMesh roof = new MolaMesh();
        MolaMesh partition = new MolaMesh();
        MolaMesh wallroof = new MolaMesh();

        floor = MeshSubdivision.SubdivideMeshExtrude(floor, dZ);
        #region original code
        ////Debug.Log(floor.FacesCount());

        //List<int> floorfaces = new List<int>();
        //List<int> resultList = new List<int>();
        //int count;
        //for (count = 1; count <= floor.FacesCount(); count++)
        //{
        //    floorfaces.Add(count); 
        //}
        //for (count = floorfaces.Count - 2; count >= 0; count -= 2)
        //{
        //    Debug.Log(floorfaces[count]); 
        //    resultList.Add(floorfaces[count]);
        //}
        //wallroof = floor.CopySubMesh(resultList, false);

        //List<int> wallfaces = new List<int>();
        //List<int> resultList2 = new List<int>();
        //int x;
        //for (x = 1 ; x <= wallroof.FacesCount(); x++)
        //{
        //    wallfaces.Add(x);
        //}
        //for (x = wallfaces.Count - 2; x >= 0; x -= 2)
        //{
        //    //Debug.Log(wallfaces[x]);
        //    resultList2.Add(wallfaces[x]);

        //}

        //roof = wallroof.CopySubMesh(resultList2, true);
        //wall = wallroof.CopySubMesh(resultList2, false);
        //partition = floor.CopySubMesh(resultList, true);
        #endregion
        
        // WEN COMMENT: I sense this is what you want to do
        roof = floor.CopySubMeshByModulo(3, 4);
        wall = floor.CopySubMeshByModulo(1, 4);
        partition = floor.CopySubMeshByModulo(2, 4);

        AddLODMeshes(
            new List<MolaMesh> { roof, wall, partition }, 
            new List<string> { "Grey", "Grey", "Grey" }
            );

        #region original code
        ////LOD1

        //if (molaMeshes.Count != 0)

        //{
        //    wall = molaMeshes[0];
        //    roof = molaMeshes[1];
        //    partition = molaMeshes[2];
        //}

        //// 02 operate in the current level
        //// wall operation

        //bool[] randomMask = new bool[wall.FacesCount()];
        //for (int i = 0; i < randomMask.Length; i++)
        //{
        //    if (UnityEngine.Random.value > mask) randomMask[i] = true;
        //}
        //newWall = wall.CopySubMesh(randomMask);
        //randomMask = randomMask.Select(a => !a).ToArray();
        //wall = wall.CopySubMesh(randomMask);
        #endregion

        // wall operation
        // recommend to use the new method
        wall = MeshSubdivision.SubdivideMeshGrid(wall, division, division);

        MolaMesh newWall = new MolaMesh();
        List<int> randomMask = new List<int>();
        for (int i = 0; i < wall.FacesCount(); i++)
        {
            if (UnityEngine.Random.value > mask) randomMask.Add(i);
        }

        newWall = wall.CopySubMesh(randomMask);
        wall = wall.CopySubMesh(randomMask, true);

        #region original
        //MolaMesh newRoof2 = new MolaMesh();
        //bool[] randomMask2 = new bool[roof.FacesCount()];
        //for (int i = 0; i < randomMask2.Length; i++)
        //{
        //    if (UnityEngine.Random.value > mask) randomMask2[i] = true;
        //}

        //newRoof2 = roof.CopySubMesh(randomMask2);
        //randomMask2 = randomMask2.Select(a => !a).ToArray();
        //roof = roof.CopySubMesh(randomMask2);
        #endregion

        //// roof operation
        MolaMesh newRoof = new MolaMesh();
        List<int> randomMask2 = new List<int>();
        for (int i = 0; i < roof.FacesCount(); i++)
        {
            if (UnityEngine.Random.value > mask) randomMask2.Add(i);
        }

        newRoof = roof.CopySubMesh(randomMask2);
        roof = roof.CopySubMesh(randomMask2, true);

        #region original
        //MolaMesh newPartition = new MolaMesh();
        //bool[] randomMask3 = new bool[partition.FacesCount()];
        //for (int i = 0; i < randomMask3.Length; i++)
        //{
        //    if (UnityEngine.Random.value > mask) randomMask3[i] = true;
        //}

        //newPartition = partition.CopySubMesh(randomMask3);
        //randomMask3 = randomMask3.Select(a => !a).ToArray();
        //partition = partition.CopySubMesh(randomMask3);
        #endregion
        // dont operate on partition. you will get broken mesh

        // operate subdivision on one mesh - THIS PART HERE

        newWall = MeshSubdivision.SubdivideMeshGrid(newWall, gridNum, gridNum);
        newWall = MeshSubdivision.SubdivideMeshExtrudeTapered(newWall, extrudetapered, tapered);
        newRoof = MeshSubdivision.SubdivideMeshExtrude(newRoof, extrudeHeightroof);

        // seperate result new roof mesh into floor and wall by orientation
        #region original
        //MolaMesh newRoof2 = new MolaMesh();
        //bool[] orientationMask = new bool[newWall.FacesCount()];
        //for (int i = 0; i < orientationMask.Length; i++)
        //{
        //    if (Mola.Mathf.Abs(UtilsFace.FaceAngleVertical(newWall.FaceVertices(i))) > 1)
        //    {
        //        orientationMask[i] = true;
        //    }
        //}
        //newRoof = newWall.CopySubMesh(orientationMask);
        //orientationMask = orientationMask.Select(a => !a).ToArray();
        //newWall = newWall.CopySubMesh(orientationMask);
        #endregion

        MolaMesh facingUp = newWall.CopySubMeshByNormalZ(0.1f, 1.1f); // face facing up with normalz 1, facing down with normalz -1, vertial 0 
        MolaMesh facingSide = newWall.CopySubMeshByNormalZ(0.1f, 1.1f, false, true);

        roof.AddMesh(facingUp);
        wall.AddMesh(facingSide);

        MolaMesh facingUp2 = newRoof.CopySubMeshByNormalZ(0.1f, 1.1f); // face facing up with normalz 1, facing down with normalz -1, vertial 0 
        MolaMesh facingSide2 = newRoof.CopySubMeshByNormalZ(0.1f, 1.1f, false, true);

        roof.AddMesh(facingUp2);
        wall.AddMesh(facingSide2);

        #region original
        ////LOD0

        //if (molaMeshes.Count != 0)
        //{
        //    wall = molaMeshes[0];
        //    roof = molaMeshes[1];
        //    partition = molaMeshes[2];
        //}

        //MolaMesh window = new MolaMesh();
        //wall = MeshSubdivision.SubdivideMeshExtrudeTapered(wall, extrude, taperedwindow);

        //// seperate mesh into wall and window by index. every 5th face is window
        //bool[] indexMusk = new bool[wall.FacesCount()];
        //for (int i = 0; i < wall.FacesCount(); i++)
        //{
        //    indexMusk[i] = (i + 1) % 5 == 0; // get every 5th item
        //}
        //window = wall.CopySubMesh(indexMusk);

        //indexMusk = indexMusk.Select(a => !a).ToArray();
        //wall = wall.CopySubMesh(indexMusk);

        #endregion

        // 02 operate on wall
        MolaMesh window = new MolaMesh();
        wall = MeshSubdivision.SubdivideMeshExtrudeTapered(wall, 0, taperedwindow);

        window = wall.CopySubMeshByModulo(4, 5);
        wall = wall.CopySubMeshByModulo(4, 5, true);

        List<MolaMesh> molaMeshes = new List<MolaMesh>() { wall, roof, partition, window };
        List<string> materials = new List<string>() { "White", "Green", "White", "Grey" };
        AddLODMeshes(molaMeshes, materials);

        AddLODsToObject();
    }

}
