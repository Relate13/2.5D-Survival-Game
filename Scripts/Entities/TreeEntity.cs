using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeEntity : Plantable
{
    class TreeData
    {
        public int max;
        public int cur;
        public TreeData(int maxHeight,int current) { max = maxHeight; cur = current; }
    }
    private Vector3 nextGrow;
    private TreeData selfData;
    public GameObject trunk;
    public GameObject leaves;
    private GameObject leavesObject;
    public int minHeight = 4;
    public int maxHeight = 12;
    public override void Grow()
    {
        ++status;
        ++selfData.cur;
        health = status;
        Instantiate(trunk, nextGrow, Quaternion.identity, transform);
        nextGrow.y += 1;
        Vector3 leafPos=leavesObject.transform.position;
        leafPos.y += 1;
        leavesObject.transform.position = leafPos;
    }

    public override void Harvest()
    {
        int timberNum = selfData.cur;
        //Vector3 trophyPos = new Vector3();
        for (int i = 1; i <= timberNum; i++)
        {
            Instantiate(trophies[0], nextGrow+new Vector3(0,-i,0), Quaternion.identity); 
        }
        if (Stage - status <= 1)//if grown up
        {
            int fruitNum = Random.Range(1, 5);
            for (int i = 0; i < fruitNum; ++i)
            {
                Vector3 bias = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
                Instantiate(trophies[1], nextGrow + bias, Quaternion.identity);
            }
        }
        AudioClip[] sounds = GameDataHolder.getInstance().TreeSound;
        SoundManager.GetInstance().PlaySound(sounds[Random.Range(0, sounds.Length)]);
        SelfDestruct();
    }

    public override void harmEffect()
    {
        base.harmEffect();
        AudioClip[] sounds = GameDataHolder.getInstance().TreeSound;
        SoundManager.GetInstance().PlaySound(sounds[Random.Range(0, sounds.Length)]);
    }

    public override void Init()
    {
        Stage = selfData.max;
        status = selfData.cur;
        health = status;
        nextGrow = transform.position;
        for(int i = 0; i < status; i++)
        {
            Instantiate(trunk, nextGrow, Quaternion.identity, transform);
            nextGrow.y += 1;
        }
        nextGrow.z -= 0.01f;
        leavesObject = Instantiate(leaves, nextGrow, Quaternion.identity, transform);
        nextGrow.z += 0.01f;
    }

    public override void LoadData()
    {
        //Debug.Log("Parsing" + selfECU.data);
        selfData = JsonUtility.FromJson<TreeData>(selfECU.data);
        Init();
        //Debug.Log("Tree Load INFO:" + selfECU.data);
    }

    public override void SaveData()
    {
        selfECU.data = JsonUtility.ToJson(selfData);
        //Debug.Log("Tree Save INFO:" + selfECU.data);
    }
    // Start is called before the first frame update

    // Update is called once per frame

    public override EntityControlUnit GenerateECU(int mode)
    {
        EntityControlUnit ecu = new EntityControlUnit();
        int max = Random.Range(minHeight, maxHeight);
        int cur = 0;
        switch (mode) 
        {
            case 0: cur = max; break;
            case 1: cur = 1; break;
            default: cur = max; break;
        }
        TreeData td = new TreeData(max, cur);
        ecu.entityId = EntityID;
        ecu.data = JsonUtility.ToJson(td);
        return ecu;
    }
}
