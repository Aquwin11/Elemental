using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour
{
    public int instances;
    public Vector3 maxPos;
    public Mesh objectMesh;
    public Material objMat;
    private List<List<ObjData>> batches = new List<List<ObjData>>();
    // Start is called before the first frame update
    void Start()
    {
        int batchIndexNum = 0;
        List<ObjData> currBatch = new List<ObjData>();
        for (int i = 0; i < instances; i++)
        {
            AddObj(currBatch, i);
            batchIndexNum++;
            if(batchIndexNum>=1000)
            {
                batches.Add(currBatch);
                currBatch = BuildNewBAtch();
                batchIndexNum = 0;
            }
        }
    }

    private void AddObj(List<ObjData> currBatch, int i)
    {
        Vector3 position = new Vector3(Random.Range(-maxPos.x, maxPos.x), 0.443f, Random.Range(-maxPos.z, maxPos.z));
        currBatch.Add(new ObjData(position, Vector3.one, Quaternion.identity));
    }

    private List<ObjData> BuildNewBAtch()
    {
        return new List<ObjData>();
    }

    // Update is called once per frame
    void Update()
    {
        RenderBatcher();
    }

    private void RenderBatcher()
    {
        foreach (var batch in batches)
        {
            Graphics.DrawMeshInstanced(objectMesh, 0, objMat, batch.Select((a) => a.matrix).ToList());
        }
    }
}

public class ObjData
{
    public Vector3 pos;
    public Vector3 scale;
    public Quaternion rot;


    public Matrix4x4 matrix
    {
        get
        {
            return Matrix4x4.TRS(pos, rot, scale);
        }
    }
    public ObjData(Vector3 pos,Vector3 Scale, Quaternion Rot)
    {
        this.pos = pos;
        this.scale = Scale;
        this.rot = Rot;
    }
}
