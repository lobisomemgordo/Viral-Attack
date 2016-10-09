using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MiniGameIntegracao : MonoBehaviour {
	
	//referência da particula
	[SerializeField]private ParticleSystem particleEffect;
	//referência do transform da particula para posiciona-la
	private Transform particleEffectTransform;
	//referência dos segmentos do dna celular, que serão usados para ativar/desativar de acordo com o vírus
	[SerializeField]private GameObject[] segmentsDnaCell;
	//referência do dna viral
	private MiniGameDNAViral miniGameDnaViral;
	//referência do mini game atual
	private MiniGameDNA miniGameDNA;
	//referência do transform para posicionamento
	private Transform myTransform;
	//posição inicial
	private float originalLocalY;
	//layers que serão usados: um que ignora todas colisões e outro não
	private int miniGameLayer = 12,ignoreLayer = 13;
	//o virus atual
	private int currentVirusID;
	//variável temporária que pegará as informações do vírus atual
	private VirusInfo currentVirusInfo;
	
	void Start () {
		//guarda o transform
		myTransform = transform;
		//guarda o transform da particula
		particleEffectTransform = particleEffect.transform;
		//busca o dna viral
		miniGameDnaViral = GetComponentInChildren<MiniGameDNAViral>();	
		//guarda a posição inicial do mini game
		originalLocalY = myTransform.localPosition.y;
	}
	
	public void StartMiniGame(int currentVirus)
	{
		//interpola a posição para que fique visível
		myTransform.DOLocalMoveY(0f, 1f, false).SetEase(Ease.InExpo);
		
		//são 3 possibilidades: retornar o virus id 2 que já possui 2 segmentos, retornar o virus id 4 que já possui 4 segmentos
		//ou retornar o virus id 7 que no caso possui 6 segmentos então o valor do currentVirus é alterado
		if (currentVirus == 7) currentVirus =6;
		
		//manda preparar os segmentos do dna celular
		SetUpDnaCellSegments(currentVirus);
		
		//garante que possua a referência para evitar erros
		if (miniGameDnaViral == null) miniGameDnaViral = GetComponentInChildren<MiniGameDNAViral>();
		//manda preparar o dna viral
		miniGameDnaViral.SetUpDnaViralSegments(currentVirus);
		
		//ativa os colisodres
		EnableColliders(true);
		//guarda o ID do virus atual
		currentVirusID = GameController.instance.player.currentVirusID;
		//guarda a informação do vírus atual
		currentVirusInfo = VirusInfos.instance.allTheVirus[currentVirusID];
	}
	
	void EnableColliders(bool enableAll)
	{
		//essa função alterará o layer de todos os elementos do mini game para que ignore ou receba as colisões
		
		if (enableAll)
		{
			miniGameDnaViral.gameObject.layer = miniGameLayer;
			
			for (int c=0;c<segmentsDnaCell.Length;c++)
			{
				segmentsDnaCell[c].gameObject.layer = miniGameLayer;
				for (int i=0;i<segmentsDnaCell[c].transform.childCount;i++)
				{
					segmentsDnaCell[c].transform.GetChild(i).gameObject.layer = miniGameLayer;
				}
			}
		}
		else
		{
			miniGameDnaViral.gameObject.layer = ignoreLayer;
			
			for (int c=0;c<segmentsDnaCell.Length;c++)
			{
				segmentsDnaCell[c].gameObject.layer = ignoreLayer;
				for (int i=0;i<segmentsDnaCell[c].transform.childCount;i++)
				{
					segmentsDnaCell[c].transform.GetChild(i).gameObject.layer = ignoreLayer;
				}
			}
		}
	}
	
	public void FinishedMiniGame()
	{
		//será chamado quando o jogo for finalizado
		
		//desativa os colliders
		EnableColliders(false);
		//mostra o painel de vitória junto com as informações do vírus como nome, doença causada e sintomas
		GameController.instance.interfaceScript.victoryMenu.ShowVictoryMenu("Fase completa! O vírus <color=red>" + currentVirusInfo.name + "</color> causa a doença <color=red>" + currentVirusInfo.disease.name + "</color> e os sintomas são: <color=red>" + currentVirusInfo.disease.symptoms + "</color>.");
	}
	
	public void PlayParticleEffect(Vector3 position)
	{
		//será chamado pelo DNA viral
		
		//posicionará e tocará a partícula de acordo com a posição passada
		particleEffectTransform.position = position;
		particleEffect.Stop();
		particleEffect.Play();
	}
	
	public void SetUpDnaCellSegments(int segmentQnt)
	{
		//irá preparar os segmentos de dna
		
		//conforme o vírus, a quantidade de filamentos de DNA variam podendo ser 2(mínimo), 4 ou 6(máximo)
		for (int c=0;c<segmentsDnaCell.Length;c++)
		{
			miniGameDNA = segmentsDnaCell[c].GetComponent<MiniGameDNA>();
			if (c<segmentQnt)
			{
				segmentsDnaCell[c].SetActive(true);
				//reseta o dna celular
				miniGameDNA.ResetDna();
			}
			else segmentsDnaCell[c].SetActive(false);
		}
	}
}
