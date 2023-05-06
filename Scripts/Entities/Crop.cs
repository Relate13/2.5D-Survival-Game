using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : Plantable
{
    public SpriteRenderer image0;
    //public SpriteRenderer image1;
    public Sprite[] assets=new Sprite[5];
    // Start is called before the first frame update
    public override void Init()
    {
        image0.sprite = assets[status];
        //image1.sprite = assets[status];
    }
    public override void Grow() {
        ++status;
        image0.sprite = assets[status];
    }
    public override void LoadData()
    {
        //Debug.Log("Parsing" + selfECU.data);
        status = int.Parse(selfECU.data);
        Init();
        //Debug.Log("Corn Load INFO:"+selfECU.data);
    }
    public override void SaveData()
    {
        selfECU.data = status+"";
        //Debug.Log("Corn Save INFO:" + selfECU.data);
    }

    // Update is called once per frame
    public override void Harvest()
    {
        //Debug.Log("harvesting"+gameObject);
        //
        //{
        //    foreach (GameObject trophy in trophies)
        //    {
        //        Vector3 pos = transform.position;
        //        pos.y += 10;
        //        GameObject trophie = Instantiate(trophy, transform.position, Quaternion.identity);
        //        trophie.GetComponent<Rigidbody>().AddForce(new Vector3(0, 100, 0));
        //    }
        //}
        if (status >= Stage - 1)
            GenerateTrophies();
        AudioClip[] sounds = GameDataHolder.getInstance().CropSound;
        SoundManager.GetInstance().PlaySound(sounds[Random.Range(0, sounds.Length)]);
        SelfDestruct();
    }
    public override EntityControlUnit GenerateECU(int mode)
    {
        EntityControlUnit ecu=new EntityControlUnit();
        ecu.entityId = EntityID;
        if (mode == 0)
            ecu.data = "" + Random.Range(0, Stage);
        else if (mode == 1)
            ecu.data = "0";
        return ecu;
    }
}
