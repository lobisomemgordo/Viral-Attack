using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class ProtectorModeController : MonoBehaviour {
	
	//instancia que poderá ser usado pelas outras classes
	public static ProtectorModeController instance;
	
	//usado para verificar os tutoriais do modo protetor que foram completados
	public int tutorialsFinished;
	//guarda a onda atual
	public int wave;
	
	//wave multiplier seria o multiplicador de virus instanciados por onda.
	//Ex com o waveMultiPlier =8: Onda 1 teria 8 vírus no total, Onda 2 teria 24 vírus no total, Onda 3 teria 32 vírus no total,etc..
	[SerializeField]private int waveMultiplier = 8;
	//prefab de todos os virus
	[SerializeField]private GameObject[] virus;
	//guarda quantos pontos o jogador já fez. 1 ponto é ganho quando um vírus é destruído por um leucocito ou pela vacina
	[SerializeField]private int points;
	//texto que irá informar os pontos
	[SerializeField]private Text txtPoints;
	//os botões da vacina e dos leucocitos. Serão utilizados para serem ativados/desativados
	[SerializeField]private GameObject vacineBtn, neutrofiloBtn, macrofagoBtn;
	//referência do controlador de instanciamento para avisar se algum vírus foi destruído ou para começar a instancia-los
	private VirusSpawnerController virusSpawnerController;
	//é a força do efeito especial de esacalonamento que será utilizado no texto dos pontos
	//sempre que é ganho um ponto, acontece uma animação de escalonamento para haver o destaque
	private Vector2 xyVector = new Vector2(0.3f,0.3f);
	//variavel que verifica se pode fazer a animação de escalonamento do texto dos pontos
	//para não dar problema de escala, só vai ser possível fazer o escalonamento se o escalonamento do texto dos pontos estiver normal
	private bool canTween;
	//essa variável evita que o jogo seja iniciado mais de uma vez
	private bool waitingForWave;
	//referência da vacina pois será acionado funções dela
	private Vacine vacine;
	
	void Awake()
	{
		//guarda a instancia que será usada pelas outras classes
		instance = this;
		//começa como true para ser possível fazer a primeira animação do escalonamento
		canTween = true;
	}
	
	void Start () {
		//guarda as referências
		virusSpawnerController = FindObjectOfType<VirusSpawnerController>();
		vacine = FindObjectOfType<Vacine>();
		
		//desativa os botões, eles serão ativados posteriormente
		ShowNeutrofiloBtn(false);
		ShowMacrofagoBtn(false);
		ShowVacineBtn(false);
		
		//toca a musica do menu principal
		AudioManager.instance.PlayMusic(AudioManager.instance.music_ProtectorMode);
		//mostra o primeiro tutorial do modo protetor: objetivo do modo protetor
		GameController.instance.interfaceScript.tutorialPanel.ShowTutorial(14);
	}
	
	public void ShowVacineBtn(bool show)
	{
		//método que ativa/desativa o botão da vacina
		vacineBtn.SetActive(show);
	}
	
	public void ShowNeutrofiloBtn(bool show)
	{
		//método que ativa/desativa o botão do neutrófilo
		neutrofiloBtn.SetActive(show);
	}
	
	public void ShowMacrofagoBtn(bool show)
	{
		//método que ativa/desativa o botão do macrófago
		macrofagoBtn.SetActive(show);
	}
	
	public void BtnVacine()
	{
		//se é pra parar, sai
		if (GameController.instance.stop) return;
		
		//toca o som de click
		AudioManager.instance.PlayOneShotSound(AudioManager.instance.sound_click);
		
		//aciona o poder da vacina e desativa o botão da mesma
		vacine.ShowVacine(true);
		ShowVacineBtn(false);	
	}
	
	public void AddPoint()
	{
		//adiciona e atualiza os pontos na tela
		points++;
		txtPoints.text = "" + points;
		
		//se não estiver no meio de uma animação de escalonamento
		if (canTween)
		{
			//define que está animando, ou seja, não pode interpolar
			canTween = false;
			//faz o efeito de escalonamento para destacar o texto dos pontos
			txtPoints.transform.DOPunchScale(xyVector, 1f, 5, 1f).OnComplete(TweenCompleted);
		}
	}
	
	void TweenCompleted()
	{
		//será acionado quando a animação de escalonamento for completada
		
		//permite que seja acionado novamente
		canTween = true;
	}
	
	public void VirusFinished()
	{
		//evita que toque mais de uma vez
		if (waitingForWave) return;
		
		//avisa que um vírus foi destruido e já guarda quantos ainda restam
		int remainingVirus = virusSpawnerController.LessVirusInScene();
		
		//se já acabou todos
		if (remainingVirus <=0)
		{
			//deixa guardado que já foi iniciado o próximo
			waitingForWave = true;
			//passa para a próxima onda
			wave++;
			
			//faz a preparação
			PrepareWave();
		}
	}
	
	public void PrepareWave()
	{
		//mostra qual é a onda atual
		GameController.instance.interfaceScript.simpleInfo.ShowSimpleText("ONDA " + (wave+1));
		//depois que acaba a animação informando qual é a onda atual, o game controller irá acionar funções desse script
		//se já acabou todos os tutoriais, será chamado o StartWave que começará a onda atual
		//se ainda há tutoriais a serem vistos, será chamado o UpdateTutorial
	}
	
	public void UpdateTutorial()
	{
		//se já acabou os tutoriais disponíveis, sai
		if (tutorialsFinished > 3) return;
		
		//se for o segundo tutorial(o primeiro foi logo no início do modo protetor)
		if (tutorialsFinished == 1)//se for a primeira onda, mostra o tutorial sobre o neutrófilo
		{
			ShowNeutrofiloBtn(true);
			GameController.instance.interfaceScript.tutorialPanel.ShowTutorial(15);
		}
		//se for o terceiro tutorial
		else if (tutorialsFinished == 2)//se for a segunda onda, mostra o tutorial sobre o macrófago
		{
			ShowMacrofagoBtn(true);
			GameController.instance.interfaceScript.tutorialPanel.ShowTutorial(16);
		}
		//se for o quarto tutorial
		else if (tutorialsFinished == 3)//se for a terceira onda, mostra o tutorial sobre a vacina
		{
			ShowVacineBtn(true);//ativa o botão da vacina
			GameController.instance.interfaceScript.tutorialPanel.ShowTutorial(17);//vacina
		}
		
		//passa para o próximo tutorial
		tutorialsFinished ++;
	}
	
	public void StartWave()
	{
		//se for a terceira onda ou se o jogador der sorte, irá conceder o poder especial vacina
		if (wave == 2 || Random.Range(0,4) == 0 && tutorialsFinished>3) ShowVacineBtn(true);
		else ShowVacineBtn(false);//se não der sorte e não for a terceira onda deixa desativado o botão da vacina
		
		//desativa o booleano para que que a próxima onda possa ser preparada
		waitingForWave = false;
		//avisa ao controlador de instanciadores de virus para começar a instanciar
		virusSpawnerController.StartSpawning(wave, (wave+1) * waveMultiplier);
	}
}
