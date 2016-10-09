using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MiniGameVirusGroup : MonoBehaviour {
	
	//referência do pai do grupo de vírus atual
	//será usado apenas para pegar as referências das sprites dos grupos virais
	[SerializeField]private Transform spritesFather;
	//referência de todas as sprites que começam transparentes e terminam completamente visíveis
	//seriam os receptores e os envelopes exteriores
	private SpriteRenderer[] spritesToFadeIn;
	//cor original guardaria a cor inicial(transparente)
	//cor completa guardaria a cor final(visível)
	private Color originalColor, fullColor;
	
	void Start () {
		//guarda todas as referências das sprites que precisam aparecer/sumir
		spritesToFadeIn = spritesFather.GetComponentsInChildren<SpriteRenderer>();
		
		//pega a cor das sprites
		fullColor = spritesToFadeIn[0].color;
		//define a cor original que seria igual a full color, só que com alfa zero
		originalColor = fullColor; originalColor.a = 0f;
	}
	
	public void ShowSprites(bool show)
	{
		//comando que mostrará ou esconderá todas as sprites que precisam sumir/aparecer
		for (int c=0;c<spritesToFadeIn.Length;c++)
		{
			if (show)
			{
				spritesToFadeIn[c].DOColor(fullColor, 1.5f).SetEase(Ease.InOutSine);
			}
			else
			{
				spritesToFadeIn[c].color = originalColor;
			}
		}
		
	}
}
