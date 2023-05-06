using UnityEngine;
using AccidentalNoise;
using System.Collections.Generic;

public class Generator {

	// Adjustable variables for Unity Inspector
	[SerializeField]
	int Width = 512;
	[SerializeField]
	int Height = 512;
	[SerializeField]
	int TerrainOctaves = 6;
	[SerializeField]
	double TerrainFrequency = 1.25;
	[SerializeField]
	float DeepWater = 0.2f;
	[SerializeField]
	float ShallowWater = 0.35f;	
	[SerializeField]
	float Sand = 0.5f;
	[SerializeField]
	float Grass = 0.7f;
	[SerializeField]
	float Forest = 0.8f;
	[SerializeField]
	float Rock = 0.9f;

	// private variables
	ImplicitFractal HeightMap;
	MapData HeightData;
	MyTile[,] Tiles;
	int Seed = 0;

	List<TileGroup> Waters = new List<TileGroup> ();
	List<TileGroup> Lands = new List<TileGroup> ();
	
	// Our texture output gameobject
	MeshRenderer HeightMapRenderer;
	public void SetSize(int mapsize)
    {
		Width = mapsize;
		Height = mapsize;
    }
	public void SetSeed(int seed)
    {
		Seed = seed;
    }

	public MyTile[,] GetTileMaps()
	{
		Initialize ();
		GetData (HeightMap, ref HeightData);
		LoadTiles ();

		UpdateNeighbors ();
		UpdateBitmasks ();
		FloodFill ();

		return Tiles;
	}


	private void Initialize()
	{
		// Initialize the HeightMap Generator
		HeightMap = new ImplicitFractal (FractalType.MULTI, 
		                               BasisType.SIMPLEX, 
		                               InterpolationType.QUINTIC, 
		                               TerrainOctaves, 
		                               TerrainFrequency, 
		                               Seed);
	}
	
	// Extract data from a noise module
	private void GetData(ImplicitModuleBase module, ref MapData mapData)
	{
		mapData = new MapData (Width, Height);

		// loop through each x,y point - get height value
		for (var x = 0; x < Width; x++) {
			for (var y = 0; y < Height; y++) {

				//Wrap on x-axis only
//				//Noise range
//				float x1 = 0, x2 = 1;
//				float y1 = 0, y2 = 1;				
//				float dx = x2 - x1;
//				float dy = y2 - y1;
//
//				//Sample noise at smaller intervals
//				float s = x / (float)Width;
//				float t = y / (float)Height;
//
//				// Calculate our 3D coordinates
//				float nx = x1 + Mathf.Cos (s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);
//				float ny = x1 + Mathf.Sin (s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);
//				float nz = t;
//
//				float heightValue = (float)HeightMap.Get (nx, ny, nz);
//
//				// keep track of the max and min values found
//				if (heightValue > mapData.Max)
//					mapData.Max = heightValue;
//				if (heightValue < mapData.Min)
//					mapData.Min = heightValue;
//
//				mapData.Data [x, y] = heightValue;



				// WRAP ON BOTH AXIS
				// Noise range
				float x1 = 0, x2 = 2;
				float y1 = 0, y2 = 2;				
				float dx = x2 - x1;
				float dy = y2 - y1;

				// Sample noise at smaller intervals
				float s = x / (float)Width;
				float t = y / (float)Height;

			
				// Calculate our 4D coordinates
				float nx = x1 + Mathf.Cos (s*2*Mathf.PI) * dx/(2*Mathf.PI);
				float ny = y1 + Mathf.Cos (t*2*Mathf.PI) * dy/(2*Mathf.PI);
				float nz = x1 + Mathf.Sin (s*2*Mathf.PI) * dx/(2*Mathf.PI);
				float nw = y1 + Mathf.Sin (t*2*Mathf.PI) * dy/(2*Mathf.PI);
			
				float heightValue = (float)HeightMap.Get (nx, ny, nz, nw);
				// keep track of the max and min values found
				if (heightValue > mapData.Max) mapData.Max = heightValue;
				if (heightValue < mapData.Min) mapData.Min = heightValue;

				mapData.Data[x,y] = heightValue;
			}
		}	
	}

