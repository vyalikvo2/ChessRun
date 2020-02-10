using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tst : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(GetComponent<SpriteRenderer>().sprite.pixelsPerUnit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
