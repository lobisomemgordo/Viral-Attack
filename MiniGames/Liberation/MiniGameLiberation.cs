using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MiniGameLiberation : MonoBehaviour {
	
	//keepGoing seria a velocidade do grupo de vírus que brotou da célula
	//multiplier seria o multiplicador do tempo da interpolação do movimento, podendo deixar mais lerdo ou mais rápido
	[SerializeField]private float keepGoingSpeed = 2f, multiplier = 1f;
	//são as posições em que começa e termina o movimento de acertar no limite da célula
	private Transform startPos, endPos;
	//referência para o controlador da camera para acionar a função Shake
	private CameraController camController;
	//transform do player para a interpolação do movimento
	private Transform playerTransform;
	//é a força da tremida da camera da etapa liberação
	private Vector3 xyVector = new Vector3(0.05f,0.05f,0f);
	//o contador das vezes em que o o jogador atingiu o limite da célula
	[SerializeField]private int step;
	//keepGoin fará com que o grupo de vírus continue subindo automaticamente, sem necessidade de toque
	//stopMovingUp seria para evitar que o jogador consiga tocar na tela no momento em que o grupo de vírus está decendo(indo em direção ao startPos)
	//stop seria para parar tudo
	[SerializeField]private bool keepGoing, stopMovingUp, stop;
	//vectors3 que serão usados
	private Vector3 vecUp = Vector3.up, vecOne = Vector3.one, vecZero = Vector3.zero;
	//referência de todos os grupos de virus
	private MiniGameVirusGroup[] miniGameVirusGroups;
	//referência do pai dos grupos do virus para fazer com que fiquem invisíveis(escalonamento zero)
	[SerializeField]private Transform miniGameVirusGroupsParent;
	//força para mover no toque. Quanto mais força, mais rápido será o movimento
	private float strengthToMove;
	//informação do vírus atual como nome, doença e sintomas.
	private VirusInfo currentVirusInfo;
	
	void Start () {
		//busca as referências necessárias
		camController = FindObjectOfType<CameraController>();
		playerTransform = FindObjectOfType<PlayerController>().transform;
		miniGameVirusGroups = miniGameVirusGroupsParent.GetComponentsInChildren<MiniGameVirusGroup>();
		
		//escalona para zero o scale para assim fazer com que fique "invisível"
		miniGameVirusGroupsParent.localScale = vecZero;
		//faz com que o pai dos grupos do vírus seja filho do player
		miniGameVirusGroupsParent.SetParent(playerTransform);
		//zera a posição do pai dos grupos do vírus para que fique centralizado ao player
		miniGameVirusGroupsParent.localPosition = vecZero;
		//começa parado
		stop = true;
	}
	
	public void StartMiniGame(Transform _startPos, Transform _endPos)
	{
		//dá zoom na câmera
		camController.Zoom(true);
		//esconde todos os vírus para só mostrar os grupos de virus do mini game
		GameController.instance.player.HideAllVirus();
		//seta os valores iniciais
		keepGoing = false;
		multiplier = 0.7f;
		step = 0;
		startPos = _startPos;
		endPos = _endPos;
		stop = false;
		currentVirusInfo = VirusInfos.instance.allTheVirus[GameController.instance.player.currentVirusID];
		playerTransform.position = startPos.position;
		playerTransform.rotation = startPos.rotation;
		//mostra o grupo de vírus de acordo com o vírus atual
		ShowVirusGroup(true);
		
		//desativa o envelope e as espiculas
		miniGameVirusGroups[GameController.instance.player.currentVirusID].ShowSprites(false);
		//escalona o pai dos grupos de vírus para que fique visível
		miniGameVirusGroupsParent.localScale = vecOne;
	}
	
	void FinishedMiniGame()
	{
		//volta com os valores iniciais
		multiplier = 0.7f;
		step =0;
		keepGoing = false;
		
		//avisa ao game controller que acabou
		GameController.instance.StartLifeCycle();
		//mostra o painel de sucesso com o nome do vírus, a doença causada por ele e os sintomas
		GameController.instance.interfaceScript.victoryMenu.ShowVictoryMenu("Fase completa! O vírus <color=red>" + currentVirusInfo.name + "</color> causa a doença <color=red>" + currentVirusInfo.disease.name + "</color> e os sintomas são: <color=red>" + currentVirusInfo.disease.symptoms + "</color>.");
	}
	
	void ShowVirusGroup(bool show)
	{
		//essa função fará com que seja ativado apenas o grupo do vírus atual
		//ou esconder todos eles
		for (int c=0;c<miniGameVirusGroups.Length;c++)
		{
			if (show)
			{
				if (c == GameController.instance.player.currentVirusID) miniGameVirusGroups[c].gameObject.SetActive(true);
				else miniGameVirusGroups[c].gameObject.SetActive(false);
			}
			else//esconde todos
				miniGameVirusGroups[c].gameObject.SetActive(false);
		}
	}
	
	void Update () {
		
		//se não for o ciclo de liberação, sai
		if (GameController.instance.cicloDeVida != GameController.CicloDeVida.liberacao) return;
		
		//se não existir a referência do player transform, sai
		if (playerTransform == null) return;
		
		//se for pra continuar seguindo, irá mover o vírus automaticamente para cima
		if (keepGoing) playerTransform.Translate(vecUp * keepGoingSpeed * Time.deltaTime);
		
		//para evitar erros, verifica se as referências são nulas ou se o jogo parou. Se sim, sai
		if (startPos == null || endPos == null || GameController.instance.stop) return;
		
		//enquanto houver toque na tela
		if (GameController.instance.touchCounter.touchCount>0)
		{
			//se não for para parar de subir
			if (!stopMovingUp)
			{
				//calcula a força e move o player(grupo de vírus)
				strengthToMove = ((float)step+1*20) * GameController.instance.touchCounter.touchCount * 0.00010f;
				playerTransform.Translate(vecUp * strengthToMove);
				//verifica se a posição do player for perto da posição final
				if (Vector3.Distance(playerTransform.position, endPos.position) < 0.1f)
				{
					//se for, adiciona mais um step e assim verifica se já acabou ou não
					FinishedStep();
				}
			}
		}
		else if (!keepGoing)
		{
			//volta mais rapido se ele já chegou no topo
			if (stopMovingUp)
				playerTransform.position = Vector3.Lerp(playerTransform.position, startPos.position, multiplier * Time.deltaTime);
			else//se ele ta voltando por não clicar, volta mais lento
				playerTransform.position = Vector3.Lerp(playerTransform.position, startPos.position, multiplier /2f * Time.deltaTime);
		}
		
		//se a posição do player for perto da posição inicial
		if (Vector3.Distance(playerTransform.position, startPos.position) < 0.3f)
		{
			//habita que o jogar consiga tocar na tela para mover para cima o grupo de vírus
			stopMovingUp = false;
			//ativa o contador de toques
			GameController.instance.touchCounter.enabled = true;
		}
	}
	
	void FinishedStep()
	{
		//se ja acabou, não vai de novo
		if (stop) return;
		
		//aumenta um passo
		step++;
		//aumenta o multiplicador de tempo para que fique mais rápido a interpolação do movimento
		multiplier+= 0.3f;
		//ativa o shake da câmera
		ShakeCamera();
		
		//se ainda não acabou
		if (step < 3)
		{
			//reseta o contador de toques
			GameController.instance.touchCounter.ResetCount();
			GameController.instance.touchCounter.enabled = false;
			//impede que o jogador consiga tocar na tela para mover o grupo de vírus para cima
			stopMovingUp = true;
		}
		else//se acabou(se atingiu 3+ vezes o limite da célula)
		{
			//para tudo
			stop = true;
			//mostra os receptores e os envelopes de todos os vírus do grupo
			miniGameVirusGroups[GameController.instance.player.currentVirusID].ShowSprites(true);
			//aciona o comando para mostrar o evelope exterior da célula
			Invoke("EnableCellExterior",1f);
			//mostra uma informação básica que explica o que está ocorrendo
			GameController.instance.interfaceScript.simpleInfo.ShowSimpleTip("É neste momento que o vírus adquire o envelope e se torna um vírus maduro chamado de vírion");
			//depois de 8 segundos será acionado a finalização do mini game
			Invoke("FinishedMiniGame", 8f);
			//depois que acabar, ativa o keepGoing para o jogador continuar subindo e assim sair da célula completamente
			keepGoing = true;
		}
	}
	
	void EnableCellExterior()
	{
		//essa função será acionada quando o grupo de vírus sair da célula
		//quando sair, avisa para a célula mostrar a parte exterior dela mesma
		GameController.instance.currentCell.DisableAnimations();
	}
	
	void ShakeCamera()
	{
		//ativa a função de vibrar o aparelho
		Handheld.Vibrate();
		//ativa o shake da câmera
		camController.ShakeCamera(xyVector * (step*4));
	}
}
