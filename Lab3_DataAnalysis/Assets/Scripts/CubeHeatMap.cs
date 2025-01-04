using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeHeatMap : MonoBehaviour
{
    public HeatMapParent heatMapParent;
    private Material mat;
    public int dangerZone = 1;


    // Start is called before the first frame update
    void Start()
    {
        mat = gameObject.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveToHitPos(Vector3 pos)
    {
        gameObject.transform.position = pos;
        heatMapParent.cubeHeatMapsList.Add(gameObject);
    }

    public void CheckCollisionWithOtherCubes()
    {
        int collisions = 0;
        for (int i = 0; i < heatMapParent.cubeHeatMapsList.Count; i++)
        {
            GameObject otherObject = heatMapParent.cubeHeatMapsList[i];
            if (otherObject == gameObject) continue;

            Collider thisCollider = gameObject.GetComponent<Collider>();
            Collider otherCollider = otherObject.GetComponent<Collider>();

            Vector3 thisSize = (thisCollider != null) ? thisCollider.bounds.size : gameObject.transform.localScale;
            Vector3 otherSize = (otherCollider != null) ? otherCollider.bounds.size : otherObject.transform.localScale;

            if (AreColliding(gameObject.transform.position, otherObject.transform.position, thisSize, otherSize))
            {
                collisions++;
            }
        }
        IncreaseDangerZone(collisions + 1);
    }

    private bool AreColliding(Vector3 pos1, Vector3 pos2, Vector3 size1, Vector3 size2)
    {
        bool xOverlap = (pos1.x + size1.x > pos2.x) && (pos1.x < pos2.x + size2.x);
        bool yOverlap = (pos1.y + size1.y > pos2.y) && (pos1.y < pos2.y + size2.y);
        bool zOverlap = (pos1.z + size1.z > pos2.z) && (pos1.z < pos2.z + size2.z);
        return xOverlap && yOverlap && zOverlap;
    }

    public void IncreaseDangerZone(int i)
    {
        if (i > 5) i = 5;
        dangerZone = i;
        if (mat == null) mat = heatMapParent.cubeHeatMapReference.GetComponent<MeshRenderer>().material;
        switch (dangerZone)
        {
            case 1:
                {
                    mat.color = new Color(0.1391918f, 1.0f, 0.0f, 0.8705882f);
                    break;
                }
            case 2:
                {
                    mat.color = new Color(0.6836615f, 1.0f, 0.0f, 0.8705882f);
                    break;
                }
            case 3:
                {
                    mat.color = new Color(1.0f, 0.8518007f, 0.0f, 0.8705882f);
                    break;
                }
            case 4:
                {
                    mat.color = new Color(1.0f, 0.5203068f, 0.0f, 0.8705882f);
                    break;
                }
            case 5:
                {
                    mat.color = new Color(1.0f, 0.0f, 0.0f, 0.8705882f);
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
}
