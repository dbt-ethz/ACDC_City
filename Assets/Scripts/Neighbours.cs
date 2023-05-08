using UnityEngine;
using Mola;
using System;
using System.Collections.Generic;
public class Neighbours : MolaMonoBehaviour
{
    public int nX = 32;
    public int nY = 32;
    [Range(2, 9)]
    public int maxZ = 4;

    public int nCalculations = 100;
    
    int[,] grid = null;
    int[,] gridBackup;

    [Range(0, 8)]
    public int targetNeighbours = 2;
    public float fitness;
    public bool reset;

    public void Update()
    {
        if (reset || grid == null)
        {
            InitRandom();
            reset = false;
        }
        for (int i = 0; i < nCalculations; i++)
        {
            MutateForBetter();
        }
        UpdateMesh();
    }

    void InitRandom()
    {
        grid = new int[nX, nY];
        for (int x = 0; x < nX; x++)
        {
            for (int y = 0; y < nY; y++)
            {
                grid[x, y] = UnityEngine.Random.Range(0, maxZ);
            }
        }
    }

    public void UpdateMesh()
    {
        MolaMesh mesh1 = new MolaMesh();
        MolaMesh mesh2 = new MolaMesh();
        MolaMesh mesh3 = new MolaMesh();
        MolaMesh mesh4 = new MolaMesh();
        MolaMesh mesh5 = new MolaMesh();
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] > 1)
                {
                    float dBox = 0.5f;
                    if (grid[x, y] == 2)
                    {
                        dBox = 0.45f;
                    }
                    float hBox = (grid[x, y] - 1)*1.5f;

                    mesh1.AddMesh(MeshFactory.CreateBox(x - dBox, y - dBox, 0, x + dBox, y + dBox, hBox));
                    if (grid[x, y] ==3) {
                        mesh5.AddMesh(MeshFactory.CreateQuad(dBox * 2f, dBox*2, x, y, hBox + 0.01f));
                    }
                    if (grid[x, y] == 2)
                    {
                        mesh4.AddMesh(MeshFactory.CreateQuad(dBox * 2f, dBox * 2, x, y, hBox + 0.01f));
                    }
                    dBox = 0.5f;
                    mesh1.AddMesh(MeshFactory.CreateBox(x - dBox, y - dBox, 0, x + dBox, y + dBox, 0.1f));
                   
                }
                else if (grid[x, y] == 0)
                {
                    mesh2.AddMesh(MeshFactory.CreateSphere(0.5f, x, y, 1, 5, 5));
                    mesh2.AddMesh(MeshFactory.CreateQuad(1, 1, x, y));
                }
                else if (grid[x, y] == 1)
                {
                    mesh3.AddMesh(MeshFactory.CreateQuad(1, 1, x, y));
                }
            }
        }
        mesh1.Translate(-grid.GetLength(0) * 0.5f + 0.5f, -grid.GetLength(1) * 0.5f + 0.5f, 0);
        mesh2.Translate(-grid.GetLength(0) * 0.5f + 0.5f, -grid.GetLength(1) * 0.5f + 0.5f, 0);
        mesh3.Translate(-grid.GetLength(0) * 0.5f + 0.5f, -grid.GetLength(1) * 0.5f + 0.5f, 0);
        mesh4.Translate(-grid.GetLength(0) * 0.5f + 0.5f, -grid.GetLength(1) * 0.5f + 0.5f, 0);
        mesh5.Translate(-grid.GetLength(0) * 0.5f + 0.5f, -grid.GetLength(1) * 0.5f + 0.5f, 0);


        mesh1.SeparateVertices();
        MolaMonoBehaviour.FabricateMeshObject(gameObject, new List<MolaMesh> { mesh1, mesh2, mesh3,mesh4,mesh5 }, new List<string> { "White", "Green", "Yellow","Red","LightBlue" });
    }

    void Mutate()
    {
        int rX1 = UnityEngine.Random.Range(0, grid.GetLength(0));
        int rY1 = UnityEngine.Random.Range(0, grid.GetLength(1));
        int rX2 = UnityEngine.Random.Range(0, grid.GetLength(0));
        int rY2 = UnityEngine.Random.Range(0, grid.GetLength(1));
        int temp = grid[rX1, rY1];
        grid[rX1, rY1] = grid[rX2, rY2];
        grid[rX2, rY2] = temp;
    }

    void MutateForBetter()
    {
        fitness = CalculateFitness();
        gridBackup = grid.Clone() as int[,];
        Mutate();
        float NewFitness = CalculateFitness();
        if (NewFitness >= fitness)
        {
            fitness = NewFitness;
        }
        else
        {
            grid = gridBackup.Clone() as int[,];
        }
    }

    float CalculateFitness()
    {
        float fitness = 0;
        for (int x = 1; x < grid.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < grid.GetLength(1) - 1; y++)
            {
                int sameNbs = 0;
                if (grid[x, y] == grid[x + 1, y]) sameNbs++;
                if (grid[x, y] == grid[x, y + 1]) sameNbs++;
                if (grid[x, y] == grid[x, y - 1]) sameNbs++;
                if (grid[x, y] == grid[x - 1, y]) sameNbs++;

                if (grid[x, y] == grid[x + 1, y + 1]) sameNbs++;
                if (grid[x, y] == grid[x - 1, y + 1]) sameNbs++;
                if (grid[x, y] == grid[x + 1, y - 1]) sameNbs++;
                if (grid[x, y] == grid[x - 1, y - 1]) sameNbs++;
                fitness += Math.Abs(targetNeighbours - sameNbs) * -1;
            }
        }
        return fitness;
    }
}