using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instancer : MonoBehaviour
{
    public int numberOfInstance;
    public Mesh mesh;
    public Material[] materials;
    private List<List<Matrix4x4>> batches = new List<List<Matrix4x4>>();

    // Start is called before the first frame update
    void Start()
    {
        batches.Add(new List<Matrix4x4>());

        int addMatrices = 0;
        for (int i = 0; i < numberOfInstance; i++)
        {
            batches[batches.Count - 1].Add(Matrix4x4.TRS(
                        pos: new Vector3(x: Random.Range(-20, 20), y: Random.Range(-10, 10), z: Random.Range(0, 300)),
                        q: Random.rotation,
                        s: new Vector3(x: Random.Range(1, 3), y: Random.Range(1, 3), z: Random.Range(1, 3))));
            addMatrices += 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        RenderBatches();
    }

    void RenderBatches()
    {
        foreach (var batch in batches)
        {
            for (int i = 0; i < mesh.subMeshCount; i++)
            {

                Graphics.DrawMeshInstanced(mesh,
                    i,
                    materials[i],
                    batch);
                /*Graphics.DrawMeshInstanced()
                Graphics.DrawMeshInstancedIndirect(mesh,i,materials[i],)*/
            }
        }
    }
}