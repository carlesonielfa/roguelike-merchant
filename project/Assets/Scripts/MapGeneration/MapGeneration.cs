using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityAtoms.BaseAtoms;
public class MapGeneration : MonoBehaviour
{
    [SerializeField] int nCities = 16;
    [SerializeField] float minDistanceBetweenCities = 3f;
    [SerializeField] float borderCitySpawnLimit = 0.2f;
    // Width and height of the texture in pixels.
    [SerializeField] int width = 128;
    [SerializeField] int height = 128;

    // The origin of the sampled area in the plane.
    [SerializeField] int seed;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    [SerializeField] float scale = 5f;

    [SerializeField] Color waterColor;
    [SerializeField] Color earthColor;
    [SerializeField] float waterLevel = 0.8f;
    private float isleRadiusSquared = 0.5f;



    private Texture2D texture;
    private Color[] pixelsColor;
    private float[] heightMap;
    private Renderer rend;

    private List<Vector2> cityLocations;
    private List<CityComponent> cities = new List<CityComponent>();
    [SerializeField] GameObjectVariable currentCity;
    [SerializeField] GameObject linePrefab;
    void Start()
    {
        if (seed == 0)
        {
            seed = Random.Range(0,10000);
            Random.InitState(seed);
        }

        rend = GetComponent<Renderer>();

        cityLocations = new List<Vector2>();
        // Set up the texture and a Color array to hold pixels during processing.
        texture = new Texture2D(width, height);
        pixelsColor = new Color[texture.width * texture.height];
        heightMap = new float[texture.width * texture.height];
        rend.material.mainTexture = texture;

        DrawMap();
        //Set active city a random city
        currentCity.SetValue(cities[Random.Range(0, cities.Count)].gameObject);
    }
    void DrawMap()
    {
        ComputeMap();
        // Copy the pixel data to the texture and load it into the GPU.
        pixelsColor = heightMap.Select(i => SetColor(i)).ToArray();
        texture.SetPixels(pixelsColor);
        texture.Apply();
    }
    void InstantiateCities()
    {
        foreach (Vector2 vector in cityLocations)
        {
            GameObject g = GameData.Instance.GetRandomCity();
            Debug.Log(g.GetComponent<CityComponent>().city.name);
            g.transform.SetParent(transform.parent);
            g.transform.position = vector;
            g.GetComponent<RectTransform>().sizeDelta = new Vector2(0.75f, 0.75f);
            cities.Add(g.GetComponent<CityComponent>());
        }

    }
    void ComputeMap()
    {
        PlaceCities();
        InstantiateCities();
        GeneratePaths();
        
        for(float y = 0f; y < texture.height; y++)
        {
            for (float x = 0f;  x < texture.width; x++)
            {
                float xNorm = x / texture.width * scale;
                float yNorm = y / texture.height * scale;
                float h = 0;
                foreach(CityComponent city in cities)
                {
                    //TODO no need to check all cities this could be more efficient
                    float distance = MinDistanceToCity(RelativeCoordsToWorld(new Vector2(xNorm, yNorm) / scale));
                    if (distance < isleRadiusSquared){
                        h += (isleRadiusSquared-distance)/isleRadiusSquared * Mathf.PerlinNoise(seed + xNorm, seed + yNorm);
                    }
                }
                heightMap[(int)y * texture.width + (int)x] = h;
            }
        }


    }

    private void GeneratePaths()
    {
        foreach (CityComponent city in cities)
        {
            if (city.connectedCities.Count < 2)
            {
                IEnumerable orderedCities =  cities.Except(city.connectedCities).
                    OrderBy(otherCity => (city.GetPosition() - otherCity.GetPosition()).sqrMagnitude).
                    Skip(1).
                    Take(2 - city.connectedCities.Count);

                foreach (CityComponent otherCity in orderedCities)
                {
                    city.connectedCities.Add(otherCity);
                    otherCity.connectedCities.Add(city);
                    AddLine(city.GetPosition(), otherCity.GetPosition());
                }
            }
        }
    }

    private void AddLine(Vector2 cityPos1, Vector2 cityPos2)
    {
        float margin = 0.85f;
        Vector2 direction = (cityPos2 - cityPos1).normalized;
        Vector2 start = cityPos1 +direction * margin;
        Vector2 end = cityPos2 - direction * margin;
        GameObject laneInstance = Instantiate(linePrefab, transform);
        LineRenderer lineRenderer = laneInstance.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(new Vector3[]{new Vector3(start.x, start.y, 0), new Vector3(end.x, end.y, 0) });
    }

    private void PlaceCities()
    {
        Vector2 pos = Vector2.zero;
        for (int i = 0; i < nCities; i++)
        {
            float minDistance = 0;
            int retries = 10;
            while (minDistance < minDistanceBetweenCities)
            {
                pos = new Vector2(Random.Range(borderCitySpawnLimit, 1 - borderCitySpawnLimit),
                Random.Range(borderCitySpawnLimit, 1 - borderCitySpawnLimit));
                pos = RelativeCoordsToWorld(pos);
                minDistance = MinDistanceToCity(pos);
                retries--;
                if (retries == 0)
                {
                    Debug.LogWarning("Couldn't spawn city respecting min distance");
                    break;
                }
            }
            cityLocations.Add(pos);
        }
    }
    float MinDistanceToCity(Vector2 pos)
    {
        float minDistance = float.MaxValue;
        foreach (Vector2 city in cityLocations)
        {
            float distance = (pos - city).sqrMagnitude;
            if (distance < minDistance)
                minDistance = distance;
        }
        return minDistance;
    }

    Color SetColor(float h)
    {
        if (h > waterLevel)
        {
            return earthColor;
        }
        else
        {
            return waterColor;
        }
    }
    void OnDrawGizmos()
    {
        Vector2 bottomLeftBorderCorner = RelativeCoordsToWorld(new Vector2(borderCitySpawnLimit, borderCitySpawnLimit));
        Vector2 topLeftBorderCorner = RelativeCoordsToWorld(new Vector2(borderCitySpawnLimit, 1 - borderCitySpawnLimit));
        Vector2 topRightBorderCorner = RelativeCoordsToWorld(new Vector2(1 - borderCitySpawnLimit, 1 - borderCitySpawnLimit));
        Vector2 bottomRightBorderCorner = RelativeCoordsToWorld(new Vector2(1 - borderCitySpawnLimit, borderCitySpawnLimit));

        Gizmos.DrawLine(bottomLeftBorderCorner, bottomRightBorderCorner);
        Gizmos.DrawLine(bottomLeftBorderCorner, topLeftBorderCorner);
        Gizmos.DrawLine(topLeftBorderCorner, topRightBorderCorner);
        Gizmos.DrawLine(bottomRightBorderCorner, topRightBorderCorner);

        if (cities != null)
        {
            foreach (CityComponent city in cities)
            {
                Gizmos.DrawSphere(city.GetPosition(), 0.15f);
                foreach (CityComponent otherCity in city.connectedCities)
                {
                    Gizmos.DrawLine(city.GetPosition(), otherCity.GetPosition());
                }
            }
        }

    }
    Vector2 RelativeCoordsToWorld(Vector2 pos)
    {
        return (pos * transform.localScale) - new Vector2(transform.localScale.x, transform.localScale.y) / 2f;
    }

}
