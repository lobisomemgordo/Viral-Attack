using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CellProtectorMode : MonoBehaviour {
	
	//guarda a quantidade de vidas que a célula possui. Caso perca todas, haverá o game over
	[SerializeField]private int lifes = 3;
	//referência do transform para fazer a interpolação do escalonamento
	private Transform myTransform;
	//referência usada para verificar se foi um vírus que colidiu com a célula
	private VirusProtectorMode virus;
	//guarda todas as sprites da célula. Ex: parte exterior, face,etc..
	private SpriteRenderer[] sprites;
	//cor que será usada para avisar visualmente que a célula foi atingida
	private Color redColor = Color.red;
	//vetor de booleanos para verificar se tal sprite já está vermelha
	private bool[] isRed;
	//interpolação do escalonamento ou posição que ficará na célula desde o começo
	private Tweener tween;
	
	void Start () {
		//guarda as referências
		myTransform = transform;
		sprites = GetComponentsInChildren<SpriteRenderer>();
		
		//ativa as animações de escalonamento ou de posição
		if (name == "CelulaNervosa") myTransform.DOShakePosition(2f, 1f,5,30, false).SetLoops(-1, LoopType.Yoyo);
		else tween = myTransform.DOShakeScale(2f, 0.1f,1,50).SetLoops(-1, LoopType.Yoyo);
		
		//aloca o espaço do vetor de acordo com o tamanho do vetor de sprites
		isRed = new bool[sprites.Length];
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		//tenta pegar o component dos vírus
		virus = other.GetComponent<VirusProtectorMode>();
		
		//se não for nulo, quer dizer que tinha o componente vírus, ou seja, é um vírus
		if (virus != null)
		{
			//avisa ao vírus que a célula foi atingida
			virus.VirusEnteredTheCell();
			//diminui as vidas
			lifes--;
			
			//percorre todas as sprites da célula
			for (int c=0;c<sprites.Length;c++)
			{
				//só vai ficar vermelho se ainda não estiver vermelho, evita bugar a cor da sprite
				if (!isRed[c])
				{
					//faz o comando para tornar a sprite vermelha
					sprites[c].DOColor(redColor, 0.3f).SetLoops(2, LoopType.Yoyo).OnComplete(CompleteColor);
					//marca que ficou vermelho
					isRed[c] = true;
				}
			}
			
			//verifica se a célula já morreu
			if (lifes <= 0)
			{
				//tira a animação que fica ligada desde o início
				if (tween!= null) tween.Kill();
				//celula destruida
				myTransform.DOScale(Vector3.zero, 1f);
				//chama o game over
				GameController.instance.interfaceScript.gameOverMenu.ShowGameOverMenu();
			}
		}
	}
	
	void CompleteColor()
	{
		//essa função será chamada quando a animação de ficar vermelha for completada(ficou vermelha e voltou para a cor normal)
		
		//percorre todo o vetor de boolean e desativa
		for (int c=0;c<isRed.Length;c++)
		{
			isRed[c] = false;
		}
	}
}
