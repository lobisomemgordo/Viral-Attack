using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MiniGameRnaViral : MonoBehaviour {
	
	//é a posição do movimento
	public float speed = 2f;
	
	//velocidade original
	private float originalSpeed =2f;
	//referência do mini game atual
	private MiniGameIntegracaoHIV miniGameIntegracaoHIV;
	//referência do transform do rna que será usado para posicionamento
	private Transform myTransform;
	//ridbody que será usado para movimentar
	private Rigidbody2D rb;
	//posição original
	private Vector3 originalLocalPos;
	//refeRência dos segmentos do rna viral
	private SpriteRenderer[] segmentSprites;
	//referência da cor original do segmento
	private Color segmentOriginalColor;
	//referência da cor azul que representa o dna
	[SerializeField]private Color fullColor;
	//guarda o segmento atual(que ainda não foi convertido para dna)
	private int currentSegment;
	//booleano que servirá para parar o rna viral
	public bool stop;
	//vector3 que será usado
	private Vector3 vecZero = Vector3.zero;
	//rotação original
	private Quaternion originalRotation;
	
	void Awake () {
		//guarda o transform do rna viral
		myTransform = transform;
		//guarda a referência do rigdoby que será utilizado para movimento
		rb = GetComponent<Rigidbody2D>();	
		//guarda os valores iniciais
		originalLocalPos = myTransform.localPosition;
		originalRotation = myTransform.rotation;
		originalSpeed = speed;
		
		//guarda as sprites dos segmentos que vão ter a cor alterada
		segmentSprites = GetComponentsInChildren<SpriteRenderer>();
		//guarda a cor original dos segmentos(vermelho)
		segmentOriginalColor = segmentSprites[0].color;
		//começa parado
		stop = true;
	}
	
	void Start()
	{
		//busca o mini game atual
		miniGameIntegracaoHIV = FindObjectOfType<MiniGameIntegracaoHIV>();
	}
	
	void FixedUpdate () {
		if (GameController.instance.stop || stop)
		{
			//se é pra parar, reseta a velocidade e sai
			rb.velocity = vecZero;
			return;
		}
		
		//move o rna viral para frente(no 2d a frente é cima)
		rb.velocity = myTransform.up*speed;
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		//se for a enzima transcriptase reversa
		if (other.CompareTag("TranscriptaseReversa"))
		{
			//seta a cor do segmento de RNA para Azul, transformando-se em um segmento de Dna
			segmentSprites[currentSegment].DOColor(fullColor, 1f);
			//avisa ao mini game que foi pego a enzima transcriptase
			miniGameIntegracaoHIV.RnaGetTheCoin();
			
			//passa para o próximo segmento de rna
			currentSegment++;
			
			//a cada em que se pega a enzima, é aumentado a velocidade
			speed+=0.5f;
			
			//length-1 pq o ultimo elemento é a flecha que indica a frente do rna
			//quer dizer que foi o ultimo segmento de rna que acaba de ser convertido para dna
			if (currentSegment==segmentSprites.Length-1)
			{
				//finalizo
				//zera a velocidade
				rb.velocity = vecZero;
				
				GameController.instance.interfaceScript.simpleInfo.ShowSimpleTip("O HIV consegue realizar o ciclo lisogênico devido à enzima Transcriptase reversa que converte seu RNA em DNA");
				//chama a função FinishedMiniGame do minigameatual após 7 segundos
				miniGameIntegracaoHIV.Invoke("FinishedMiniGame", 7f);
				//reseta a contagem de segmentos
				currentSegment = 0;
				//declara pra parar
				stop = true;
			}
		}
	}
	
	void OnCollisionEnter2D(Collision2D other)
	{
		//se ta parado, ignora
		if (stop) return;
		
		//reseta a velocidade
		rb.velocity = vecZero;
		//chama o gameOverMenu
		GameController.instance.interfaceScript.gameOverMenu.ShowGameOverMenu();
		//declara para parar
		stop = true;
	}
	
	public void SetUpRnaViral()
	{
		//seta os valores iniciais
		speed = originalSpeed;
		rb.velocity = vecZero;
		currentSegment = 0;
		myTransform.rotation = originalRotation;
		myTransform.localPosition = originalLocalPos;
		
		//length-1 pq o ultimo elemento é a flecha que indica a frente do rna
		for (int i=0;i<segmentSprites.Length-1;i++)
		{
			//reseta as cores de todos os segmentos para a cor original
			segmentSprites[i].color = segmentOriginalColor;
		}
		
		//depois de 1 segmento, o rna irá se mover
		Invoke("StartMoving", 1f);
	}
	
	void StartMoving()
	{
		//declara pra parar
		stop = false;
	}
}
