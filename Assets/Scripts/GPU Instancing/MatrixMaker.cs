using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixMaker : MonoBehaviour
{
    private Vector3 position, rotation, scale;


    public void Start()
    {
        Matrix4x4 matrix4X4 = Matrix4x4.TRS(position, Quaternion.Euler(rotation), scale);
    }
}
