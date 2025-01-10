using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapParent : MonoBehaviour
{
    public List<GameObject> cubeHeatMapsList;
    public SendToServer sendToServer;
    public GameObject cubeHeatMapReference;

    [HideInInspector]
    public int cubeSize = 5; //2 to 15

    [HideInInspector]
    public float transparency = 0.8f; //0 to 1

    public void OnEditParameters()
    {
        Vector3 newScale = new Vector3(cubeSize, cubeSize, cubeSize);
        cubeHeatMapReference.GetComponent<Transform>().localScale = newScale;
        cubeHeatMapReference.GetComponent<CubeHeatMap>().CheckCollisionWithOtherCubes();

        for (int index = 0; index < colorRamp.Count; index++)
        {
            Color color = colorRamp[index];
            color.a = transparency;
            colorRamp[index] = color;
        }

        foreach (var cubeHeatMap in cubeHeatMapsList)
        {
            cubeHeatMap.GetComponent<Transform>().localScale = newScale;
        }
        foreach (var cubeHeatMap in cubeHeatMapsList)
        {
            cubeHeatMap.GetComponent<CubeHeatMap>().CheckCollisionWithOtherCubes();
        }
    }

    public List<Color> colorRamp;

    private void OnValidate()
    {
        if (colorRamp == null || colorRamp.Count < 2)
        {
            colorRamp = new List<Color>
            {
                Color.blue,
                Color.red
            };
        }
    }

    public void ClearAll()
    {
        foreach (var go in cubeHeatMapsList)
        {
            if (go != null) DestroyImmediate(go);
        }
        cubeHeatMapsList.Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        cubeHeatMapsList = new List<GameObject>();
    }

    public void CreateCubeOnHeatMap(Vector3 pos)
    {
        GameObject newDot = Instantiate(cubeHeatMapReference);
        newDot.GetComponent<CubeHeatMap>().MoveToHitPos(pos);

        CheckCubesCollisions();
    }

    public void CheckCubesCollisions()
    {
        foreach (var cubeHeatMap in cubeHeatMapsList)
        {
            cubeHeatMap.GetComponent<CubeHeatMap>().CheckCollisionWithOtherCubes();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
