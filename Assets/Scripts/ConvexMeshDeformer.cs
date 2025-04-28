using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class ConvexMeshDeformer : MonoBehaviour
{
    public float bulgeStrength = 0.5f;

    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = Instantiate(mf.mesh); // Faz uma cópia para não alterar o mesh original
        mf.mesh = mesh;

        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 posXZ = new Vector2(vertices[i].x, vertices[i].z);
            float dist = posXZ.magnitude;
            float bulge = (1.0f - dist * dist) * bulgeStrength;
            bulge = Mathf.Max(bulge, 0);
            vertices[i].y += bulge;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Atualiza o MeshCollider também
        MeshCollider mc = GetComponent<MeshCollider>();
        mc.sharedMesh = mesh;
    }
}