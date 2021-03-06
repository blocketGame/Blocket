using System;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Class that is used to store running variables of an world
/// TODO: Move to TerrainGeneeration
/// <b>Author : Cse19455 / Thomas Boigner</b>
/// </summary>
public class WorldData : MonoBehaviour {
	public static WorldData Singleton { get; protected set; }
	public GameObject ChunkParent => Grid.gameObject;

	public GameObject mobParent;
	public Grid Grid { get; private set; }

	#region Fields
	[SerializeField]
	private int chunkDistance; //Settings
	[SerializeField]
	private int seed;
	[SerializeField]
	private float scale;
	[SerializeField]
	private int octives;
	[SerializeField]
	[Range(0, 1)]
	private float persistance;
	[SerializeField]
	private float lacurinarity;
	[SerializeField]
	private float offsetX;
	[SerializeField]
	private float offsetY;
	[SerializeField]
	private int heightMultiplier;
	[SerializeField]
	private AnimationCurve heightcurve;
	[SerializeField]
	private float groupdistance;
	[SerializeField]
	private float pickUpDistance;
	[SerializeField]
	private float initCaveSize;
	[SerializeField]
	private float stoneSize;
	#endregion

	#region Properties
	public float Persistance { get => persistance; set => persistance = value; }
	public float Lacurinarity { get => lacurinarity; set => lacurinarity = value; }
	public float OffsetX { get => offsetX; set => offsetX = value; }
	public float OffsetY { get => offsetX; set => offsetX = value; }
	public int HeightMultiplier { get => heightMultiplier; set => heightMultiplier = value; }
	public AnimationCurve Heightcurve { get => heightcurve; set => heightcurve = value; }
	public int Octives { get => octives; set => octives = value; }
	public float Scale { get => scale; set => scale = value; }
	public int Seed { get => seed; set => seed = value; }
	public int ChunkDistance { get => chunkDistance; set => chunkDistance = value; }

	//Shortcuts
	[Obsolete]
	public int ChunkHeight => WorldAssets.ChunkLength;
	[Obsolete]
	public int ChunkWidth => WorldAssets.ChunkLength;
	[Obsolete]
	public BlockData[] Blocks => WorldAssets.Singleton.blocks.ToArray();
	public float Groupdistance { get => groupdistance; set => groupdistance = value; }
	public float PickUpDistance { get => pickUpDistance; set => pickUpDistance = value; }
	public Dictionary<int, float[]> Noisemaps { get; set; } = new Dictionary<int, float[]>();
	public float InitCaveSize { get => initCaveSize; set => initCaveSize = value; }
	public float StoneSize { get => stoneSize; set => stoneSize = value; }
	#endregion

	/// <summary>Stores this class to <see cref="GlobalVariables"/></summary>

	public void Awake(){
		Singleton = this;
		Grid = GetComponentInChildren<Grid>();
	}

}



#region WorldDataAssets

[System.Serializable]
public struct OreData
{
	[SerializeField]
	private string name;

	[SerializeField]
	private byte blockID;

	public string Name { get => name; set => name = value; }
	public byte BlockID { get => blockID; set => blockID = value; }
}

[System.Serializable]
public struct RegionData
{
	public string description;
	[SerializeField]
	private int regionRange; //-1 => infinite range
	[SerializeField]
	private byte blockID;

	public int RegionRange { get => regionRange; set => regionRange = value; }
	public byte BlockID { get => blockID; set => blockID = value; }
}


#endregion