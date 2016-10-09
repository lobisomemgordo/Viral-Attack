using UnityEngine;
using System.Collections;

public class JoystickController : MonoBehaviour {
	
	//booleano para verificar se é o joystick de movimento ou de rotação
	[SerializeField]private bool analogJoystick;
	//alcance máximo do joystick
	public float maxRange = 50f;
	//referência do player para setar o vetor de movimento(inputAxis)
	public PlayerController player;
	
	//transforms que serão usados para rotação e o transform do joystick 
	private Transform playerTransform, playerRnaTransform, myTransform;
	//cria os vectors3 que serão usados para não ter que criar toda hora
	private Vector3 vecZero = Vector3.zero, rot = Vector3.zero;
	//guarda a posição do mouse e a posição local do joystick
	private Vector3 mousePos, localPos;
	//booleano para ver se o joystick foi pressionado
	private bool isPressed;
	//referência que será usada para rotacionar o rna viral de um mini game
	private MiniGameRnaViral miniGameRnaViral;
	
	void Start () {
		//guarda a referência do transform para ser alterado a posição do joystick
		myTransform = transform;
		
		//acha o playerController
		player = FindObjectOfType<PlayerController>();
		//guarda o transform do playerController, será utilizado para fazer a rotação do player
		playerTransform = player.transform;
		
		//guarda as referências do mini game
		miniGameRnaViral= FindObjectOfType<MiniGameRnaViral>();
		playerRnaTransform = miniGameRnaViral.transform;
	}
	
	// Update is called once per frame
	void Update () {
		//se o botão/toque estiver pressionado, atualiza a posição do joystick
		if (isPressed) UpdatePos();	
	}
	
	public void UpdatePos()
	{
		//pega a posição do toque na tela e converte para uma coordenada do mundo
		mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//define o Z como o Z da câmera para ter mais precisão
		mousePos.z = Camera.main.transform.position.z;
		
		//seta a posição do joystick de acordo com o toque na tela
		myTransform.position = mousePos;
		
		//guarda a posição local
		localPos = myTransform.localPosition;
		
		//guarda a nova posição, mas limitando ela de acordo com o maxRange
		
		//se for o joystick de movimento, verifica o eixo X e Y
		if (analogJoystick)
			localPos.Set(Mathf.Clamp(localPos.x, -maxRange,maxRange), Mathf.Clamp(localPos.y, -maxRange,maxRange),0f);
		else //o de rotação só vai mudar o x
			localPos.Set(Mathf.Clamp(localPos.x, -maxRange,maxRange), 0f,0f);
		
		//seta a posição local de acordo com o limite
		myTransform.localPosition = localPos;
		
		//se for o joystick de movimento, seta o vetor de movimento(inputAxis) do playerController
		if (analogJoystick) player.inputAxis.Set (localPos.x/maxRange, localPos.y/80f, 0f);
		else
		{
			//se for mini game de pegar a transcriptase reversa
			if (GameController.instance.cicloDeVida == GameController.CicloDeVida.integracao1_HIV)//mini game snake
			{
				//se o mini game ta parado, não rotaciona
				if (miniGameRnaViral.stop) return;
				
				//limpa a rotação anterior
				rot = vecZero;
				
				//seta a rotação de acordo com o valor do joystick
				//ta invertido então foi multiplicado por -1
				rot.Set(0f,0f,(localPos.x /80f) * -1f);
				
				//rotaciona o rna viral do minigame
				//foi multiplicado para ser mais rápido
				playerRnaTransform.Rotate(rot*6);
			}
			else//jogo normal
			{
				//limpa a rotação
				rot = vecZero;
				
				//seta a rotação de acordo com o valor do joystick
				//ta invertido então foi multiplicado por -1
				rot.Set(0f,0f,(localPos.x /80f) * -1f);
				
				//rotaciona o player(que no caso são os vírions normais fora da célula)
				playerTransform.Rotate(rot);
			}
		}
	}
	
	public void MouseDown()
	{
		//quando o mouse/toque estiver pressionado, aciona o booleano
		isPressed = true;
	}
	
	public void MouseUp()
	{
		//se o mouse/toque foi solto, reseta o joystick
		ResetJoystick();
	}
	
	public void ResetJoystick()
	{
		//desativa o boolean para parar de atualizar
		isPressed = false;
		
		//zera o inputAxis(vetor que guarda o valor do movimento) do player
		if (player != null) player.inputAxis = vecZero;
		//zera a posição do joystick, ficando bem no centro
		myTransform.localPosition = vecZero;
	}
}
