using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class TouchCounter : MonoBehaviour {
	
	//tempo que levará para resetar o contador caso o jogador não esteja tocando na tela
	public float timeToReset = 0.2f;
	//contador dos toques na tela
	public int touchCount;
	//habilita ou desativa a contagem
	public bool enableCount;
	
	//texto que informará ao jogador para tocar na tela rapidamente
	[SerializeField]private Text txtFastTouch;
	//guarda o último tempo em que foi checado
	private float lastTimer;
	//interpolação da cor do texto informativo. Irá alternar entre transparênte total e um pouco visível
	private Tweener tween;
	//cores que serão usadas pelo texto
	private Color noColor = new Color(1f,1f,1f,0f), initialColor = new Color(1f,1f,1f,0.2f), fullColor = new Color(1f,1f,1f,0.7f);
	
	void Update () {
		//se é pra parar ou se não é pra contar, sai
		if (!enableCount || GameController.instance.stop) return;
		if (GameController.instance.stop) return;
		
		//se houve toque na tela
		if (Input.GetMouseButtonDown(0))
		{
			//incrementa o contador de toques
			touchCount++;
			//atualiza o ultimo tempo, ou seja, o próximo tempo que será checado novamente
			lastTimer = Time.time+ timeToReset;
		}
		
		//verifica se o tempo atual já é maior do que o tempo necessário para aguardar
		if (Time.time > lastTimer)
		{
			//zera o contador
			ResetCount();
		}
	}
	
	public void StartCounting()
	{
		//ativa a contagem
		enableCount = true;
		//seta a cor inicial do texto informativo
		txtFastTouch.color = initialColor;
		//interpola a cor do texto informativo para que alterne entre transparência baixa e alta
		tween = txtFastTouch.DOColor(fullColor, 0.6f).SetLoops(-1, LoopType.Yoyo);
	}
	
	public void StopCounting()
	{
		//desativa o contador
		enableCount = false;
		//elimina a interpolação caso exista
		if (tween!= null) tween.Kill();
		
		//seta a cor sem alfa para que suma da ela
		txtFastTouch.color = noColor;
		
		//zera o contador
		ResetCount();
	}
	
	public void ResetCount()
	{
		//zera as variáveis
		touchCount =0;
		lastTimer = 0f;
	}
}
