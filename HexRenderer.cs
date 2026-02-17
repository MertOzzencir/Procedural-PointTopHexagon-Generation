using System;
using System.Collections.Generic;
using System.Security;
using NUnit.Framework.Internal;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class HexRenderer : MonoBehaviour
{
    private MeshRenderer mRenderer;
    private MeshFilter mFilter;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    Mesh mesh;
    void Awake()
    {
        mRenderer = GetComponent<MeshRenderer>();
        mFilter = GetComponent<MeshFilter>();
    }

    public void SetVerticesAndTriangles(Vector3 center, int hexCount, float outerSize,float innerRadius,float height)
    {
        for (int edgeIndex = 0; edgeIndex < 6; edgeIndex++)
        {
            FindPoints(innerRadius, outerSize, edgeIndex, height / 2, center);
            FindPoints(innerRadius, outerSize, edgeIndex, center.y, center);
        }

        for (int faceCount = 0 + hexCount * 24; faceCount <= 20 + hexCount * 24; faceCount += 4)
        {
            if (faceCount == 20 + 24 * hexCount)
            {
                #region Üst Yüzey
                triangles.Add(faceCount);
                triangles.Add(1 + 24 * hexCount);
                triangles.Add(faceCount + 1);

                triangles.Add(faceCount);
                triangles.Add(0 + 24 * hexCount);
                triangles.Add(1 + 24 * hexCount);
                #endregion

                #region Alt Yüzey
                triangles.Add(faceCount + 2);
                triangles.Add(faceCount + 3);
                triangles.Add(3 + 24 * hexCount);

                triangles.Add(faceCount + 2);
                triangles.Add(3 + 24 * hexCount);
                triangles.Add(2 + 24 * hexCount);
                #endregion
                #region Diş kenar
                triangles.Add(faceCount + 1);
                triangles.Add(1 + 24 * hexCount);
                triangles.Add(3 + 24 * hexCount);

                triangles.Add(faceCount + 1);
                triangles.Add(3 + 24 * hexCount);
                triangles.Add(faceCount + 3);
                #endregion
                #region İç kenar
                triangles.Add(faceCount);
                triangles.Add(2 + hexCount * 24);
                triangles.Add(0 + 24 * hexCount);

                triangles.Add(faceCount);
                triangles.Add(faceCount + 2);
                triangles.Add(2 + hexCount * 24);
                #endregion

                break;
            }
            #region Üst Yüzey
            triangles.Add(faceCount);
            triangles.Add(faceCount + 5);
            triangles.Add(faceCount + 1);

            triangles.Add(faceCount);
            triangles.Add(faceCount + 4);
            triangles.Add(faceCount + 5);

            #endregion
            #region Alt Yüzey 
            triangles.Add(faceCount + 2);
            triangles.Add(faceCount + 3);
            triangles.Add(faceCount + 7);

            triangles.Add(faceCount + 2);
            triangles.Add(faceCount + 7);
            triangles.Add(faceCount + 6);

            #endregion
            #region Diş kenar
            triangles.Add(faceCount + 1);
            triangles.Add(faceCount + 5);
            triangles.Add(faceCount + 7);

            triangles.Add(faceCount + 1);
            triangles.Add(faceCount + 7);
            triangles.Add(faceCount + 3);
            #endregion
            #region İç kenar
            triangles.Add(faceCount);
            triangles.Add(faceCount + 6);
            triangles.Add(faceCount + 4);

            triangles.Add(faceCount);
            triangles.Add(faceCount + 2);
            triangles.Add(faceCount + 6);
            #endregion

        }
    }

    private void FindPoints(float innerRadius, float outerRadius, int index, float height, Vector3 center)
    {
        vertices.Add(GetPoint(innerRadius, index, height, center));
        vertices.Add(GetPoint(outerRadius, index, height, center));
    }

    private Vector3 GetPoint(float size, int index, float height, Vector3 center)
    {
        float angle_deg = 60 * index - 30;
        float angle_rad = Mathf.PI / 180f * angle_deg;
        return new Vector3(center.x + size * Mathf.Cos(angle_rad), center.y + height, center.z + size * Mathf.Sin(angle_rad));
    }
    public void ClearMeshData()
    {
        triangles.Clear();
        vertices.Clear();
    }
    public void GenerateMesh()
    {
        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mFilter.mesh = mesh;
    }
}
