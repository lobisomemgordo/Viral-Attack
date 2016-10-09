using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Virus : MonoBehaviour {
	
	//guarda os game objects que precisam ser libertados, que seriam as espículas e o envelope viral
	[SerializeField]private Transform[] objectsToSetFree;
	//guarda as sprites do capsídio e da face para animar(interpolar a cor para tornar visível/invisível)
	[SerializeField]private SpriteRenderer capsidioSprite, faceSprite;
	//transform do vírus para descobrir o pai e assim poder "tremer"
	private Transform myTransform;
	//são as cores que serão usadas na parte de liberar o ácido nucleico
	//irá deixar o capsídio e a face cada vez mais invisível
	private Color noColor, lowColor, halfColor, fullColor;
	//força da tremida
	private Vector3 xyVector = new Vector3(0.1f,0.1f,0f);//utilizado pra controlar a tremida da parte de penetração
	//guarda todos os colisores do vírus que serão desativados quando o envelope e as espículas forem libertados
	private Collider2D[] colliders;
	
	void Start () {
		//guarda todos os valores iniciais
		myTransform = transform;
		fullColor = capsidioSprite.color;
		noColor = lowColor = halfColor = fullColor;
		noColor.a = 0f;
		lowColor.a = 0.4f;
		halfColor.a = 0.8f;
		
		//guarda os colisores do vírus
		colliders = GetComponentsInChildren<Collider2D>();
	}
	
	public void ShakeDone(int shakeCount)
	{
		//é chamado pela classe ShakeCounter quando o jogador atinge uma quantidade de deslizamentos necessárias
		
		//apenas verifica se for a etapa correta do ciclo de vida
		if (GameController.instance.cicloDeVida == GameController.CicloDeVida.penetracao2)
		{
			//vibra o aparelho
			Handheld.Vibrate();
			//treme o player(vírus)
			myTransform.parent.DOShakePosition(1f,xyVector,10,90,false);
			
			//irá ser alternado a cor do capsídio e da face de acordo com o passo de deslizamentos
			if (shakeCount == 1) capsidioSprite.color = faceSprite.color = halfColor;
			if (shakeCount == 2) capsidioSprite.color = faceSprite.color = lowColor;
			if (shakeCount == 3) //é o último passo
			{
				//deixa invisível o capsídio e a face
				capsidioSprite.color = faceSprite.color = noColor;
				//avisa para o game controller que finalizou a etapa
				GameController.instance.StartLifeCycle();
			}
		}
	}
	
	public void SetObjectsFree()
	{
		//libertará todas as espículas e o capsídio
		for (int i=0;i<objectsToSetFree.Length;i++)
		{
			objectsToSetFree[i].SetParent(null);
		}
		
		//desativa todos os colisores do vírus
		for (int i=0;i<colliders.Length;i++)
		{
			colliders[i].enabled = false;
		}
		
		//se não for o virus papiloma humano, pois ele não tem envelope
		if (GameController.instance.player.currentVirusID != 4)
			GameController.instance.interfaceScript.simpleInfo.ShowSimpleTip("O envelope viral é deixado para trás");
	}
	
}
