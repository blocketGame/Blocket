using System;
using System.Collections.Generic;

using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
	public enum NoiseMode
	{
		snoise,
		cellular
	}

	public static float[] GenerateNoiseMap1D(int mapWith, int seed, float scale, int octaves, float persistance, float lacunarity, float offset)
	{
		//Noise
		float[] noiseMap = new float[mapWith];

		//Random
		System.Random prng = new System.Random(seed);

		float[] octaveOffsets = new float[octaves];

		float amplitude = 1;

		for (int i = 0; i < octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + offset;

			octaveOffsets[i] = offsetX;

			amplitude *= persistance;
		}

		if (scale <= 0)
		{
			scale = 0.0001f;
		}

		float halfWidth = WorldAssets.ChunkLength / 2f;


		for (int x = 0; x < mapWith; x++)
		{
			amplitude = 1;
			float frequency = 1, noiseHeight = 0;
			for (int i = 0; i < octaves; i++)
			{
				float sample = (x - halfWidth + octaveOffsets[i]) / scale * frequency;

				float perlinValue = Unity.Mathematics.noise.snoise(new Vector2(sample, 0));
				noiseHeight += perlinValue * amplitude;

				amplitude *= persistance;
				frequency *= lacunarity;
			}
			noiseMap[x] = Mathf.Clamp(noiseHeight, 0, 1);
		}
		return noiseMap;
	}

	public static float[,] GenerateNoiseMap2D(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NoiseMode noiseMode)
	{
		//Noise
		float[,] noiseMap = new float[mapWidth, mapHeight];

		//Random
		System.Random prng = new System.Random(seed);

		Vector2[] octaveOffsets = new Vector2[octaves];

		float amplitude = 1;

		for (int i = 0; i < octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + offset.x;
			float offsetY = prng.Next(-100000, 100000) + offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		if (scale <= 0)
		{
			scale = 0.0001f;
		}

		float halfWidth = WorldAssets.ChunkLength / 2f;
		float halfHeight = WorldAssets.ChunkLength / 2f;

		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;
				for (int i = 0; i < octaves; i++)
				{
					float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
					float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;
					Unity.Mathematics.float2 perlinValue = new Unity.Mathematics.float2(0, 0);

					if (noiseMode == NoiseMode.snoise)
					{
						perlinValue = Unity.Mathematics.noise.snoise(new Vector2(sampleX, sampleY));
					}
					else
					if (noiseMode == NoiseMode.cellular)
					{
						perlinValue = Unity.Mathematics.noise.cellular(new Unity.Mathematics.float2(sampleX, sampleY));
					}
					noiseHeight += perlinValue.x * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}
				noiseMap[x, y] = noiseHeight;
			}
		}
		return noiseMap;
	}

	public static int[,] GenerateBiom(int mapWidth, int mapHeight, int seed, int octaves, float persistance, float lacunarity, Vector2 offset, List<Biom> bioms)
	{
		int[,] biomnoisemaps = new int[mapWidth, mapHeight];

		int offsets = 0;
		foreach (Biom biom in bioms)
		{
			float[,] biomn = GenerateNoiseMap2D(mapWidth, mapHeight, (seed + offsets), biom.Size, octaves, persistance, lacunarity, offset, NoiseMode.cellular);

			offsets += 10000;
			for (int y = 0; y < mapHeight; y++)
			{
				for (int x = 0; x < mapWidth; x++)
				{
					if (biomn[x, y] >= (0.9f) && (biom.Index != 0))
					{
						biomnoisemaps[x, y] = biom.Index;
					}
				}
			}
		}
		return biomnoisemaps;
	}

	public static byte[,] GenerateOreNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NoiseMode noiseMode, List<Biom> bioms)
	{
		byte[,] oreNoiseMap = new byte[mapWidth, mapHeight];

		int offsets = 0;
		foreach (Biom biom in bioms)
		{
			foreach (OreData ore in biom.Ores)
			{
				float[,] orenoise = GenerateNoiseMap2D(mapWidth, mapHeight, (seed + offsets), scale, octaves, persistance, lacunarity, offset, noiseMode);

				offsets += 10000;
				for (int y = 0; y < mapHeight; y++)
				{
					for (int x = 0; x < mapWidth; x++)
					{
						if (orenoise[x, y] >= (0.9f)
							&& orenoise[x, y] <= 0.905f
							|| orenoise[x, y] >= (0.1f)
							&& orenoise[x, y] <= 0.105f
							&& (ore.BlockID != 0))
						{
							oreNoiseMap[x, y] = ore.BlockID;
						}
					}
				}
			}
		}
		return oreNoiseMap;
	}

	public static bool GenerateStructureCoordinates1d(int x, int seed, int probability, int structureId)
	{
		System.Random random = new System.Random((int)(seed + Math.Pow(x, 2) + structureId));
		return random.Next() % probability == 0;
	}

	public static bool GenerateStructureCoordinates2d(int x, int y, int seed, int probability, int structureId)
	{
		System.Random random = new System.Random((int)(seed + Math.Pow(x, 2) * y + Math.Pow(y, 2) * x + structureId));
		return random.Next() % probability == 0;
	}
}
