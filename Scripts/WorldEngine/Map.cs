using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapBlock
{
    public int tileBase;
    public bool occupied;
    public EntityControlUnit ECU;
    public MapBlock()
    {
        occupied = false;
        tileBase = 0;
        ECU = null;
    }
}
public class Chunk
{
    public Vector2Int ChunkID;
    public static readonly int CHUNK_WIDTH = 8;
    public MapBlock[][] BlockMatrix;
    public LinkedList<EntityControlUnit> Entities;
    public List<MobileControlUnit> Mobiles;
    public Chunk()
    {
        BlockMatrix = new MapBlock[CHUNK_WIDTH][];
        for (int i = 0; i < BlockMatrix.Length; i++)
        {
            BlockMatrix[i] = new MapBlock[CHUNK_WIDTH];
        }
        Entities = new LinkedList<EntityControlUnit>();
        Mobiles = new List<MobileControlUnit>();
    }
    public MapBlock GetMapBlock(Vector2Int InChunkPos)
    {
        return BlockMatrix[InChunkPos.x][InChunkPos.y];
    }
    public void Save()
    {
        string saveURL = SaveManager.SAVE_PATH + SaveManager.CURRENT_WORLD_FOLDER + SaveManager.CHUNK_FOLDER + "/" + ChunkID.x + "-" + ChunkID.y + ".ck";
        Debug.Log("saving to "+ saveURL);
        string[] entitesStr = new string[Entities.Count+Mobiles.Count+1];
        int i = 0;
        foreach (EntityControlUnit entity in Entities)
        {
            entitesStr[i] = entity.GenerateSaveString();
            ++i;
        }
        entitesStr[i] = "ENTITY_END";
        ++i;
        foreach(MobileControlUnit mobile in Mobiles)
        {
            entitesStr[i] = mobile.GenerateSaveString();
        }
        //File.Create(saveURL);
        File.WriteAllLines(saveURL, entitesStr);
    }
    public void Load()
    {
        string saveURL = SaveManager.SAVE_PATH + SaveManager.CURRENT_WORLD_FOLDER + SaveManager.CHUNK_FOLDER + "/" + ChunkID.x + "-" + ChunkID.y + ".ck";
        Debug.Log("loading from " + saveURL);
        string[] entitesStr = File.ReadAllLines(saveURL);
        Entities = new LinkedList<EntityControlUnit>();
        Mobiles = new List<MobileControlUnit>();
        bool entityMode = true;
        for(int i = 0; i < entitesStr.Length; i++)
        {
            if(entitesStr[i] == "ENTITY_END")
            {
                entityMode = false;
                continue;
            }
            if (entityMode)
            {
                EntityControlUnit entity = EntityControlUnit.CreateECUFromString(entitesStr[i]);
                Debug.Log(entity);
                Entities.AddFirst(entity);
            }
            else
            {
                MobileControlUnit mobile = MobileControlUnit.CreateMCUFromString(entitesStr[i]);
                if (mobile != null)
                {
                    Mobiles.Add(mobile);
                    //Debug.LogError(mobile);
                    //Debug.LogError("ErrorFile:" + saveURL);
                    //Debug.LogError("Caused by String:" + entitesStr[i]);
                }
                
            }
        }
        Debug.Log("Succesfully Loaded");
    }
    public void AssociateECU(EntityControlUnit ecu,Vector3 inChunkPos)
    {
        ecu.chunkID = ChunkID;
        ecu.inChunkPos=inChunkPos;
        Entities.AddFirst(ecu);
    }
    public void AssociateMCU(MobileControlUnit mcu, Vector3 inChunkPos)
    {
        mcu.chunkID = ChunkID;
        mcu.inChunkPos = inChunkPos;
        Mobiles.Add(mcu);
    }
}
public class GameMap
{
    public int BlockSize { get; private set; }
    public int ChunkSize { get; private set; }
    public Chunk[][] ChunkMatrix;
    public GameMap(int size)
    {
        Debug.Assert(size > 0 && size < 256);
        BlockSize = size * Chunk.CHUNK_WIDTH;
        ChunkSize = size;
        ChunkMatrix = new Chunk[ChunkSize][];
        for (int i = 0; i < ChunkMatrix.Length; i++)
        {
            ChunkMatrix[i] = new Chunk[ChunkSize];
            for (int j = 0; j < ChunkMatrix[i].Length; j++)
            { 
                ChunkMatrix[i][j] = new Chunk();
                ChunkMatrix[i][j].ChunkID = new Vector2Int(i, j);
            }
        }
    }
    public Chunk GetChunk(Vector2Int chunkID)
    {
        return ChunkMatrix[chunkID.x][chunkID.y];
    }
    public void SaveGameMap()
    {
        foreach (var mc in ChunkMatrix)
        {
            foreach (Chunk mc2 in mc)
            {
                mc2.Save();
            }
        }
    }
    public void LoadGameMap()
    {
        foreach (var mc in ChunkMatrix)
        {
            foreach (Chunk mc2 in mc)
            {
                mc2.Load();
            }
        }
    }
}
class GameMapGenerator
{
    GameMap map;
    System.Random random;
    MapBlock[][] matrix;
    float magnification = 20f;
    int x_offset = 0; // <- +>
    int y_offset = 0;
    public void GenerateTiles(int size)
    {
        //int x, y;
        //int[] collideSet = new int[0];
        ////generate polars
        ////for(int i = 0; i < matrix.Length/8; i+=8)
        ////{
        ////    WanderRender(i * 8, 0, 0.6, 0.6, 4, collideSet, map.BlockSize / 8 * map.BlockSize / 8);
        ////}
        ////for (int i = 0; i < matrix.Length / 8; i += 8)
        ////{
        ////    WanderRender(i * 8, matrix.Length/8, 0.6, 0.6, 4, collideSet, map.BlockSize / 8 * map.BlockSize / 8);
        ////}
        ////for (int i = 0; i < matrix.Length / 8; i++)
        ////{
        ////    for(int j = 0; j < matrix[i].Length; j++) { matrix[j][i].tileBase = 4; }
        ////}
        ////generate water lakes

        //for (int i = 0; i < size / 4; ++i)
        //{
        //    x = random.Next(size);
        //    y = random.Next(size);
        //    WanderRender(x, y, 0.5, 0.3, 2, collideSet);
        //}
        ////generate dirts
        //collideSet = new int[] { 4, 2 };
        //for (int i = 0; i < size / 16; ++i)
        //{
        //    x = random.Next(size);
        //    y = random.Next(size);
        //    WanderRender(x, y, 0.5, 0.4, 3, collideSet);
        //}
        ////generate grassland
        //collideSet = new int[] { 4, 2 };
        //for (int i = 0; i < size; ++i)
        //{
        //    x = random.Next(size);
        //    y = random.Next(size);
        //    WanderRender(x, y, 0.45, 0.5, 1, collideSet);
        //}
        ////generate moss field
        ////for (int i = 0; i < size/16; ++i)
        ////{
        ////    x = random.Next(size);
        ////    y = random.Next(size);
        ////    WanderRender(x, y, 0.45, 0.5, 6, collideSet, map.BlockSize / 4 * map.BlockSize / 4);
        ////}
        ////generate entities(temp)
        
        Generator generator = new Generator();
        generator.SetSize(size);
        generator.SetSeed(random.Next());
        MyTile[,] tileMaps = generator.GetTileMaps();
        for (int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                Debug.Log("i:" + i+",j:"+j);
                MyTile tile = tileMaps[i,j];
                
                switch (tile.HeightType)
                {
                    case HeightType.DeepWater:
                        matrix[i][j].tileBase = 4;
                        break;
                    case HeightType.ShallowWater:
                        matrix[i][j].tileBase = 0;
                        break;
                    case HeightType.Sand:
                        matrix[i][j].tileBase = 5;
                        break;
                    case HeightType.Grass:
                        matrix[i][j].tileBase = 1;
                        break;
                    case HeightType.Forest:
                        matrix[i][j].tileBase = 6;
                        break;
                    case HeightType.Rock:
                        matrix[i][j].tileBase = 2;
                        break;
                    case HeightType.Snow:
                        matrix[i][j].tileBase = 2;
                        break;
                }
            }
        }
        //for(int i = 0; i < matrix.Length; i++)
        //{
        //    for (int j = 0; j < matrix[i].Length; j++)
        //        matrix[i][j].tileBase = GetIdUsingPerlin(i, j);
        //}
    }
    public GameMap Generate(int chunkSize, int seed)
    {
        random = new System.Random(seed);
        map = new GameMap(chunkSize);
        int size = map.BlockSize;
        
        matrix = new MapBlock[map.BlockSize][];
        for (int i = 0; i < matrix.Length; i++)
        {
            matrix[i] = new MapBlock[map.BlockSize];
            for (int j = 0; j < matrix[i].Length; j++) { matrix[i][j] = new MapBlock(); }
        }//generate template map
        GenerateTiles(size);
        // generate entities
        bool SpecialGenerated = false;
        for (int i = 0; i < matrix.Length; i++)
        {
            for (int j = 0; j < matrix[i].Length; j++)
            {
                switch (matrix[i][j].tileBase)
                {
                    case 0://for surface
                        {
                            if (random.NextDouble() > 0.98)
                            {
                                SetEntity(6, i, j);
                            }
                            if (random.NextDouble() > 0.9995)
                            {
                                SetEntity(17, i, j);
                                SpecialGenerated = true;
                            }
                            break;
                        }
                    case 1://for meadow
                        {
                            if (random.NextDouble() > 0.5)
                            {
                                SetEntity(4, i, j);
                            }
                            else if (random.NextDouble() > 0.975)
                            {
                                SetEntity(0, i, j);
                            }
                            else if (random.NextDouble() > 0.95)
                            {
                                SetEntity(1, i, j);
                            }
                            else if (random.NextDouble() > 0.95)
                            {
                                SetEntity(2, i, j);
                            }
                            else if (random.NextDouble() > 0.9)
                            {
                                SetEntity(5, i, j);
                            }
                            else if (random.NextDouble() > 0.999)
                            {
                                SetEntity(11, i, j);
                            }
                            else if (random.NextDouble() > 0.999)
                            {
                                SetEntity(13, i, j);
                            }
                            break;
                        }
                    case 2://for water
                        {
                            SetEntity(27, i, j);
                            break;
                        }
                    case 5://for sand
                        {
                            if (random.NextDouble() > 0.95)
                            {
                                SetEntity(15, i, j);
                            }
                            else if (random.NextDouble() > 0.99)
                            {
                                SetEntity(3, i, j);
                            }
                            else if (random.NextDouble() > 0.99)
                            {
                                SetEntity(18, i, j);
                            }
                            else if (random.NextDouble() > 0.995)
                            {
                                SetEntity(19, i, j);
                            }
                            else if (random.NextDouble() > 0.999)
                            {
                                SetEntity(12, i, j);
                            }
                            break;
                        }
                    case 4://for snow
                        {
                            if (random.NextDouble() > 0.99)
                            {
                                SetEntity(22, i, j);
                            }
                            else if (random.NextDouble() > 0.995)
                            {
                                SetEntity(23, i, j);
                            }
                            else if (random.NextDouble() > 0.8)
                            {
                                SetEntity(24, i, j);
                            }
                            break;
                        }
                    case 6://for moss
                        {
                            if (random.NextDouble() > 0.99)
                            {
                                SetEntity(20, i, j);
                            }
                            else if (random.NextDouble() > 0.995)
                            {
                                SetEntity(21, i, j);
                            }
                            else if (random.NextDouble() > 0.5)
                            {
                                SetEntity(1, i, j);
                            }
                            else if (random.NextDouble() > 0.75)
                            {
                                SetEntity(2, i, j);
                            }
                            else if (random.NextDouble() > 0.999)
                            {
                                SetEntity(10, i, j);
                            }
                            break;
                        }
                    default: break;
                }
            }
        }
        while (!SpecialGenerated)
        {
            int x = random.Next(0, map.BlockSize);
            int y = random.Next(0, map.BlockSize);
            if (matrix[x][y].tileBase == 0)
            {
                SetEntity(17, x, y);
                SpecialGenerated = true;
            }
            
        }
        // copy template matix to map
        //todo:
        for (int i = 0; i < map.ChunkSize; ++i)
        {
            for (int j = 0; j < map.ChunkSize; ++j)
            {
                //for each chunk(i,j) in map:
                Chunk currentChunk = map.ChunkMatrix[i][j];
                for (int k = 0; k < Chunk.CHUNK_WIDTH; ++k)
                {
                    for (int l = 0; l < Chunk.CHUNK_WIDTH; ++l)
                    {
                        //load each block:
                        currentChunk.BlockMatrix[k][l] = matrix[i * Chunk.CHUNK_WIDTH + k][j * Chunk.CHUNK_WIDTH + l];
                    }
                }
            }
        }
        return map;
    }
    public GameMap GenerateEmptyMap(int chunkSize)
    {
        map = new GameMap(chunkSize);
        int size = map.BlockSize;
        matrix = new MapBlock[map.BlockSize][];
        for (int i = 0; i < matrix.Length; i++)
        {
            matrix[i] = new MapBlock[map.BlockSize];
            for (int j = 0; j < matrix[i].Length; j++) { matrix[i][j] = new MapBlock(); }
        }
        for (int i = 0; i < map.ChunkSize; ++i)
        {
            for (int j = 0; j < map.ChunkSize; ++j)
            {
                //for each chunk(i,j) in map:
                Chunk currentChunk = map.ChunkMatrix[i][j];
                for (int k = 0; k < Chunk.CHUNK_WIDTH; ++k)
                {
                    for (int l = 0; l < Chunk.CHUNK_WIDTH; ++l)
                    {
                        //load each block:
                        currentChunk.BlockMatrix[k][l] = matrix[i * Chunk.CHUNK_WIDTH + k][j * Chunk.CHUNK_WIDTH + l];
                    }
                }
            }
        }
        return map;
    }
    public GameMap GenerateTilesOnly(int chunkSize,int seed)
    {
        random = new System.Random(seed);
        map = new GameMap(chunkSize);
        int size = map.BlockSize;
        matrix = new MapBlock[map.BlockSize][];
        for (int i = 0; i < matrix.Length; i++)
        {
            matrix[i] = new MapBlock[map.BlockSize];
            for (int j = 0; j < matrix[i].Length; j++) { matrix[i][j] = new MapBlock(); }
        }//generate template map
        GenerateTiles(size);
        for (int i = 0; i < map.ChunkSize; ++i)
        {
            for (int j = 0; j < map.ChunkSize; ++j)
            {
                //for each chunk(i,j) in map:
                Chunk currentChunk = map.ChunkMatrix[i][j];
                for (int k = 0; k < Chunk.CHUNK_WIDTH; ++k)
                {
                    for (int l = 0; l < Chunk.CHUNK_WIDTH; ++l)
                    {
                        //load each block:
                        currentChunk.BlockMatrix[k][l] = matrix[i * Chunk.CHUNK_WIDTH + k][j * Chunk.CHUNK_WIDTH + l];
                    }
                }
            }
        }
        return map;
    }
    public bool SetEntity(int entityID,int i,int j)
    {
        Vector2Int ChunkID = new Vector2Int(i / Chunk.CHUNK_WIDTH, j / Chunk.CHUNK_WIDTH);
        Vector3Int InChunkPos = new Vector3Int(i % Chunk.CHUNK_WIDTH, 0, j % Chunk.CHUNK_WIDTH);
        EntityControlUnit newEntity = TileTerrain.GetInstance().Entitylist[entityID].GetComponent<Entity>().GenerateECU(0);
        matrix[i][j].ECU = newEntity;
        matrix[i][j].occupied = true;
        newEntity.chunkID = ChunkID;
        newEntity.inChunkPos = InChunkPos;
        map.ChunkMatrix[ChunkID.x][ChunkID.y].Entities.AddFirst(newEntity);
        return true;
    }
    public void WanderRender(int SourceX, int SourceY, double UDProb, double LRProb, int color, int[] collide,int maxSize=0)
    {
        int RenderedTile = 0;
        if (matrix[SourceX][SourceY].tileBase == color)
            return;
        Queue<int[]> queue = new Queue<int[]>();
        int[] i = new int[2];
        i[0] = SourceX;
        i[1] = SourceY;
        int original;
        bool skip = false;
        queue.Enqueue(i);
        while (queue.Count > 0)
        {
            skip = false;
            i = queue.Dequeue();
            SourceX = (int)i[0];
            SourceY = (int)i[1];
            original = matrix[SourceX][SourceY].tileBase;
            if (original == color)
                skip = true;
            foreach (int k in collide)
            {
                if (original == k)
                {
                    skip = true;
                    break;
                }
            }
            if (skip)
                continue;
            matrix[SourceX][SourceY].tileBase = color;
            RenderedTile += 1;
            if (maxSize > 0)
            {
                if (RenderedTile >= maxSize)
                    return;
            }
            if (random.NextDouble() < UDProb)
            {
                i = new int[2];
                i[0] = (SourceX - 1 + map.BlockSize) % map.BlockSize;
                i[1] = SourceY;
                queue.Enqueue(i);
            }
            if (random.NextDouble() < UDProb)
            {
                i = new int[2];
                i[0] = (SourceX + 1 + map.BlockSize) % map.BlockSize;
                i[1] = SourceY;
                queue.Enqueue(i);
            }
            if (random.NextDouble() < LRProb)
            {
                i = new int[2];
                i[1] = (SourceY - 1 + map.BlockSize) % map.BlockSize;
                i[0] = SourceX;
                queue.Enqueue(i);
            }
            if (random.NextDouble() < LRProb)
            {
                i = new int[2];
                i[1] = (SourceY + 1 + map.BlockSize) % map.BlockSize;
                i[0] = SourceX;
                queue.Enqueue(i);
            }

        }
        return;
    }
    int GetIdUsingPerlin(int x, int y)
    {
        /** Using a grid coordinate input, generate a Perlin noise value to be
            converted into a tile ID code. Rescale the normalised Perlin value
            to the number of tiles available. **/

        float raw_perlin = Mathf.PerlinNoise(
            (x - x_offset) / magnification,
            (y - y_offset) / magnification
        );
        int TileSize = TileTerrain.GetInstance().tiles.Length;
        float clamp_perlin = Mathf.Clamp01(raw_perlin); // Thanks: youtu.be/qNZ-0-7WuS8&lc=UgyoLWkYZxyp1nNc4f94AaABAg
        float scaled_perlin = clamp_perlin * TileSize;

        // Replaced 4 with tileset.Count to make adding tiles easier
        if (scaled_perlin == TileSize)
        {
            scaled_perlin = (TileSize - 1);
        }
        return Mathf.FloorToInt(scaled_perlin);
    }
}
