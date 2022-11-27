using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FieldOfView : MonoBehaviour
{
    // Layers to check for.
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask playerMask;
    // Mesh size variables.
    [SerializeField] public float fov;
    [SerializeField] public float baseAngle;
    [SerializeField] public int rayCount;
    [SerializeField] public float viewDistance;
    // The mesh itself.
    private Mesh mesh;
    private void Start()
    {
        mesh = new Mesh();
        mesh = GetComponent<MeshFilter>().mesh;
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }
    private void Update()
    {
        float angle = baseAngle;
        // Have the rays originate from 0, 0.
        Vector3 origin = Vector3.zero;
        float angleIncrease = fov / rayCount;
        // Needed arrays.
        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];
        // Have the rays originate from 0, 0.
        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            // Radian calculation.
            float angleRad = angle * (Mathf.PI / 180f);
            Vector3 vertex;
            // Cast the ray.
            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)), viewDistance, layerMask);
            // If nothing hit, cast ray to maximum distance.
            if (hit.collider == null)
            {
                vertex = origin + (new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * viewDistance);
            }
            // If something hit, cast ray to point of impact.
            else
            {
                vertex = hit.point;
                vertex -= transform.position;
            }
            vertices[vertexIndex] = vertex;
            // Set the mesh vertices.
            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }
            // Incramentals.
            vertexIndex += 1;
            angle -= angleIncrease;

            // If the player enters within the area the mesh was drawn, kill the player and reload the same scene.
            RaycastHit2D playerHit = Physics2D.Raycast(transform.position, new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)), hit.distance, playerMask);
            if (playerHit.collider != null)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        // Draw the mesh.
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}