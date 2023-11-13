using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class TerrainToObjConverter : EditorWindow
{
    [MenuItem("Terrain/Convert to OBJ TGA")]
    static void Init()
    {
        TerrainToObjConverter window = (TerrainToObjConverter)EditorWindow.GetWindow(typeof(TerrainToObjConverter));
        window.Show();
    }

    private Terrain terrain;
    private List<Texture2D> textures = new List<Texture2D>();
    private List<string> outputPaths = new List<string>();

    void OnGUI()
    {
        GUILayout.Label("Terrain To OBJ Converter", EditorStyles.boldLabel);

        terrain = EditorGUILayout.ObjectField("Terrain", terrain, typeof(Terrain), true) as Terrain;
        int textureCount = EditorGUILayout.IntField("Texture Count", textures.Count);
        while (textureCount < textures.Count)
        {
            textures.RemoveAt(textures.Count - 1);
            outputPaths.RemoveAt(outputPaths.Count - 1);
        }
        while (textureCount > textures.Count)
        {
            textures.Add(null);
            outputPaths.Add("Texture" + (textures.Count) + ".tga");
        }

        for (int i = 0; i < textures.Count; i++)
        {
            textures[i] = EditorGUILayout.ObjectField("Texture " + (i + 1), textures[i], typeof(Texture2D), true) as Texture2D;
            outputPaths[i] = EditorGUILayout.TextField("Output Path " + (i + 1), outputPaths[i]);
        }

        if (GUILayout.Button("Convert"))
        {
            ConvertTerrainToObj();
        }
    }

    void ConvertTerrainToObj()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain is not assigned.");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        int terrainWidth = terrainData.heightmapResolution;
        int terrainHeight = terrainData.heightmapResolution;

        Vector3 terrainSize = terrainData.size;
        float[,] heights = terrainData.GetHeights(0, 0, terrainWidth, terrainHeight);

        StreamWriter sw = new StreamWriter("Terrain.obj");

        for (int z = 0; z < terrainHeight; z++)
        {
            for (int x = 0; x < terrainWidth; x++)
            {
                float height = heights[z, x];
                Vector3 position = new Vector3(x / (float)terrainWidth, height, z / (float)terrainHeight);
                Vector3 worldPosition = Vector3.Scale(position, terrainSize);
                worldPosition.x = -worldPosition.x;

               ///UV
                Vector2 uv = new Vector2((x / (float)terrainWidth), (z / (float)terrainHeight));

                sw.WriteLine("v " + worldPosition.x + " " + worldPosition.y + " " + worldPosition.z);
                sw.WriteLine("vt " + uv.x + " " + uv.y);
            }
        }

        for (int z = 0; z < terrainHeight - 1; z++)
        {
            for (int x = 0; x < terrainWidth - 1; x++)
            {
                int index = z * terrainWidth + x;

                sw.WriteLine("f " + (index + 1) + "/" + (index + 1) + " " + (index + 2) + "/" + (index + 2) + " " + (index + terrainWidth + 2) + "/" + (index + terrainWidth + 2));
                sw.WriteLine("f " + (index + 2) + "/" + (index + 2) + " " + (index + terrainWidth + 3) + "/" + (index + terrainWidth + 3) + " " + (index + terrainWidth + 2) + "/" + (index + terrainWidth + 2));
            }
        }

        sw.Close();

        Debug.Log("Terrain converted to obj format.");

        if (textures.Count > 0)
        {
            for (int i = 0; i < textures.Count; i++)
            {
                Texture2D texture = textures[i];
                string outputPath = outputPaths[i];

                if (texture != null)
                {
                    byte[] tgaData = texture.EncodeToTGA();
                    string tgaPath = Path.ChangeExtension(outputPath, ".tga");
                    File.WriteAllBytes(tgaPath, tgaData);
                    Debug.Log("Texture exported as TGA format: " + tgaPath);
                }
            }
        }
    }
}
//by lujun hangzhou babuyou Email:16519579@qq.com