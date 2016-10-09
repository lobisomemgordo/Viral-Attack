using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class Celula : MonoBehaviour {
	
	//referência do element info da célula que irá mostrar seus detalhes
	[HideInInspector]public ElementInfo elementInfo;
	//referência do tipo de espícula(RECEPTOR) que a célula possui
	//será utilizada para fazer comparações com a espícula viral
	[HideInInspector]public Espicula espicula;
	//guarda as posições iniciais e finais do mini game da liberação
	//inicial seria quase que no meio da célula e o final seria bem no limite dela
	public Transform miniGameStartPosToLiberate, miniGameEndPosToLiberate;
	//transform do núcleo que será utilizado como alvo na parte de levar o dna/rna viral até o núcleo
	public Transform nucleusTransform;
	
	//guarda o estado da invasão. Essa invasão seria checada por vários colliders colocados na célula
	//Ex: estado0 seria quando atingisse o primeiro collider, estado1 seria o segundo collider e assim vai..
	[SerializeField]private int intrusionState;
	//guarda as sprites do exterior da célula, do núcleo e da face. Serão usadas para animação da cor(ativar/desativar)
	[SerializeField]private SpriteRenderer cellExteriorSprite, nucleoSprite, faceSprite;
	//lista de todas as animações usando interpolação que estão sendo usadas
	private List<Tweener> loopTweens = new List<Tweener>();
	//cores que serão usadas. Cor normal que seria a inicial. Cor transparente e cor meio transparente
	//a cor meio transparência seria usada pelo núcleo, seria um loop então ficaria variando entre meio transparente e completamente visível
	private Color normalColor = new Color(1f,1f,1f,1f), fadeOutColor = new Color(1f,1f,1f,0f), 
		halfColor = new Color(0.5f,0.5f,0.5f,0.7f), cellSpriteColor, nucleoSpriteColor;
	//referência de todos os colliders da célula para ativar/desativar
	private Collider2D[] colliders;
	
	void Awake()
	{
		//guarda todas as referências
		colliders = GetComponents<Collider2D>();
		cellSpriteColor = cellExteriorSprite.color;
		nucleoSpriteColor = nucleoSprite.color;
		elementInfo = GetComponent<ElementInfo>();
		nucleusTransform = nucleoSprite.transform;
	}
	
	void Start()
	{
		//busca o tipo de espícula da célula
		espicula = GetComponentInChildren<Espicula>();
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		//tenta buscar o componente playerController
		PlayerController playerController = other.GetComponentInParent<PlayerController>();
		
		//se existe playerControoler, quer dizer que é o player.
		//se não for uma espícula e não for o dna/rna viral do mini game onde é preciso ir até o núcleo
		if (playerController && !other.CompareTag("Espicula") && !other.CompareTag("MovingAcidNucleic"))
		{
			//incrementa o estado da intrusão
			intrusionState++;
			
			//atingiu o primeiro trigger
			if (intrusionState == 1)
			{
				//se ainda não viu
				if (!elementInfo.hasSeen)
				{
					//mostra a informação da célula
					GameController.instance.ShowElementInfo(elementInfo);
					elementInfo.hasSeen = true;
				}
			}
			//atingiu o segundo trigger
			if (intrusionState == 2)
			{
				//se as espículas forem iguais(compatíveis)
				if (playerController.espicula.espiculaSprite == espicula.espiculaSprite)
				{
					//começa a fase de fixação
					GameController.instance.StartLifeCycle();
				}
				//dá zoom na câmera
				GameController.instance.cameraController.Zoom(true);
			}
			//terceiro trigger, ta entrando na célula. Só vai dar se o receptor for compatível
			if (intrusionState == 3 && playerController.espicula.espiculaSprite == espicula.espiculaSprite)
			{
				//avisa ao game controller que encaixou para assim liberar todas as partes removíveis do vírus
				GameController.instance.VirusInsideTheCell();
				//então retira a parte externa dela para ver o interior e inicia a animação do núcleo
				EnableAnimations();
				//ativa o collider que vai impedir que o dna/rna viral saia da célula
				colliders[4].enabled = true;
			}
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		//se não for player, sai
		if (other.transform.parent.CompareTag("Player"))
		{
			//diminui o estado da intrusão
			intrusionState--;
			//garante que é no mínimo 0
			if (intrusionState<1) intrusionState = 0;
			
			//se for zero
			if (intrusionState <= 0) 
			{
				//reinicia o ciclo de vida
				GameController.instance.ResetCycle();
			}
			
			//se for 1
			if (intrusionState == 1) 
			{
				//reinicia o ciclo de vida e tira o zoom
				GameController.instance.ResetCycle();
				GameController.instance.cameraController.Zoom(false);
			}
			
			//se for 2, acaba de sair de dentro da célula
			if (intrusionState == 2)
			{
				//desativa as animações do interior e ativa a parte externa da célula
				DisableAnimations();
			}
		}
	}
	
	public void EnableCollision(bool enabled)
	{
		//3 é o collider que não é trigger
		//ativa para que o vírus não consiga entrar na célula, a não ser pelos receptores
		colliders[3].enabled = enabled;
	}
	
	public void EnableAnimations()
	{
		//interpola a cor do exterior da célula e da face para que elas desapareçao
		cellExteriorSprite.DOColor(fadeOutColor, 1f);
		faceSprite.DOColor(fadeOutColor, 1f);
		
		//adiciona na lista de interpolações a interpolação da cor do núcleo que seria um loop
		loopTweens.Add(nucleoSprite.DOColor(halfColor,1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad));
	}
	
	public void DisableAnimations()
	{
		//interpola a cor do exterior da célula e da face para que fiquem visíveis
		cellExteriorSprite.DOColor(cellSpriteColor, 1f);
		faceSprite.DOColor(normalColor, 1f);
		
		//destroi todas as interpolações que tem na lista
		for (int i=0;i<loopTweens.Count;i++)
		{
			loopTweens[i].Kill(false);
		}
		
		//volta com a cor original do núcleo
		nucleoSprite.color = nucleoSpriteColor;
	}
}
