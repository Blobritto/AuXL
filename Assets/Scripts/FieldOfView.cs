using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] public float fov;
    [SerializeField] public float baseAngle;
    [SerializeField] public int rayCount;
    [SerializeField] public float viewDistance;
    private Mesh mesh;
    
    // Start is called before the first frame update
    private void Start()
    {
        mesh = new Mesh();
        mesh = GetComponent<MeshFilter>().mesh;
    }

    private void Update()
    {
        float angle = baseAngle;
        Vector3 origin = Vector3.zero;
        float angleIncrease = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            float angleRad = angle * (Mathf.PI / 180f);
            Vector3 vertex;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)), viewDistance, layerMask);
            if (hit.collider == null)
            {
                vertex = origin + (new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * viewDistance);
            }
            else
            {
                vertex = hit.point;
                vertex -= transform.position;
            }
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }
            vertexIndex += 1;
            angle -= angleIncrease;

            RaycastHit2D playerHit = Physics2D.Raycast(transform.position, new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)), viewDistance, playerMask);
            if (playerHit.collider != null)
            {
                if (GameObject.FindWithTag("Coin") != null)
                {
                    GameObject.Destroy(GameObject.FindWithTag("Coin").gameObject);
                }
                playerHit.collider.gameObject.transform.position = spawnPoint.transform.position;
            }
        }
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}
