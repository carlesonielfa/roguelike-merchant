using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{

    // Width and height of the texture in pixels.
    [SerializeField] int width = 128;
    [SerializeField] int height = 128;

    // The origin of the sampled area in the plane.
    [SerializeField] float seed;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    [SerializeField] float scale = 5f;

    [SerializeField] Color waterColor;
    [SerializeField] Color earthColor;
    [SerializeField] float waterLevel = 0.6f;
    
    //Cities
    [SerializeField] float distanceBetweenCities = 3f;
    [SerializeField] float additionalHeightForCity = 0.1f;
    [SerializeField] float borderCitySpawnLimit = 0.2f;
    private Texture2D texture;
    private Color[] pixels;
    private Renderer rend;

    private List<Vector2> cities;

    [SerializeField] List<GameObject> citiesPrefabs;
    void Start()
    {
        if (seed == 0)
            seed = Random.Range(0,10000);
        rend = GetComponent<Renderer>();

        cities = new List<Vector2>();
        // Set up the texture and a Color array to hold pixels during processing.
        texture = new Texture2D(width, height);
        pixels = new Color[texture.width * texture.height];
        rend.material.mainTexture = texture;

        DrawMap();
        InstantiateCities();
    }

    void InstantiateCities()
    {
        foreach (Vector2 vector in cities)
        {
            Instantiate(citiesPrefabs[0], vector, Quaternion.identity,transform.parent);
        }
    }

    void DrawMap()
    {
        ComputeMap();
        // Copy the pixel data to the texture and load it into the GPU.
        texture.SetPixels(pixels);
        texture.Apply();
    }
    void ComputeMap()
    {
        for(float y = 0f; y < texture.height; y++)
        {
            for (float x = 0f;  x < texture.width; x++)
            {
                float xNorm = x / texture.width * scale;
                float yNorm = y / texture.height * scale;
                float h = Mathf.PerlinNoise(seed + xNorm, seed+ yNorm);
                TryToAddCity(x, y, h);
                pixels[(int)y * texture.width + (int)x] = SetColor(h);
            }
        }


    }

    void TryToAddCity(float x, float y, float h)
    {
        Vector2 pos = new Vector2(x/texture.width,y/texture.height);
        if (pos.x < borderCitySpawnLimit || pos.x > 1 - borderCitySpawnLimit
           || pos.y < borderCitySpawnLimit || pos.y > 1 - borderCitySpawnLimit)
        {
            return;
        }
        pos = RelativeCoordsToWorld(pos);
        //TODO: Add configurable params
        if (h > waterLevel + additionalHeightForCity && (cities.Count == 0 || MinDistanceToCity(pos) > distanceBetweenCities))
        {
            cities.Add(pos);
        }

    }
    float MinDistanceToCity(Vector2 pos)
    {
        float minDistance = float.MaxValue;
        foreach(Vector2 city in cities)
        {
            float distance = (pos - city).magnitude;
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
            foreach (Vector2 vector in cities)
            {
                Gizmos.DrawSphere(vector, 0.15f);
            }
        }

    }
    Vector2 RelativeCoordsToWorld(Vector2 pos)
    {
        return (pos * transform.localScale) - new Vector2(transform.localScale.x, transform.localScale.y) / 2f;
    }

}
