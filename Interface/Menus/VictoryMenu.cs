using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class VictoryMenu : MonoBehaviour {

	//guarda a referência do transform
	private Transform myTransform;
	
	//guarda a posição local inicial
	private float originalLocalPosY;
	
	//é o texto que será utilizado para mostrar uma mensagem de vitória customizada de acordo com o vírus
	[SerializeField]private Text txtSuccess;
	
	//booleano para verificar se a animação de "entrar" está completa
	//evita que o jogador consiga clicar nos botões antes da animação estar completa
	private bool animComplete;
	
	void Awake()
	{
		//guarda o transform
		myTransform = transform;
		
		//pega a posição local inicial(só será alterado o Y)
		originalLocalPosY = myTransform.localPosition.y;	
		
		//começa ativada para poder chamar pela primeira vez
		animComplete = true;
	}
	
	public void BtnNextLevel()
	{
		//se a animação não estiver completa, sai
		if (!animComplete) return;
		
		//toca o som de click
		AudioManager.instance.PlayOneShotSound(AudioManager.instance.sound_click);
		
		//se for qualquer vírus, menos o hiv, vai executar o seguinte comando. Se for o HIV, vai ter um elementInfo próprio que aparecerá
		if (GameController.instance.player.currentVirusID < 7)//hiv, é o ultimo
		{
			//se for só o ciclo de vida e for o virus influenza, volta pro menu
			if (PlayerPrefs.GetInt("isLifeCycleOnly") == 1 && GameController.instance.player.currentVirusID == 0)
			{
				//toca a musica do menu principal
				AudioManager.instance.PlayMusic(AudioManager.instance.music_MainTheme);
				
				//carrega o menu principal
				GameController.instance.LoadSecene("MainMenu");
			}
			else//se não for o modo ciclo de vida, passa para o próximo vírus
			{
				//passa para o próximo index que seria outro vírus
				GameController.instance.player.currentVirusID++;
				//salva os dados
				GameController.instance.SaveData();
				//recarrega a cena que iniciará com os novos dados
				GameController.instance.ReloadScene();
			}
		}
	}
	
	public void BtnMainMenu()
	{
		//se a animação não estiver completa, sai
		if (!animComplete) return;
		
		//toca o som de click
		AudioManager.instance.PlayOneShotSound(AudioManager.instance.sound_click);
		
		//toca a musica do menu principal
		AudioManager.instance.PlayMusic(AudioManager.instance.music_MainTheme);
		
		//carrega o menu principal
		GameController.instance.LoadSecene("MainMenu");
	}
	
	public void ShowVictoryMenu(string msgToShow)
	{
		//se a animação não estiver completa, sai
		if (!animComplete) return;
		
		//toca o som de acerto
		AudioManager.instance.PlayOneShotSound(AudioManager.instance.sound_right);
		
		//desativa o boolean que só voltará a ser ativado quando a animação estiver completa
		animComplete = false;
		
		//se for o HIV
		if (GameController.instance.player.currentVirusID == 7)//hiv, é o ultimo
		{
			//mostra o painel element info dizendo que ele finalizou o modo viral
			//o botão ok irá redirecionar o jogador ao menu principal
			GameController.instance.interfaceScript.elementInfoPanel.ShowElementInfo(GameController.instance.interfaceScript.elementInfoPanel.viralModeComplete);
		}
		else//se não for o HIV
		{
			//pausa o jogo
			GameController.instance.SetPauseGame(true);
			//atualiza o texto do painel de vitória
			txtSuccess.text = msgToShow;
			//interpola a posição para que fique visível ao jogador
			myTransform.DOLocalMoveY(0f, 2f, false).SetEase(Ease.OutBounce).OnComplete(AnimComplete);
		}
		
	}
	
	void AnimComplete()
	{
		//esse metodo será chamado no momento em que a interpolação de movimento estar concluída
		
		//seta o booleano da animação concluída
		animComplete = true;
	}
	
	void HideVictoryMenu()
	{
		//despausa o jogo
		GameController.instance.SetPauseGame(false);
		//interpola a posição para que saia da tela do jogador
		myTransform.DOLocalMoveY(originalLocalPosY, 0.5f, false).SetEase(Ease.InSine).OnComplete(AnimComplete);
	}
}

