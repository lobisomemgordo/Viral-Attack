using UnityEngine;
using System.Collections;

public class Leucocito : MonoBehaviour {
	
	//guarda o tipo de leucócito: se é neutrófilo ou macrófago
	public LeucocitoTipo leucocitoType;
	//velocidade do movimento
	public float speed;
	//a partir de qual distância vai ser reconhecido que chegou na posição desejada
	//é feito isso por que o vector3.lerp demora para chegar na posição desejada
	public float closeDist = 0.1f;
	//variável que ira parar completamente o leucócito
	public bool stop;
	//transform publico pq ele vai ser usado pela classe pai do modo protetor chamada LeucocitoProtectorMode
	[HideInInspector]public Transform myTransform;
	[HideInInspector]public Vector3 vecZero = Vector3.zero, vecUp = Vector3.up;
	[HideInInspector]public LineRenderer line;
	
	//guarda a nova posição da linha. Será utilizado no vector3.lerp
	private Vector3 newPos = Vector3.zero;
	//guarda qual é o alvo para seguir
	private Transform target;
	//variáveis para saber se o leucócito está atacando ou se está "voltando" que é usado só pelo macrófago
	private bool attacking, goingBack;
	//rigidbody para movimento
	private Rigidbody2D rb;
	//usado no calculo da rotação para que o leucócito sempre "olhe" para o alvo
	private float newAngle, rad2deg = Mathf.Rad2Deg;
	//referência do controller da cãmera para saber se está com o zoom ativo ou não
	//caso esteja com zoom, o leucócito não anda
	private CameraController camController;
	
	//enum dos tipos de leucócito
	public enum LeucocitoTipo
	{
		bigBrother,
		smallWarrior,
	}
	
	void Awake () {
		
		//guarda o transform para buscar a posição do leucócito e também definir sua rotação
		myTransform = transform;	
		
		//se for o macrófago
		if (leucocitoType == LeucocitoTipo.bigBrother)
		{
			//busca o componente line renderer
			line = GetComponent<LineRenderer>();
			//define que vai ter 2 pontos(o inicial e o final)
			line.SetVertexCount(2);
			//os 2 pontos já começam na posição inicial pois ainda não há alvo
			line.SetPosition(0, myTransform.position);
			line.SetPosition(1, myTransform.position);
		}
		
		//guarda o rigidbody para movimento
		rb = GetComponent<Rigidbody2D>();
	}
	
	void Start()
	{
		//referência da câmera para saber do zoom
		camController = FindObjectOfType<CameraController>();
	}
	
	void LateUpdate () {
	
		if (GameController.instance.stopNpc || GameController.instance.stop || stop || GameController.instance.isInLifeCycle)
		{
			//se for nulo, guarda a referência do rb. Evita que aconteça erros de null reference
			if (rb==null)
			{
				rb = GetComponent<Rigidbody2D>();
				return;
			}
			//zera a velocidade para ele não se mover
			rb.velocity = vecZero;
			return;
		}
		
		//se estiver no modo viral
		if (!GameController.instance.isProtectorMode)
		{
			//se a câmera estiver com zoom
			if (camController.zoom)
			{
				//zera a velocidade para não se mover e sai
				rb.velocity = vecZero;
				return;
			}
		}
		
		//se for o macrófago
		if (leucocitoType == LeucocitoTipo.bigBrother)
		{
			//se a linha que segue o alvo está voltando
			if (goingBack)
			{
				//a linha inicial sempre será a posição atual o leucócito
				line.SetPosition(0,myTransform.position);
				//define a nova posição de acordo que irá interpolar a posição atual da linha final até a posição INICIAL
				newPos = Vector3.Lerp(newPos, myTransform.position, Time.deltaTime);
				//seta a posição final da linha que seria um pouco mais próxima da inicial
				line.SetPosition(1,newPos);
				
				//verifica se já está perto da posição desejada
				if (Vector3.Distance(newPos,myTransform.position) <= closeDist)
				{
					//se estiver, para de se retrair
					goingBack = false;
				}
			}
			
			//se tiver um alvo e ele não for nulo
			if (attacking && target)
			{
				//a linha inicial sempre será a posição atual o leucócito
				line.SetPosition(0,myTransform.position);
				//define a nova posição de acordo que irá interpolar a posição atual da linha final até a posição do ALVO
				newPos = Vector3.Lerp(newPos, target.position, speed * Time.deltaTime);
				//seta a nova posição da linha final que ficará um pouco mais perto da posição do alvo
				line.SetPosition(1,newPos);
				
				//verifica se já está perto da posição desejada
				if (Vector3.Distance(newPos,target.position) <= closeDist)
				{
					//se for o modo protetor, irá destruir o vírus do modo protetor
					if (GameController.instance.isProtectorMode)
					{
						target.GetComponent<VirusProtectorMode>().DestroyVirus(true);
					}
					else//se for o modo viral
					{
						//toca o som de morte do vírus
						AudioManager.instance.DeadSound();
						//mostra o game over
						GameController.instance.interfaceScript.gameOverMenu.ShowGameOverMenu();
						//destroi o vírus
						Destroy(target.gameObject);
					}
					
					//reseta as variáveis para que para de perseguir/atacar e comece a se retrair(goingBack)
					attacking = false;
					target = null;
					goingBack = true;
				}
			}
		}
		
		//se for neutrófilo
		if (leucocitoType == LeucocitoTipo.smallWarrior)
		{
			if (attacking && target)
			{
				if (rb == null)
				{
					//garante que tenha a referência do rigidboy, evita null reference
					rb = GetComponent<Rigidbody2D>();
				}
				
				rb.isKinematic = true;
				
				//Vector2.Distance não calcula o Z, então o Z não vai atrapalhar
				//calcula a distancia da posição atual e a do alvo. Se estiver na distância do ataque(closeDis)
				if (Vector2.Distance(myTransform.position, target.position) <= closeDist)
				{
					//para de perseguir
					attacking = false;
					
					//se for o modo protetor
					if (GameController.instance.isProtectorMode)
					{
						//para de atacar e destroi o vírus do modo protetor
						target.GetComponent<VirusProtectorMode>().DestroyVirus(true);
						target = null;
					}
					else//se for modo viral
					{
						//toca o som de morte do vírus
						AudioManager.instance.DeadSound();
						//mostra o game over
						GameController.instance.interfaceScript.gameOverMenu.ShowGameOverMenu();
						//destroi o virus
						Destroy(target.gameObject);
						//tira o alvo para que pare de seguir
						target = null;
					}
				}
				else//se estiver longe do alvo, quer dizer que é para seguir
				{
					//guarda a posição em qu
					newPos = target.position - myTransform.position;
					newAngle = Mathf.Atan2(newPos.y,newPos.x) * rad2deg;
					
					//seta a nova rotação
					//por ajustes foi diminuído 90 do z
					myTransform.rotation = Quaternion.Euler(0f,0f, newAngle - 90f);
					
					//guarda a velocidade do movimento
					newPos = myTransform.TransformDirection(vecUp * speed);
					//faz com que o leucócito se mova
					rb.velocity = newPos;
				}
			}
		}
		
	}
	
