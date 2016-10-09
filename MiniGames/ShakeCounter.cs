using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class ShakeCounter : MonoBehaviour {
	
	//contadores de deslizamentos e toques na tela
	public int slideCount, touchCount;
	//booleano que ativa a lógica da contagem
	public bool enableCount;
	//referência do vírus atual para acionar o método ShakeDone que informa que um passo foi dado
	public Virus currentVirus;
	
	//texto que informará ao jogador para deslizar o dedo horizontalmente
	[SerializeField]private Text txtSlideFast;
	//interpolação do texto acima. Servirá para deixar o txtSlideFast alternando entre transparência alta e baixa
	private Tweener tween;
	//as cores que serão usadas na interpolação de cor do texto informativo
	private Color noColor = new Color(1f,1f,1f,0f), initialColor = new Color(1f,1f,1f,0.2f), fullColor = new Color(1f,1f,1f,0.7f);
	
	//boleano para verificar se o jogador está tocando na tela
	private bool isTouching;
	//vector3 que determinará a posição inicial, utilizada para medir se houve um deslizamento ou não
	private Vector3 startPos;
	//vector3 que determinará qual é a posição alvo para que o deslizamento seja válido
	private Vector3 targetPos;
	//vector3 que guardará a posição do mouse, mas só do valor x
	private Vector3 mousePosOnlyX;
	//a distância em que o jogador terá que deslizar a partir da posição inicial
	[SerializeField]private float targetDistance = 150f;
	
	void Update () {
		//se ta desativado ou parado, sai
		if (!enableCount || GameController.instance.stop) return;
		
		//para haver mais alternativas em relação ao deslizamento no toque, foi utilizado o toque na tela também
		
		//se tocou na tela
		if (Input.GetMouseButtonDown(0))
		{
			//de acordo com a contagem será acionado a função vírus atual que irá fazer com vibre o aparelho, 
			//mude a cor do vírus e avise ao gameController caso já ganhou
			touchCount++;
			if (touchCount == 5) currentVirus.ShakeDone(1);
			if (touchCount == 15) currentVirus.ShakeDone(2);
			if (touchCount == 25) currentVirus.ShakeDone(3);
		}
		
		//se está segurando o toque
		if (Input.GetMouseButton(0))
		{
			//se a variável isTouching for verdadeira, quer dizer que já possui uma posição inicial e a distância alvo
			if (isTouching)
			{
				if (targetPos.x > startPos.x)//quer dizer que tem que ir pra direita
				{
					//se já passou do ponto alvo
					if (Input.mousePosition.x >= targetPos.x)
					{
						//incrementa o contador de deslizamentos
						slideCount++;
						//define uma nova posição alvo, que seria para a esquerda agora
						targetPos = startPos; targetPos.x -= targetDistance;
						
						//de acordo com a contagem será acionado a função vírus atual que irá fazer com vibre o aparelho, 
						//mude a cor do vírus e avise ao gameController caso já ganhou
						if (slideCount == 5) currentVirus.ShakeDone(1);
						if (slideCount == 15) currentVirus.ShakeDone(2);
						if (slideCount == 25) currentVirus.ShakeDone(3);
					}
				}
				else//quer dizer que tem que ir pra esquerda
				{
					//se já passou do ponto alvo
					if (Input.mousePosition.x <= targetPos.x)
					{
						//incrementa o contador de deslizamentos
						slideCount++;
						//define uma nova posição alvo, que seria para a direita agora
						targetPos = startPos; targetPos.x += targetDistance;
						
						//de acordo com a contagem será acionado a função vírus atual que irá fazer com vibre o aparelho, 
						//mude a cor do vírus e avise ao gameController caso já ganhou
						if (slideCount == 5) currentVirus.ShakeDone(1);
						if (slideCount == 15) currentVirus.ShakeDone(2);
						if (slideCount == 25) currentVirus.ShakeDone(3);
					}
				}
			}
			else//se a variavel isTouching é falsa, quer dizer que ainda não tem uma posição alva, ou seja, é a primeira vez
			{
				//define que já possui uma posição inicial e a distância alvo
				isTouching = true;
				
				//guarda a posição inicial do mouse
				startPos = Input.mousePosition;
				startPos.y = startPos.z = 0f;
				//define a posição alvo de acordo com a posição inicial, só mudando o X
				targetPos = startPos; targetPos.x += targetDistance;
			}
		}
		
		//se o jogador soltou o toque na tela
		if (Input.GetMouseButtonUp(0))
		{
			//desativa o booleano para que seja inicializado novamente a próxima vez que houver toque na tela
			isTouching = false;
		}
	}
	
	public void StartCounting()
	{
		//guarda qual é o vírus atual
		currentVirus = GameController.instance.player.currentVirus;
		
		//ativa o contador
		enableCount = true;
		//seta a cor inicial do texto informativo
		txtSlideFast.color = initialColor;
		//começa a animação do texto que alterará a cor do texto informativo
		tween = txtSlideFast.DOColor(fullColor, 0.6f).SetLoops(-1, LoopType.Yoyo);
	}
	
	public void StopCounting()
	{
		//desativa o contador
		enableCount = false;
		//elimina a interpolação de cor se existir
		if (tween!= null) tween.Kill();
		
		//seta a cor invisível para que o texto desapareça da tela
		txtSlideFast.color = noColor;
		
		//zera os contadores
		touchCount = slideCount = 0;
	}
}
