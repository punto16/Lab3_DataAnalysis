using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathParent : MonoBehaviour
{
    public List<GameObject> dotsList;
    public List<GameObject> lines;

    public GameObject spherePathDotReference;

    // Start is called before the first frame update
    void Start()
    {
        dotsList = new List<GameObject>();
    }

    public void AddToDotList(GameObject go)
    {
        if (dotsList.Count > 0)
        {
            GameObject lastDot = dotsList[dotsList.Count - 1];
            float distance = Vector3.Distance(lastDot.transform.position, go.transform.position);

            if (distance <= 5)
            {
                CreateLine(lastDot.transform.position, go.transform.position, lastDot.GetComponent<MeshRenderer>().material);
            }
        }
        dotsList.Add(go);
    }

    public void ClearAll()
    {
        foreach (var dot in dotsList)
        {
            if (dot != null) DestroyImmediate(dot);
        }
        dotsList.Clear();
        foreach (var dot in lines)
        {
            if (dot != null) DestroyImmediate(dot);
        }
        lines.Clear();
    }

    public void CreateSphereOnPathDot(Vector3 pos)
    {
        GameObject newDot = Instantiate(spherePathDotReference);
        newDot.GetComponent<SpherePathDot>().MoveToPathPos(pos);
    }

    private void CreateLine(Vector3 startPos, Vector3 endPos, Material material)
    {
        GameObject lineObject = new GameObject("Line");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = material;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
        lineRenderer.useWorldSpace = true;

        lines.Add(lineObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
