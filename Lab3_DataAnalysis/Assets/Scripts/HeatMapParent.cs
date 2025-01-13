using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapParent : MonoBehaviour
{
    public List<GameObject> cubeHeatMapsList;
    public List<GameObject> cubeKillMapsList;
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
        foreach (var cubeHeatMap in cubeKillMapsList)
        {
            cubeHeatMap.GetComponent<Transform>().localScale = newScale;
        }
        foreach (var cubeHeatMap in cubeHeatMapsList)
        {
            cubeHeatMap.GetComponent<CubeHeatMap>().CheckCollisionWithOtherCubes();
        }
        foreach (var cubeHeatMap in cubeKillMapsList)
        {
            cubeHeatMap.GetComponent<CubeHeatMap>().CheckCollisionWithOtherCubes(true);
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

    public void ClearAllKillMap()
    {
        foreach (var go in cubeKillMapsList)
        {
            if (go != null) DestroyImmediate(go);
        }
        cubeKillMapsList.Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        cubeHeatMapsList = new List<GameObject>();
        cubeKillMapsList = new List<GameObject>();
    }

    public void CreateCubeOnHeatMap(Vector3 pos, bool kill = false)
    {
        GameObject newDot = Instantiate(cubeHeatMapReference);
        newDot.GetComponent<CubeHeatMap>().MoveToHitPos(pos, kill);

        CheckCubesCollisions(kill);
    }

    public void CheckCubesCollisions(bool kill = false)
    {
        foreach (var cubeHeatMap in (kill ? cubeKillMapsList : cubeHeatMapsList))
        {
            cubeHeatMap.GetComponent<CubeHeatMap>().CheckCollisionWithOtherCubes(kill);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
