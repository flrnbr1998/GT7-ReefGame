using System.Collections.Generic;
using UnityEngine;

public class TerrainTextureQuery
{
    private readonly Terrain _terrain;
    private readonly int _textureIndex;

    public TerrainTextureQuery(Terrain terrain, int textureIndex)
    {
        _terrain = terrain;
        _textureIndex = textureIndex;
    }

    public List<Vector3> GetSpawnablePositions()
    {
        var result = new List<Vector3>();
        TerrainData data = _terrain.terrainData;
        int width = data.alphamapWidth;
        int height = data.alphamapHeight;

        if (_textureIndex < 0 || _textureIndex >= data.alphamapLayers)
        {
            Debug.LogError($"[TerrainTextureQuery] textureIndex {_textureIndex} out of range (layers: {data.alphamapLayers})");
            return result;
        }

        TerrainLayer[] layers = data.terrainLayers;
        if (layers == null || _textureIndex >= layers.Length)
        {
            Debug.LogError("[TerrainTextureQuery] terrainLayers not set or textureIndex out of range.");
            return result;
        }

        float[,,] alphaMaps = data.GetAlphamaps(0, 0, width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (alphaMaps[x, y, _textureIndex] >= 0.5f)
                {
                    float worldX = x * data.size.x / width;
                    float worldZ = y * data.size.z / height;
                    float worldY = data.GetHeight(
                        Mathf.RoundToInt(x * data.heightmapResolution / width),
                        Mathf.RoundToInt(y * data.heightmapResolution / height));
                    result.Add(new Vector3(worldX, worldY, worldZ));
                }
            }
        }

        return result;
    }
}
