using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSelector : MonoBehaviour
{
    public GameObject player;
    public LineRenderer lineRenderer;
    Vector3 currentSelected;
    public float width=0.01f;
    public Color color=Color.red;
    public Vector3 frameBias=new Vector3();
    public Vector3 screenBias=new Vector3();
    // Start is called before the first frame update
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 6;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        currentSelected = new Vector3 (0, 0, 0);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentSelected = player.transform.position;
        currentSelected.x=Mathf.Round(currentSelected.x);
        currentSelected.y=Mathf.Round(currentSelected.y);
        currentSelected.z=Mathf.Round(currentSelected.z);
        Vector3 mouseVector = GetMouseVector();
        if (mouseVector.magnitude > Screen.width / 20)
        {
            float angle=Vector3.Angle(new Vector3(0,1,0), mouseVector.normalized);
            //Debug.Log(angle);
            if (angle < 22.5)
            {
                currentSelected.z += 1;
            }
            else if (angle < 67.5)
            {
                currentSelected.z += 1;
                if(mouseVector.x>0)
                    currentSelected.x += 1;
                else currentSelected.x -= 1;
            }
            else if(angle < 112.5)
            {
                if (mouseVector.x > 0)
                    currentSelected.x += 1;
                else currentSelected.x -= 1;
            }
            else if (angle < 157.5)
            {
                currentSelected.z -= 1;
                if (mouseVector.x > 0)
                    currentSelected.x += 1;
                else currentSelected.x -= 1;
            }
            else
            {
                currentSelected.z -= 1;
            }
        }
        DrawSelectionSquare(currentSelected);
        GetMouseVector();
    }
    public Vector3 GetSelection()
    {
        return currentSelected;
    }
    void DrawSelectionSquare(Vector3 pivot)
    {
        Vector3 pos = pivot+frameBias;
        pos.x -= 0.5f;
        pos.z -= 0.5f;
        lineRenderer.SetPosition(0, pos);
        pos.x += 1;
        lineRenderer.SetPosition(1, pos);
        pos.z += 1;
        lineRenderer.SetPosition(2, pos);
        pos.x -= 1;
        lineRenderer.SetPosition(3, pos);
        pos.z -= 1;
        lineRenderer.SetPosition(4, pos);
        pos.x += 1;
        lineRenderer.SetPosition(5, pos);
    }
    Vector3 GetMouseVector()
    {
        return Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0) + screenBias;
    }
}
