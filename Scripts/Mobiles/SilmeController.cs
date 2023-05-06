using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Strategy { IDLE, MOVING, FOLLOWING, FREEZE }
[System.Serializable]
public class TrophyInfo
{
    public GameObject Trophy;
    public float DropRate;
    public int DropTime;
}

public class MobileControlUnit
{
    public Mobile mobile;
    public int MobileID;
    public Vector2Int chunkID;
    public Vector3 inChunkPos;
    public string GenerateSaveString()
    {
        return JsonUtility.ToJson(this);
    }
    public static MobileControlUnit CreateMCUFromString(string saveString)
    {
        return JsonUtility.FromJson<MobileControlUnit>(saveString);
    }
}
public class Mobile : MonoBehaviour
{
    public int MobileID;
    public MobileControlUnit selfMCU;
    public int health;
    public bool isConsistent;
    public TrophyInfo[] Trophies;
    public Rigidbody rb;

    public AudioSource audioSource;
    public virtual void DeathEffect()
    {
        return;
    }
    public void Mount(MobileControlUnit mcu) { selfMCU = mcu; selfMCU.mobile = this; /*LoadData();*/ }
    public void unMount() {
        if (!isConsistent)
        {
            SelfDestruct();
        }
        else 
        {
            selfMCU.mobile = null; /*SaveData()*/;
            selfMCU.inChunkPos = TileTerrain.GetInstance().WorldPosTransInChunkPos(transform.position);
            Destroy(gameObject); 
        }
    }
    public void RemoveMCUFromChunk(Vector2Int ChunkID)
    {
        TileTerrain.GetInstance().gameMap.GetChunk(ChunkID).Mobiles.Remove(selfMCU);
    }
    public void AddMCUToChunk(Vector2Int ChunkID)
    {
        TileTerrain.GetInstance().gameMap.GetChunk(ChunkID).Mobiles.Add(selfMCU);
        selfMCU.chunkID = ChunkID;
    }
    public void CrossChunkDetection()
    {
        Vector2Int CurrentChunkID = TileTerrain.GetInstance().WorldPosTransChunkID(transform.position);
        if (CurrentChunkID != selfMCU.chunkID)
        {
            Debug.Log("Cross Border Detected");
            RemoveMCUFromChunk(selfMCU.chunkID);
            AddMCUToChunk(CurrentChunkID);
            if (!TileTerrain.GetInstance().IsChunkActive(CurrentChunkID))
            {
                unMount();
            }
        }
        if (transform.position.y < -2)
        {
            Vector3 newPos = transform.position;
            newPos.y = 0;
            transform.position = newPos;
        }
    }
    public void StartDetection()
    {
        InvokeRepeating("CrossChunkDetection", 0.5f, 0.5f);
    }
    public virtual MobileControlUnit GenerateMCU(int mode)
    {
        MobileControlUnit mcu = new MobileControlUnit();
        mcu.MobileID = MobileID;
        return mcu;
    }
    public void SelfDestruct()
    {
        RemoveMCUFromChunk(selfMCU.chunkID);
        Destroy(gameObject);
    }
    public virtual void Harm(int damage)
    {
        health -= damage;
        if(health <= 0)
            Die();
        HarmEffect();
    }
    public virtual void HarmEffect()
    {
        return;
    }
    public void GenerateTrophy(TrophyInfo t)
    {
        for (int i = 0; i < t.DropTime; ++i)
        {
            if (Random.Range(0f, 1f) < t.DropRate)
            {
                Vector3 TrophyBias = Random.insideUnitSphere;
                TrophyBias.y = 0;
                Instantiate(t.Trophy, transform.position + TrophyBias, Quaternion.identity);
            }
        }
    }
    public void GenerateTrophies()
    {
        foreach (TrophyInfo t in Trophies)
        {
            GenerateTrophy(t);
        }
    }
    public void Die()
    {
        DeathEffect();
        GenerateTrophies();
        SelfDestruct();
    }
}
public class SilmeController : Mobile
{
    public float maxForce;
    public float minForce;
    public GameObject followingObject;
    public float DiscoverRange;
    public float ForgiveRange;
    public Vector2Int ATK;
    bool following = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        following = false;
        InvokeRepeating("Think", 0.2f, 0.2f);
        StartDetection();
    }
    // Update is called once per frame
    public void Think()
    {
        if ((GameDataHolder.getInstance().player.transform.position - transform.position).magnitude < DiscoverRange)
        {
            following = true;
            followingObject = GameDataHolder.getInstance().player.gameObject;
        }
        if (rb.velocity.magnitude <= 0)
            Jump();
    }
    public override void HarmEffect()
    {
        AudioClip[] sounds = GameDataHolder.getInstance().SlimeHit;
        audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
    }
    void Jump()
    {
        float force = Random.Range(maxForce, minForce);
        Vector3 forceVector = Vector3.up * force;
        Vector3 bias;
        if (following)
        { 
            bias = followingObject.transform.position - transform.position; 
        }
        else
        {
            bias = new Vector3(Random.Range(-5f,5f),0,Random.Range(-5f,5f));
        }
        bias.Normalize();
        forceVector += bias * force;
        rb.AddForce(forceVector);
        AudioClip[] sounds = GameDataHolder.getInstance().SlimeJump;
        audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
        if (following)
        {
            if (Vector3.Distance(followingObject.transform.position, transform.position) > ForgiveRange)
                following = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out Player player))
        {
            player.PlayerHurt(Random.Range(ATK.x,ATK.y));
        }
    }
    public override void DeathEffect()
    {
        AudioClip[] sounds = GameDataHolder.getInstance().SlimeDeath;
        SoundManager.GetInstance().PlaySound(sounds[Random.Range(0, sounds.Length)]);
    }
}
