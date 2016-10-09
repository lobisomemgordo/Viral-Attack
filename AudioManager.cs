using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {
	
	//instancia que será usada pelas outras classes para facilitar acesso
	public static AudioManager instance;
	
	[Header("MUSICAS")]
	//guarda as musicas do jogo
	public AudioClip music_MainTheme;
	public AudioClip music_gameOver;
	public AudioClip music_ViralMode;
	public AudioClip music_ProtectorMode;
	public AudioClip music_LifeCycle;
	
	[Space]
	[Header("EFEITOS SONOROS")]
	//guarda os efeitos sonoros do jogo
	public AudioClip sound_click;
	public AudioClip sound_right;
	public AudioClip sound_wrong;
	public AudioClip sound_victory;
	public AudioClip sound_escape;
	public AudioClip[] sound_dead;
	public AudioClip[] sound_leucocito;
	public AudioClip[] sound_virus;
	
	private AudioSource[] audioSource;
	//guarda os volumes iniciais e os atuais
	private float[] startVolume = new float[2], currentVolume = new float[2];
	//referência do menu de configuração para acessar o valor das scroolbars de volume e som
	private ConfigMenu configMenu;
	
	void Awake () {
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}
		
		//define a instancia caso ainda não tenha
		instance = this;
		//marca o audio manager como um object que não é destruido, ele irá se manter em todas as cenas
		DontDestroyOnLoad(gameObject);
	}
	
	void Start()
	{
		//busca os audios sources. 0 seria o de efeito sonoro e 1 seria o da música
		audioSource = GetComponents<AudioSource>();
		
		//define os volumes iniciais
		startVolume[0] = audioSource[0].volume;
		startVolume[1] = audioSource[1].volume;
		
		//define os volumes atuais de acordo com os dados salvos
		SetSoundVolume(PlayerPrefs.GetFloat("soundVolume", 0.5f));
		SetMusicVolume(PlayerPrefs.GetFloat("musicVolume", 0.05f));
		
		//busca a referência do menu de configuração para acessar os scroolbars
		configMenu = FindObjectOfType<ConfigMenu>();
		
		//toca a musica do menu principal
		PlayMusic(music_MainTheme);
	}
	
	public void DeadSound()
	{
		if (GameController.instance.isProtectorMode)
			//no modo protetor, para o audio não atrapalhar, foi deixado apenas os sons curtos
			PlayOneShotSound(sound_dead[Random.Range(0,2)]);//varia entre os 2 primeiros sons
		//se for modo viral vai todos os sons
		else PlayOneShotSound(sound_dead[Random.Range(0, sound_dead.Length)]);
	}
	
	public void LeucocitoSound()
	{
		//toca o som dos leucócitos
		PlayOneShotSound(sound_leucocito[Random.Range(0, sound_leucocito.Length)]);
	}
	
	public void VirusSound()
	{
		//toca o som dos vírus
		PlayOneShotSound(sound_virus[Random.Range(0, sound_virus.Length)]);
	}
	
	public void PlayOneShotSound(AudioClip soundClip)
	{
		//toca um som som que será enviado por parâmetro
		audioSource[0].PlayOneShot(soundClip);
	}
	
	public void PlayOneShotSound(AudioClip soundClip, float volumeClip)
	{
		//toca um som som que será enviado por parâmetro com opção de volume
		audioSource[0].PlayOneShot(soundClip, volumeClip);
	}
	
	public void PlaySound(AudioClip soundClip)
	{
		//irá tirar o som atual e irá tocar o som enviado por parâmetro
		audioSource[0].Stop();
		audioSource[0].clip = soundClip;
		audioSource[0].Play();
	}
	
	public void PlaySound(AudioClip soundClip, float volumeClip)
	{
		//irá tirar o som atual e irá tocar o som enviado por parâmetro com opção de volume
		audioSource[0].Stop();
		audioSource[0].clip = soundClip;
		audioSource[0].volume = volumeClip;
		audioSource[0].Play();
	}
	
	public void PlayMusic(AudioClip musicClip)
	{
		//só vai parar a música e tocar se for uma música diferente
		if (musicClip == audioSource[1].clip) return;
		
		//para a musica e toca a musica enviada por parâmetro
		audioSource[1].Stop();
		audioSource[1].clip = musicClip;
		audioSource[1].Play();
	}
	
	public void PlayPauseSound()
	{
		//seria uma opção de checkBox, iria pausar se estivesse tocando e despausaria se estivesse pausado
		if (audioSource[0].volume == currentVolume[0]) audioSource[0].volume = 0f;
		else audioSource[0].volume = currentVolume[0];
	}
	
	public void PlayPauseMusic()
	{
		//seria uma opção de checkBox, iria pausar se estivesse tocando e despausaria se estivesse pausado
		if (audioSource[1].isPlaying) audioSource[1].Pause();
		else audioSource[1].UnPause();
	}
	
	public void SetSoundVolume()
	{
		//esse método seria utilizado pelo menu de configuração(que seria acionado quando houvesse alteração no scroolbar)
		
		//se for nulo procura novamente(previne erros de null reference)
		if (configMenu == null) configMenu = FindObjectOfType<ConfigMenu>();
		
		//guarda o volume de acordo com a scroolbar do som
		float volume = configMenu.scroolbars[0].value;
		
		//atualiza o volume do audioSource da da variavel atual
		audioSource[0].volume = volume;
		currentVolume[0] = volume;
		
		//salva o novo volume do som
		PlayerPrefs.SetFloat("soundVolume", volume);
	}
	
	public void SetMusicVolume()
	{
		//esse método seria utilizado pelo menu de configuração(que seria acionado quando houvesse alteração no scroolbar)
		
		//se for nulo procura novamente
		if (configMenu == null) configMenu = FindObjectOfType<ConfigMenu>();
		
		//guarda o volume de acordo com a scroolbar da música
		//o audio é muito alto então foi dividido por 5f
		float volume = configMenu.scroolbars[1].value /5f;
		
		//atualiza o volume do audioSource da da variavel atual
		audioSource[1].volume = volume;
		currentVolume[1] = volume;
		
		//salva o novo volume da música
		PlayerPrefs.SetFloat("musicVolume", volume);
	}
	
	public void SetSoundVolume(float volume)
	{
		//esse método seria usado na inicialização(Start)
		
		//atualiza o volume do audioSource da da variavel atual
		audioSource[0].volume = volume;
		currentVolume[0] = volume;
		
		//salva o novo volume do som
		PlayerPrefs.SetFloat("soundVolume", volume);
	}
	
	public void SetMusicVolume(float volume)
	{
		//esse método seria usado na inicialização(Start)
		
		//atualiza o volume do audioSource da da variavel atual
		audioSource[1].volume = volume;
		currentVolume[1] = volume;
		
		//salva o novo volume da música
		PlayerPrefs.SetFloat("musicVolume", volume);
	}
	
	public void ResetVolumes()
	{
		//resetaria os volumes de som e música
		for (int c=0; c<2; c++)
		{
			audioSource[c].volume = startVolume[c];
		}
	}
}
