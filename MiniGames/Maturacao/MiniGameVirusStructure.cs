using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MiniGameVirusStructure : MonoBehaviour {
	
	//guarda o nome do vírus
	public string virusName;
	//guarda as referências das sprites de todas as partes do vírus e o vírus completo
	//será usado no mini game de maturação onde o jogador deve selecionar as partes corretas do vírus atual
	public Sprite virion, espicula, envelope, capsidio, acidoNucleico, face;
}
