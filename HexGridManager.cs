using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class HexGridManager : MonoBehaviour
{
    [SerializeField] private int hexCircleAmount;

    [Range(0, 100)]
    [SerializeField] private float innerSize;

    [Range(0, 100)]
    [SerializeField] private float outerSize;
    [SerializeField] private float height;
    [SerializeField] private Vector3 worldPos;
    private Dictionary<GameObject, HexCoord> hexGrids = new Dictionary<GameObject, HexCoord>();
    private HexRenderer hexRenderer;
    void Awake()
    {
        hexRenderer = FindAnyObjectByType<HexRenderer>();
    }
    void Start()
    {
        HexGeneration();
    }

    private void HexGeneration()
    {
        if (hexRenderer == null)
            return;
        hexRenderer.ClearMeshData();
        ClearDict();
        for (int q = -hexCircleAmount; q <= hexCircleAmount; q++)
        {
            for (int r = -hexCircleAmount; r <= hexCircleAmount; r++)
            {
                if (Mathf.Abs(q + r) <= hexCircleAmount)
                {
                    HexCoord currentHex = new HexCoord(q, r);
                    GameObject name = new GameObject();
                    hexGrids.Add(name, currentHex);
                }
            }
        }
        int i = 0;
        foreach (var a in hexGrids)
        {
            hexRenderer.SetVerticesAndTriangles(a.Value.GetWorldPosition(outerSize), i, outerSize, innerSize, height);
            i++;
        }
        hexRenderer.GenerateMesh();
    }
    private void ClearDict()
    {
        foreach (var a in hexGrids)
        {
            Destroy(a.Key);
        }
        hexGrids.Clear();
    }


    [ContextMenu("Test")]
    public void TestWorldPosition()
    {
        HexCoord result = HexCoord.FromWorldPositionToHex(worldPos);
        Debug.Log($"HexCoard[{result.q},{result.r}]");
    }

    void OnDrawGizmos()
    {
        if (hexGrids == null) return;

        foreach (var a in hexGrids)
        {
            Gizmos.DrawSphere(a.Value.GetWorldPosition(outerSize), 0.2f);
        }
        Gizmos.DrawSphere(worldPos, 0.2f);
    }
    private void OnValidate()
    {
        if (outerSize < innerSize)
            outerSize = innerSize;
       
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null)
                HexGeneration();
        };
#endif
    }
}
[Serializable]
public struct HexCoord
{
    public int q;
    public int r;
    private static readonly HexCoord[] directions =
    {
        new HexCoord(1,0),
        new HexCoord(-1,0),
        new HexCoord(0,1),
        new HexCoord(0,-1),
        new HexCoord(1,-1),
        new HexCoord(-1,1)
    };
    public HexCoord(int q, int r)
    {
        this.q = q;
        this.r = r;
    }
    public Vector3 GetWorldPosition(float size)
    {
        float x = Mathf.Sqrt(3) * q * size + Mathf.Sqrt(3) * size / 2f * r;
        float z = 3f * size / 2f * r;
        return new Vector3(x, 0, z);
    }
    public HexCoord GetNeighbor(int index)
    {
        return new HexCoord(q + directions[index].q, r + directions[index].r);
    }

    public static HexCoord FromWorldPositionToHex(Vector3 worldPos)
    {
        float r = worldPos.z / 1.5f;
        float q = (worldPos.x - Mathf.Sqrt(3) / 2f * r) / Mathf.Sqrt(3);
        float s = -q - r;

        int roundQ = Mathf.RoundToInt(q);
        int roundR = Mathf.RoundToInt(r);
        int roundS = Mathf.RoundToInt(s);

        float diffQ = Mathf.Abs(roundQ - q);
        float diffR = Mathf.Abs(roundR - r);
        float diffS = Mathf.Abs(roundS - s);

        if (diffQ > diffR && diffQ > diffS)
            roundQ = -roundR - roundS;
        else if (diffR > diffS)
            roundR = -roundQ - roundS;

        return new HexCoord(roundQ, roundR);
    }

}