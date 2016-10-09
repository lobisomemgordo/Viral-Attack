using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MiniGameDNA : MonoBehaviour {
	
	//referência do segmento que o jogador deverá atingir
	[SerializeField]private SpriteRenderer targetDnaSegment;
	//cor com alfa e cor completa
	private Color currentColor, fullColor;
	//referência do transform para se movimentar
	private Transform myTransform;
	//referência dos colliders
	private BoxCollider2D[] colliders;
	
	void Start () {
		//guarda o transform
		myTransform = transform;
		//pega a cor do segmento, seria a com alfa baixo
		currentColor = targetDnaSegment.color;
		//guarda a cor completa, alfa cheio
		fullColor = currentColor; fullColor.a = 1f;
		//pega os colliders
		colliders = GetComponentsInChildren<BoxCollider2D>();	
	}
	
	public void PlayerArrived()
	{
		//esse método e chamado quando o player atinge o segmento de dna certo
		
		//interpola para a cor completa, com alfa cheio
		targetDnaSegment.DOColor(fullColor, 1f);
		//interpola a posição para que saia da tela
		myTransform.DOLocalMoveX(-20f, 2f, false).SetEase(Ease.InSine);
		
		//desativa os collisores
		EnableColliders(false);
	}
	
	void EnableColliders(bool enable)
	{
		//esse método irá desativar/ativar todos os colliders
		for (int i=0;i<colliders.Length;i++)
		{
			colliders[i].enabled = enable;
		}
	}
	
	public void ResetDna()
	{
		//volta com a cor inicial
		targetDnaSegment.color = currentColor;
		
		//ativa os colisores
		EnableColliders(true);
	}
}
