using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnElements : MonoBehaviour {
	
	//guarda os prefabs das células
	[SerializeField]private GameObject[] cells;
	//guarda os blocos de elementos(seriam os blocks com os leucócitos)
	[SerializeField]private GameObject[] elementBlockPrefab;
	//guarda o transform do pai dos leucócitos e das células
	//servirá para que os leucócitos e células instanciados sejam instanciados como filhos deles
	[SerializeField]private Transform protectorsFather, cellsFather;
	//guarda os leveis de blocos para instanciar. São 3 leveis
	[SerializeField]private Transform[] spawnBlockFathers;
	//seriam todos os blocks(sprites quadradas) que podem ser instanciados dos 3 leveis.
	private SpriteRenderer[] spawnBlocks0, spawnBlocks1, spawnBlocks2;
	//guarda todos os blocos disponíveis para ser instanciado as células
	//é feito isso porque os blocos da extremidade não serão todos usados, há apenas 6 células, ou seja, só precisa 6 blocos
	//os blocos onde ficariam as células seriam aleatórios
	private List<Transform> availableBlocks2 = new List<Transform>();
	//uma variável temporária que guarda a posição em que tal elemento vai ser instânciado
	private Vector3 position = Vector3.zero;
	
	void Start () {
		//busca os blocos que podem ser instanciados dos 3 leveis
		spawnBlocks0 = spawnBlockFathers[0].GetComponentsInChildren<SpriteRenderer>();
		spawnBlocks1 = spawnBlockFathers[1].GetComponentsInChildren<SpriteRenderer>();
		spawnBlocks2 = spawnBlockFathers[2].GetComponentsInChildren<SpriteRenderer>();
		
		//o bloco de spawn da extremidade(index=2) seria o maior
		//neste caso foi aproveitado o for dele fazendo desnecessário criar um for para os 3 leveis
		for (int c= 0;c<spawnBlocks2.Length;c++)
		{
			//enquanto existir blocos, de acordo com o tamanho do vetor, irá desativar a sprite(quadrado branco)
			if (c<spawnBlocks0.Length) spawnBlocks0[c].enabled = false;
			if (c<spawnBlocks1.Length) spawnBlocks1[c].enabled = false;
			
			//spawnBlocks2 sempre vai ter pq o for já é o limite dele
			spawnBlocks2[c].enabled = false;
			//adiciona na lista dos blocos da extremidade que estão disponíveis
			availableBlocks2.Add(spawnBlocks2[c].transform);
		}
		
		//manda instanciar tudo
		SpawnAll();
	}
	
	void SpawnAll () {
		//instancia as células e os protetores
		SpawnCells();
		SpawnProtectors();		
	}
	
	void SpawnCells()
	{
		//guarda os valores iniciais salvos
		int randNum, virusID = PlayerPrefs.GetInt("currentVirusID"), isLifeCycleOnly = PlayerPrefs.GetInt("isLifeCycleOnly");
		Vector3 newPos;
		
		if (isLifeCycleOnly==1)
		{
			if (virusID==0)//influenza
			{
				//irá instanciar a célula ciliada no bloco inicial
				Instantiate(cells[0], spawnBlocks0[7].transform.position, Quaternion.identity, cellsFather);
			}
			if (virusID==7)//hiv
			{
				//irá instanciar a célula CD4 no bloco inicial
				Instantiate(cells[5], spawnBlocks0[7].transform.position, Quaternion.identity, cellsFather);
			}
		}
		else//se for modo viral normal
		{
			//instância cada uma das células
			for (int c=0;c<cells.Length;c++)
			{
				//a posição das células sempre será nas extremidades(blocks2)
				//escolhe uma posição aleatoria que esta disponível
				randNum = Random.Range(0, availableBlocks2.Count);
				//guarda a posição daquele bloco
				newPos = availableBlocks2[randNum].position;
				//ja foi usado, então pode remover da lista
				availableBlocks2.RemoveAt(randNum);
				
				//instancia na cena a célula
				Instantiate(cells[c], newPos, Quaternion.identity, cellsFather);
			}
		}
	}
	
	void SpawnProtectors()
	{
		//guarda os valores iniciais salvos
		int randNum, min, virusID = PlayerPrefs.GetInt("currentVirusID"), isLifeCycleOnly = PlayerPrefs.GetInt("isLifeCycleOnly");
		
		//se for apenas o ciclo de vida, não é pra instanciar os protetores
		if (isLifeCycleOnly == 1) return;
		
		if (virusID == 0) virusID = 1;//previne que fique sem aparecer element blocks
		
		for (int c=0;c<spawnBlocks0.Length;c++)
		{
			//define a partir de quais elements blocks irão ser mostrados
			min = (int)((float)virusID/1.5f);
			//os elements blocks vão variar desde nada(0) até o id do vírus multiplicado por 2
			randNum = Random.Range(min, virusID*2);
	
			//se passar do limite de elements blocks que há para instanciar, seta como o máximo
			if (randNum >= elementBlockPrefab.Length) randNum = elementBlockPrefab.Length-1;
			
			//só vai instanciar se haver algo no element block
			if (randNum>0){
				Instantiate(elementBlockPrefab[randNum], spawnBlocks0[c].transform.position, Quaternion.identity, protectorsFather);
			}
				
		}
		
		for (int c=0;c<spawnBlocks1.Length;c++)
		{
			//define a partir de quais elements blocks irão ser mostrados
			min = (int)((float)virusID/1.5f);
			
			//os elements blocks vão variar desde nada(0) até o id do vírus multiplicado por 2
			randNum = Random.Range(min, virusID*2);
			
			//se passar do limite de elements blocks que há para instanciar, seta como o máximo
			if (randNum >= elementBlockPrefab.Length) randNum = elementBlockPrefab.Length-1;
			
			if (randNum>0)//só vai instanciar se haver algo no element block
				Instantiate(elementBlockPrefab[randNum], spawnBlocks1[c].transform.position, Quaternion.identity, protectorsFather);
		}
	}
}
