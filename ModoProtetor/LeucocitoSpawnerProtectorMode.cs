using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeucocitoSpawnerProtectorMode : MonoBehaviour {
	
	//verifica se está pronto para instanciar
	[SerializeField]private bool instantiatingInScene;
	//quantidade de leucocitos que inicializarão instanciados na cena e poderão ser usados quando requisitados
	[SerializeField]private int initializeQnt = 10;
	//prefab do leucocito
	[SerializeField]private GameObject[] leucocitosPrefab;
	//lista dos neutrófilos instanciados
	private List<LeucocitoProtectorMode> smallLeucocitos = new List<LeucocitoProtectorMode>();
	//lista dos macrófagos instanciados
	private List<LeucocitoProtectorMode> bigLeucocitos = new List<LeucocitoProtectorMode>();
	//game object instanciado temporariamente, seria os novos leucocitos
	private GameObject tmpGO;
	//câmera do jogo que será usada para converter a posição da tela para o mundo do jogo
	private Camera cam;
	//guarda qual leucocito o jogador selecionou e quantos estão sendo usados no tado
	private int leucocitoChoosen, leucocitoInUse;
	//guardará a posição do toque na tela, ou seja, onde o jogador quer colocar o leucocito
	private Vector3 tmpPos;
	//referência temporária do leucocito para acionar suas funções no momento em que for instanciado
	private LeucocitoProtectorMode tmpLeucocito;
	
	void Start () {
		//guarda a referência da câmera
		cam = Camera.main;
		
		//os leucocitos usam algumas referências de outras classes
		//se instanciasse agora, eles não teriam ainda essas referências de outras classes pois elas ainda não foram instanciadas
		//para evitar erros de null reference foi colocado uma espera na inicialização
		Invoke("DelayStart", 1f);
	}
	
	void DelayStart()
	{
		for (int k=0;k<initializeQnt;k++)
		{
			//percorre o vetor de prefabs dos leucocitos que no caso são 2
			for (int i=0;i<2;i++)
			{
				//instancia, seta ele como filho do leucocitoSpawner e já guarda na lista
				tmpGO = Instantiate(leucocitosPrefab[i], transform.position, Quaternion.identity, transform) as GameObject;
				tmpLeucocito = tmpGO.GetComponent<LeucocitoProtectorMode>();
				tmpLeucocito.HideLeucocito();
				
				//adiciona na lista
				if (i==0) smallLeucocitos.Add(tmpLeucocito);
				else bigLeucocitos.Add(tmpLeucocito);
			}
		}
	}
	
	void Update () {
		//se ele é pra instanciar
		if (instantiatingInScene && GameController.instance.stop == false)
		{
			//se tocou na tela e pode ser instanciado
			if (Input.GetMouseButtonDown(0) && CanSpawn())
			{
				//guarda a posição do toque na tela. A posição é na tela então é preciso converter a posição da tela para o mundo
				tmpPos = cam.ScreenToWorldPoint(Input.mousePosition);
				//define o Z igual ao do spawner para que não tenha problemas do leucocito sumir ou não aparecer na tela
				tmpPos.z = transform.position.z;
				
				//se tocou em uma área permitida
				if (tmpPos.x <=6f)
				{
					//ativa um leucocito
					EnableLeucocito(tmpPos);
					
					//tira o booleano para parar a verificação
					instantiatingInScene = false;
				}
				else//ele tocou na borda onde há os botões
				{
					//toca o som de erro
					AudioManager.instance.PlayOneShotSound(AudioManager.instance.sound_wrong);
				}
			}
		}
	}
	
	bool CanSpawn()
	{
		//só vai poder dar o spawn se a quantidade de leucocitos na cena for menor que a wave atual(+1 pq a wave inicial é 0)
		if (leucocitoInUse < ProtectorModeController.instance.wave+1) return true;
		else
		{
			//toca o som de erro
			AudioManager.instance.PlayOneShotSound(AudioManager.instance.sound_wrong);
			return false;
		}
	}
	
	public void LeucocitoDead()
	{
		//diminui a contagem dos leucocitos usados
		leucocitoInUse --;
	}
	
	public void SelectLeucocito(int num)
	{
		//o jogador selecionou algum dos leucocitos
		
		//se prepara pra instanciar na cena
		instantiatingInScene = true;
		//guarda qual foi o leucócito que foi selecionado
		leucocitoChoosen = num;
	}
	
	void EnableLeucocito(Vector3 position)
	{
		//incrementa o contador de leucocitos usados
		leucocitoInUse ++;
		
		//toca o som de click
		AudioManager.instance.PlayOneShotSound(AudioManager.instance.sound_click);
		
		//se for neutrófilo
		if (leucocitoChoosen == 0)//small
		{
			//percorre todo o vetor de neutrofilos para buscar algum disponível
			for (int c=0;c<smallLeucocitos.Count;c++)
			{
				if (smallLeucocitos[c].stop)
				{
					//se existe um disponível, ativa ele
					smallLeucocitos[c].SpawnLeucocito(position);
					break;
				}
				
				//se já chegou na ultima contagem quer dizer que não existe nenhum disponível
				if (c==smallLeucocitos.Count-1)
				{
					//então vai ser instanciado um novo
					tmpGO = Instantiate(leucocitosPrefab[leucocitoChoosen], transform.position, Quaternion.identity, transform) as GameObject;
					
					//guarda a referência dele para avisar que está ativado
					tmpLeucocito = tmpGO.GetComponent<LeucocitoProtectorMode>();
					tmpLeucocito.SpawnLeucocito(position);
					//adiciona na lista
					smallLeucocitos.Add(tmpLeucocito);
					break;
				}
			}
		}
		else//é o macrófago
		{
			//percorre todo o vetor de macrófagos para buscar algum disponível
			for (int c=0;c<bigLeucocitos.Count;c++)
			{
				if (bigLeucocitos[c].stop)
				{
					bigLeucocitos[c].SpawnLeucocito(position);
					break;
				}
				
				//se já chegou na ultima contagem quer dizer que não existe nenhum disponível
				if (c==bigLeucocitos.Count-1)
				{
					//então vai ser instanciado um novo
					tmpGO = Instantiate(leucocitosPrefab[leucocitoChoosen], transform.position, Quaternion.identity, transform) as GameObject;
					//guarda a referência dele para avisar que está ativado
					tmpLeucocito = tmpGO.GetComponent<LeucocitoProtectorMode>();
					tmpLeucocito.SpawnLeucocito(position);
					//adiciona na lista
					bigLeucocitos.Add(tmpLeucocito);
					break;
				}
			}
		}
	}
}