	public void ResetLeucocito(bool showLine)
	{
		//essa função será chamada pelo script LeucocitoProtectorMode
		
		//zera o target para parar de perseguir
		target = null;
		attacking = false;
		//a posição "alvo" passa a ser ele mesmo
		newPos = myTransform.position;
		
		if (line!=null) 
		{
			//reseta a posição do line renderer para que a ponta inicial fique "escondida"
			line.SetPosition(0, myTransform.position);
			line.SetPosition(1, myTransform.position);
			line.enabled = showLine;
		}
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		//se for um vírus que foi avistado
		if (other.CompareTag("Virus"))
		{
			//só vai tocar do som do leucócito se não for o modo protetor
			//se for o modo protetor, o som do leucócito será tocado na hora que ele mata o vírus
			if (!GameController.instance.isProtectorMode && !camController.zoom)
				AudioManager.instance.LeucocitoSound();
			//se o newPos não está sendo atualizado(usado pelo macrófago), seta para que seja a posição atual do leucócito
			if (!goingBack) newPos = myTransform.position;
			goingBack = false;
			//se for neutrófilo e NÃO tiver um alvo, ele irá setar o novo alvo.
			//isso serve para que o leucócito vá atrás do primeiro vírus que tenha sido avistado e ignore o resto
			//Já no caso do macrófago, toda vez que um vírus é avistado é alterado o alvo do macrófago, ou seja, altera toda hora
			if (leucocitoType == LeucocitoTipo.smallWarrior && target == null || leucocitoType != LeucocitoTipo.smallWarrior) 
				target = other.transform;
			
			//seta que está perseguindo
			attacking = true;
			
			//vai vibrar quando encontrar algum vírus, mas só no modo viral
			if (!GameController.instance.isProtectorMode)Handheld.Vibrate();
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		//se for vírus que saiu do alcance
		if (other.CompareTag("Virus"))
		{
			//só vai tocar o som do vírus escapando se não for o modo protetor e não estiver no zoom(quer dizer que ta na parte do ciclo)
			if (!GameController.instance.isProtectorMode && !camController.zoom)
				AudioManager.instance.PlayOneShotSound(AudioManager.instance.sound_escape);
			
			//reseta todas as variáveis para que o leucócito para de perseguir e de se movimentar
			target = null;
			attacking = false;
			//começa a retrair o ponto final da linha do macrófago
			goingBack = true;
			if (rb) rb.velocity = vecZero;
		}
	}
	
	void OnTriggerStay2D(Collider2D other)
	{
		//essa função é chamada quando um vírus sai do alcance do leucócito, ou seja, o leucócito perde o alvo
		//enquanto não tiver alvo, essa função buscará se ainda existem vírus no alcance do leucócito
		//se ainda existir vírus no alcance, define que o novo alvo será ele
		if (!target)
		{
			if (other.CompareTag("Virus"))
			{
				if (!goingBack) newPos = myTransform.position;
				goingBack = false;
				target = other.transform;
				attacking = true;
			}
		}
	}
}
