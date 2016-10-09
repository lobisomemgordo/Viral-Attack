using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class MiniGameMaturacao : MonoBehaviour {
	
	//guarda os paineis do mini game, o primeiro seria onde mostra o vírus e todas suas partes(é onde se memoriza)
	//o segundo seria o painel onde você seleciona as partes corretas
	[SerializeField]private Transform[] panels;
	//referência das imagens que das partes do vírus e o vírus completo
	[SerializeField]private Image virusImage, faceImage, capsidioImage, envelopeImage, acidoNucleicoImage, receptorImage;
	//referência dos textos do jogo: txtNoEnvelope seria o X que apareceria no capsídio caso o vírus não possua o mesmo
	//txtVirusName seria o texto do nome do vírus atual
	[SerializeField]private Text txtNoEnvelope, txtVirusName;
	//guarda a cor do acido nucléico para ficar de acordo com oque é passado: azul para dna e vermelho para rna
	[SerializeField]private Color[] acidoNucleicoColor;
	//guarda a referência do painel do mini game onde está a lógica da seleção de partes/verificação se acertou
	private MiniGameMaturacaoGamePanelController gamePanelController;
	//guarda todas as sprites das partes do vírus e ele mesmo completo
	private MiniGameVirusStructure[] virusStructures;
	//referência do transform para posiconamento do painel do mini game
	private Transform myTransform;
	//posição original para reposicionar o painel de mini game
	private float originalLocalY;
	
	void Start () {
		//busca todas as referências
		myTransform = transform;
		originalLocalY = myTransform.localPosition.y;
		virusStructures = GetComponentsInChildren<MiniGameVirusStructure>();
		gamePanelController = GetComponentInChildren<MiniGameMaturacaoGamePanelController>();
	}
	
	public void StartMiniGame(int currentVirus)
	{
		//no momento em que começa, carrega o primeiro painel que seria o de memorização do vírus atual
		LoadFirstPanel(currentVirus);
		
		//interpola o movimento do painel do mini game para que seja visível ao jogador
		myTransform.DOLocalMoveY(0f, 1f, false).SetEase(Ease.InExpo);
	}
	
	public void BtnStartGame()
	{
		//botão do painel de memorização
		
		//se é pra parar, ignora o toque
		if (GameController.instance.stop) return;
		
		//carrega o segundo painel que seria o de selecionar as partes
		LoadSecondPanel();
	}
	
	public void FinishedMiniGame()
	{
		//essa função será chamada quando o mini game for finalizado
		
		//interpola a posição para que o painel saia da tela
		myTransform.DOLocalMoveY(originalLocalY, 4f, false).SetEase(Ease.InSine);
		//avisa ao game controller que acabou
		GameController.instance.StartLifeCycle();	
	}
	
	void LoadFirstPanel(int currentVirus)
	{
		//se for papiloma humano;
		if (currentVirus == 4)
		{
			// ativa o texto com um X para dizer que não há envelope
			txtNoEnvelope.enabled = true;
			//desativa a imagem do envelope
			envelopeImage.sprite = null;
		}
		else//desativa o X se não for o papiloma virus 
			txtNoEnvelope.enabled = false;
		
		//atualiza as informações e partes do painel de memorização de acordo com o vírus atual
		virusImage.sprite = virusStructures[currentVirus].virion;
		txtVirusName.text = virusStructures[currentVirus].virusName;
		acidoNucleicoImage.sprite = virusStructures[currentVirus].acidoNucleico;
		receptorImage.sprite = virusStructures[currentVirus].espicula;
		faceImage.sprite = virusStructures[currentVirus].face;
		capsidioImage.sprite = virusStructures[currentVirus].capsidio;
		envelopeImage.sprite = virusStructures[currentVirus].envelope;
		
		// se for ciclo DNA e NÃO for o HIV(ele não é um vírus DNA)
		if (GameController.instance.player.IsVirusDNA() && currentVirus!= 7)
			acidoNucleicoImage.color = acidoNucleicoColor[0];//cor azul
		else acidoNucleicoImage.color = acidoNucleicoColor[1];//cor vermelha
		
		//ativa o primeiro painel(memorizar as partes) e desativa o segundo(selecionar as partes)
		panels[0].DOScale(1f, 1f);
		panels[1].DOScale(0f, 0.5f);
	}
	
	void LoadSecondPanel()
	{
		//avisa para a lógica do painel de seleção das partes do vírus que é para ser iniciada
		gamePanelController.StartPanel(virusStructures[GameController.instance.player.currentVirusID]);
		
		//desativa o primeiro painel(memorizar as partes) e ativa o segundo(selecionar as partes)
		panels[0].DOScale(0f, 0.5f);
		panels[1].DOScale(1f, 1f);
	}
}
