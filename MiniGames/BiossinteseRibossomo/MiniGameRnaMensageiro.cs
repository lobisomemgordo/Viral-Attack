using UnityEngine;
using System.Collections;

public class MiniGameRnaMensageiro : MonoBehaviour {
	
	//velocidades do movimento e rotação
	public float speed = 2f, rotateSpeed = 1.5f;
	//nome do alvo para verificar se foi o alvo certo
	public string targetName;
	
	//velocidade da rotação original do rna viral
	private float originalRotateSpeed =2f;
	//referência do mini game que possui a lógica do mini game atual
	private MiniGameBiossintese miniGameBiossintese;
	//referência do transform para pegar a direção do movimento e o posicionamento inicial
	private Transform myTransform;
	//referência do rigidbody para rotacionar/movimentar
	private Rigidbody2D rb;
	//posicionamento inicial
	private Vector3 originalLocalPos;
	//booleano que parará os movimentos do rna viral
	public bool stop;
	//vector3 que será usado
	private Vector3 vecZero = Vector3.zero;
	//rotação original
	private Quaternion originalRotation;
	//booleano para checar se está se movendo ou não
	[SerializeField]private bool moving;
	//booleano para ver se já acabou
	private bool finished;
	
	void Awake () {
		//guarda o transform
		myTransform = transform;
		//guarda o rigidbody para movimento/rotação
		rb = GetComponent<Rigidbody2D>();	
		//guarda os valores iniciais
		originalLocalPos = myTransform.localPosition;
		originalRotation = myTransform.rotation;
		originalRotateSpeed = rotateSpeed;
		
		//começa parado
		stop = true;
	}
	
	void Start()
	{
		//busca o mini game atual
		miniGameBiossintese = FindObjectOfType<MiniGameBiossintese>();
	}
	
	void Update () {
		//se já acabou, sai
		if (finished) return;
		
		//se está se movendo, adiciona velocidade na direção da frente(no 2d, frente é cima)
		if (moving) rb.velocity = myTransform.up*speed;
		//se não estiver se movendo, estará rotacionando
		else rb.angularVelocity = rotateSpeed;
		
		//se é pra parar, sai
		if (GameController.instance.stop || stop) return;
		
		//se houve o toque, inicia o movimento
		if (Input.GetMouseButtonDown(0)) StartMoving(true);
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		//se for algum ribossomo
		if (other.CompareTag("Ribossomo"))
		{
			//se for o ribossomo alvo
			if (other.name == targetName)
			{
				//avisa ao mini game que foi pego o ribossomo certo
				miniGameBiossintese.RnaGetTheRibo();
				
				//para de se mover
				StartMoving(false);
			}
		}
	}
	
	void ResetVelocity()
	{
		//reseta a velocidade do movimento e rotação
		rb.velocity = vecZero;
		rb.angularVelocity = 0f;
	}
	
	void OnCollisionEnter2D(Collision2D other)
	{
		//se ta parado, ignora toda colisão
		if (stop) return;
		
		//só para parar de rotacionar quando morrer
		finished = true;
		//para de se mover
		StartMoving(false);
		//chama o menu de game over
		GameController.instance.interfaceScript.gameOverMenu.ShowGameOverMenu();
		//para
		stop = true;
		//reseta a velocidade de rotação e movimento
		ResetVelocity();
	}
	
	void StartMoving(bool isMoving)
	{
		//começa ou para de se mover
		moving = isMoving;
		
		if (moving) rb.angularVelocity = 0f;
		else rb.velocity = vecZero;
		
		stop = false;
	}
	
	public void FinishedMiniGame()
	{
		//declara que acabou e para de se mover
		finished = true;
		StartMoving(false);
		//finalizo. Chama a mensagem que irá mostrar uma informação do mini game
		GameController.instance.interfaceScript.simpleInfo.ShowSimpleTip("O RNA mensageiro utiliza os ribossomos da célula para produzir as proteínas virais");
		//depois de 7 segundos irá chamar o método FinishedMiniGame do mini game atual
		miniGameBiossintese.Invoke("FinishedMiniGame", 7f);
		//para tudo e reseta a velocidade
		stop = true;
		ResetVelocity();
	}
	
	public void SetUpRnaMensageiro(int currentVirusID)
	{
		//declara que não acabou
		finished = false;
		//a cada virus, aumenta a velocidade de rotação
		rotateSpeed = originalRotateSpeed - ((float)currentVirusID * 15f);
		//reseta a velocidade de movimento e rotação
		ResetVelocity();
		
		//seta a posição e rotação inicial
		myTransform.rotation = originalRotation;
		myTransform.localPosition = originalLocalPos;
		
		//declara que não está parado
		stop = false;
	}
}
