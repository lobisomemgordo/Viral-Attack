using UnityEngine;
using System.Collections;
using DG.Tweening;

public class VirusProtectorMode : MonoBehaviour {
	
	//velocidade em que o vírus se movimentará
	public float speed = 2f;
	//booleanos que verificarão se o vírus está se movimento ou se pode ser instanciado
	public bool moving, canRespawn;
	
	//vectors3 que serão usados seja pelo valor em sí ou parar guardar a posição e escalonamento
	private Vector3 vecZero = Vector3.zero, originalScale, bigScale, vecRight = Vector3.right, initialPosition;
	//usado vector2 pois não tem perigo do Z atrapalhar o shake e fazer com que a sprite suma
	private Vector2 xyVector = new Vector2(0.1f,0.1f);
	//transform do vírus para movimento
	private Transform myTransform;
	//gráfico do vírus
	private SpriteRenderer sprite;
	//cores que serão usadas: totalmente transparente e a original.
	private Color fadeOutColor, originalColor;
	//colisor do vírus
	private Collider2D myCollider;
	//booleano que verifica se o vírus já foi inicializado ou se já foi morto
	private bool firstTime, dead;
	
	void Awake () {
		//faz a inicialização
		FirstTime();
	}
	
	void FirstTime()
	{
		//guarda a sprite do vírus
		sprite = GetComponent<SpriteRenderer>();
		//começa parado
		moving = false;
		//pega o transform para movimento
		myTransform = transform;
		//guarda a escala original
		originalScale = myTransform.localScale;
		//define a escala grande que será um efeito ativado quando o vírus entrar na célula
		bigScale = originalScale*3f;
		//guarda a cor original do vírus
		originalColor = sprite.color;
		//a cor transparênte seria a original, mas sem o alfa
		fadeOutColor = originalColor; fadeOutColor.a = 0f;
		//busca o colisor que começa desativado
		myCollider = GetComponent<Collider2D>();
		myCollider.enabled = false;
		//define que ele pode ser instanciado
		canRespawn = true;
		//começa vivo
		dead = false;
		
		//marca que foi inicializado
		firstTime = true;
	}
	
	void Update()
	{
		//se não é pra se mover, sai
		if (!moving || GameController.instance.stop) return;
		
		//move o vírus para a direita
		myTransform.Translate(vecRight * speed * Time.deltaTime);
	}
	
	public void StartMoving(Vector3 position)
	{
		//garante que possui todas as referências necessárias
		if (!firstTime) FirstTime();
		
		//não pode ser instanciado já que já está sendo usado
		canRespawn = false;
		//a posição inicial passa a ser a passada por parâmetro
		initialPosition = position;
		//seta a posição inicial
		transform.position = position;
		//marca que está se movendo e não está morto
		moving = true;
		dead = false;
		//ativa o colisor para verificações de colisão
		myCollider.enabled = true;
	}
	
	public void StopMoving()
	{
		//simplismente para de se mover
		moving = false;
	}
	
	public void VirusEnteredTheCell()
	{
		//esse método é chamado pela célula invadida para informar que o vírus atual atingiu ela
		
		//se ta morto, sai fora
		if (dead || myCollider.enabled == false) return;
		
		//toca o som de erro quando a célula for atingida
		AudioManager.instance.PlayOneShotSound(AudioManager.instance.sound_wrong);
		
		//desativa os colisores e os booleanos necessários
		myCollider.enabled = false;
		canRespawn = false;
		moving = false;
		myCollider.enabled = false;
		//escalona o vírus para que fique grande
		myTransform.DOScale(bigScale, 1f);
		//interpola a cor do vírus para que ele vá desaparecendo
		sprite.DOColor(fadeOutColor, 2f).OnComplete(ResetVirus);
	}
	
	public void DestroyVirus(bool deadByLeucocito)
	{
		//não pode destruir se já foi destruido ou se ta na posição atual ou se o collider ta desabilitado
		if (dead || myTransform.position == initialPosition || myCollider.enabled == false) return;
		
		//se foi um leucocito que matou o vírus, toca o som do leucócito. Se for vacina que matou, toca nada
		if (deadByLeucocito) AudioManager.instance.LeucocitoSound();
		
		//toca o som de morte do vírus
		AudioManager.instance.DeadSound();
		//adiciona ponto ao controlador do modo protetor
		ProtectorModeController.instance.AddPoint();
		//define que ta morto, que não pode ser instanciado e que não está se movendo
		dead = true;
		canRespawn = false;
		moving = false;
		//desativa o colisor para não haver novas detecções
		myCollider.enabled = false;
		//faz o vírus dar uma "tremida"
		myTransform.DOShakePosition(1f, xyVector*2, 10, 90f);
		//escalona o vírus até que ele desapareça
		myTransform.DOScale(vecZero, 1.5f).OnComplete(ResetVirus);
	}
	
	void ResetVirus()
	{
		//esse método será usado quando acabar o vírus sumir da cena(seja por ser transparente ou por ser escalonado)
		
		//se ele já passou por aqui, sai fora
		if (canRespawn) return;
		
		//deixa os valores iniciais
		myCollider.enabled = false;
		dead = false;
		canRespawn = true;
		moving = false;
		//volta para a última posição inicial e a escala original
		myTransform.position = initialPosition;
		myTransform.localScale = originalScale;
		//volta com a cor original
		sprite.color = originalColor;
		
		//avisa ao modo protetor que o vírus foi resetado
		ProtectorModeController.instance.VirusFinished();
	}
}
