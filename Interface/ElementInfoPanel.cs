using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class ElementInfoPanel : MonoBehaviour {
	
	//imagem do elemento
	public Image imgElement;
	//texto do nome e informação do elemento
	public Text txtElementName, txtElementInfo;
	//o botão de Ok que será animado
	public Transform btnOk;
	//elementInfos que não possuem um game object próprio
	[HideInInspector]public ElementInfo wrongCellElementInfo,cicloLitico, cicloLisogenico, viralModeComplete;
	
	//Transform do painel
	private Transform myTransform;
	//vetors3 que serão usados e não precisarão ser criados novos
	private Vector3 vecZero = Vector3.zero, vecOne = Vector3.one;
	//waitForSeconds que serão usados e não precisarão ser criados novos
	private WaitForSeconds waitToShowOkButton = new WaitForSeconds(6f);
	//referência da interpolação do botão
	private Tweener btnTweener;
	
	// Use this for initialization
	void Start () {
		myTransform = transform;
		myTransform.localScale = vecZero;
		btnOk.localScale = vecZero;
		
		//pega os element info que estão no game object
		ElementInfo[] elementsInfo = GetComponents<ElementInfo>();
		
		//guarda a referência do element info que informará que a célula é a errada
		wrongCellElementInfo = elementsInfo[0];
		
		//guarda a referência do element info que informará o ciclo lítico
		cicloLitico = elementsInfo[1];
		
		//guarda a referência do element info que informará o ciclo lisogênico
		cicloLisogenico = elementsInfo[2];
		
		//guarda a referência do element info que informará que o modo viral acabou. É o element info do ao finalizar com o HIV
		viralModeComplete = elementsInfo[3];
	}
	
	public void ShowElementInfo(ElementInfo elementInfo)
	{
		//garante que apareça algo
		if (elementInfo == null) elementInfo = wrongCellElementInfo;
		
		//se ja viu , sai fora
		if (elementInfo.hasSeen && elementInfo!= wrongCellElementInfo)
		{
			//despausa o jogo
			GameController.instance.SetPauseGame(false);
			//sai da função
			return;
		}
		
		//pausa o jogo
		GameController.instance.SetPauseGame(true);
		
		//se não existir imagem do elemento, desativa a imagem do painel
		//Se existir, ativa a imagem do painel
		if (elementInfo.elementSprite == null) imgElement.gameObject.SetActive(false);
		else imgElement.gameObject.SetActive(true);
		
		//atualiza as informações de acordo com o element info passado
		imgElement.sprite = elementInfo.elementSprite;
		txtElementName.text = elementInfo.elementName;
		txtElementInfo.text = elementInfo.elementInfo;
		
		//declara que leu para não mostrar mais
		elementInfo.hasSeen = true;
		
		//escalona para que seja visível ao jogador
		myTransform.DOScale(vecOne, 2f).SetEase(Ease.OutBounce);
		
		//inicia a corrotina que fará com que apareça o botão Ok depois de 6 segundos
		StartCoroutine(ShowOkButton());
	}
	
	public void ShowLifeCycleInfo(bool isLitico)
	{
		//mostra o element info do ciclo de vida
		if (isLitico) ShowElementInfo(cicloLitico);
		else ShowElementInfo(cicloLisogenico);
	}
	
	public void HideElementInfo()
	{
		//escalona o painel para que desapareça da tela
		myTransform.DOScale(vecZero, 0.6f);
		//avisa ao gamecontroller que o botão Ok foi pressionado e o painel foi retraído
		GameController.instance.ElementInfoOk();
	}
	
	IEnumerator ShowOkButton()
	{
		//se já existe uma animação do botão, sai da corrotina
		if (btnTweener!= null) yield break;
		
		//garante que o botão desapareça da tela
		btnOk.localScale = vecZero;
		
		//espera um tempo para que o botão seja mostrado, atualmente é 6 segundos
		yield return waitToShowOkButton;
		
		//escalona o botão para que apareça na tela
		btnTweener = btnOk.DOScale(vecOne, 0.5f).OnComplete(OnCompleteShowBtn);
	}
	
	void OnCompleteShowBtn()
	{
		//esse método será chamado quando a animação do botão estar completada
		
		//da um efeito no botão(escalonamento alto e baixo)
		btnOk.DOPunchScale(vecOne, 0.7f, 2, 1f);
		//tira a referência para que seja feito uma animação novamente
		btnTweener = null;
	}
}
