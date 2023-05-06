using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : Decorative
{
    public GameObject UpModel;
    public GameObject DownModel;
    public GameObject LeftModel;
    public GameObject RightModel;
    public Fence[] Neighbors=new Fence[4];
    public class FenceData
    {
        public bool up;
        public bool down;
        public bool left;
        public bool right;
        public FenceData(bool up, bool down, bool left, bool right)
        {
            this.up = up;
            this.down = down;
            this.left = left;
            this.right = right;
        }
        public bool isEqual(FenceData Other)
        {
            return up == Other.up && down == Other.down && left == Other.left && right == Other.right;
        }
    }
    public FenceData fenceData;
    public override EntityControlUnit GenerateECU(int mode)
    {
        EntityControlUnit ecu = new EntityControlUnit();
        ecu.entityId = EntityID;
        ecu.data = "";
        return ecu;
    }

    public override void Harvest()
    {
        GenerateTrophies();
        SelfDestruct();
    }

    public override void LoadData()
    {
        if (selfECU.data != "") 
        {
            fenceData = JsonUtility.FromJson<FenceData>(selfECU.data);
            SetModels(fenceData);
        }
        else
            AnalyseEnvironment();
    }

    public override void SaveData()
    {
        selfECU.data = JsonUtility.ToJson(fenceData);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            AnalyseEnvironment();
        }
    }
    public void AnalyseEnvironment()
    {
        Debug.Log("Analysing environment");
        bool up = false, down = false, left = false, right = false;
        float maxDistance = 1f;
        Vector3 rayStart = transform.position + new Vector3(0, -0.5f, 0);
        //up
        Vector3 BlockUp = rayStart + new Vector3(0, 0, 1);
        Vector3 rayDir = transform.up;
        Vector3 rayEnd = BlockUp + rayDir * maxDistance;
        Ray ray = new Ray(BlockUp, rayDir);
        Debug.DrawLine(rayStart, rayEnd, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            print(hit.point);
            print(hit.transform.position);
            print(hit.collider.gameObject);
            if (hit.collider.gameObject.TryGetComponent(out Fence interactableEntitie))
            {
                Neighbors[0] = interactableEntitie;
                up = true;
            }
        }
        //down
        Vector3 BlockDown = rayStart + new Vector3(0, 0, -1);
        ray = new Ray(BlockDown, rayDir);
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            //print(hit.point);
            //print(hit.transform.position);
            //print(hit.collider.gameObject);
            if (hit.collider.gameObject.TryGetComponent(out Fence interactableEntitie))
            {
                Neighbors[1] = interactableEntitie;
                down = true;
            }
        }
        //left
        Vector3 BlockLeft = rayStart + new Vector3(-1, 0, 0);
        ray = new Ray(BlockLeft, rayDir);
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            //print(hit.point);
            //print(hit.transform.position);
            //print(hit.collider.gameObject);
            if (hit.collider.gameObject.TryGetComponent(out Fence interactableEntitie))
            {
                Neighbors[2] = interactableEntitie;
                left = true;
            }
        }
        //right
        Vector3 BlockRight = rayStart + new Vector3(1, 0, 0);
        ray = new Ray(BlockRight, rayDir);
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            //print(hit.point);
            //print(hit.transform.position);
            //print(hit.collider.gameObject);
            if (hit.collider.gameObject.TryGetComponent(out Fence interactableEntitie))
            {
                Neighbors[3] = interactableEntitie;
                right = true;
            }
        }
        FenceData newFenceData = new FenceData(up, down, left, right);
        if (fenceData == null)
        {
            Debug.Log("Fence data is null");
            fenceData = newFenceData;
            foreach (Fence neighbor in Neighbors)
            {
                if(neighbor != null)
                    neighbor.AnalyseEnvironment();
            }
        }
        else
        {
            Debug.Log("Fence data isn't null");
            if (!newFenceData.isEqual(fenceData))
            {
                fenceData = newFenceData;
                //notify neighbors
                foreach (Fence neighbor in Neighbors)
                {
                    if (neighbor != null)
                        neighbor.AnalyseEnvironment();
                }
            }
        }
        SetModels(fenceData);
    }
    public void SetModels(FenceData data)
    {
        UpModel.SetActive(data.up);
        DownModel.SetActive(data.down);
        LeftModel.SetActive(data.left);
        RightModel.SetActive(data.right);
    }
}
