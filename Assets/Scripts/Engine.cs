using UnityEngine;
using System.Collections;

public class Engine : MonoBehaviour 
{
	
	public GameObject Instance(GameObject prefab)
	{
		return Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
	}

	// Use this for initialization
	void Start () 
	{
        
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
}
