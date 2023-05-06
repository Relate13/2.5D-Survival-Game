using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

class ChunkContolUnit
{
    public static int ChunkMaxLifeTime = 3;
    public Vector2Int ChunkID;
    public Vector2Int ChunkSrc;
    public GameObject ChunkCollider;
    public int ChunkLifeTime;
    public ChunkContolUnit(Vector2Int ChunkID,Vector2Int ChunkSrc,GameObject collider)
    {
        this.ChunkID = ChunkID;
        this.ChunkSrc = ChunkSrc;
        ChunkCollider = collider;
        ChunkLifeTime = ChunkMaxLifeTime;
    }
}
public class TileTerrain : MonoBehaviour
{
    private static TileTerrain instance;
    public static TileTerrain GetInstance() { return instance; }
    public TileTerrain()
    {
        instance = this;
    }
    public int MapSize;
    public int Seed;
    public Tilemap tilemap;
    public AnimatedTile[] animatedTiles=new AnimatedTile[2];
    public Tile[] tiles = new Tile[4];
    public GameObject[] Entitylist;
    public GameObject[] Mobilelist;
    public GameMap gameMap;
    public GameObject Player;
    public int DrawDistance = 16;
    public GameObject ChunkCollider;
    private Hashtable LoadedChunkList=new Hashtable();
    public int MaxMobilePerChunk = 1;
    public int MinMobileGenerationDistance = 30;
    public int MaxMobileGenerationDistance = 40;

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach (GameObject entity in Entitylist)
        {
            entity.GetComponent<Entity>().EntityID = i;
            ++i;
        }
        i = 0;
        foreach (GameObject mobile in Mobilelist)
        {
            mobile.GetComponent<Mobile>().MobileID = i;
            ++i;
        }
        //Init Inventory, set all observers;
        GameDataHolder.getInstance().player.playerInventory.Initialize();
        //GenerateNewWorld();
        switch (SaveManager.CURRENT_GAME_STARTMODE)
        {
            case GameStartMode.CREATE:
                {
                    GenerateNewWorld();
                    break;
                }
            case GameStartMode.LOAD:
                {
                    LoadWorldFromFolder();
                    break;
                }
        }

