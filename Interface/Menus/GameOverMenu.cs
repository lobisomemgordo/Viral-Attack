using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GameOverMenu : MonoBehaviour {
	
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
	
	public void BtnReplay()
	{
		//é o botão de "Tentar novamente" e "Reiniciar jogo". Bascamente vai recarregar o jogo
		
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
		else//se não for o modo protetor
		{
			//se o jogador NÃO estiver no ciclo de vida
			if (!GameController.instance.isInLifeCycle)
			{
				//toca a musica do modo viral
				AudioManager.instance.PlayMusic(AudioManager.instance.music_ViralMode);
				//recarrega a cena
				GameController.instance.ReloadScene();
			}
			else//se ta dentro do ciclo de vida quer dizer que ele perdeu no mini game
			{
				//toca a musica do ciclo de vida
				AudioManager.instance.PlayMusic(AudioManager.instance.music_LifeCycle);
				//esconde o menu de gameOver
				HideGameOverMenu();
				//recarrega o mini game atual
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
		
		//carrega a cena do menu principal
		GameController.instance.LoadSecene("MainMenu");
	}
	
	public void ShowGameOverMenu()
	{
		//se a animação não estiver completa, sai
		if (!animComplete) return;
		
		//se for o modo protetor, toca o audio de erro. Se não for, irá tocar o som de "morto" pelo virus(no script Leucocito)
		if (GameController.instance.isProtectorMode)
			AudioManager.instance.PlayOneShotSound(AudioManager.instance.sound_wrong);
		
		//desativa o booleano para evitar que o jogador consiga selecionar os botões
		animComplete = false;
		
		//toca a musica do game over
		AudioManager.instance.PlayMusic(AudioManager.instance.music_gameOver);
		
		//pausa o jogo
		GameController.instance.SetPauseGame(true);
		//interpolara a posição para que fique vísivel ao jogador
		myTransform.DOLocalMoveY(0f, 2f, false).SetEase(Ease.OutBounce).OnComplete(AnimComplete);
	}
	
	void AnimComplete()
	{
		//esse metodo será chamado no momento em que a interpolação de movimento estar concluída
		
		//seta o booleano da animação concluída
		animComplete = true;
	}
	
	void HideGameOverMenu()
	{
		//desativa o boolean 
		animComplete = false;
		
		//despausa o jogo
		GameController.instance.SetPauseGame(false);
		//interpola a posição do painel para fora da tela
		myTransform.DOLocalMoveY(originalLocalPosY, 0.5f, false).SetEase(Ease.InSine).OnComplete(AnimComplete);
	}
}
