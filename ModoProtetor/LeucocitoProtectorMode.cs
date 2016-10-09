using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LeucocitoProtectorMode : Leucocito {
	
	//é o tempo de duração do leucocito. Depois que acaba esse tempo, ele é desativado da cena
	public float timeToDisable = 5f;
	
	//escalonamento original para voltar ao normal
	private Vector3 originalScale;
	//usado vector2 pois não tem perigo do Z atrapalhar o shake e fazer com que a sprite suma
	private Vector2 xyVector = new Vector2(0.2f,0.2f);
	//é o waitForSeconds que fará com que haja a espera da duração da vida do leucocito
	private WaitForSeconds waitForSeconds = new WaitForSeconds(1f);
	//booleano para verificar se já foi rodado pela primeira vez. Se não, inicializará tudo
	private bool firstTime;
	//referência do instanciador para avisar ao mesmo caso o leucocito seja desativado da cena
	private LeucocitoSpawnerProtectorMode leucocitoSpawner;
	
	void Awake()
	{
		//começa parado
		stop = true;
		
		//inicializa tudo
		FirstTime();
	}
	
	void FirstTime () {
		//guarda todas as referências
		leucocitoSpawner = FindObjectOfType<LeucocitoSpawnerProtectorMode>();
		myTransform = transform;
		originalScale = transform.localScale;
		line = GetComponent<LineRenderer>();
		//define a quantidade de pontos que a linha terá. uma seria no início e a outra seria o final
		if (line!=null) line.SetVertexCount(2);
		//cria o waitForSeconds de acordo com o tempo colocado
		waitForSeconds = new WaitForSeconds(timeToDisable);
		
		//"desativa" ele
		myTransform.localScale = vecZero;
		//define que já foi inicializado
		firstTime = true;
		
	}
	
	public void HideLeucocito()
	{
		//se ainda não pegou as referencias iniciais, vai pegar antes de seguir
		if (!firstTime) FirstTime();
		
		//verifica se possui line renderer(fagócito grande). Se possuir, desabilita
		if (line!=null) line.enabled = false;
		
		//escalona para que suma da tela
		myTransform.localScale = vecZero;
		
		//reseta o leucocito(função derivada da classe Leucócito)
		ResetLeucocito(false);
	}
	
	public void SpawnLeucocito(Vector3 position)
	{
		//se ainda não pegou as referencias iniciais, vai pegar antes de seguir
		if (!firstTime) FirstTime();
		
		ResetLeucocito(true);
		
		//seta a posição e aumenta o escalonamento para que fique visível
		myTransform.position = position;
		myTransform.DOScale(originalScale, 0.3f).OnComplete(CompleteScale);
		//ativa a lógica de perseguir
		stop = false;
		
		//depois de instanciado, será iniciado o contador da vida para que ele seja retirado da cena após o tempo pedido
		StartCoroutine(DisableLeucocitoAfterTime());
	}
	
	void CompleteScale()
	{
		//esse método será chamado quando o leucocito estiver totalmente visível
		
		//dá um efeito de aumentar e diminuir rápidamente
		myTransform.DOPunchScale(xyVector, 1f, 3, 10f);
	}
	
	IEnumerator DisableLeucocitoAfterTime()
	{
		//espera o tempo de duração do leucocito
		yield return waitForSeconds;
		
		//desativa ele
		DisableLeucocito();
	}
	
	
	void DisableLeucocito()
	{
		//chama a função derivada da classe Leucocito para resetar ele
		ResetLeucocito(false);
		//"desativa" ele
		myTransform.DOScale(vecZero,0.5f).OnComplete(StopLeucocito);
	}
	
	void StopLeucocito()
	{
		//caso ainda não tenha achado a referência, busca novamente
		if (leucocitoSpawner == null) leucocitoSpawner = FindObjectOfType<LeucocitoSpawnerProtectorMode>();
		
		//para ele
		stop = true;
		
		//avisa que o leucocito foi desativado para que seja possível ser instanciado novamente
		leucocitoSpawner.LeucocitoDead();
	}
}
