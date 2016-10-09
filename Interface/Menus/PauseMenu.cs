using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PauseMenu : MonoBehaviour {
	
	//guarda a referência do transform
	private Transform myTransform;
	
	//guarda a posição local inicial
	private float originalLocalPosY;
	
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
	
	public void BtnContinueGame()
	{	
		//é o botão de continuar jogo
		
		//se a animação não estiver completa, sai
		if (!animComplete) return;
		
		//esconde o menu de pausa
		HidePauseMenu();
	}
	
	public void BtnReplay()
	{
		//é o botão de reiniciar jogo, ele vai recarregar a cena de acordo com o modo de jogo
		
		//se a animação não estiver completa, sai
		if (!animComplete) return;
		
		//toca o som de click
		AudioManager.instance.PlayOneShotSound(AudioManager.instance.sound_click);
		
		//se for o modo protetor
		if (GameController.instance.isProtectorMode)
		{
			//toca a musica do modo protetor
			AudioManager.instance.PlayMusic(AudioManager.instance.music_ProtectorMode);
			//recarrega a cena
			GameController.instance.ReloadScene();
		}
		else//se for o modo viral
		{
			//se NÃO estiver no ciclo de vida
			if (!GameController.instance.isInLifeCycle)
			{
				//toca a musica do modo viral
				AudioManager.instance.PlayMusic(AudioManager.instance.music_ViralMode);
				//recarrega a cena
				GameController.instance.ReloadScene();
			}
			else//se ta dentro do ciclo de vida quer dizer que ele perdeu no mini game
			{
				//esconde o menu de pausa
				HidePauseMenu();
				//recarrega o minigame atual
				GameController.instance.ReloadMiniGame();
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
	
	public void ShowPauseMenu()
	{
		//se a animação não estiver completa, sai
		if (!animComplete) return;
	
		//desativa o booleano para evitar que o jogador selecione algum botão antes da hora
		animComplete = false;
		
		//pausa o jogo
		GameController.instance.SetPauseGame(true);
		//interpola a posição para que o painel de pause seja visível
		myTransform.DOLocalMoveY(0f, 2f, false).SetEase(Ease.OutBounce).OnComplete(AnimComplete);
	}
	
	void AnimComplete()
	{
		//esse metodo será chamado no momento em que a interpolação de movimento estar concluída
		
		//seta o booleano da animação concluída
		animComplete = true;
	}
	
	void HidePauseMenu()
	{
		//toca o som de click
		AudioManager.instance.PlayOneShotSound(AudioManager.instance.sound_click);
		
		//desativa o booleano para evitar que o jogador selecione algum botão antes da hora
		animComplete = false;
		
		//despausa o jogo
		GameController.instance.SetPauseGame(false);
		//interpola a posição do painel para que fique fora da tela
		myTransform.DOLocalMoveY(originalLocalPosY, 0.5f, false).SetEase(Ease.InSine).OnComplete(AnimComplete);
	}
}
