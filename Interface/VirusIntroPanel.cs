using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class VirusIntroPanel : MonoBehaviour {
	
	//referência do transform do botão OK. Será usado para animação de escalonamento
	public Transform btnOk;
	[Space]
	[Header("Virus")]
	//imagem do vírus
	public Image imgVirus;
	//o nome do vírus
	public Text txtVirusName;
	[Space]
	[Header("Target Cell")]
	//imagem da célula alvo
	public Image imgTargetCell;
	//nome da célula alvo
	public Text txtTargetCellName;
	
	//referência do transform do painel
	private Transform myTransform;
	//vectors3 que serão usados e não precisarão ser criados posteriormente
	private Vector3 vecZero = Vector3.zero, vecOne = Vector3.one;
	//waitForSeconds que será usado
	private WaitForSeconds waitToShowOkButton = new WaitForSeconds(5f);
	//referência da interpolação do botão Ok
	private Tweener btnTweener;
	
	void Start () {
		//guarda o transform
		myTransform = transform;
		//seta o escalonamento do painel e do botão para zero, para assim não aparecer na tela
		myTransform.localScale = vecZero;
		btnOk.localScale = vecZero;
	}
	
	public void ShowVirusIntro(int virusIndex)
	{
		//é o método que irá mostrar o painel de introdução de acordo com o ID passado
		
		//pausa o jogo
		GameController.instance.SetPauseGame(true);
		
		//se for maior que 7(é o hiv, o último), limita para 7
		if (virusIndex >7) virusIndex = 7;
		
		//atualiza as informações do painel de introdução de acordo com o vírus passado
		txtVirusName.text = VirusInfos.instance.allTheVirus[virusIndex].name;
		txtTargetCellName.text = VirusInfos.instance.allTheVirus[virusIndex].targetCell;
		imgVirus.sprite = VirusInfos.instance.allTheVirus[virusIndex].sprite;
		imgTargetCell.sprite = VirusInfos.instance.allTheVirus[virusIndex].targetCellSprite;
		
		//escalona o painel para que fique visível
		myTransform.DOScale(vecOne, 2f).SetEase(Ease.OutBounce);
		
		//corrotina para mostrar o botão OK
		StartCoroutine(ShowOkButton());
	}
	
	public void HideVirusIntroPanel()
	{
		//escalona o painel para que não seja mostrado
		myTransform.DOScale(vecZero, 0.6f);
		//avisa ao gameController que o painel foi fechado
		GameController.instance.VirusIntroOk();
	}
	
	IEnumerator ShowOkButton()
	{
		//se já existe animação do botão, sai para não bugar
		if (btnTweener!= null) yield break;
		
		//escalona o botão para que não apareça
		btnOk.localScale = vecZero;
		
		//espera os segundos determinados
		yield return waitToShowOkButton;
		
		//interpola o escalonamento para que o botão fique visível
		btnTweener = btnOk.DOScale(vecOne, 0.5f).OnComplete(OnCompleteShowBtn);
	}
	
	void OnCompleteShowBtn()
	{
		//esse método será chamado após o escalonamento do botão estar completo
		
		//irá dar um efeito especial que destacará o botão
		btnOk.DOPunchScale(vecOne, 0.7f, 2, 1f);
		//tira a referência para poder ser usado novamente
		btnTweener = null;
	}

}
