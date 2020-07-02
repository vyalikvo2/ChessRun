using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCache : MonoBehaviour
{
    private static Dictionary<string, Sprite> spritesDict = new Dictionary<string, Sprite>();
    private static Dictionary<string, GameObject> prefabsDict = new Dictionary<string, GameObject>();

    public static Sprite getSprite(string path)
    {
        if (!spritesDict.ContainsKey(path))
        {
            spritesDict.Add(path, Resources.Load<Sprite>(path));
        }

        return spritesDict[path];
    }
    
    public static GameObject getPrefab(string path)
    {
        if (!prefabsDict.ContainsKey(path))
        {
            prefabsDict.Add(path, Resources.Load<GameObject>(path));
        }

        return prefabsDict[path];
    }
}
