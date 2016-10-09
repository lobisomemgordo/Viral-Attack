using UnityEngine;
using System.Collections;

public class Espicula : MonoBehaviour {
	
	//guarda o gráfico do receptor
	public Sprite espiculaSprite;
	//booleano para verificar se a espícula está voltando para a posição inicial ou não
	private bool goingBack;
	//seria a quantidade do movimento de da espícula em voltar ao ponto inicial
	private float goingBackQnt = 0.005f;
	//guarda a referência do transform para posicionamento
	private Transform myTransform;
	//vectors3 que serão usados: vecUp seria a direção para se aprofundar na célula
	//originalPos guardaria a posição inicial
	//tmpOriginalPos seria a nova posição inicial que seria passada pelo gameController
	//targetPos é a posição alvo na qual é o valor máximo que a espícula consegue entrar na célula
	private Vector3 vecUp = Vector3.up, newPos = Vector3.zero, originalPos, tmpOriginalPos, targetPos;
	//essa variavel verifica se a espícula é de um VÍRUS(se possui o component Virus é) ou se é de uma célula(se não possui o componente)
	private Virus myVirus;
	
	void Start () {
		//guarda as referências e os valores iniciais
		myTransform = transform;
		originalPos = myTransform.position;
		tmpOriginalPos = originalPos;
		//define a posição máxima na qual a espícula é permitida a se afundar na célula
		targetPos = myTransform.TransformDirection(vecUp*-1.5f) + myTransform.position;
		//tenta pegar o componenete virus
		//se for um vírus, terá a referência. Se não for, quer dizer que é espícula da célula
		myVirus = GetComponentInParent<Virus>();
		//guarda qual é o gráfico da espicula que será usado para fazer comparações
		espiculaSprite = GetComponent<SpriteRenderer>().sprite;
	}
	
	void Update()
	{
		//se não for o ciclo de vida penetração, sai
		if (GameController.instance.cicloDeVida != GameController.CicloDeVida.penetracao1) return;
		
		//se é para a espicula se afundar na célula
		if (goingBack)
		{
			//se houver toques na tela
			if (GameController.instance.touchCounter.touchCount > 0)
			{
				//define a nova posição da espícula
				//o valor irá se basear na quantidade de toques na tela, quanto mais toques, mais rápido irá se mover
				newPos = myTransform.TransformDirection(vecUp * -GameController.instance.touchCounter.touchCount/1000) + myTransform.position;
				
				//seta a nova posição
				myTransform.position = newPos;
				
				//se a distância da espícula e a posição alvo for baixa
				if (Vector3.Distance(targetPos,myTransform.position) <=0.2f)
				{
					//agora a lógica é desativada
					goingBack = false;
					//se inicia a próxima etapa do ciclo de vida
					GameController.instance.StartLifeCycle();
				}
			}
			else//se não estiver havendo toques na tela
			{
				//a espícula retornará lentamente para sua posição inicial
				myTransform.position = Vector3.Lerp(myTransform.position, tmpOriginalPos, 0.01f);
			}
		}
	}
	
	public void SetNewOriginalPos()
	{
		//essa função é chamada pelo gameController quando o vírus começa a entrar na célula
		//ela altera a posição inicial da espícula para que ela não faça com que o vírus saia novamente de dentro da célula
		tmpOriginalPos = newPos;
	}
	
	public void SetFixed()
	{
		//essa função é chamada pelo receptor do vírus e é direcionada ao receptor da célula
		
		//desativa as colisões da célula para que o vírus consiga entrar nela
		GetComponentInParent<Celula>().EnableCollision(false);
		
		//começa a lógica de afundar a espícula para assim levar o vírus para dentro
		goingBack = true;
	}
	
	void OnCollisionEnter2D(Collision2D other)
	{
		//se não for uma espícula(receptor), retorna
		if (!other.collider.CompareTag("Espicula")) return;
		//se a espícula não for compatível(igual), retorna
		if (espiculaSprite != other.collider.GetComponent<Espicula>().espiculaSprite)
		{
			//mostra a mensagem do motivo de não ser possível conectar as duas espículas
			GameController.instance.ShowElementInfo(null);
			return;
		}
		
		//se o myVirus não for nulo, quer dizer que é a espícula de um VÍRUS
		//se for um vírus
		if (myVirus != null)
		{
			//faz com que o player(que é pai dos vírus) se torne filho da espícula da célula
			//é feito isso para que o player(vírus) acompanhe o movimento da espícula da célula
			myVirus.transform.root.SetParent(other.transform);
			//manda ao gameController a espícula da célula para que mais tarde seja alterado a posição inicial(devido ao ciclo de penetração)
			GameController.instance.SetEspicula(other.collider.GetComponent<Espicula>());
			//se inicia a próxima etapa do ciclo de vida
			GameController.instance.StartLifeCycle();
			//faz com que a célula da espícula comece a se retrair e também desativa o colisor da célula para que o vírus consiga entrar
			other.collider.GetComponent<Espicula>().SetFixed();
		}
	}
}
