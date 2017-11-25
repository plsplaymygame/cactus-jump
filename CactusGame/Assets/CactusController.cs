using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusController : MonoBehaviour
{
	public GameObject cactusPrefab;
	
	private IList<GameObject> cactusClones = new List<GameObject>();
	
	// Use this for initialization
	void Start ()
	{
		var ypos = transform.position.y;
		var cactusNum = 5;
		for (var i = 0; i < cactusNum; i++)
		{
			Vector3 pos = new Vector3(transform.position.x, ypos, transform.position.z);
			GameObject obj = Instantiate(cactusPrefab, pos, Quaternion.identity) as GameObject;
			obj.GetComponent<CactusBehaviour>().id = i;
			cactusClones.Add(obj);
			ypos += 2;
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {		
		// call late update for each instance in order
		foreach (GameObject obj in cactusClones)
		{
			CactusBehaviour behaviour = obj.GetComponent<CactusBehaviour>();
			behaviour.LateUpdateMe();
		}
	}
}
