using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class TutorialInfo{
	//classe que irá guardar as informações dos tutoriais
	
	//nome do tutorial, descrição do tutorial
	public string tutorialName, tutorialInfo;
	//imagem do tutorial
	public Sprite tutorialSprite;
	//booleano para ver se já foi visto
	[HideInInspector]public bool hasSeen;
}

public class TutorialPanel : MonoBehaviour {
	
	[Header("Panel Reference")]
	//referência da imagem do painel
	public Image tutorialImage;
	//referência dos textos do painel como o nome e informação
	public Text txtTutorialName, txtTutorialInfo;
	[Space]
	[Header("Tutorials")]
	//guarda os tutoriais que terão no jogo, serão criados direto no inspector
	public TutorialInfo[] tutorials;
	
	//referência do transform do painel para interpolação do movimento
	private Transform myTransform;
	
	void Awake () {
		//guarda o transform dele
		myTransform = transform;
	}
	
	public void ShowTutorial(int tutorialID)
	{
		//no modo protetor não se usa o player, então verifica antes se o player não é nulo para evitar erro
		if (GameController.instance.player!=null)
			GameController.instance.player.ResetVelocity();
		
		//pausa o jogo
		GameController.instance.SetPauseGame(true);
		
		//define que o tutorial já foi visto
		tutorials[tutorialID].hasSeen = true;
		//atualiza a imagem e os textos do tutorial de acordo com o ID
		tutorialImage.sprite = tutorials[tutorialID].tutorialSprite;
		txtTutorialName.text = tutorials[tutorialID].tutorialName;
		txtTutorialInfo.text = tutorials[tutorialID].tutorialInfo;
		
		//interpola a posição para que fique visível
		myTransform.DOLocalMoveX(0f,2f,false);
	}
	
	public void HideTutorial()
	{
		//no modo protetor não se usa o player, então verifica antes se o player não é nulo para evitar erro
		if (GameController.instance.player!=null)
			GameController.instance.player.ResetVelocity();
		
		//interpola a posição para que saia da tela
		myTransform.DOLocalMoveX(1052f,2f,false);
		//avisa ao gameController que foi escondido o painel de tutorial
		StartCoroutine(GameController.instance.TutorialOk());
	}
}
