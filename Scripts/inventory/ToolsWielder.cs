using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsWielder : MonoBehaviour
{
    public static ToolsWielder instance;
    public static ToolsWielder getInstance() { return instance; }
    public AudioClip[] wieldSound;
    public Vector3 bias;
    public GameObject AttachedTo;
    public ToolsWielder()
    {
        instance = this;
    }
    bool wielding = false;
    Vector3 pos;
    float wieldAngle;
    GameObject toolEntity;
    GameObject WieldingObject;
    float wieldTime;
    public void WieldTool(GameObject toolEntity,float wieldTime,Vector3 pos,float wieldAngle)
    {
        if (!wielding)
        {
            Debug.Log("Wielded");
            this.toolEntity = toolEntity;
            this.wieldTime = wieldTime;
            this.pos = pos;
            this.wieldAngle = wieldAngle;
            SoundManager.GetInstance().PlaySound(wieldSound[Random.Range(0, wieldSound.Length)]);
            StartCoroutine("WieldStart");
        }
    }
    public void Update()
    {
        transform.position=AttachedTo.transform.position;
        if (wielding)
        {
            if (WieldingObject != null)
                WieldingObject.transform.Rotate(new Vector3(0, wieldAngle * (Time.deltaTime/wieldTime), 0));
        }
    }
    float VectorAngle(Vector2 from, Vector2 to)
    {
        float angle;
        Vector3 cross = Vector3.Cross(from, to);
        angle = Vector2.Angle(from, to);
        return cross.z > 0 ? angle : -angle;
    }
    IEnumerator WieldStart()
    {
        Vector3 playerPos = GameDataHolder.getInstance().player.gameObject.transform.position;
        Vector3 line = pos - playerPos;
        float angleBias = VectorAngle(new Vector2(line.x, line.z), new Vector2(Vector3.back.x, Vector3.back.z));
        wielding = true;
        //GameDataHolder.getInstance().player.freezePlayer(wieldTime);
        WieldingObject = Instantiate(toolEntity, playerPos + bias, Quaternion.identity, transform);
        Debug.Log("Angle bias:" + angleBias);
        WieldingObject.transform.Rotate(new Vector3(0, angleBias, 0));
        yield return new WaitForSeconds(wieldTime);
        Destroy(WieldingObject);
        wielding = false;
    }
}
