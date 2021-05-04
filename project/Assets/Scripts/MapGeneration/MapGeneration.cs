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
    //[SerializeField] int width = 128;
    //[SerializeField] int height = 128;

    // The origin of the sampled area in the plane.
    [SerializeField] int seed;


    private List<Vector2> cityLocations;
    private List<CityComponent> cities = new List<CityComponent>();
    [SerializeField] GameObjectVariable currentCity;
    [SerializeField] GameObject linePrefab;
    void Start()
    {
        if (seed == 0)
        {
            seed = Random.Range(0,10000);
        }
        Random.InitState(seed);

        cityLocations = new List<Vector2>();

        ComputeMap();
        //Set active city a random city
        currentCity.SetValue(cities[Random.Range(0, cities.Count)].gameObject);
    }
    void InstantiateCities()
    {
        foreach (Vector2 vector in cityLocations)
        {
            GameObject g = GameData.Instance.GetRandomCity();
            g.transform.SetParent(transform);
            Debug.Log(g.GetComponent<CityComponent>().city.name);
            g.transform.position = vector;
            cities.Add(g.GetComponent<CityComponent>());
        }

    }
    void ComputeMap()
    {
        PlaceCities();
        InstantiateCities();
        GeneratePaths();
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
