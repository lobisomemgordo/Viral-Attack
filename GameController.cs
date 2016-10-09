using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	
	//instância que será usada pelas outras classes para facilidade de acesso
	public static GameController instance;
	
	//enumeração de todas as etapas dos ciclos de vida
	public enum CicloDeVida
	{
		nenhum,
		adesao,
		penetracao1,
		penetracao2,
		biossintese1,
		biossintese2,
		integracao1,
		integracao2,
		integracao1_HIV,
		integracao2_HIV,
		integracao3_HIV,
		maturacao,
		liberacao
	}
	
	//booleano que verifica se está no modo protetor
	//será setado direto no inspector
	public bool isProtectorMode;
	//variável que guarda qual é a etapa do ciclo de vida atual
	public CicloDeVida cicloDeVida = CicloDeVida.nenhum;
	//variáveis que verificarão se o jogo está parado, se está parado só para npc ou se está no ciclo de vida
	public bool stop, stopNpc, isInLifeCycle;
	//referência do contador de toques e deslizamentos que serão usados por algumas interações do ciclo de vida
	public TouchCounter touchCounter;
	public ShakeCounter shakeCounter;
	//guarda o receptor da célula em que o vírus se fixou. Usado para atualizar a nova posição inicial do receptor
	//a posição inicial do receptor muda no momento em que o vírus entra na célula, é feito isso para que o receptor não deixe o vírus sair novamente
	[HideInInspector]public Espicula currentEspicula;
	//referência do controlador da câmera que será usado para dar zoom in ou zoom out
	[HideInInspector]public CameraController cameraController;
	//referência do player controller para acessar qual é a espícula dele, qual o ID do vírus atual e qual é o vírus(classe) que ele possui
	[HideInInspector]public PlayerController player;
	//guada a célula atual que será utilizada pelo mini game de liberação para pegar qual é a posição inicial e a posição limite da célula
	[HideInInspector]public Celula currentCell;
	
	//referência que possuirá todas as referências dos mini games do jogo
	private MiniGameController miniGameController;
	//guarda todas as células do jogo. Será usado para achar qual é a célula alvo do vírus atual
	private Celula[] cells;
	//refer~encia que possuirá todas as referências das classes de interface
	[HideInInspector]public InterfaceScript interfaceScript;
	//waitForSeconds que serão usados nas corrotinas
	private WaitForSeconds waitFor2seconds = new WaitForSeconds(2f), waitFor1seconds = new WaitForSeconds(1f), waitFor4seconds = new WaitForSeconds(4f);
	
	
	void Awake()
	{
		if (instance!= null)
		{
			Destroy(gameObject);
			return;
		}
		
		//guarda a instância do jogo que será usado caso ainda não exista uma
		instance = this;
	}
	
	void Start () {
		//guarda todas as referências necessárias
		player = FindObjectOfType<PlayerController>();
		interfaceScript = FindObjectOfType<InterfaceScript>();
		cameraController = FindObjectOfType<CameraController>();
		miniGameController = FindObjectOfType<MiniGameController>();	
		Invoke("DelayStart",0.1f);
		
		//carrega qual foi o último vírus jogado
		//irá setar o currentVirusID no playerController
		LoadData();
	}
	
	void DelayStart()
	{
		//cells tem que ta no delayStart pq são instanciadas depois
		//busca as células do jogo
		cells = FindObjectsOfType<Celula>();
		
		//desativa o joystick de rotação
		GameController.instance.interfaceScript.ShowJoysticks(false, 1);
	}
	
	public void FindTargetCell()
	{
		//quer dizer que começou o modo viral, toca a musica desse modo
		AudioManager.instance.PlayMusic(AudioManager.instance.music_ViralMode);
		
		//percorre todas as células em busca da célula alvo
		for (int c=0;c<cells.Length;c++)
		{
			//a célula alvo seria a célula com os receptores compatíveis, ou seja, iguais
			if (cells[c].espicula.espiculaSprite == player.espicula.espiculaSprite)
			{
				player.targetCellTransform = cells[c].transform;
				currentCell = cells[c];
				break;
			}
		}
		
		//mostra o painel de introdução do vírus mostrando qual é a célula alvo
		interfaceScript.virusIntroPanel.ShowVirusIntro(player.currentVirusID);
		//toca a risadinha do virus
		AudioManager.instance.VirusSound();
	}
	
	public void FindTargetCellNucleus()
	{
		//esse método irá setar o alvo da parte onde o jogador leva o rna/dna até o núcleo
		//o alvo seria o núcleo, então é guardado a referência para que a seta que indica a direção aponte corretamente
		for (int c=0;c<cells.Length;c++)
		{
			if (cells[c].espicula.espiculaSprite == player.espicula.espiculaSprite)
			{
				player.targetNucleusTransform = cells[c].nucleusTransform;
				break;
			}
		}
	}
	
	public void SetEspicula(Espicula _espicula)
	{
		//é chamado pelo receptor da célula no qual o vírus se conectou
		//guarda qual é a espícula da célula que está sendo usada
		//serve para, posteriormente, alterar a posição inicial desse receptor na parte da penetração
		currentEspicula = _espicula;
	}
	
	public void VirusInsideTheCell()
	{
		//irá setar uma nova posição inicial para o receptor da célula, que seria uma posição dentro da célula
		//é feito isso para que o receptor não leve o vírus para fora da célula
		currentEspicula.SetNewOriginalPos();
		//libera o capsídio e os receptores virais
		player.currentVirus.SetObjectsFree();
	}
	
	public void ReloadScene()
	{
		//recarrega a cena atual
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
	
	public void LoadSecene(string sceneName)
	{
		//carrega a cena de acordo com o parâmetro passado
		SceneManager.LoadScene(sceneName);
	}
	
	public void ShowElementInfo(ElementInfo elementInfo)
	{
		//usado por algumas classes para mostrar informação do elemento
		interfaceScript.elementInfoPanel.ShowElementInfo(elementInfo);
	}
	
	public void SetPauseGame(bool newValue)
	{
		//esse método irá pausar/despausar o jogo
		stop = newValue;
		
		//se for para pausar, pausa os npcs tbm
		//se for para despausar, inicia uma corrotina que despausará os npcs depois de 2 segundos
		if (newValue) stopNpc = newValue;
		else StartCoroutine(ResumeNpcs());
	}
	
	IEnumerator ResumeNpcs()
	{
		//é despausado depois de 2 segundos para que dê tempo de o jogador acionar o joystick de movimento e iniciar seu movimento
		
		//espera 2 segundos
		yield return waitFor2seconds;
		
		//despausa os npcs
		stopNpc = false;
	}
	
	public void StartLifeCycle()
	{
		//o padrão é tudo pausado
		stop = stopNpc = true;
		
		//define que está no ciclo de vida
		isInLifeCycle = true;
		
		//verifica qual a etapa do ciclo de vida
		switch(cicloDeVida)
		{
			//se não estiver em nenhuma etapa
			case CicloDeVida.nenhum:
				//inicia a adesão que seria a primeira etapa do ciclo de vida
				//aqui o jogador deverá conectar o receptor do vírus no da célula
				cicloDeVida = CicloDeVida.adesao;
				
				//toca a risadinha do virus
				AudioManager.instance.VirusSound();
				//inicia a musica que ficará até o fim do ciclo de vida
				AudioManager.instance.PlayMusic(AudioManager.instance.music_LifeCycle);
			
				//mostra uma informação simples sobre os receptores
				interfaceScript.simpleInfo.ShowSimpleTip("A adesão só é possível se o receptor do vírion for compatível com o da célula");
				interfaceScript.ShowJoysticks(true, 0);//ativa o joystick de movimento
				interfaceScript.ShowJoysticks(true, 1);//ativa o joystick de rotação
			
				//se for virus que possui DNA, mostra a informação sobre o ciclo lisogênico
				//se for vírus RNA, mostra a informação sobre o ciclo lítico
				if (player.IsVirusDNA())
					interfaceScript.elementInfoPanel.ShowLifeCycleInfo(false);
				else interfaceScript.elementInfoPanel.ShowLifeCycleInfo(true);
				break;
			
			case CicloDeVida.adesao:
				//inicia a etapa penetração na qual o jogador deverá tocar na tela rápidamente para fazer com que o vírus entre na célula
				cicloDeVida = CicloDeVida.penetracao1;
			
				interfaceScript.ShowJoysticks(false, -1);//desativa todos joysticks, só vai usar o toque na tela
				interfaceScript.tutorialPanel.ShowTutorial(3);//clique na tela rapidamente
				//inicia o contador de toques
				touchCounter.StartCounting();
				break;
			
			case CicloDeVida.penetracao1:
				//inicia a etapa penetração 2 em que o jogador deverá deslizar seu dedo horizontalmente para liberar o capsídio viral
				cicloDeVida = CicloDeVida.penetracao2;
			
				//na etapa de adesão, o player(que possui todos os vírus), se torna filho do receptor da célula
				//se torna filho para assim seguir os movimentos do receptor da célula
				//agora que acabou a parte de penetração, o player volta a não ter pai, ou seja, é livre para se movimentar do jeito que quiser
				player.SetPlayerParent(null);
				interfaceScript.tutorialPanel.ShowTutorial(4);//deslize seu dedo horizontalmente para liberar o capsídio
				//começa a contar os deslizamentos
				shakeCounter.StartCounting();
				//para a contagem dos toques
				touchCounter.StopCounting();
				break;
			
			case CicloDeVida.penetracao2:
				//para a contagem dos deslizamentos
				shakeCounter.StopCounting();
			
				//se for virus DNA(ciclo lisogênico)
				if (player.IsVirusDNA())
				{
					if (player.currentVirusID == 7)// se for o hiv
					{
						//inicia a integracao 1 exclusiva para o HIV
						//o objetivo é converter seu RNA em DNA, miniGame da cobrinha
						cicloDeVida = CicloDeVida.integracao1_HIV;
						
						//desativa o joystick de movimento
						interfaceScript.ShowJoysticks(false, 0);
						//ativa o joystick de rotação
						interfaceScript.ShowJoysticks(true, 1);
						//integração 3.1 - snake tutorial
						interfaceScript.tutorialPanel.ShowTutorial(9);
						//inicia o mini game
						miniGameController.integracaoHIV.StartMiniGame();
						//dá zoom out
						cameraController.Zoom(false);
					}
					else//se for qualquer outro vírus de dna(papiloma ou varicela zóster)
					{
						//inicia a integracao1 normal
						//o objetivo é mover o vírus até o núcleo da célula
						cicloDeVida = CicloDeVida.integracao1;
						
						//busca o núcleo da célula para que a seta que indica a direção direcione para o núcleo
						FindTargetCellNucleus();
						//dá zoom in
						cameraController.Zoom(true);
						//ativa o joystick de movimento
						interfaceScript.ShowJoysticks(true, 0);
						//desativa o joystick de rotação
						interfaceScript.ShowJoysticks(false, 1);
						//integracao1- levar o dna viral até o núcleo da célula
						interfaceScript.tutorialPanel.ShowTutorial(7);
						//desabilita o virus atual e habilita só o rna viral
						player.ShowOnlyTheNucleicAcid(true, true);
						break;
					}
				}
				else//se for vírus RNA(lítico)
				{
					//inicia a biossintese1 em que o jogador deverá levar o rna mensageiro até os ribossomos para criar proteínas virais
					cicloDeVida = CicloDeVida.biossintese1;
					
					//busca o núcleo da célula para que a seta que indica a direção direcione para o núcleo
					FindTargetCellNucleus();
					//dá zoom in
					cameraController.Zoom(true);
					//ativa o joystick de movimento
					interfaceScript.ShowJoysticks(true, 0);
					//desativa o joystick de rotação
					interfaceScript.ShowJoysticks(false, 1);
					//biossintese 3.1 - mover até o núcleo da célula
					interfaceScript.tutorialPanel.ShowTutorial(5);
					//desabilita o virus HIV e habilita só o dna viral
					player.ShowOnlyTheNucleicAcid(true, false);
				}
				break;
			
			case CicloDeVida.integracao1_HIV:
				//inicia a etapa integracao2 onde é necessário levar o dna viral até o núcleo da célula
				cicloDeVida = CicloDeVida.integracao2_HIV;
			
				//busca o núcleo da célula para que a seta que indica a direção direcione para o núcleo
				FindTargetCellNucleus();
				//dá zoom in
				cameraController.Zoom(true);
				//ativa o joystick de movimento
				interfaceScript.ShowJoysticks(true, 0);
				//desativa o joystick de rotação
				interfaceScript.ShowJoysticks(false, 1);
				//integraçãoHIV 3.2 - mover até o núcleo da célula
				interfaceScript.tutorialPanel.ShowTutorial(10);
				//desabilita o virus HIV e habilita só o dna viral
				player.ShowOnlyTheNucleicAcid(true, true);
				break;
			
			case CicloDeVida.integracao2_HIV:
				//inicia a ultima etapa do HIV, o jogador deverá integrar o dna viral no dna da célula
				cicloDeVida = CicloDeVida.integracao3_HIV;
			
				//dá zoom out
				cameraController.Zoom(false);
				//desativa todos joysticks, só vai usar o toque na tela
				interfaceScript.ShowJoysticks(false, -1);
				//integraçãoHIV 3.3 - toque na tela para mover o dna viral
				interfaceScript.tutorialPanel.ShowTutorial(11);
				//inicia o mini game
				miniGameController.integracao.StartMiniGame(player.currentVirusID);
				break;
				
			case CicloDeVida.integracao1:
				//inicia a etapa integracao2 em que o jogador deve integrar o dna viral no dna da célula
				cicloDeVida = CicloDeVida.integracao2;
			
				//da zoom out
				cameraController.Zoom(false);
				//desativa todos joysticks, só vai usar o toque na tela
				interfaceScript.ShowJoysticks(false, -1);
				//integração 3.2 - toque na tela para mover o dna viral
				interfaceScript.tutorialPanel.ShowTutorial(8);
				//inicia o mini game
				miniGameController.integracao.StartMiniGame(player.currentVirusID);
				break;
			
			case CicloDeVida.integracao2: case CicloDeVida.integracao3_HIV:
				//finaliza o ciclo lisogênico
				//já é finalizado no método ElementInfoOk
				break;
			
			case CicloDeVida.biossintese1:
				//inicia a etapa biossintese2 onde o jogador deverá levar o rna mensageiro até os ribossomos para criar as proteínas virais
				cicloDeVida = CicloDeVida.biossintese2;
			
				//da zoom out
				cameraController.Zoom(false);
				//desativa todos joysticks, só vai usar o toque na tela
				interfaceScript.ShowJoysticks(false, -1);
				//integraçãoHIV 3.2 - toque na tela para mover o rna viral que rotaciona
				interfaceScript.tutorialPanel.ShowTutorial(6);
				//inicia o mini game
				miniGameController.biossintese.StartMiniGame(player.currentVirusID);
				break;
			
			case CicloDeVida.biossintese2:
				//inicia a etapa de maturação onde o jogador deverá memorizar as estruturas virais
				cicloDeVida = CicloDeVida.maturacao;
			
				//maturação 4 - memorize e acerte as partes virais
				interfaceScript.tutorialPanel.ShowTutorial(12);
				//inicia o mini game
				miniGameController.maturacao.StartMiniGame(player.currentVirusID);
				break;
			
			case CicloDeVida.maturacao:
				//inicia o processo de liberação: o jogador deverá tocar rapidamente para atingir o limite da célula e assim liberar os novos vírus
				cicloDeVida = CicloDeVida.liberacao;
			
				//desativa os acidos nucleicos do miniGame maturação
				player.ShowOnlyTheNucleicAcid(false,false);
				//começa a contar os toques na tela
				touchCounter.StartCounting();
				//liberação 5 - toque o mais rápido que puder para romper a célula
				interfaceScript.tutorialPanel.ShowTutorial(13);
				//inicia o mini game passando qual seria o ponto inicial(meio da célula) e o final(limite da célula)
				miniGameController.liberation.StartMiniGame(currentCell.miniGameStartPosToLiberate, currentCell.miniGameEndPosToLiberate);
				break;
			
			case CicloDeVida.liberacao:
				//é a ultima etapa, então reseta o ciclo para sair
				ResetCycle();
			
				//para o contador
				touchCounter.StopCounting();
				break;
		}
	}
	
	public void ResetCycle()
	{
		//finaliza o ciclo
		interfaceScript.ShowJoysticks(false, 1);
		isInLifeCycle = false;
		cicloDeVida = CicloDeVida.nenhum;
		
		//se for modo protetor, toca a musica dele
		if (isProtectorMode) AudioManager.instance.PlayMusic(AudioManager.instance.music_ProtectorMode);
		else//se não for, é o modo viral
			AudioManager.instance.PlayMusic(AudioManager.instance.music_ViralMode);
	}
	
	public void ReloadMiniGame()
	{
		//irá recarregar o mini game de acordo com a etapa do ciclo de vida
		switch(cicloDeVida)
		{
			//seria o mini game da cobrinha onde o jogador deve coletar a transcriptase reversa para converter seu rna em dna
			case CicloDeVida.integracao1_HIV:
				//mostra a informação básica do mini game
				interfaceScript.simpleInfo.ShowSimpleText("Cuidado para não bater nas bordas!");
				//manda iniciar o mini game novamente
				miniGameController.integracaoHIV.StartMiniGame();
				//dá zoom out
				cameraController.Zoom(false);
				break;
			
			//seria o mini game onde irá ocorrer a integração do dna viral no dna da celula
			case CicloDeVida.integracao2: case CicloDeVida.integracao3_HIV:
				shakeCounter.StopCounting();
				
				if (player.IsVirusDNA())
				{
					//mostra a informação básica do mini game
					interfaceScript.simpleInfo.ShowSimpleText("Toque na tela para mover o vírus!");
					//manda iniciar o mini game novamente
					miniGameController.integracao.StartMiniGame(player.currentVirusID);
					//dá zoom out
					cameraController.Zoom(false);
				}
				break;
			
			//seria o mini game onde o jogador precisa tocar na tela para mover o rna mensageiro
			//o objectivo é usar o rna mensageiro para atingir os ribossomos e assim criar as proteínas virais
			case CicloDeVida.biossintese2:
				//mostra a informação básica do mini game
				interfaceScript.simpleInfo.ShowSimpleText("Toque na tela no momento certo!");
				//manda iniciar o mini game novamente
				miniGameController.biossintese.StartMiniGame(player.currentVirusID);
				//dá zoom out
				cameraController.Zoom(false);
				break;
			
			//seria o mini game de memorização da estrutura viral
			case CicloDeVida.maturacao:
				//mostra a informação básica do mini game
				interfaceScript.simpleInfo.ShowSimpleText("Memorize a estrutura do vírus!");
				//manda iniciar o mini game novamente
				miniGameController.maturacao.StartMiniGame(player.currentVirusID);
				break;
		}
	}
	
	public void ElementInfoOk()
	{
		//despausa o jogo
		SetPauseGame(false);
		
		//se for o ciclo de adesão, inicia o tutorial que informa ao jogador para conectar o receptor do vírus no da célula
		if (cicloDeVida == CicloDeVida.adesao) interfaceScript.tutorialPanel.ShowTutorial(2);
		
		//se for integracao2 ou integracao3_HIV, quer dizer que aquele modo acabou
		//se acabou o ciclo lisogênico
		if (cicloDeVida == CicloDeVida.integracao2 || cicloDeVida == CicloDeVida.integracao3_HIV)
		{
			//toca a música do menu principal
			AudioManager.instance.PlayMusic(AudioManager.instance.music_MainTheme);
			//carrega a cena
			LoadSecene("MainMenu");
		}
	}
	
	public void VirusIntroOk()
	{
		//depois que apertar o botão Ok do painel de introdução, inicia o tutorial do movimento
		interfaceScript.tutorialPanel.ShowTutorial(0);
	}
	
	public IEnumerator TutorialOk()
	{
		//espera 1 segundo para dar tempo do tutorial sair da tela
		yield return waitFor1seconds;
		
		//se for o modo viral
		if (!isProtectorMode)
		{
			//irá mandar uma informação simples de acordo com a etapa do ciclo de vida
			if (cicloDeVida == CicloDeVida.nenhum)
				interfaceScript.simpleInfo.ShowSimpleText("Siga a seta para encontrar a célula alvo!");
			if (cicloDeVida == CicloDeVida.penetracao2)
				interfaceScript.simpleInfo.ShowSimpleText("Arraste seu dedo da direita para esquerda rapidamente!");
			if (cicloDeVida == CicloDeVida.integracao1_HIV)
				interfaceScript.simpleInfo.ShowSimpleText("Cuidado para não bater nas bordas!");
			if (cicloDeVida == CicloDeVida.biossintese1 || cicloDeVida == CicloDeVida.integracao1 || cicloDeVida == CicloDeVida.integracao2_HIV)
				interfaceScript.simpleInfo.ShowSimpleText("Siga a seta para encontrar o núcleo da célula!");
			if (cicloDeVida == CicloDeVida.integracao2 || cicloDeVida == CicloDeVida.integracao3_HIV)
				interfaceScript.simpleInfo.ShowSimpleText("Toque na tela para mover o vírus!");
			if (cicloDeVida == CicloDeVida.biossintese2)
				interfaceScript.simpleInfo.ShowSimpleText("Toque na tela no momento certo!");
			if (cicloDeVida == CicloDeVida.maturacao)
				interfaceScript.simpleInfo.ShowSimpleText("Memorize a estrutura do vírus!");
		}
		else//se for modo protetor. Só será chamado nas 3 primeiras ondas
		{
			//se for o ID 0, quer dizer que o jogador clicou no ok do tutorial que explica sobre o modo protetor
			if (ProtectorModeController.instance.tutorialsFinished ==0)
			{
				ProtectorModeController.instance.PrepareWave();//mostra a mensagem da onda atual
				ProtectorModeController.instance.tutorialsFinished++;//passa para o proximo passo do tutorial
			}
			else//se já passou pelo primeiro tutorial, inicia a onda diretamente
				ProtectorModeController.instance.StartWave();//inicia a onda depois que o painel do tutorial é fechado
		}
		
		SetPauseGame(false);
	}
	
	public void SimpleInfoOk()
	{
		//despausa o jogo
		SetPauseGame(false);
		
		//se for o modo protetor
		if (isProtectorMode)
		{
			//só vai iniciar a onda, logo após o simpleInfo, caso já tenha visto todos os tutoriais. Se ainda não viu, o início será dado
			//depois que o jogador fechar o painel do tutorial
			if (ProtectorModeController.instance.tutorialsFinished > 3)
				ProtectorModeController.instance.StartWave();
			else//se ainda não viu todos os tutoriais, ativa o tutorial
				ProtectorModeController.instance.UpdateTutorial();
		}
	}
	
	public void SaveData()
	{
		//salva o vírus atual
		PlayerPrefs.SetInt("currentVirusID", player.currentVirusID);
	}
	
	public void LoadData()
	{
		//o modo protetor começa do zero
		if (isProtectorMode) return;
		
		//define qual é o ultimo id do vírus jogado
		player.currentVirusID = PlayerPrefs.GetInt("currentVirusID");
	}
	
}
