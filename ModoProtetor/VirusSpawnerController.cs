using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class VirusSpawnerController : MonoBehaviour {
	
	//tempo mínimo e máximo para ser acionado o comando que irá mandar os instanciadores de vírus instanciarem
	[SerializeField]private float minTimeToSpawn = 2f, maxTimeToSpawn = 5f;
	//quantidade de vírus que serão instanciados ao mesmo tempo
	[SerializeField]private int qntToSpawnSameTime = 3;
	//instanciadores dos vírus que serão instanciados
	[SerializeField]private VirusSpawner[] virusSpawners;
	//imagem da barra que irá ser preenchida de acordo com a quantidade de vírus que faltam ser destruídos
	[SerializeField]private Image remainingVirusImage;
	//guarda a onda atual, a quantidade total de vírus para instanciar e instanciadas e a quantidade de vírus ativos no momento
	//tmpWave guardaria a wave atual+1, pois a primeira onda é 0. 
	//tmpWave servirá para achar o máximo de instanciadores disponíveis de acordo com a wave atual
	private int wave, totalVirusToSpawn, totalVirusSpawned, totalVirusInScene, tmpWave;
	//é o timer que irá guardar o tempo que falta para acionar os instanciadores de vírus
	private float tmrToSpawn;
	//booleano que verifica se está sendo instanciado
	private bool spawning;
	
	void Start () {
		//guarda todos os instanciadores de vírus
		virusSpawners = GetComponentsInChildren<VirusSpawner>();
	}
	
	void FixedUpdate () {
		
		//se não está instanciado, sai
		if (!spawning) return;
		
		//verifica se ta na hora de acionar os instanciadores
		if (Time.time > tmrToSpawn)
		{
			//percorre o vetor de acordo com o máximo que é pra spawnar ao mesmo tempo
			for (int c=0;c<=qntToSpawnSameTime;c++)
			{
				//enquanto não for instanciado todos os vírus, continua
				if (totalVirusSpawned < totalVirusToSpawn)
				{
					//incrementa a quantidade de vírus instanciado
					totalVirusSpawned++;
					//guarda o valor que será usado para achar o máximo de spawner disponivel
					tmpWave = wave+1;
					//previne que passa do limite disponível de spawner
					if (tmpWave>virusSpawners.Length) tmpWave = virusSpawners.Length;
					
					//ativa um instanciador aleatório
					virusSpawners[Random.Range(0, tmpWave)].VirusToSpawn();
					//seta o próximo timer
					tmrToSpawn = Time.time+ Random.Range(minTimeToSpawn, maxTimeToSpawn);
				}
				else//se ja estiver spawnado todos os vírus
				{
					//para de acionar os instanciadores
					spawning = false;
					break;
				}
			}
		}
	}
	
	public void StartSpawning(int _wave, int qnt)
	{
		//zera a barra
		remainingVirusImage.fillAmount = 0f;
		//guarda qual é a onda atual
		wave = _wave;
		//guarda a quantidade certa de vírus a serem instanciados
		totalVirusToSpawn = totalVirusInScene = qnt;
		//zera o contador
		totalVirusSpawned = 0;
		
		//ativa o spawner
		spawning = true;
	}
	
	public int LessVirusInScene()
	{
		//diminui o contador de vírus que estão ativos
		totalVirusInScene--;
		
		//ele diminui ao invés de aumentar, então foi colocado para subtrair por 1(que é o valor máximo)
		remainingVirusImage.fillAmount = 1f - ((float)totalVirusInScene/(float)totalVirusToSpawn);
		
		//retorna a quantidade atual de vírus ativos
		return totalVirusInScene;
	}
}
