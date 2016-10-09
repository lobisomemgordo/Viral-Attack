using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MiniGameMaturacaoGamePanel : MonoBehaviour {
	
	//são as informações da parte atual. Ex: Nome: Capísio. Info: Envoltório protéico.. etc..
	//será usado no controlador do painel de seleção de partes(MiniGameMaturacaoGamePanelController)
	public string panelName, panelInfo;
	
	//referência dos botões que serão utilizados para pegar as referências de suas imagens
	private Button[] buttons;
	//as imagens dos botões seriam para ser alteradas de cor. Quando o jogador seleciona tal botão, a imagem dele fica na cor verde
	//as imagens(Images) são as imagens que ficam dentro dos botões(são filhas dos botões)
	//as Images servirão para que possa ser alterado o gráfico de acordo com a parte e o vírus atual
	private Image[] buttonsImages, images;
	//vectors3 que seriam usados
	private Vector3 vecZero = Vector3.zero, originalLocalScale;
	//referência do transform do painel de selecionaras partes
	private Transform myTransform;
	//cores que seriam utilizadas
	private Color white = Color.white, green = Color.green;
	
	void Start()
	{
		//guarda as referências
		myTransform = transform;
		originalLocalScale = myTransform.localScale;
		
		buttons = GetComponentsInChildren<Button>();
		
		//cria um vetor com o tamanho devido
		buttonsImages = new Image[buttons.Length];
		
		for (int c=0;c<buttonsImages.Length;c++)
		{
			//guarda as referências das imagens dos botões
			buttonsImages[c] = buttons[c].GetComponent<Image>();
		}
		
		//cria um vetor com o tamanho devido
		images = new Image[buttons.Length];
		
		for (int c=0;c<images.Length;c++)
		{
			//guarda as referências das imagens que são filhas dos botões
			images[c] = buttons[c].transform.GetChild(0).GetComponent<Image>();
		}
		
		//começa com o painel desativado
		ShowPanel(false);
	}
	
	public void ShowPanel(bool show)
	{
		//escalona para que seja visível
		if (show) myTransform.localScale = originalLocalScale;
		else//se for pra esconder
		{
			//reseta as cores dos botões(para que todas voltem a ficar brancas)
			RestartPanel();
			//escalona o painel para que fique "invisível"
			myTransform.localScale = vecZero;
		}
	}
	
	public int GetRightChoice(Sprite rightSprite)
	{
		//essa função percorrerá todos as imagens que são filhas dos botões(essas imagens guardam as partes dos vírus)
		for (int c=0;c<images.Length;c++)
		{
			//se a sprite passada for a mesma da imagem que é filha do botão, quer dizer que acertou
			if (rightSprite == images[c].sprite)
			{
				return c;
				break;
			}
		}
		
		//se não achou nada é por que foi a imagem errada
		return -1;
	}
	
	public void SetButtonColor(int buttonID)
	{
		//essa função irá colorir a imagem do botão
		//verde para selecionado e branco para nada
		for (int c=0;c<images.Length;c++)
		{
			//se tiver um id do botão vai setar a cor verde para declarar que foi selecionado
			if (c==buttonID) buttonsImages[c].color = green;
			//se não, deixa todos os botões com a cor padrão(branca)
			else buttonsImages[c].color = white;
		}
	}
	
	public void RestartPanel()
	{
		//todas as imagens dos botões ficarão com a cor padrão
		SetButtonColor(-1);
	}
}
