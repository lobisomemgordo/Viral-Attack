using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	//velocidade do movimento
	public float speed = 5f;
	//referência para a seta que indica onde está a célula alvo
	public Transform arrowHolder;
	//transform da célula alvo e de seu núcleo
	public Transform targetCellTransform, targetNucleusTransform;
	//guarda o ID do vírus atual
	public int currentVirusID;
	//será modificado pela classe Joystick. Esse valor é o da direção a qual o jogador escolheu no joystick.
	//Este valor será multiplicado pela velocidade do movimento
	[HideInInspector]public Vector3 inputAxis;
	//referência do receptor viral para fins de comparação
	[HideInInspector]public Espicula espicula;
	//guarda o vírus atual(classe Virus)
	[HideInInspector]public Virus currentVirus;
	//guarda todos os vírus da cena(desde influenza até hiv)
	[SerializeField]private GameObject[] virusInScene;
	//rigidbody para movimentação
	private Rigidbody2D rb;
	//variável temporária da velocidade que fará a movimentação
	private Vector3 newVelocity = Vector3.zero;
	//referência do transform do player para escalonamento e buscar a posição para comparação
	private Transform myTransform;
	//referência da câmera para verificar se está com zoom ativo ou não
	private CameraController camController;
	//variáveis para calcular a rotação da seta que indicará onde está a célula/núcleo alvo
	private float rad2deg = Mathf.Rad2Deg, angle;
	//variável que guardará o ponto no mundo no qual a seta deve ser orientada
	private Vector3 difference = Vector3.zero;
	//utilizado pelo ciclo do HIV
	[SerializeField]private GameObject dnaViral;
	//utilizado pelo ciclo lítico, na parte de mover até o núcleo da célula
	[SerializeField]private GameObject rnaMensageiro;
	//vectors3 que serão usados para o escalonamento
	private Vector3 vecZero = Vector3.zero, vecOne = Vector3.one;
	
	void Start () {
		//guarda as referências
		rb = GetComponent<Rigidbody2D>();
		camController = FindObjectOfType<CameraController>();
		myTransform = transform;
		
		//inicia uma corrotina para carregar o vírus atual depois de um pequeno intervalo
		StartCoroutine(StartInitialVirus());
		
		//esconde o jogador enquanto não foi carregado
		myTransform.localScale = vecZero;
	}
	
	void FixedUpdate () {
		//se é pra parar
		if (GameController.instance.stop)
		{
			//zera a velocidade para não andar mais
			rb.velocity = vecZero;
			inputAxis = vecZero;
			return;
		}
		
		//se for a liberação, fica sem seta, pois não será necessária
		if (GameController.instance.cicloDeVida != GameController.CicloDeVida.liberacao)
		{
			//se existir um alvo e se não estiver com zoom, mostra a seta
			if (targetCellTransform!= null && !camController.zoom || targetNucleusTransform != null)
			{
				//se existir um núcleo alvo, tem preferência para que seja o foco da seta
				if (targetNucleusTransform!=null) difference = targetNucleusTransform.position - myTransform.position;
				else//se não houver núcleo alvo, será a célula alvo
					//calcula a direção da seta de acordo com o alvo
					difference = targetCellTransform.position - myTransform.position;
				
				angle = Mathf.Atan2(difference.y, difference.x) * rad2deg;
				
				//define a nova rotação da seta
				//por ajustes foi colocado -90 para que seja direcionada corretamente
				arrowHolder.rotation = Quaternion.Euler(0f,0f, angle - 90f);
				
				//mostra a seta que indica a célula alvo
				arrowHolder.gameObject.SetActive(true);
			}
			else //se não existir um alvo ou se ta com zoom, esconde a seta
				arrowHolder.gameObject.SetActive(false);
		}
		
		//se for a etapa do ciclo onde o jogador leva o rna/dna viral até o núcleo, é um pouco mais rápido que o normal
		if (GameController.instance.cicloDeVida == GameController.CicloDeVida.biossintese1 || GameController.instance.cicloDeVida == GameController.CicloDeVida.integracao2_HIV || GameController.instance.cicloDeVida == GameController.CicloDeVida.integracao1) 
			newVelocity = inputAxis*(speed/3f);
		//se a câmera estiver com zoom, é mais lento
		else if (camController.zoom) newVelocity = inputAxis * (speed/12f);
		//se não for nada disso, é normal
		else newVelocity = inputAxis * speed;
		rb.velocity = newVelocity;
	}
	
	public bool IsVirusDNA()
	{
		//verifica se o vírus faz o ciclo lisogênico
		if (currentVirusID == 2 || currentVirusID == 4 || currentVirusID == 7)//varicela, papiloma humano e hiv vão ter o ciclo lisogênico
			return true;
		else return false;
	}
	
	public void ResetVelocity()
	{
		//reseta a velocidade para que pare de se mover
		//é chamado pela classe TutorialPanel para parar de se mover quando um tutorial for iniciado
		rb.velocity = vecZero;
		inputAxis = vecZero;
	}
	
	public void HideAllVirus()
	{
		//essa função irá esconder todos os vírus e a seta de indicação
		for (int c=0;c<virusInScene.Length;c++)
		{
			virusInScene[c].SetActive(false);
		}
		arrowHolder.gameObject.SetActive(false);
	}
	
	public void ShowOnlyTheNucleicAcid(bool show, bool hasDna)
	{
		//se for pra mostrar
		if (show)
		{
			//esconde o vírus atual
			currentVirus.gameObject.SetActive(false);
			//se for dna
			if (hasDna)
			{
			//mostra apenas o dna viral utilizado no ciclo lisogênico
				dnaViral.SetActive(true);
			}
			else//mostra apenas o rna mensageiro que foi criado no núcleo, utilizado no ciclo lítico
				rnaMensageiro.SetActive(true);
		}
		else//se não for pra mostrar, é pra esconder
		{
			dnaViral.SetActive(false);
			rnaMensageiro.SetActive(false);
		}
	}
	
	IEnumerator StartInitialVirus()
	{
		//espera 0.2 segundos
		yield return new WaitForSeconds(0.2f);
		
		//inicia o último vírus carregado
		ChangeVirus(currentVirusID);
	}
	
	public void ChangeVirus(int num)
	{
		//guarda o ID do novo vírus que será usado
		currentVirusID = num;
		
		//percorre o vetor dos vírus na cena para ativar somente o atual
		for (int c=0;c<virusInScene.Length;c++)
		{
			if (c==currentVirusID) virusInScene[c].gameObject.SetActive(true);
			else virusInScene[c].gameObject.SetActive(false);
		}
		
		//guarda a referência do script Vírus do vírus atual. Usado pelo game controller para liberar o capsídio e espículas
		currentVirus = virusInScene[currentVirusID].GetComponent<Virus>();
		
		//guarda a espícula do vírus atual
		espicula = virusInScene[currentVirusID].GetComponentInChildren<Espicula>();
		
		//pede ao game controller para procurar qual é a célula alvo
		GameController.instance.FindTargetCell();
		//escalona para que o vírus seja visível
		myTransform.localScale = vecOne;
	}
	
	public void SetPlayerParent(Transform parentTransform)
	{
		//essa função é chamada pelo game controller na etapa da penetração
		//mais especificamente no momento em que o receptor viral se conecta ao da célula
		myTransform.SetParent(parentTransform);
	}
}
