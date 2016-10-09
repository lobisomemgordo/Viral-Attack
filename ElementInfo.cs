using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ElementInfo : MonoBehaviour {
	
	//guarda as informações do elemento como o gráfico, o nome e a informação dele
	public Sprite elementSprite;
	public string elementName;
	public string elementInfo;
	//variável para verificar se já foi mostrado
	public bool hasSeen;
}
