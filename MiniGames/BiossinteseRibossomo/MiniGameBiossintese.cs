using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class MiniGameBiossintese : MonoBehaviour {
	
	//referência das sprites dos ribossomos
	[SerializeField]private SpriteRenderer[] ribossomosSprites;
	//referência da partícula que será tocada no momento em que o jogador chega no ribossomo
	[SerializeField]private ParticleSystem particleEffect;
	//referência do transform da particula para que ela seja posicionada no lugar certo
	private Transform particleEffectTransform;
	//referência do rna mensageiro para ser acessado alguns métodos
	private MiniGameRnaMensageiro miniGameRnaMensageiro;
	//referência do transform do mini game para interpolar o movimento
	private Transform myTransform;
	//posição inicial Y
	private float originalLocalY;
	//vector3 que será usado
	private Vector3 newPos = Vector3.zero;
	//os layers que serão usados: um ignora toda física e o outro é ativado
	private int miniGameLayer = 12,ignoreLayer = 13;
	//background do minigame. Será usado para desabilitar os colliders da borda
	private GameObject background;
	//guarda a cor de destaque
	[SerializeField]private Color glowColor;
	//guarda as cores originais e a cor com alfa baixo
	private Color originalColor, halfColor;
	//guarda o ribossomo alvo
	private int currentRibo;
	//lista que servirá como auxiliar para ver quais ribossomos estão disponíveis para serem escolhidos
	[SerializeField]private List<int> availableRibos = new List<int>();
	//referência da interpolação
	private Tweener tween;
	//guarda qual foi o numero aleatorio
	int randInt;
	
	void Start () {
		//guarda a cor original baseada no primeiro ribossomo(são iguais então da na mesma)
		originalColor = ribossomosSprites[0].color;
		//define a cor com alfa baixo baseado na cor original
		halfColor = originalColor; halfColor.a = 0.2f;
		
		//guarda o transform do mini game
		myTransform = transform;
		//guarda o rna mensageiro
		miniGameRnaMensageiro = GetComponentInChildren<MiniGameRnaMensageiro>();
		//guarda a particula para tocá-la
		particleEffect = GetComponentInChildren<ParticleSystem>();
		//guarda o transform da particula para posicionamento
		particleEffectTransform = particleEffect.transform;
		//guarda a posição local Y do painel do minigame
		originalLocalY = myTransform.localPosition.y;
		//guarda o gameobject do background, que será usado para alterar o layer e assim tirar/colocar os colliders da borda
		background = myTransform.Find("background").gameObject;
	}
	
	public void StartMiniGame(int currentVirusID)
	{
		//limpa a lista anterior
		availableRibos.Clear();
		//se estiver tocando animação do ribossomo, destroi ela
		if (tween != null) tween.Kill();
		
		//influenza é virus ID0 então foi adiciona+1 para aparecer pelo menos 1 ribossomo
		int riboQnt = currentVirusID+1;
		//6 é o máximo de ribossomo então previne que passe disso
		if (riboQnt > 6) riboQnt = 6;
		
		//percorre todas as sprites dos ribossomos
		for (int c=0;c<ribossomosSprites.Length;c++)
		{
			if (c<riboQnt)
			{
				//adiciona os ribossomos disponíveis
				availableRibos.Add(c);
				//seta a cor original dele
				ribossomosSprites[c].color = originalColor;
				//ativa
				ribossomosSprites[c].gameObject.SetActive(true);
			}
			else//se passou da quantidade disponível do mini game atual(baseado no vírus)
			{
				//desativa o ribossomo
				ribossomosSprites[c].gameObject.SetActive(false);
			}
		}
		
		//interpola a posição do painel para que fique visível
		myTransform.DOLocalMoveY(0f, 1f, false).SetEase(Ease.InExpo);
		
		//evita erros caso ainda não tenha achado o miniGameRnaViral
		if (miniGameRnaMensageiro == null) miniGameRnaMensageiro = GetComponentInChildren<MiniGameRnaMensageiro>();
		//envia o comando para o rna viral se preparar de acordo com o vírus atual
		miniGameRnaMensageiro.SetUpRnaMensageiro(currentVirusID);
		
		//escolhe um ribossomo aleatório
		RandomizeRibossomo();
		//ativa os colliders do miniGame
		EnableColliders(true);
	}
	
	void EnableColliders(bool enableAll)
	{
		//essa função fará que os layers dos objectos do minigame sejam alterados
		//alterando o layer fará com que haja ou não as colisões do mini game
		if (enableAll)
		{
			background.layer = miniGameRnaMensageiro.gameObject.layer = miniGameLayer;
			
			for (int c=0;c<ribossomosSprites.Length;c++)
			{
				ribossomosSprites[c].gameObject.layer = miniGameLayer;
			}	
		}
		else
		{
			background.layer = miniGameRnaMensageiro.gameObject.layer = ignoreLayer;
			
			for (int c=0;c<ribossomosSprites.Length;c++)
			{
				ribossomosSprites[c].gameObject.layer = ignoreLayer;
			}
		}
	}
	
	public void RnaGetTheRibo()
	{
		//se está tocando a animação do ribossomo, destroi ela
		if (tween != null) tween.Kill();
		
		//"desativa" visualmente o ribossomo que o jogador pegou
		ribossomosSprites[currentRibo].color = halfColor;
		
		//posiciona e toca a particula no local do ribossomo atual
		particleEffectTransform.position = ribossomosSprites[currentRibo].transform.position;
		particleEffect.Stop();
		particleEffect.Play();
		
		//se acabou os ribossomos, avisa pro rna mensageiro
		if (availableRibos.Count <1) miniGameRnaMensageiro.FinishedMiniGame();
		else//se não acabou, continua o mini game
			RandomizeRibossomo();
	}
	
	public void FinishedMiniGame()
	{
		//desativa os colliders do mini game
		EnableColliders(false);
		//interpola a posição do painel para que saia da tela
		myTransform.DOLocalMoveY(originalLocalY, 4f, false).SetEase(Ease.InSine);
		//avisa para o game controller que acabou
		GameController.instance.StartLifeCycle();	
	}
	
	void RandomizeRibossomo()
	{
		//escolhe aleatoriamente um ribossomo dentre os disponíveis
		randInt = Random.Range(0, availableRibos.Count);
		
		//pega a referência do ribossomo escolhido
		currentRibo = availableRibos[randInt];
		
		//atualiza o alvo do rna mensageiro
		miniGameRnaMensageiro.targetName = ribossomosSprites[currentRibo].name;
		
		//faz a animação do ribossomo escolhido para ficar alterando a cor
		tween = ribossomosSprites[currentRibo].DOColor(glowColor, 1f).SetLoops(-1, LoopType.Yoyo);
		
		//remove o ribossomo escolhido dos ribossomos disponíveis
		availableRibos.RemoveAt(randInt);
	}
}
