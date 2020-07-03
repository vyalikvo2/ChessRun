using System.Collections.Generic;
using UnityEngine;

public class ResourceCache : MonoBehaviour
{
    private static Dictionary<string, Sprite> _spritesDict = new Dictionary<string, Sprite>();
    private static Dictionary<string, GameObject> _prefabsDict = new Dictionary<string, GameObject>();

    public static Sprite getSprite(string path)
    {
        if (!_spritesDict.ContainsKey(path))
        {
            _spritesDict.Add(path, Resources.Load<Sprite>(path));
        }

        return _spritesDict[path];
    }
    
    public static GameObject getPrefab(string path)
    {
        if (!_prefabsDict.ContainsKey(path))
        {
            _prefabsDict.Add(path, Resources.Load<GameObject>(path));
        }

        return _prefabsDict[path];
    }
}
