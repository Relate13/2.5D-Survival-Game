using UnityEngine;
using System.Collections.Generic;

public enum TileGroupType
{
	Water, 
	Land
}

public class TileGroup  {
	
	public TileGroupType Type;
	public List<MyTile> Tiles;

	public TileGroup()
	{
		Tiles = new List<MyTile> ();
	}
}
