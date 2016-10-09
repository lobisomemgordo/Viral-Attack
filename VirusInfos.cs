using UnityEngine;
using System.Collections;

[System.Serializable]
public class DiseaseInfo
{
	//essa classe guarda o nome e os sintomas da doença causada pelo vírus
	public string name;
	public string symptoms;
}

[System.Serializable]
public class VirusInfo
{
	//essa classe guarda todas as informações do vírus
	//como o nome, acido nucleico, formato, célula alvo, ciclo de vida, estrutura e as sprites do vírus em sí e da célula alvo
	public string name;
	public string nucleicAcid;
	public string shape;
	public string targetCell;
	public Sprite targetCellSprite;
	public string lifeCycle;
	public string structure;
	public Sprite sprite;
	
	//todo vírus possui uma doença que será definido na própria informação do vírus
	public DiseaseInfo disease;
}

public class VirusInfos : MonoBehaviour {
	
	//instância da classe que será usada para facilitar o acesso de outras classes
	public static VirusInfos instance;
	
	//guarda todas as informações dos vírus e suas doenças
	public VirusInfo[] allTheVirus = new VirusInfo[8];
	
	void Awake()
	{
		//se já houver uma instância, deleta a atual
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}
		
		//define que a instância é a própria classe
		instance = this;
		
		//ordena que o game object não seja destruído, ou seja, irá persistir entre as cenas
		DontDestroyOnLoad(gameObject);
	}
	
}
