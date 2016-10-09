using UnityEngine;
using System.Collections;

public class InterfaceScript : MonoBehaviour {
	
	//é guardado a referência de todos os scripts de interface
	[HideInInspector]public ElementInfoPanel elementInfoPanel;
	[HideInInspector]public SimpleInfo simpleInfo;
	[HideInInspector]public VirusIntroPanel virusIntroPanel;
	[HideInInspector]public TutorialPanel tutorialPanel;
	[HideInInspector]public GameOverMenu gameOverMenu;
	[HideInInspector]public VictoryMenu victoryMenu;
	
	//guarda a referência dos joysticks
	private JoystickController[] joysticks;
	
	void Start () {
		//busca as referências de todos os scripts de interface
		joysticks = FindObjectsOfType<JoystickController>();
		elementInfoPanel = FindObjectOfType<ElementInfoPanel>();
		simpleInfo = FindObjectOfType<SimpleInfo>();
		virusIntroPanel = FindObjectOfType<VirusIntroPanel>();
		tutorialPanel = FindObjectOfType<TutorialPanel>();
		gameOverMenu = FindObjectOfType<GameOverMenu>();
		victoryMenu = FindObjectOfType<VictoryMenu>();
	}
	
	public void ShowJoysticks(bool show, int joystickID)
	{
		//se for o modo protetor, retorna. Essa função é exclusiva do modo viral
		if (GameController.instance.isProtectorMode) return;
		
		//se for um joystick único
		if (joystickID >= 0 && joystickID<=1)
		{
			//se é pra esconder, reseta a posição e valore do joystick
			if (!show) joysticks[joystickID].ResetJoystick();
			
			//desativa o joystick
			joysticks[joystickID].transform.parent.gameObject.SetActive(show);//background do joystick
			joysticks[joystickID].gameObject.SetActive(show);//joystick em si
		}
		else//se for outro valor, seta os 2
		{
			for (int c=0;c<joysticks.Length;c++)
			{
				//se é pra esconder, reseta as posições e valores dos joysticks
				if (!show) joysticks[c].ResetJoystick();
				
				//desativa os joysticks
				joysticks[c].transform.parent.gameObject.SetActive(show);//background do joystick
				joysticks[c].gameObject.SetActive(show);//joystick em si
			}
		}
	}
	
	public void BtnRestart()
	{
		//botão usado para recarregar a cena
		
		//recarrega a cena
		GameController.instance.ReloadScene();
	}
	
	public void BtnMainMenu()
	{
		//botão usado para ir para o menu principal
		
		//toca a musica do menu principal
		AudioManager.instance.PlayMusic(AudioManager.instance.music_MainTheme);
		//carrega o menu principal
		GameController.instance.LoadSecene("MainMenu");
	}
}
