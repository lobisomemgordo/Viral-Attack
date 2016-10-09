using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Vacine : MonoBehaviour {
	
	//é o tempo que durará o poder especial vacina
	[SerializeField]private float timeToDisable = 20f;
	//particula que seria a própria vacina
	private ParticleSystem particle;
	//referência temporária para verificar se foi um vírus que colidiu com a vacina
	private VirusProtectorMode virus;
	
	void Start () {
		particle = GetComponentInChildren<ParticleSystem>();
		
		//começa desativado
		ShowVacine(false);
	}
	
	public void ShowVacine(bool show)
	{
		//vai mostrar ou esconder a vacina
		if (show)
		{
			//cancela o invoke antigo, caso tenha algum
			CancelInvoke("DisableAfterTime");
			//depois do tempo colocado para desativar, será acionado o método que desativa a vacina
			Invoke("DisableAfterTime", timeToDisable);
			
			//ativa a vacina
			particle.gameObject.SetActive(true);
		}
		else
		{
			//desativa a vacina
			particle.gameObject.SetActive(false);
		}
	}
	
	void DisableAfterTime()
	{
		//irá desativar a vacina após o tempo de duração da vida acabar
		ShowVacine(false);
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		//tenta pegar o componente do vírus
		virus = other.GetComponent<VirusProtectorMode>();
		
		//se não for nulo, ou seja, se for vírus
		if (virus != null)
		{
			//não funciona com HIV
			if (other.name != "virus_HIV(Clone)")
			{
				//destroi
				virus.DestroyVirus(false);
			}
		}
	}
}
