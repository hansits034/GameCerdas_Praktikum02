using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    [Header("Icon Settings")]
    [SerializeField] private Color iconColor = Color.white;
    [SerializeField] private float iconSize = 1.2f;
    [SerializeField] private float heightOffset = 3f;
    [SerializeField] private int segments = 16;

    private void Awake()
    {
        BuildIcon();
    }

    private void BuildIcon()
    {
        GameObject icon = new GameObject("MinimapIcon");
        icon.transform.SetParent(transform, false);
        icon.transform.localPosition = Vector3.up * heightOffset;

        int minimapLayer = LayerMask.NameToLayer("MinimapIcon");

        if (minimapLayer == -1)
        {
            Debug.LogWarning("MinimapIcon: no layer named 'MinimapIcon' found. Create one in Project Settings > Tags and Layers, then exclude it from the Main Camera's Culling Mask so icons stop showing in the main view.");
        }
        else
        {
            icon.layer = minimapLayer;
        }

        MeshFilter meshFilter = icon.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = icon.AddComponent<MeshRenderer>();

        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        Material material = new Material(Shader.Find("Sprites/Default"));
        material.color = iconColor;
        meshRenderer.material = material;

        meshFilter.mesh = GenerateDiscMesh(iconSize, segments);
    }

    private Mesh GenerateDiscMesh(float radius, int segmentCount)
    {
        Mesh mesh = new Mesh { name = "MinimapIconMesh" };

        Vector3[] vertices = new Vector3[segmentCount + 1];
        int[] triangles = new int[segmentCount * 3];

        vertices[0] = Vector3.zero;

        float angleStep = 360f / segmentCount;

        for (int i = 0; i < segmentCount; i++)
        {
            float rad = (angleStep * i) * Mathf.Deg2Rad;

            vertices[i + 1] = new Vector3(
                Mathf.Sin(rad) * radius,
                0f,
                Mathf.Cos(rad) * radius
            );
        }

        for (int i = 0; i < segmentCount; i++)
        {
            int current = i + 1;
            int next = (i + 1) % segmentCount + 1;

            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = current;
            triangles[i * 3 + 2] = next;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
