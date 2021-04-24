using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GK;
public class MapGenerationVoronoi : MonoBehaviour
{
    // Start is called before the first frame update
    VoronoiDiagram diagram;
    [SerializeField] int nCities = 7;
    [SerializeField] float minDistanceBetweenCities = 0.1f;
    // Thickness of the border surrounding the map were we don't want cities
    [SerializeField] float spawnLimits = 0.2f;
    [SerializeField] int width = 128;
    [SerializeField] int height = 128;
    void Start()
    {
        VoronoiCalculator calc = new VoronoiCalculator();
        List<Vector2> sites = new List<Vector2>(nCities);

        for (int i = 0; i < nCities; i++)
        {
            sites.Add(GenerateCityPosition(sites));
        }

        diagram = calc.CalculateDiagram(sites);
        DrawMap();
        InstantiateCities();
    }
    private void DrawMap()
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixelsColor = new Color[texture.width * texture.height];
        VoronoiClipper clip = new VoronoiClipper();
        Renderer rend = GetComponent<Renderer>();
        List<Vector2> polygon = new List<Vector2> { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(1, 1) };
        rend.material.mainTexture = texture;

        List<Vector2> clipped = new List<Vector2>();
        for (int i = 0; i < diagram.Sites.Count; i++)
        {
            int x = Mathf.FloorToInt((diagram.Sites[i].x + 0.5f) * width);
            int y = Mathf.FloorToInt((diagram.Sites[i].y + 0.5f) * height);
            clip.ClipSite(diagram, polygon, i, ref clipped);
        }

        //TODO: Move to multithreaded function
        for (int x = 0; x< width; x++)
        {
            for(int y = 0; y<height; y++)
            {
                pixelsColor[x + y * width] = Color.cyan;
            }
        }
        texture.SetPixels(pixelsColor);
        texture.Apply();
    }
    private void InstantiateCities()
    {

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        if (diagram != null)
        {
            foreach (Vector2 site in diagram.Sites)
            {
                Gizmos.DrawSphere(transform.TransformPoint(site), 0.25f);
            }
            List<int> triangles = diagram.Triangulation.Triangles;
            List<Vector2> triangulationVerts = diagram.Triangulation.Vertices;
            for (int i = 0; i < triangles.Count - 3; i += 3)
            {
                Gizmos.DrawLine(GetWorldVertice(diagram.Triangulation.Vertices, triangles[i]),
                    GetWorldVertice(diagram.Triangulation.Vertices, triangles[i + 1]));
                Gizmos.DrawLine(GetWorldVertice(diagram.Triangulation.Vertices, triangles[i]),
                    GetWorldVertice(diagram.Triangulation.Vertices, triangles[i + 2]));
                Gizmos.DrawLine(GetWorldVertice(diagram.Triangulation.Vertices, triangles[i + 1]),
                    GetWorldVertice(diagram.Triangulation.Vertices, triangles[i + 2]));
            }

            foreach (VoronoiDiagram.Edge edge in diagram.Edges)
            {
                Vector2 start = diagram.Vertices[edge.Vert0];
                Vector2 end;
                if (edge.Type == VoronoiDiagram.EdgeType.Segment)
                {
                    end = diagram.Vertices[edge.Vert1];
                }
                else
                {
                    end = diagram.Vertices[edge.Vert0] + edge.Direction * 2f;
                }
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.TransformPoint(start), transform.TransformPoint(end));
            }
        }
    }
    private Vector2 GetWorldVertice(List<Vector2> vertices, int vertexIndex)
    {
        return transform.TransformPoint(vertices[vertexIndex]);
    }
    private Vector2 GenerateCityPosition(List<Vector2> positions)
    {
        Vector2 pos = Vector2.zero;
        float minDistance = 0;
        int retries = 10;
        while (minDistance < minDistanceBetweenCities)
        {
            pos = new Vector2(Random.Range(spawnLimits, 1 - spawnLimits) - 0.5f,
                Random.Range(spawnLimits, 1 - spawnLimits) - 0.5f);
            minDistance = MinDistanceToCity(pos, positions);
            retries--;
            if (retries == 0)
            {
                Debug.LogWarning("Couldn't spawn city respecting min distance");
                break;
            }
        }
        return pos;
    }
    float MinDistanceToCity(Vector2 pos, List<Vector2> positions)
    {
        float minDistance = float.MaxValue;
        foreach (Vector2 other in positions)
        {
            float distance = (pos - other).sqrMagnitude;
            if (distance < minDistance)
                minDistance = distance;
        }
        return minDistance;
    }

    /*
    public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }
    */
}
