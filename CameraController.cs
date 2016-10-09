using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraController : MonoBehaviour {
	
	//referência do alvo que a câmera seguiria(player)
	public Transform target;
	
	//referência do transform da câmera para ajustar a posição
	private Transform myTransform;
	//vector3 que seria usado para definir a nova posição da câmera
	private Vector3 newPos = Vector3.zero;
	//booleano para saber se a câmera está com o zoom ativado ou não
	[HideInInspector]public bool zoom;
	//variavel que verifica se é necessário atualizar o zoom da câmera e outra variável que verifica se é pra parar tudo
	private bool updateZoom, stop;
	//referência da câmera para poder mudar o field of view(no 2d isso seria o tamanho ortografico)
	private Camera cam;
	//guarda os valores originais
	//zoom size seria qual o valor do zoom(o mais próximo que pode chegar)
	private float originalCamSize, originalPosZ, zoomSize;
	//variável temporária que guarda a posição do jogador
	private Vector3 playerPos;

	void Start () {
		//guarda o transform do player para poder seguir
		target = FindObjectOfType<PlayerController>().transform;
		//guarda seu próprio transform para posicionamento
		myTransform = transform;	
		//guarda a câmera(é a unica da cena)
		cam = Camera.main;
		//guarda os valores iniciais
		originalCamSize = cam.orthographicSize;
		originalPosZ = myTransform.position.z;
		newPos = myTransform.position;
		
		//define o valor máximo de zoom
		zoomSize = 2f;
	}
	
	void Update () {
		//se é pra parar, sai
		if (stop) return;
		
		//se possui um alvo
		if (target) 
		{
			//guarda a posição do alvo
			playerPos = target.position;
			//o z da câmera continua sendo o original, só mudaria o X e o Y
			playerPos.z = originalPosZ;
			
			//se está no zoom, a interpolação do movimento é mais lenta
			//se não está no zoom, a interpolação do movimento é mais rápida
			if (zoom) newPos = Vector3.Slerp(myTransform.position,playerPos, Time.deltaTime * 2f);
			else newPos = Vector3.Slerp(myTransform.position,playerPos, Time.deltaTime * 5f);
			
			//seta a nova posição da câmera
			myTransform.position = newPos;
		}
		
		//se for para atualizar o zoom
		if (updateZoom)
		{
			//chama o método
			UpdateZoom();
		}
	}
	
	public void ShakeCamera(Vector3 strenght)
	{
		//esse método será usado pela classe MiniGameLiberation
		//é acionado no momento em que o jogador atingi o grupo de vírus no limite da célula
		
		//para de seguir o alvo
		stop = true;
		//dá uma tremida na posição da câmera
		myTransform.DOShakePosition(1f,strenght,10,90,false).OnComplete(ShakeDone);
	}
	
	void ShakeDone()
	{
		//depois que acabar a tremida, volta a seguir o alvo
		stop = false;
	}
	
	public void Zoom(bool isZoom)
	{
		//irá ativar/desativar o zoom
		zoom = isZoom;
		updateZoom = true;
	}
	
	void UpdateZoom()
	{
		//se estiver no zoom, irá se aproximar do alvo
		if (zoom)
		{
			//enquanto não atingiu o valor necessário, continua indo. Se atingiu, para de atualizar.
			if (cam.orthographicSize > zoomSize) cam.orthographicSize-=Time.deltaTime*3f;
			else updateZoom = false;
		}
		else//se não estiver no zoom, irá se afastar do alvo
		{
			//enquanto não atingiu o valor necessário, continua indo. Se atingiu, para de atualizar.
			if (cam.orthographicSize < 5.0f) cam.orthographicSize+=Time.deltaTime*3f;
			else updateZoom = false;
		}
	}
}
