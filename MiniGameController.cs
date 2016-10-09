using UnityEngine;
using System.Collections;

public class MiniGameController : MonoBehaviour {
	
	//são todas as referências dos mini games usados pelo game controller
	public MiniGameIntegracao integracao;
	public MiniGameIntegracaoHIV integracaoHIV;
	public MiniGameBiossintese biossintese;
	public MiniGameMaturacao maturacao;
	public MiniGameLiberation liberation;
	
	void Start () {
		//guarda todas as referências dos mini games que serão usados no game controller
		integracao = FindObjectOfType<MiniGameIntegracao>();
		integracaoHIV = FindObjectOfType<MiniGameIntegracaoHIV>();	
		biossintese = FindObjectOfType<MiniGameBiossintese>();
		maturacao = FindObjectOfType<MiniGameMaturacao>();
		liberation = FindObjectOfType<MiniGameLiberation>();
	}
}
