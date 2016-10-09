using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MiniGameDNAViral : MonoBehaviour {
	
	//velocidade do movimento
	public float speed=1f;
	//distância máxima vertical
	public float distanceToMove = 4f;
	//booleano para saber se ta parado ou não
	public bool stop = true;
	
	//referência dos pontos(segmentos) de DNA em que precisam ser atingidos
	[SerializeField]Transform[] pointsToMove;
	//segmento atual que precisa ser atingido
	private int currentPoint;
	//posição inicial
	private Vector3 originalLocalPos;
	//referência do transform para posicionamento
	private Transform myTransform;
	//booleano para saber se está indo para baixo ou para cima
	private bool goDown;
	//variáveis temporárias que serão usadas no movimento
	private Vector3 newLocalPos, newHorizontalPos;
	//interpolação do movimento horizontal
	private Tweener moveTween;
	//variável que guardará a distância entre o dna viral e o ponto do qual precisa ser atingido
	private float distanceBetween = float.MaxValue;
	//referência do mini game atual
	private MiniGameIntegracao miniGameIntegracao;
	//são os filamentos de DNA, serão desativados/ativados de acordo com o vírus
	private GameObject[] segments;
	//guarda o máximo de segmentos do DNA viral(de acordo com o vírus) e o ultimo segmento que ainda não foi integrado ao DNA da célula
	private int totalSegments, currentSegment;
	
	void Start () {
		//guarda o transform para posicionamento
		myTransform = transform;
		//guarda os valores iniciais
		originalLocalPos = myTransform.localPosition;
		//busca o mini game atual
		miniGameIntegracao = FindObjectOfType<MiniGameIntegracao>();
		
		//reserva o tamanho do vetor de acordo com o numero de segmentos
		segments = new GameObject[myTransform.childCount];
		for (int c=0;c<myTransform.childCount;c++)
		{
			//guarda os segmentos que seriam os filhos do myTransform
			segments[c] = myTransform.GetChild(c).gameObject;	
		}
		
		//a variável temporária começa com a posição inicial
		newLocalPos = myTransform.localPosition;
	}
	
	public void SetUpDnaViralSegments(int segmentQnt)
	{
		//esse método é para preparar o dna viral para iniciar um jogo do zero
		
		//volta para a posição original
		myTransform.localPosition = originalLocalPos;
		//a velocidade do movimento será de acordo com a quantidade de segmentos
		speed = segmentQnt;
		//guarda o total de segmentos que será usado para verificar se já acabou
		totalSegments = segmentQnt;
		//o segmento atual seria o último segmento disponível
		currentSegment = segmentQnt-1;
		//começa no primeiro ponto
		currentPoint = 0;
		
		//percorre o vetor de segmentos para ativar/desativar de acordo com a quantidade pedida
		for (int c=0;c<segments.Length;c++)
		{
			if (c < segmentQnt) segments[c].SetActive(true);
			else segments[c].SetActive(false);
		}
		
		//começa parado
		stop = false;
	}
	
	void OnCollisionEnter2D(Collision2D other)
	{
		//essa função é acionada quando o jogador perde
		
		//se não for algo sobre o mini game, sai
		if (!other.transform.CompareTag("MiniGameDNA")) return;
		
		//perdeu então para
		stop = true;
		
		//se existir uma interpolação, destroi ela para que a variável seja usada depois
		if (moveTween != null) moveTween.Kill();
		//volta para o ponto inicial
		currentPoint = 0;
		//chama o menu de game over
		GameController.instance.interfaceScript.gameOverMenu.ShowGameOverMenu();
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		//essa função é acionada para verificar se o segmento de dna atingiu o ponto necessário
		
		//se não for algo relacionado ao mini game, sai
		if (!other.transform.parent.CompareTag("MiniGameDNA")) return;
		
		//toca a particula na posição atual
		miniGameIntegracao.PlayParticleEffect(myTransform.position);
		
		//avisa ao filamento de dna da célula que o player(dna viral) atingiu o alvo
		other.transform.parent.GetComponent<MiniGameDNA>().PlayerArrived();
		
		//se existir uma interpolação, destroi ela para poder usar mais tarde
		if (moveTween != null) moveTween.Kill();
		
		//desativa o segmento atual. A cada acerto vai diminuindo a quantidade de segmentos virais
		segments[currentSegment].SetActive(false);
		//atualiza o segmento atual
		currentSegment--;
		
		//passa para o próximo ponto
		currentPoint++;
		//para
		stop = false;
		
		
		//aqui é verificado se o ponto atual já é o total de pontos, ou seja, se já passou por todos os pontos
		if (currentPoint>= totalSegments)
		{
			//mostra a informação sobre o ciclo lisogênico
			GameController.instance.interfaceScript.simpleInfo.ShowSimpleTip("O ciclo lisogênico é o motivo de algumas doenças demorarem a aparecer");
			//depois de 7 segundos será acionado o método FinishedMiniGame do mini game atual para finalizar
			miniGameIntegracao.Invoke("FinishedMiniGame", 7f);
			//para
			stop = true;
			//volta para o ponto inicial
			currentPoint = 0;
		}
	}
	
	void Move()
	{
		//se é pra parar, não faz nada
		if (stop || GameController.instance.stop) return;
		
		//guarda qual a posição em que o dna viral deve ir, que seria até o ponto atual
		newHorizontalPos.Set(pointsToMove[currentPoint].position.x, myTransform.position.y,0f);
		//para de se mover verticalmente
		stop = true;
		//interpola a posição até o ponto
		moveTween = myTransform.DOMoveX(newHorizontalPos.x,0.5f).SetEase(Ease.InQuad).OnComplete(MoveComplete);
	}
	
	void MoveComplete()
	{
		//é chamado após acabar o movimento horizontal
		//ele irá voltar a se mover verticalmente
		stop = false;
	}
	
	void Update()
	{
		//se é pra parar, sai
		if (stop || GameController.instance.stop) return;
		
		//se houve o toque na tela, move o dna viral
		if (Input.GetMouseButtonDown(0)) Move();
		
		//DAQUI PARA BAIXO É A LÓGICA DO MOVIMENTO VERTICAL
		//guarda a distância entre a posição atual e a posição em qual ele deve chegar(newLocalPos)
		distanceBetween = Vector3.Distance(newLocalPos, myTransform.localPosition);
		
		//interpola a posição para que chegue até a posição alvo(newLocalPos)
		myTransform.localPosition = Vector3.Lerp(myTransform.localPosition, newLocalPos, speed * Time.deltaTime / distanceBetween);
		
		//se a distância for baixa
		if (distanceBetween <=0.1f)
		{
			//se a variavel goDown é verdadeira
			if (goDown)
			{
				//altera para que se torna para baixo
				goDown = false;
				//adiciona a distância para mover no eixo y
				newLocalPos.y=originalLocalPos.y+distanceToMove;
			}
			else//se goDown for falso
			{
				//altera para que se torne para cima
				goDown = true;
				//adiciona a distância para mover no eixo y
				newLocalPos.y=originalLocalPos.y-distanceToMove;
			}
		}
	}
}
