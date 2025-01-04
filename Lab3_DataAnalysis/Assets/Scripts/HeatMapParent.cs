using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapParent : MonoBehaviour
{
    public List<GameObject> cubeHeatMapsList;

    public GameObject cubeHeatMapReference;

    // Start is called before the first frame update
    void Start()
    {
        cubeHeatMapsList = new List<GameObject>();
    }

    public void CreateCubeOnHeatMap(Vector3 pos)
    {
        GameObject newDot = Instantiate(cubeHeatMapReference);
        newDot.GetComponent<CubeHeatMap>().MoveToHitPos(pos);

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
