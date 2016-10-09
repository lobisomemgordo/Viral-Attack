using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SimpleInfo : MonoBehaviour {
	
	[Header("SimpleText")]
	//referência do transform do texto que começa de baixo e vai subindo
	[SerializeField]private Transform simpleInfoTransform;
	//referência do texto que começa de baixo e vai subindo
	[SerializeField]private Text txtSimpleText;
	[Space]
	[Header("SimpleTip")]
	//referência do transform do texto que começa da esquerda e vai pra direita
	[SerializeField]private Transform simpleTipTransform;
	//referência do texto que começa da esquerda e vai pra direita
	[SerializeField]private Text txtSimpleTipText;
	
	//posição original dos 2 paineis
	private Vector3 simpleInfoOriginalLocalPos, simpleTipOriginalLocalPos;
	//waitForSeconds criado no início para não ter que criar depois
	private WaitForSeconds waitFor5Seconds = new WaitForSeconds(5f);
	
	void Awake () {
		//guarda as posições locais iniciais dos 2 paineis
		simpleInfoOriginalLocalPos = simpleInfoTransform.localPosition;	
		simpleTipOriginalLocalPos = simpleTipTransform.localPosition;
	}
	
	public void ShowSimpleText(string text)
	{
		//é o comando de mostrar o painel que começa de baixo e vai para cima
		
		//pausa o jogo
		GameController.instance.SetPauseGame(true);
		
		//atualiza o texto do painel de acordo com a informação enviada
		txtSimpleText.text = text;
		
		//guarda uma sequência no DOTween
		//essa sequência será usada para dar aquele efeito de slow motion
		Sequence seq = DOTween.Sequence();
		
		seq.Append(simpleInfoTransform.DOLocalMoveY(simpleInfoOriginalLocalPos.y /4f, 0.5f, false));
		seq.Append(simpleInfoTransform.DOLocalMoveY((simpleInfoOriginalLocalPos.y/4f) * -1f, 3f, false).SetEase(Ease.InCirc));
		seq.Append(simpleInfoTransform.DOLocalMoveY(simpleInfoOriginalLocalPos.y * -1f, 0.5f, false));
		seq.OnComplete(SimpleTextCompleted);
	}
	
	void SimpleTextCompleted()
	{
		//é o metodo que será acionado depois que a animação do painel que começa de baixo e vai para cima for completada
		
		//seta a posição original do painel
		simpleInfoTransform.localPosition = simpleInfoOriginalLocalPos;
		//despausa o jogo
		GameController.instance.SetPauseGame(false);
		//avisa ao gameController que acabou a animação do painel
		GameController.instance.SimpleInfoOk();
	}
	
	public void ShowSimpleTip(string text)
	{
		//é o metodo que irá mostrar o painel que apareça da esquerda para direita
		
		//atualiza o texto de acordo com o que foi enviado
		txtSimpleTipText.text = text;
		
		//interpola a posição para que fique visível ao jogador
		simpleTipTransform.DOLocalMoveX(-319f,2f, false).SetEase(Ease.InOutSine).OnComplete(SimpleTipCompleted);
	}
	
	void SimpleTipCompleted()
	{
		//é o método que será acionado quando a animação do painel que começa da esquerda para direita for completada
		
		//inicia a corrotina que fará que depois de um tempo o painel se retraia
		StartCoroutine(SimpleTipBack());
	}
	
	IEnumerator SimpleTipBack()
	{
		//espera os 5 segundos
		yield return waitFor5Seconds;
		
		//interpola a posição para que saia da tela
		simpleTipTransform.DOLocalMove(simpleTipOriginalLocalPos, 2f, false).SetEase(Ease.InOutSine);
	}
}
