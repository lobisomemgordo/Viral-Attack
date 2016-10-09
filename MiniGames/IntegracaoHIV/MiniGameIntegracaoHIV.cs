using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MiniGameIntegracaoHIV : MonoBehaviour {
	
	//referência do transform da transcriptase reversa
	[SerializeField]private Transform transcriptaseReversaTransform;
	//é o valor máximo do x e y em que a transcriptase pode ser posicionada
	[SerializeField]private Vector2 limitPositionToSpawn;
	//referência da particula
	[SerializeField]private ParticleSystem particleEffect;
	//referência do transform da particula para pocionamento
	private Transform particleEffectTransform;
	//referência do rna viral(player)
	private MiniGameRnaViral miniGameRnaViral;
	//referência do transform para posicionar ou afastar da tela
	private Transform myTransform;
	//posição inicial Y
	private float originalLocalY;
	//vector3 que será usado e a posição inicial da transcriptase reversa
	private Vector3 newPos = Vector3.zero, transcriptaseReversaOriginalLocalPos;
	//os layers que serão usados. Um ignora todas colisões e o outro não
	private int miniGameLayer = 12,ignoreLayer = 13;
	//game object do background para alterar o layer e assim ativar/desativar os colliders
	private GameObject background;
	
	void Start () {
		//guarda o transform do minigame
		myTransform = transform;
		//guarda o rna viral, que será o controlado pelo player
		miniGameRnaViral = GetComponentInChildren<MiniGameRnaViral>();
		//guarda a particula para tocá-la
		particleEffect = GetComponentInChildren<ParticleSystem>();
		//guarda o transform da particula para posicionamento
		particleEffectTransform = particleEffect.transform;
		//guarda a posição inicial Y
		originalLocalY = myTransform.localPosition.y;
		//guarda a posição inicial da transcriptase reversa
		transcriptaseReversaOriginalLocalPos = transcriptaseReversaTransform.localPosition;
		//acha o gameobject do background
		background = myTransform.Find("background").gameObject;
	}
	
	public void StartMiniGame()
	{
		//interpola a posição do mini game para que seja visível
		myTransform.DOLocalMoveY(0f, 1f, false).SetEase(Ease.InExpo);
		
		//seta a posição inicial da transcriptase reversa
		transcriptaseReversaTransform.localPosition = transcriptaseReversaOriginalLocalPos;
		//se ainda não tem a referência do rna viral, vai buscar
		if (miniGameRnaViral == null) miniGameRnaViral = GetComponentInChildren<MiniGameRnaViral>();
		//manda o rna viral se preparar
		miniGameRnaViral.SetUpRnaViral();
		
		//ativa todos colliders
		EnableColliders(true);
	}
	
	void EnableColliders(bool enableAll)
	{
		//esse método irá alterar os layers e assim ativando/desativando as colisões
		if (enableAll) background.layer = miniGameRnaViral.gameObject.layer = transcriptaseReversaTransform.gameObject.layer = miniGameLayer;
		else background.layer = miniGameRnaViral.gameObject.layer = transcriptaseReversaTransform.gameObject.layer = ignoreLayer;
	}
	
	public void RnaGetTheCoin()
	{
		//posiciona e toca a particula
		particleEffectTransform.position = transcriptaseReversaTransform.position;
		particleEffect.Stop();
		particleEffect.Play();
		
		//seta uma posição aleatória
		RandomizeTranscriptaseReversaPosition();
	}
	
	public void FinishedMiniGame()
	{
		//desativa os colliders
		EnableColliders(false);
		//interpola a posição do mini game para que saia da tela
		myTransform.DOLocalMoveY(originalLocalY, 4f, false).SetEase(Ease.InSine);
		//avisa ao gameController que acabou
		GameController.instance.StartLifeCycle();	
	}
	
	void RandomizeTranscriptaseReversaPosition()
	{
		//irá escolher uma posição aleatória, dentro dos limites permitidos
		newPos.x = Random.Range(-limitPositionToSpawn.x, limitPositionToSpawn.x);
		newPos.y = Random.Range(-limitPositionToSpawn.y, limitPositionToSpawn.y);
		
		//seta a nova posição
		transcriptaseReversaTransform.localPosition = newPos;
	}
}
