using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MiniGameMaturacaoGamePanelController : MonoBehaviour {
	
	//é a referência do texto que definirão o nome e a informação da parte atual. Essas informações estão nos paineis de cada parte(MiniGameMaturacaoGamePanel)
	public Text txtPanelName, txtPanelInfo;
	//referência da estrutura do vírus atual que guarda as sprites de cada parte
	//serve para verificar se as partes estão corretas
	public MiniGameVirusStructure virusStructure;
	//guarda todos os paineis das partes(capsidio, receptor, envelope, acido nucleico, face..)
	private MiniGameMaturacaoGamePanel[] panels;
	//referência do mini game atual para avisar que o mini game acabou
	private MiniGameMaturacao miniGameMaturacao;
	//currentPanel guarda o painel da parte atual, conform o jogador vai acertando, aumenta-se o contador dos paineis para passar para o próximo
	//choice seria a escolha feita pelo jogador e rightChoice seria a escolha certa
	private int currentPanel = 0, choice = -1, rightChoice = -1;
	
	void Start () {
		//guarda as referências
		panels = GetComponentsInChildren<MiniGameMaturacaoGamePanel>();
		miniGameMaturacao = FindObjectOfType<MiniGameMaturacao>();
	}
	
	public void StartPanel(MiniGameVirusStructure virusInfo)
	{
		//zera todas as variáveis e esconde todos os paineis das partes
		ResetAll();
		
		//guarda a estrutura do vírus atual para poder acessar as sprites certas dele
		virusStructure = virusInfo;
		
		//mostra o painel inicial, que no caso seria o de Acido Nucleico
		ShowPanelID(0);
	}
	
	void ShowPanelID(int num)
	{
		//guarda qual seria a escolha certa
		rightChoice = panels[num].GetRightChoice(GetRightSprite());
		
		//atualiza o painel da parte de acordo com seu nome e informação
		txtPanelName.text = panels[num].panelName;
		txtPanelInfo.text = panels[num].panelInfo;
		
		//mostra o painel atual
		panels[num].ShowPanel(true);
	}
	
	void HideAllPanels()
	{
		//essa função esconderá todos os paineis da parte
		
		for (int c=0;c<panels.Length;c++)
		{
			panels[c].ShowPanel(false);
		}
	}
	
	Sprite GetRightSprite()
	{
		//essa função servirá para saber qual é a parte certa
		//os paineis são ordenados pelo inspector
		if (currentPanel == 0) return virusStructure.acidoNucleico;
		if (currentPanel == 1) return virusStructure.capsidio;
		if (currentPanel == 2) return virusStructure.face;
		if (currentPanel == 3) return virusStructure.espicula;
		if (currentPanel == 4) return virusStructure.envelope;
		
		//se não achou nada, volta nulo(apenas pq é obrigado a retornar algo)
		return null;
	}
	
	public void BtnSelect(int num)
	{
		//essa função é acionada quando o jogador seleciona algum dos botões
		//o num é o numero do botão selecionado
		
		//se é pra parar, sai fora
		if (GameController.instance.stop) return;
		
		//guarda a escolha feita pelo jogador
		choice = num;
		//como selecionou, deve alterar a cor do botão para ficar destacado
		panels[currentPanel].SetButtonColor(num);
	}
	
	public void BtnConfirm()
	{
		//essa função é chamada quando, após o jogador selecionar alguma parte, o jogador pressionar o botão confirmar
		
		//se é pra parar, sai 
		if (GameController.instance.stop) return;
		
		//se a escolha do jogador for a certa
		if (choice == rightChoice)
		{
			//desativa o painel atual
			panels[currentPanel].ShowPanel(false);
			
			//incrementa o contador do painel atual para passar para o próximo
			currentPanel++;
			
			//se o contador já é igual ao máximo de paineis que existem, quer dizer que finalizou todos
			if (currentPanel>=panels.Length)
			{
				//esconde todos os paineis
				HideAllPanels();
				
				//avisa ao controle do mini game que foi finalizado
				miniGameMaturacao.FinishedMiniGame();
			}
			else//se ainda existem paineis a serem mostrados
			{
				//mostra o novo painel
				ShowPanelID(currentPanel);
			}
		}
		else//se o jogador escolheu errado
		{
			//mostra o game over
			GameController.instance.interfaceScript.gameOverMenu.ShowGameOverMenu();
		}
	}
	
	public void ResetAll()
	{
		//reseta as variáveis
		currentPanel = 0;
		choice = -1;
		rightChoice = -1;
		
		//esconde todos os paineis
		HideAllPanels();
	}
}