        //GameDataHolder.getInstance().player.playerInventory.ObserveSystem.AttachObserver(GameDataHolder.getInstance().inventoryDisplayer);
        //GameDataHolder.getInstance().player.playerInventory.ObserveSystem.AttachObserver(GameDataHolder.getInstance().craftingMenu);
        //GameDataHolder.getInstance().player.playerInventory.ObserveSystem.Notify();
        //UpdateMap();
        InvokeRepeating("RandomGenerateMobile", 30, 30);
        InvokeRepeating("UpdateMapNew", 0, 0.5f);
    }
    public void GenerateNewWorld()
    {
        // Generate New Map From Seed
        GameDataHolder.getInstance().InitNPC();
        GameMapGenerator generator = new GameMapGenerator();
        Seed = SaveManager.WORLD_SEED;
        MapSize = SaveManager.WORLD_SIZE;
        gameMap = generator.Generate(MapSize, Seed);
        SaveManager.CreateWorldFolder();
        GenerateAllNPCs();
        gameMap.SaveGameMap();
        GameDataHolder.getInstance().Save();
        Player player = GameDataHolder.getInstance().player;
        //Set Default Inventory
        GameDataHolder.getInstance().player.playerInventory.Clear();
        GameDataHolder.getInstance().player.playerInventory.AddInitialItems();
        GameDataHolder.getInstance().player.playerInventory.Save();
        // TODO: Place Player to the Right Position
        while (GetMapBlock(player.transform.position).tileBase == 2)//if spawned on water
            player.transform.position = player.transform.position + new Vector3(1, 0, 0);//move right
        // TODO: Set Player initial Status

        // Save player Status
        GameDataHolder.getInstance().player.Save();
        // init all npc database


    }
    public void SaveWorld()
    {
        SaveManager.CreateWorldFolder();
        GameDataHolder.getInstance().Save();
        gameMap.SaveGameMap();
        TileRecordManager.GetInstance().SaveTileRecord();
        // TODO: Save Player Inventory
        GameDataHolder.getInstance().player.playerInventory.Save();
        // TODO: Save Player Status and Position
        GameDataHolder.getInstance().player.Save();
        
        
    }
    public void ReturnToMenu()
    {
        //SceneManager.UnloadSceneAsync("InGame");
        SceneManager.LoadScene("MainMenu");
        
    }
    public void LoadWorldFromFolder()
    {
        GameDataHolder.getInstance().Load();
        // Generate New Empty Map With Correct MapSize
        GameMapGenerator generator = new GameMapGenerator();
        Seed = SaveManager.WORLD_SEED;
        MapSize = SaveManager.WORLD_SIZE;
        gameMap = generator.GenerateTilesOnly(MapSize, Seed);
        gameMap.LoadGameMap();
        TileRecordManager.GetInstance().LoadTileRecord();
        TileRecordManager.GetInstance().RecoverTile();
        // TODO: Load Saved Player Inventory
        GameDataHolder.getInstance().player.playerInventory.Load();
        // TODO: Place Player to the Last Position and load Player Status
        GameDataHolder.getInstance().player.Load();

        // TODO: Load All NPC Database
    }
    public void SaveAndQuit()
    {
        SaveWorld();
        ReturnToMenu();
    }
    public void SaveOnly()
    {
        try { SaveWorld(); }
        catch (System.Exception)
        {
            MessageSystem.GetInstance().NewErrorMessage("保存时发生未知错误");
            return;
        }
        MessageSystem.GetInstance().NewTipMessage("保存成功");
    }


    public void QuitWithoutSave()
    {
        ReturnToMenu();
    }
    public Vector2Int WorldPosTransChunkID(Vector3 PlayerPos)
    {
        Vector2Int src = new Vector2Int(Mathf.FloorToInt(PlayerPos.x / Chunk.CHUNK_WIDTH), Mathf.FloorToInt(PlayerPos.z / Chunk.CHUNK_WIDTH));
        src *= Chunk.CHUNK_WIDTH;
        Vector2Int ret = new Vector2Int();
        ret.x = src.x;
        ret.y = src.y;
        ret.x = ((ret.x) % gameMap.BlockSize + gameMap.BlockSize) % gameMap.BlockSize;
        ret.y = ((ret.y) % gameMap.BlockSize + gameMap.BlockSize) % gameMap.BlockSize;
        ret.x /= Chunk.CHUNK_WIDTH;
        ret.y /= Chunk.CHUNK_WIDTH;
        //Debug.Log($"x:{ret.x};y:{ret.y};");
        return ret;
    }
    public Vector2Int WorldPosTransChunkSrc(Vector3 PlayerPos)
    {
        Vector2Int src = new Vector2Int(Mathf.FloorToInt(PlayerPos.x / Chunk.CHUNK_WIDTH), Mathf.FloorToInt(PlayerPos.z / Chunk.CHUNK_WIDTH));
        src *= Chunk.CHUNK_WIDTH;
        return src;
    }

    public Vector3 WorldPosTransInChunkPos(Vector3 WorldPosition)
    {
        Vector3 inChunkPos = Vector3.zero;
        inChunkPos.x = WorldPosition.x - WorldPosTransChunkSrc(WorldPosition).x;
        inChunkPos.z = WorldPosition.z - WorldPosTransChunkSrc(WorldPosition).y;
        return inChunkPos;
    }
    public bool IsChunkActive(Vector2Int chunkID)
    {
        return LoadedChunkList.Contains(chunkID);
    }
    private void LoadChunk(Vector2Int chunkID, Vector2Int worldChunkSrc)
    {
        //judge if chunk has been loaded
        if (LoadedChunkList.Contains(chunkID))
        {
            //Debug.Log($"chunk:{chunkID} already loaded");
            (LoadedChunkList[chunkID] as ChunkContolUnit).ChunkLifeTime = ChunkContolUnit.ChunkMaxLifeTime;
            return;
        }
        //Debug.Log($"loading chunk:{chunkID}to world space:{worldChunkSrc};");
        Debug.Log("Loading Chunk:" + chunkID + "at world src:" + worldChunkSrc);
        Vector3Int pos = new Vector3Int(worldChunkSrc.x, worldChunkSrc.y, 0);
        Chunk chunk = gameMap.GetChunk(chunkID);
        //first load tiles
        for (int i = 0; i < Chunk.CHUNK_WIDTH; i++)
        {
            for (int j = 0; j < Chunk.CHUNK_WIDTH; j++)
            {
                pos.x = worldChunkSrc.x + i;
                pos.y = worldChunkSrc.y + j;
                MapBlock block = chunk.BlockMatrix[i][j];
                if (block.tileBase == 2)
                    tilemap.SetTile(pos, animatedTiles[0]);
                else
                    tilemap.SetTile(pos, tiles[block.tileBase]);

            }
        }
        //then load entities
        foreach (EntityControlUnit ecu in chunk.Entities)
        {
            //Debug.Log(ecu.inChunkPos+" "+ecu.chunkID);
            Vector3 entityPos = new Vector3(ecu.inChunkPos.x + worldChunkSrc.x, ecu.inChunkPos.y, ecu.inChunkPos.z + worldChunkSrc.y);
            Debug.Log(entityPos);
            GameObject newEntity = Instantiate(Entitylist[ecu.entityId], entityPos, Quaternion.identity);
            newEntity.GetComponent<Entity>().Mount(ecu);
            // set occupation
            Vector2Int EntitySize = newEntity.GetComponent<Entity>().EntitySize;
            Vector2Int checkingBlock = new Vector2Int((int)ecu.inChunkPos.x, (int)ecu.inChunkPos.z) - newEntity.GetComponent<Entity>().EntityPivot;
            for (int i = 0; i < EntitySize.x; i++)
            {
                for (int j = 0; j < EntitySize.y; j++)//when the object's radius is larger than chunk width then it's not working. maybe need some enhancements, maybe not
                {
                    Vector2Int currentBlockID = checkingBlock + new Vector2Int(i, j);
                    Vector2Int currentChunkID = chunkID;
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
                    gameMap.GetChunk(currentChunkID).GetMapBlock(currentBlockID).occupied = true;
                }
            }
            /*if (block.entity > 0)
            {
                Vector3Int entityPos = new Vector3Int();
                entityPos.x = pos.x;
                entityPos.y = 0;
                entityPos.z = pos.y;
                Instantiate(entity[block.entity], entityPos, Quaternion.identity);
            }*/
        }
        //also load mobiles
        foreach (MobileControlUnit mcu in chunk.Mobiles)
        {
            if(mcu==null)
                Debug.LogWarning("NULL");
            Vector3 entityPos = new Vector3(mcu.inChunkPos.x + worldChunkSrc.x, mcu.inChunkPos.y, mcu.inChunkPos.z + worldChunkSrc.y);
            GameObject newMobile = Instantiate(Mobilelist[mcu.MobileID], entityPos, Quaternion.identity);
            newMobile.GetComponent<Mobile>().Mount(mcu);
        }
        //add chunk collider
        Vector3 colliderPos=new Vector3(worldChunkSrc.x,0,worldChunkSrc.y);
        GameObject collider= Instantiate(ChunkCollider, colliderPos, Quaternion.identity);
        //add this chunk to loaded list
        ChunkContolUnit CHU = new ChunkContolUnit(chunkID, worldChunkSrc, collider);
        LoadedChunkList.Add(chunkID, CHU);
        
    }
    private void UnloadChunk(Vector2Int chunkID,Vector2Int worldChunkSrc)
    {
        //check if chunk is loaded
        if (!LoadedChunkList.Contains(chunkID))
        {
            //Debug.Log($"chunk:{chunkID} hasn't been loaded");
            return;
        }
        Vector3Int pos = new Vector3Int(worldChunkSrc.x, worldChunkSrc.y, 0);
        Chunk chunk = gameMap.GetChunk(chunkID);
        for (int i = 0; i < Chunk.CHUNK_WIDTH; i++)
        {
            for (int j = 0; j < Chunk.CHUNK_WIDTH; j++)
            {
                pos.x = worldChunkSrc.x + i;
                pos.y = worldChunkSrc.y + j;
                MapBlock block = chunk.BlockMatrix[i][j];
                tilemap.SetTile(pos, null);

            }
        }
        foreach (EntityControlUnit ecu in gameMap.GetChunk(chunkID).Entities)
        {
            if(ecu.entity!=null)
                ecu.entity.unMount();
        }
        foreach (MobileControlUnit mcu in gameMap.GetChunk(chunkID).Mobiles.ToArray())
        {
            if(mcu.mobile!=null)
                mcu.mobile.unMount();
        }
        //delete collider:
        Destroy((LoadedChunkList[chunkID] as ChunkContolUnit).ChunkCollider);
        //remove chunk in loaded list
        LoadedChunkList.Remove(chunkID);
    }
    private void UpdateMap()
    {
        Vector3 PlayerPos = Player.transform.position;
        Vector2Int src = WorldPosTransChunkSrc(PlayerPos);
        Vector2Int chunkID = WorldPosTransChunkID(PlayerPos);
        LoadChunk(chunkID, src);
    }
    private void UpdateMapNew()
    {
        
        int r = DrawDistance;
        Vector3 PlayerPos = Player.transform.position;
        Vector2Int src = WorldPosTransChunkSrc(PlayerPos);
        Vector2Int chunkID = WorldPosTransChunkID(PlayerPos);
        chunkID.x = (chunkID.x + gameMap.ChunkSize - r) % gameMap.ChunkSize;
        chunkID.y = (chunkID.y + gameMap.ChunkSize - r) % gameMap.ChunkSize;
        src.x -= r * Chunk.CHUNK_WIDTH;
        src.y -= r * Chunk.CHUNK_WIDTH;
        Vector2Int biasChunk = new Vector2Int();
        Vector2Int biasSrc = new Vector2Int();
        //Debug.Log("Updated, should load Chunk:Src" + chunkID+"/"+ src);
        LoadChunk(chunkID, src);
        for(int i = 0; i < r * 2 + 1; ++i)
        {
            for(int j = 0; j < r * 2 + 1; ++j)
            {
                biasChunk.x = (chunkID.x + i) % gameMap.ChunkSize;
                biasChunk.y = (chunkID.y + j) % gameMap.ChunkSize;
                biasSrc.x = src.x + i * Chunk.CHUNK_WIDTH;
                biasSrc.y = src.y + j * Chunk.CHUNK_WIDTH;
                LoadChunk(biasChunk, biasSrc);
            }
        }
        LinkedList<Vector2Int> removee = new LinkedList<Vector2Int>();
        foreach (Vector2Int key in LoadedChunkList.Keys)
        {
            ChunkContolUnit chu = LoadedChunkList[key] as ChunkContolUnit;
            chu.ChunkLifeTime -= 1;
            if(chu.ChunkLifeTime < 0)
            {
                removee.AddFirst(key);
            }
        }
        foreach (Vector2Int key in removee)
        {
            ChunkContolUnit chu=LoadedChunkList[key] as ChunkContolUnit;
            UnloadChunk(chu.ChunkID, chu.ChunkSrc);
            LoadedChunkList.Remove(key);
        }
    }
    public void Update()
    {
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    gameMap.SaveGameMap();
        //}
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    Vector2Int ChunkID = WorldPosTransChunkID(Player.transform.position);
        //    gameMap.GetChunk(ChunkID).Save();
        //    Debug.Log($"Saved Chunk:{ChunkID}");
        //}
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    Vector2Int ChunkID = WorldPosTransChunkID(Player.transform.position);
        //    gameMap.GetChunk(ChunkID).Load();
        //    Debug.Log($"Loaded Chunk:{ChunkID}");
        //}
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    Vector2Int ChunkID = WorldPosTransChunkID(Player.transform.position);
        //    Vector2Int ChunkSrc = WorldPosTransChunkSrc(Player.transform.position);
        //    UnloadChunk(ChunkID, ChunkSrc);
        //}
    }
    public void GenerateAllNPCs()
    {
        int MobileID = 0;
        for (; MobileID < 4; ++MobileID)
        {
            Vector3 playerPos = GameDataHolder.getInstance().player.gameObject.transform.position;
            Vector2 DV = Random.insideUnitCircle;
            DV *= Random.Range(MinMobileGenerationDistance, MaxMobileGenerationDistance);
            Vector3 Destination = new Vector3(playerPos.x + DV.x, 0, playerPos.z + DV.y);
            while(GetMapBlock(Destination).tileBase==2)//do not spawn on water
            {
                DV = Random.insideUnitCircle;
                DV *= Random.Range(MinMobileGenerationDistance, MaxMobileGenerationDistance);
                Destination = new Vector3(playerPos.x + DV.x, 0, playerPos.z + DV.y);
            }
            GenerateMobile(MobileID, Destination, 0);
        }
    }
    public void RandomGenerateMobile()
    {
        Debug.Log("Try Generating Mobile Randomly");
        int MobileID = Random.Range(4,Mobilelist.Length);//do not generate NPC
        Vector3 playerPos = GameDataHolder.getInstance().player.gameObject.transform.position;
        Vector2 DV = Random.insideUnitCircle;
        
        DV*=Random.Range(MinMobileGenerationDistance, MaxMobileGenerationDistance);
        Vector3 Destination = new Vector3(playerPos.x + DV.x, 0, playerPos.z + DV.y);
        if (GetMapBlock(Destination).tileBase == 2)// do not spawn on water
            return;
        Vector2Int ChunkID = WorldPosTransChunkID(Destination);
        // aim chunk should be active
        if (IsChunkActive(ChunkID))
        {
            if (gameMap.GetChunk(ChunkID).Mobiles.Count < MaxMobilePerChunk)
            {
                GenerateMobile(MobileID, Destination, 0);
                Debug.Log("Generation Succeed at " + Destination);
            }
            else
            {
                Debug.Log("Generation Failed at " + Destination + ":Chunk is full");
            }
        }
        else Debug.Log("Generation Failed");
    }
    public MapBlock GetMapBlock(Vector3 WorldPosition)
    {
        Vector2Int ChunkID = WorldPosTransChunkID(WorldPosition);
        Vector3 inChunkPos = WorldPosition;
        inChunkPos.x = WorldPosition.x - WorldPosTransChunkSrc(WorldPosition).x;
        inChunkPos.z = WorldPosition.z - WorldPosTransChunkSrc(WorldPosition).y;
        return gameMap.GetChunk(ChunkID).GetMapBlock(new Vector2Int((int)inChunkPos.x, (int)inChunkPos.z));
    }
    public bool SetGameMapTile(Vector3 WorldPosition, int tileID)
    {
        MapBlock mapBlock = GetMapBlock(WorldPosition);
        if (/*mapBlock.ECU == null*/!mapBlock.occupied)
        {
            Vector3Int WorldBlockPos = new Vector3Int((int)WorldPosition.x, (int)WorldPosition.z, 0);
            mapBlock.tileBase = tileID;
            tilemap.SetTile(WorldBlockPos, tiles[tileID]);
            Vector2Int ChunkID = WorldPosTransChunkID(WorldPosition);
            Vector3 inChunkPos = WorldPosition;
            inChunkPos.x = WorldPosition.x - WorldPosTransChunkSrc(WorldPosition).x;
            inChunkPos.z = WorldPosition.z - WorldPosTransChunkSrc(WorldPosition).y;
            TileRecordManager.GetInstance().AddTileRecord(ChunkID, new Vector2Int((int)inChunkPos.x, (int)inChunkPos.z), tileID);
            GameDataHolder.getInstance().AddToStatistic("T" + tileID, 1);
            return true;//successful

        }
        else return false;//this map block has entity on it, can't change tile
    }
    public void RecoverMapTile(Vector2Int ChunkID,Vector2Int inChunkPos,int TileID)
    {
        gameMap.GetChunk(ChunkID).GetMapBlock(inChunkPos).tileBase = TileID;
    }
    /// <summary>
    /// Creates a new Entity in world
    /// </summary>
    /// <param name="EntityID"></param>
    /// <param name="WorldPosition"></param>
    /// <param name="mode"></param>
    /// <returns>whether this generation operation succeeded</returns>
    public bool GenerateEntity(int EntityID, Vector3 WorldPosition, int mode)
    {
        Vector2Int ChunkID = WorldPosTransChunkID(WorldPosition);
        Vector3 inChunkPos = WorldPosition;
        inChunkPos.x = WorldPosition.x - WorldPosTransChunkSrc(WorldPosition).x;
        inChunkPos.z = WorldPosition.z - WorldPosTransChunkSrc(WorldPosition).y;
        MapBlock selectedBlock = gameMap.GetChunk(ChunkID).GetMapBlock(new Vector2Int((int)inChunkPos.x, (int)inChunkPos.z));
        Debug.Log(selectedBlock.ECU);
        //occupation check
        bool Occupied = false;
        Vector2Int checkingBlock = new Vector2Int((int)inChunkPos.x, (int)inChunkPos.z) - Entitylist[EntityID].GetComponent<Entity>().EntityPivot;
        Vector2Int EntitySize = Entitylist[EntityID].GetComponent<Entity>().EntitySize;
        for (int i = 0; i < EntitySize.x; i++)
        {
            for(int j = 0; j < EntitySize.y; j++)//when the object's radius is larger than chunk width then it's not working. maybe need some enhancements, maybe not
            {
                Vector2Int currentBlockID = checkingBlock + new Vector2Int(i, j);
                Vector2Int currentChunkID = ChunkID;
                //dealing with inter-chunk problems
                if(currentBlockID.x<0)
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
                Debug.Log("MapSize: " + MapSize);
                Debug.Log("Current ChunkID: " + currentChunkID);
                if (gameMap.GetChunk(currentChunkID).GetMapBlock(currentBlockID).occupied)
                {
                    Occupied = true;
                    break;
                }
            }
            if (Occupied)
                break;
        }

        if (/*selectedBlock.ECU==null*/!Occupied)//empty block
        {
            //check for entity's available tile type
            bool availableTile = false;
            int[] availableTileSets = Entitylist[EntityID].GetComponent<Entity>().availableTiles;
            if (availableTileSets.Length == 0)
                availableTile = true;
            foreach (int tileID in availableTileSets)
            {
                if (selectedBlock.tileBase == tileID)
                {
                    availableTile = true;
                    break;
                }
            }
            if (availableTile)
            {
                Debug.Log("Generation Succeed");
                GameObject newEntity = Instantiate(Entitylist[EntityID], WorldPosition, Quaternion.identity);
                EntityControlUnit ecu = newEntity.GetComponent<Entity>().GenerateECU(mode);
                newEntity.GetComponent<Entity>().Mount(ecu);
                gameMap.GetChunk(ChunkID).AssociateECU(ecu, inChunkPos);
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
                        gameMap.GetChunk(currentChunkID).GetMapBlock(currentBlockID).occupied = true;
                    }
                }
                selectedBlock.ECU = ecu;
                GameDataHolder.getInstance().AddToStatistic("E" + EntityID.ToString(), 1);

                return true;
            }
            else
            {
                Debug.Log("Generation Failed:Tile not Available");
                return false;
            }
        }
        else
        {
            Debug.Log("Generation Failed:Block Occupied");
            return false;
        }
    }
    public bool GenerateMobile(int MobileID, Vector3 WorldPosition, int mode)
    {
        Vector2Int ChunkID = WorldPosTransChunkID(WorldPosition);
        Vector3 inChunkPos = WorldPosition;
        inChunkPos.x = WorldPosition.x - WorldPosTransChunkSrc(WorldPosition).x;
        inChunkPos.z = WorldPosition.z - WorldPosTransChunkSrc(WorldPosition).y;
        Debug.Log("Generation Succeed");
        GameObject newMobile = Instantiate(Mobilelist[MobileID], WorldPosition, Quaternion.identity);
        MobileControlUnit mcu = newMobile.GetComponent<Mobile>().GenerateMCU(mode);
        Mobile mobile = newMobile.GetComponent<Mobile>();
        mobile.Mount(mcu);
        gameMap.GetChunk(ChunkID).AssociateMCU(mcu, inChunkPos);
        if (!IsChunkActive(ChunkID))
        {
            mobile.unMount();
        }
        return false;
    }
}
