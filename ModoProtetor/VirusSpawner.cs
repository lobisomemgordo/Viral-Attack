using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VirusSpawner : MonoBehaviour {
	
	[SerializeField]private float timeToSpawn = 4f;
	[SerializeField]private GameObject virusPrefab;
	private List<VirusProtectorMode> allVirus = new List<VirusProtectorMode>();
	private Vector3 startPos;
	private VirusProtectorMode tmpVirus;
	private GameObject tmpGO;
	private int virusToSpawn;
	private WaitForSeconds waitFor1Second = new WaitForSeconds(1f);
	private bool canSpawn;
	private float tmrToSpawn;
	
	void Start () {
		startPos = transform.position - Vector3.right;
		InstantiateVirus(1, false);
		canSpawn = true;
	}
	
	void Update()
	{
		if (GameController.instance.stop) return;
		
		if (Time.time > tmrToSpawn)
		{
			canSpawn = true;
			
			tmrToSpawn = Time.time + Random.Range(0.3f, timeToSpawn);
		}
		
		if (virusToSpawn>0)
		{
			if (canSpawn)
			{
				virusToSpawn--;
				SpawnVirus();
			}
		}
	}
	
	public void StopAllVirus()
	{
		for (int c=0;c<allVirus.Count;c++)
		{
			allVirus[c].StopMoving();
		}
	}
	
	public void InstantiateVirus(int qnt, bool isMoving)
	{
		for (int c=0;c<qnt;c++)
		{
			tmpGO = Instantiate(virusPrefab, startPos, Quaternion.identity) as GameObject;
			tmpVirus = tmpGO.GetComponent<VirusProtectorMode>();
			tmpVirus.transform.SetParent(transform);
			if (isMoving) tmpVirus.StartMoving(startPos);
			else tmpVirus.StopMoving();
			allVirus.Add(tmpVirus);
		}
	}
	
	public void SpawnVirus()
	{
		for (int c=0;c<allVirus.Count;c++)
		{
			if (allVirus[c].canRespawn)
			{
				allVirus[c].StartMoving(startPos);
				canSpawn = false;
				break;
			}
			
			if (c==allVirus.Count-1)
			{
				//adiciona mais um virus
				InstantiateVirus(1, true);
				canSpawn = false;
				break;
			}
		}
	}
	
	public void VirusToSpawn()
	{
		virusToSpawn++;
	}
	
	IEnumerator Spawned()
	{
		canSpawn = false;
		yield return waitFor1Second;
		canSpawn = true;	
	}
}
