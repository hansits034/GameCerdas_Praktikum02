using UnityEngine;

[RequireComponent(typeof(NPCSensor))]
public class NPCVisionCone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NPCSensor sensor;

    [Header("Mesh Settings")]
    [SerializeField] private int segments = 24;
    [SerializeField] private float groundOffset = 0.05f;

    [Header("Colors")]
    [SerializeField] private Color patrolColor = new Color(0.2f, 1f, 0.3f, 0.28f);
    [SerializeField] private Color alertColor = new Color(1f, 0.15f, 0.15f, 0.4f);

    private MeshRenderer meshRenderer;
    private Material material;

    private void Awake()
    {
        if (sensor == null)
            sensor = GetComponent<NPCSensor>();

        BuildVisionCone();
    }

    private void Update()
    {
        if (material != null)
        {
            material.color = sensor.CanSeePlayer ? alertColor : patrolColor;
        }
    }

    private void BuildVisionCone()
    {
        GameObject coneObject = new GameObject("VisionCone");
        coneObject.transform.SetParent(transform, false);
        coneObject.transform.localPosition = Vector3.up * groundOffset;

        MeshFilter meshFilter = coneObject.AddComponent<MeshFilter>();
        meshRenderer = coneObject.AddComponent<MeshRenderer>();

        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material = material;

        meshFilter.mesh = GenerateFanMesh(sensor.ViewRadius, sensor.ViewAngle, segments);
    }

    private Mesh GenerateFanMesh(float radius, float angle, int segmentCount)
    {
        Mesh mesh = new Mesh { name = "VisionConeMesh" };

        Vector3[] vertices = new Vector3[segmentCount + 2];
        int[] triangles = new int[segmentCount * 3];

        vertices[0] = Vector3.zero;

        float startAngle = -angle / 2f;
        float angleStep = angle / segmentCount;

        for (int i = 0; i <= segmentCount; i++)
        {
            float currentAngle = (startAngle + angleStep * i) * Mathf.Deg2Rad;

            vertices[i + 1] = new Vector3(
                Mathf.Sin(currentAngle) * radius,
                0f,
                Mathf.Cos(currentAngle) * radius
            );
        }

        for (int i = 0; i < segmentCount; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
