using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePathDot : MonoBehaviour
{
    public PathParent pathParent;
    private Material mat;


    // Start is called before the first frame update
    void Start()
    {
        mat = gameObject.GetComponent<MeshRenderer>().material;
        mat.color = new Color(0.6836615f, 1.0f, 0.0f, 0.8705882f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveToPathPos(Vector3 pos)
    {
        gameObject.transform.position = pos;
        pathParent.AddToDotList(gameObject);
    }
}