	// Build a Tile array from our data
	private void LoadTiles()
	{
		Tiles = new MyTile[Width, Height];
		
		for (var x = 0; x < Width; x++)
		{
			for (var y = 0; y < Height; y++)
			{
				MyTile t = new MyTile();
				t.X = x;
				t.Y = y;
				
				float value = HeightData.Data[x, y];
				value = (value - HeightData.Min) / (HeightData.Max - HeightData.Min);
				
				t.HeightValue = value;
				
				//HeightMap Analyze
				if (value < DeepWater)  {
					t.HeightType = HeightType.DeepWater;
					t.Collidable = false;
				}
				else if (value < ShallowWater)  {
					t.HeightType = HeightType.ShallowWater;
					t.Collidable = false;
				}
				else if (value < Sand) {
					t.HeightType = HeightType.Sand;
					t.Collidable = true;
				}
				else if (value < Grass) {
					t.HeightType = HeightType.Grass;
					t.Collidable = true;
				}
				else if (value < Forest) {
					t.HeightType = HeightType.Forest;
					t.Collidable = true;
				}
				else if (value < Rock) {
					t.HeightType = HeightType.Rock;
					t.Collidable = true;
				}
				else  {
					t.HeightType = HeightType.Snow;
					t.Collidable = true;
				}
				
				Tiles[x,y] = t;
			}
		}
	}
	
	private void UpdateNeighbors()
	{
		for (var x = 0; x < Width; x++)
		{
			for (var y = 0; y < Height; y++)
			{
				MyTile t = Tiles[x,y];
				
				t.Top = GetTop(t);
				t.Bottom = GetBottom (t);
				t.Left = GetLeft (t);
				t.Right = GetRight (t);
			}
		}
	}

	private void UpdateBitmasks()
	{
		for (var x = 0; x < Width; x++) {
			for (var y = 0; y < Height; y++) {
				Tiles [x, y].UpdateBitmask ();
			}
		}
	}

	private void FloodFill()
	{
		// Use a stack instead of recursion
		Stack<MyTile> stack = new Stack<MyTile>();
		
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				
				MyTile t = Tiles[x,y];

				//Tile already flood filled, skip
				if (t.FloodFilled) continue;

				// Land
				if (t.Collidable)   
				{
					TileGroup group = new TileGroup();
					group.Type = TileGroupType.Land;
					stack.Push(t);
					
					while(stack.Count > 0) {
						FloodFill(stack.Pop(), ref group, ref stack);
					}
					
					if (group.Tiles.Count > 0)
						Lands.Add (group);
				}
				// Water
				else {				
					TileGroup group = new TileGroup();
					group.Type = TileGroupType.Water;
					stack.Push(t);
					
					while(stack.Count > 0)	{
						FloodFill(stack.Pop(), ref group, ref stack);
					}
					
					if (group.Tiles.Count > 0)
						Waters.Add (group);
				}
			}
		}
	}


	private void FloodFill(MyTile tile, ref TileGroup tiles, ref Stack<MyTile> stack)
	{
		// Validate
		if (tile.FloodFilled) 
			return;
		if (tiles.Type == TileGroupType.Land && !tile.Collidable)
			return;
		if (tiles.Type == TileGroupType.Water && tile.Collidable)
			return;

		// Add to TileGroup
		tiles.Tiles.Add (tile);
		tile.FloodFilled = true;

		// floodfill into neighbors
		MyTile t = GetTop (tile);
		if (!t.FloodFilled && tile.Collidable == t.Collidable)
			stack.Push (t);
		t = GetBottom (tile);
		if (!t.FloodFilled && tile.Collidable == t.Collidable)
			stack.Push (t);
		t = GetLeft (tile);
		if (!t.FloodFilled && tile.Collidable == t.Collidable)
			stack.Push (t);
		t = GetRight (tile);
		if (!t.FloodFilled && tile.Collidable == t.Collidable)
			stack.Push (t);
	}
	
	private MyTile GetTop(MyTile t)
	{
		return Tiles [t.X, MathHelper.Mod (t.Y - 1, Height)];
	}
	private MyTile GetBottom(MyTile t)
	{
		return Tiles [t.X, MathHelper.Mod (t.Y + 1, Height)];
	}
	private MyTile GetLeft(MyTile t)
	{
		return Tiles [MathHelper.Mod(t.X - 1, Width), t.Y];
	}
	private MyTile GetRight(MyTile t)
	{
		return Tiles [MathHelper.Mod (t.X + 1, Width), t.Y];
	}


}
