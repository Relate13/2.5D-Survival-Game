using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityControlUnit
{
    public Entity entity;
    public int entityId;
    public Vector2Int chunkID;
    public Vector3 inChunkPos;
    public string data;
    public string GenerateSaveString()
    {
        //Debug.Log(entity.name);
        if(entity!=null)
            entity.SaveData();
        return JsonUtility.ToJson(this);
    }
    public EntityControlUnit() { }
    public static EntityControlUnit CreateECUFromString(string saveString)
    {
        return JsonUtility.FromJson<EntityControlUnit>(saveString);
    }
}
public abstract class Entity : MonoBehaviour
{
    public EntityControlUnit selfECU;
    public int EntityID;
    public int[] availableTiles;
    public Vector2Int EntitySize = new Vector2Int(1, 1);
    public Vector2Int EntityPivot = new Vector2Int(0, 0);
    public void unMount() {selfECU.entity=null; SaveData();Destroy(gameObject); }
    public void Mount(EntityControlUnit ecu) { selfECU = ecu; selfECU.entity=this; LoadData(); }
    public abstract void LoadData();
    public abstract void SaveData();
    public abstract EntityControlUnit GenerateECU(int mode);//0 for map generation, 1 for player entity generation
    public List<GameObject> trophies = new List<GameObject>();
    public TrophyInfo[] Trophies;
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
    public void SelfDestruct()
    {
        Debug.Log("Destructing" + gameObject);
        Vector2Int ChunkID = selfECU.chunkID;
        int MapSize = TileTerrain.GetInstance().MapSize;
        Destroy(gameObject);
        TileTerrain.GetInstance().gameMap.GetChunk(ChunkID).Entities.Remove(selfECU);
        MapBlock selfBlock = TileTerrain.GetInstance().gameMap.GetChunk(ChunkID).GetMapBlock(new Vector2Int((int)selfECU.inChunkPos.x, (int)selfECU.inChunkPos.z));
        selfBlock.ECU = null;

        Vector2Int checkingBlock = new Vector2Int((int)selfECU.inChunkPos.x, (int)selfECU.inChunkPos.z) - EntityPivot;
        for (int i = 0; i < EntitySize.x; i++)
        {
            for (int j = 0; j < EntitySize.y; j++)//when the object's radius is larger than chunk width then it's not working. maybe need some enhancements, maybe not
            {
                Vector2Int currentBlockID = checkingBlock + new Vector2Int(i, j);
                Vector2Int currentChunkID = ChunkID;
                //dealing with inter-chunk problems
                if (currentBlockID.x < 0)
                {
                    currentBlockID.x = (currentBlockID.x + Chunk.CHUNK_WIDTH) % Chunk.CHUNK_WIDTH;
                    currentChunkID.x = (currentChunkID.x + MapSize - 1/*the problem below is caused by this number 1*/) % MapSize;
                }
                if (currentBlockID.x >= Chunk.CHUNK_WIDTH)
                {
                    currentBlockID.x = currentBlockID.x % Chunk.CHUNK_WIDTH;
                    currentChunkID.x = (currentChunkID.x + 1/*the problem below is caused by this number 1*/) % MapSize;
                }
                if (currentBlockID.y < 0)
                {
                    currentBlockID.y = (currentBlockID.y + Chunk.CHUNK_WIDTH) % Chunk.CHUNK_WIDTH;
                    currentChunkID.y = (currentChunkID.y + MapSize - 1/*the problem below is caused by this number 1*/) % MapSize;
                }
                if (currentBlockID.y >= Chunk.CHUNK_WIDTH)
                {
                    currentBlockID.y = currentBlockID.y % Chunk.CHUNK_WIDTH;
                    currentChunkID.y = (currentChunkID.y + 1/*the problem below is caused by this number 1*/) % MapSize;
                }
                TileTerrain.GetInstance().gameMap.GetChunk(currentChunkID).GetMapBlock(currentBlockID).occupied = false;
            }
        }
    }
}

public abstract class Harvestable : Entity
{
    public int health = 1;
    public abstract void Harvest();
    public void Harm(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Harvest();
        }
        else
            harmEffect();
    }

    public virtual void harmEffect()
    {
        return;
    }
}

public abstract class Interactable : Decorative
{
    public abstract void Interact(Player player);
}

public abstract class Plantable : Harvestable
{
    public int Stage = 5;
    public int status = 0;
    public int Interval = 20;
    public double Ratio = 0.75;
    public void Start()
    {
        //Init();
        InvokeRepeating("TryGrow", Interval, Interval);
    }
    public void TryGrow()
    {
        if (status < Stage-1)
        {
            if (Random.Range(0f, 1f) <= Ratio)
                Grow();
            if(status >= Stage - 1)
                CancelInvoke("TryGrow");
        }
        else
        {
            CancelInvoke("TryGrow");
        }
    }
    public abstract void Init();
    public abstract void Grow();
}
public abstract class Decorative : Harvestable
{

}