using UnityEngine;
using System.Collections;

public class MiniGameAcidNucleicMove : MonoBehaviour {
	
	//booleano para verificar se o mini game já acabou
	private bool isFinished;
	
	void OnTriggerEnter2D(Collider2D other)
	{
		//se acabou, sai fora
		if (isFinished) return;
		//se não for o núcleo, ignora
		if (other.name != "nucleo") return;
		
		//define que acabou
		isFinished = true;	
		
		//pausa o jogo
		GameController.instance.SetPauseGame(true);
		
		//se for o dna viral
		if (name == "DnaViral")
			//mostra informação sobre o ciclo lisogênico
			GameController.instance.interfaceScript.simpleInfo.ShowSimpleTip("No ciclo lisogênico ao chegar no núcleo da célula o DNA viral irá se integrar ao DNA da célula");
		else//se for rnaViral, mostra informação do ciclo lítico
			GameController.instance.interfaceScript.simpleInfo.ShowSimpleTip("Ao chegar no núcleo, o mesmo irá criar o RNA mensageiro!");
		
		//aciona o comando para avisar o game controller após 6 segundos
		Invoke("NextCycle", 6f);
	}
	
	
	void NextCycle()
	{
		//despausa o jogo e avisa ao game controller que acabou
		GameController.instance.SetPauseGame(false);
		GameController.instance.StartLifeCycle();
	}
}
