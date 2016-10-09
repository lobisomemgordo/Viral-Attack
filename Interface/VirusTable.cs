using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VirusTable : MonoBehaviour {
	//referência de todos os paineis do menu principal
	public GameObject panelTable, panelInfo, panelConfig, panelKnowMore, panelLifeCycle, btnOpenSite;
	
	//referência dos vírus que ficam no painel de seleção de vírus
	public GameObject[] virusTable = new GameObject[8];
	//referência dos vírus que ficam no painel de informação(detalhes sobre o vírus)
	public GameObject[] virusPanelInfo = new GameObject[8];
	
	//posição original dos paineis que serão movidos
	private Vector3 panelTableStartPos, panelConfigStartPos, panelLifeCycleStartPos;
	//guarda o virus selecionado
	private int currentVirusChoosen;
	//referência dos textos informativos sobre o vírus
	[SerializeField]private Text txtVirusName, txtVirusNucleicAcid, txtVirusShape, txtVirusTargetCell, txtVirusLifeCycle, txtVirusStructure;
	[SerializeField]private Text txtDeseaseName, txtDeseaseSymptoms;
	//referência dos textos "Selecione um vírus" e "Selecione um ciclo de vida"
	//serão usados para uma animação de escalonamento
	[SerializeField]private Text txtSelectVirus, txtSelectLifeCycle;
	
	void Start()
	{
		//muda o layer dos paineis que precisam ficar na fente dos outros
		SetSortingLayerUI2();
		
		//guarda as posições iniciais dos paineis. Será usado no momento em que tal painel será aberto
		panelTableStartPos = panelTable.transform.position;
		panelConfigStartPos = panelConfig.transform.position;
		panelLifeCycleStartPos = panelLifeCycle.transform.position;
		
		//ativa o painel e esconde da tela(tira do campo de visão da câmera)
		panelTable.SetActive(true);	
		panelTable.transform.position = Vector3.left * 30f;
		
		//ativa o painel e esconde da tela(tira do campo de visão da câmera)
		panelLifeCycle.SetActive(true);	
		panelLifeCycle.transform.position = Vector3.right * 30f;
		
		//ativa o painel e esconde da tela(tira do campo de visão da câmera)
		panelConfig.SetActive(true);	
		panelConfig.transform.position = Vector3.down * 30f;
		
		//escalona o painel para que fique invisivel
		panelInfo.transform.localScale = Vector3.zero;
		panelInfo.SetActive(true);	
		
		//escalona o painel para que fique invisivel
		panelKnowMore.transform.localScale = Vector3.zero;
		panelKnowMore.SetActive(true);	
		
		//ativa as animações dos textos "Selecione o vírus", "Selecione o modo de vida" e o botão "Curso de jogos digitais"
		btnOpenSite.transform.DOScale(txtSelectVirus.transform.localScale * 1.15f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
		txtSelectVirus.transform.DOScale(txtSelectVirus.transform.localScale * 1.3f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
		txtSelectLifeCycle.transform.DOScale(txtSelectVirus.transform.localScale * 1.3f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
	}
	
	void SetSortingLayerUI2()
	{
		//para evitar o trabalho manual, foi criado essa função que alterará para um layer que será desenhado na frente de tudo
		//esse layer é para as sprites dos vírus aparecerem na frente dos paineis
		SpriteRenderer[] sprites;
		
		for (int i=0;i<virusPanelInfo.Length;i++)
		{
			for (int c=0;c<virusPanelInfo[i].transform.childCount;c++)
			{
				sprites = virusPanelInfo[i].transform.GetComponentsInChildren<SpriteRenderer>();
				
				for (int u=0;u<sprites.Length;u++)
				{
					sprites[u].sortingLayerName = "UI2";
				}
			}
			virusPanelInfo[i].SetActive(false);
		}
	}
	
	void SetVirusTableInteractable(bool interactable)
	{
		//mesmo com a imagem da frente, ainda dava pra clicar no botão da tabela dos virus
		//essa função serve para evitar que isso aconteça
		Button[] buttons;
		
		for (int i=0;i<virusTable.Length;i++)
		{
			for (int c=0;c<virusTable[i].transform.childCount;c++)
			{
				buttons = virusTable[i].transform.GetComponentsInChildren<Button>();
				
				for (int u=0;u<buttons.Length;u++)
				{
					buttons[u].interactable = interactable;
				}
			}
		}
	}
	
	void HideAllVirusPanel()
	{
		//esconde todos os paineis de informações dos virus
		for (int i=0;i<virusPanelInfo.Length;i++)
		{
			virusPanelInfo[i].SetActive(false);
		}
	}
	
	void UpdateInfo(int num)
	{
		//aqui será atualizado os dados dos paineis de vírus, de acordo com o vírus atual escolhido
		txtVirusName.text = "Nome: <color=green>" + VirusInfos.instance.allTheVirus[num].name + "</color>";
		txtVirusNucleicAcid.text= "Ácido nucleico: <color=green>" +  VirusInfos.instance.allTheVirus[num].nucleicAcid+ "</color>";
		txtVirusShape.text= "Formato: <color=green>" +  VirusInfos.instance.allTheVirus[num].shape+ "</color>"; 
		txtVirusTargetCell.text = "Célula alvo: <color=green>" + VirusInfos.instance.allTheVirus[num].targetCell+ "</color>";
		txtVirusLifeCycle.text = "Ciclo de vida: <color=green>" + VirusInfos.instance.allTheVirus[num].lifeCycle+ "</color>";
		txtVirusStructure.text ="Estrutura: <color=green>" + VirusInfos.instance.allTheVirus[num].structure+ "</color>";
		
		txtDeseaseName.text = "Nome: <color=green>" + VirusInfos.instance.allTheVirus[num].disease.name+ "</color>";
		txtDeseaseSymptoms.text = "Sintomas: <color=green>" + VirusInfos.instance.allTheVirus[num].disease.symptoms+ "</color>";
	}
	
	public void BtnLifeCycle(bool isHiv)
	{
		//é o botão do modo Ciclo de Vida, podendo ser HIV(lisogênico) ou Influenza(Lítico)
		//foi utilizado o playerPrefs para que os dados sejam salvos e assim poderem ser usados nas outras cenas
		if (isHiv) PlayerPrefs.SetInt("currentVirusID", 7);
		else PlayerPrefs.SetInt("currentVirusID", 0);
		//define que vai ser apenas para realizar o ciclo de vida
		PlayerPrefs.SetInt("isLifeCycleOnly", 1);
		
		//carrega a cena do jogo
		SceneManager.LoadScene("PlayGround");	
	}
	
	public void BtnProtectorMode()
	{
		//é o botão do modo Protetor
		
		//carrega a cena do jogo-modo protetor
		SceneManager.LoadScene("ModoProtetor");
	}
	
	public void BtnOpenVirusTable()
	{
		//é o botão que abrira o painel que permitirá selecionar os vírus
		
		//ativa a interpolação para que ele fique visível ao jogador
		panelTable.transform.DOMove(panelTableStartPos,0.8f, false).SetEase(Ease.InOutSine);
		
		//ativa a interação com os botões do painel de seleção do vírus
		SetVirusTableInteractable(true);
	}
	
	public void BtnOpenConfigPanel()
	{
		//é o botão que abrirá o painel de configuração
		//ativa a interpolação para que ele fique visível ao jogador
		panelConfig.transform.DOMove(panelConfigStartPos,0.8f, false).SetEase(Ease.InOutSine);
	}
	
	public void BtnOpenLifeCyclePanel()
	{
		//é o botão que abrirá o painel de ciclo de vida
		//ativa a interpolação para que ele fique visível ao jogador
		panelLifeCycle.transform.DOMove(panelLifeCycleStartPos,0.8f, false).SetEase(Ease.InOutSine);
	}
	
	public void BtnOpenKnowMorePanel()
	{
		//é o botão que abrirá o painel "saiba mais" que mostra os créditos do jogo
		//escalona para que fique visível ao jogador
		panelKnowMore.transform.DOScale(1f,0.8f).SetEase(Ease.InOutSine);
	}
	
	public void BtnExit()
	{
		//é o botão de sair do jogo
		//fecha o jogo
		Application.Quit();
	}
	
	public void ClosePanelTable()
	{
		//"fecha" o painel de seleção do vírus
		//faz a interpolação para fora da tela
		panelTable.transform.DOMove(Vector3.left * 30f,0.8f, false).SetEase(Ease.InOutSine);
	}
	
	public void ClosePanelConfig()
	{
		//"fecha" o painel de configuração
		//faz a interpolação para fora da tela
		panelConfig.transform.DOMove(Vector3.down * 30f,0.8f, false).SetEase(Ease.InOutSine);
	}
	
	public void ClosePanelLifeCycle()
	{
		//"fecha" o painel de ciclo de vida
		//faz a interpolação para fora da tela
		panelLifeCycle.transform.DOMove(Vector3.right * 30f,0.8f, false).SetEase(Ease.InOutSine);
	}
	
	public void ClosePanelKnowMore()
	{
		//escalona o vírus para que ele fique invísivel
		panelKnowMore.transform.DOScale(0f,0.2f).SetEase(Ease.InOutSine);
	}
	
	public void BtnOpenSite()
	{
		//abre a pagina do navegador padrão com o site do curso
		Application.OpenURL("http://www.sociesc.org.br/pt/cursos-graduacao/conteudo.php?id=14563&mnu=13866&top=0&crs=2256");
	}
	
	public void OpenVirusInfo (int num) {
		//esconde todos os paineis
		HideAllVirusPanel();
		
		//desativa a interação dos botões do painel de seleção do vírus(se não ainda daria para selecionar os botões )
		SetVirusTableInteractable(false);
		
		//ativa o painel do vírus escolhido
		virusPanelInfo[num].SetActive(true);
		
		//atualiza as informações do vírus escolhido
		UpdateInfo(num);
		
		//define qual é o vírus escolhido. Será utilizado para carregar as informações e jogar a fase
		currentVirusChoosen = num;
		//panelTable.transform.DOMove(Vector3.left * 30f,0.2f, false).SetEase(Ease.InOutSine);
		panelInfo.transform.DOScale(1f,0.8f).SetEase(Ease.InOutSine);
	}
	
	public void CloseVirusPanelInfo()
	{
		//escalona o painel para que ele fique invisível
		panelInfo.transform.DOScale(0f,0.2f).SetEase(Ease.InOutSine);
		
		//ativa a interação do painel de seleção dos vírus
		SetVirusTableInteractable(true);
	}
	
	public void BtnStartLevel()
	{
		//seta o vírus atual(que foi selecionado previamente no metodo OpenVirusInfo(int num))
		PlayerPrefs.SetInt("currentVirusID", currentVirusChoosen);
		//define que vai ser um jogo normal
		PlayerPrefs.SetInt("isLifeCycleOnly", 0);
		
		//carrega a cena do jogo
		SceneManager.LoadScene("PlayGround");	
	}
	
}
