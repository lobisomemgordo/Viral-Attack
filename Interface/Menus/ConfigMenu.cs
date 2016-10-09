using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConfigMenu : MonoBehaviour {
	
	//guarda os scrollbars de som e música
	public Scrollbar[] scroolbars;
	
	void Start () {
		//inicia com o ultimo volume guardado
		scroolbars[0].value = PlayerPrefs.GetFloat("soundVolume", 0.5f);
		scroolbars[1].value = PlayerPrefs.GetFloat("musicVolume", 0.05f)*5f;
	}
	
	public void SetMusicVolume()
	{
		//é usado na scroolbar do volume de musica para setar o volume dela
		AudioManager.instance.SetMusicVolume();
	}
	
	public void SetSoundVolume()
	{
		//é usado na scroolbar do volume do som para setar o volume dele
		AudioManager.instance.SetSoundVolume();
	}
}
