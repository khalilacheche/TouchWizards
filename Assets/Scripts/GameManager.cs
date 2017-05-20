﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public GameObject PauseMenu;
    public Slider MusicSlider;
    public Slider SFXSlider;
    int p1taps=0;
    int p2taps = 0;
    int normalPlayerSpeed = 4;
    float lastUpdate;
    float tapDuelCheck;
	float timer = 0.0f;
    public Image timerCircle;
    public GameObject tappos;
	public GameObject SpeedBoostPrefab;
	public static  GameObject GameOver;
	public static  Text  P1_GameOverText;
	public static Text P2_GameOverText;
	public static GameObject GameOn;
    public int timeAmount = 10;
	public static  float timeLeft ;
	public static bool TimeUp;
    List<float> BoosterXPos = new List<float>();

    public float DuelDuration;

     List<string> GameObjectTags = new List<string>();

	void Start() {
        Time.timeScale = 1;
        p1taps = p2taps = 0;
		normalPlayerSpeed = 4;
		Time.timeScale=1;
		GameOn = GameObject.FindGameObjectWithTag("GameOn");
		GameOver = GameObject.FindGameObjectWithTag("GameOver");
        P1_GameOverText = GameObject.FindGameObjectWithTag("P1_GameOverText").GetComponent<Text>();
        P2_GameOverText = GameObject.FindGameObjectWithTag("P2_GameOverText").GetComponent<Text>();
        GameOver.SetActive(false);
		GameOn.SetActive(true);
		BoosterXPos.Add(-5f);
		BoosterXPos.Add(5f);
		InvokeRepeating("SpawnSpeedBoost",10,Random.Range(10,15));
		TimeUp = false;
        timeLeft = timeAmount;
        lastUpdate = Time.time;
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVol");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVol");
        GameObjectTags.AddRange (new string[]{"P1_Shield","P2_Shield","P1_Bullet","P2_Bullet","SpeedBooster"});
    }


	void Update(){
		if(timeLeft > 0) {
            timerCircle.fillAmount = timeLeft / timeAmount;
            tappos.SetActive(false);
            tapDuelCheck = Time.time;
			timeLeft -= Time.deltaTime;
		}else {
                DuelTime ();
                tappos.SetActive(true);
                timerCircle.fillAmount = (( (int) DuelDuration - (Time.time -tapDuelCheck))/DuelDuration);
                countTaps();
                TimeUp = true;
                GameOn.SetActive(false);
                float difference = p1taps - p2taps;
                
                if (Time.time-tapDuelCheck<DuelDuration) { 
                    if (Time.time-lastUpdate > 0.2f)
                    {
                        if (tappos != null)
                        {
                            tappos.transform.DOMove (new Vector3(tappos.transform.position.x,tappos.transform.position.y+(difference+Random.Range(-0.5f,0.5f))*0.5f,tappos.transform.position.z), 0.2f).SetEase(Ease.OutQuad);
                            lastUpdate = Time.time;

                        }
                    }
                }else {
                GameOver.SetActive(true);
                Time.timeScale = 0;
                if (tappos.transform.position.y > 0)
                    {
                        P1_GameOverText.text = "You Win!";
                        P2_GameOverText.text = "You Lose!";
                    }
                    else
                    {
                        P1_GameOverText.text = "You Lose!";
                        P2_GameOverText.text = "You Win!";
                    }

                }
        }
		if(P1_Controls.MoveSpeed!= P2_Controls.MoveSpeed) {
				timer+=Time.deltaTime;
		}
		if(timer >=10f) {
				P1_Controls.MoveSpeed = P2_Controls.MoveSpeed = normalPlayerSpeed;
				timer = 0;
			}
		
		
		}
    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
    private void countTaps()
    {
        for (var i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                if (Input.GetTouch(i).position.y > (Screen.height / 2) )
                {
                    p2taps++;
                }
                else
                {
                    p1taps++;
                }
            }
        }
    }
    public void changeMusicVolume()
    {
        PlayerPrefs.SetFloat("MusicVol", MusicSlider.value);
    }
    public void changeSFXVolume()
    {
        PlayerPrefs.SetFloat("SFXVol", SFXSlider.value);
    }
    public void ShowPause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void HidePauses()
    {
        Time.timeScale = 1;
        PauseMenu.SetActive(false);
    }

    public void RestartLevel () {
		Application.LoadLevel(Application.loadedLevelName);
    }
    public void GoHome()
    {
        Application.LoadLevel("Start");
    }

    void SpawnSpeedBoost () {
			GameObject SpeedBoostClone;
			SpeedBoostClone = Instantiate(SpeedBoostPrefab, new Vector3 (BoosterXPos[Random.Range(0,BoosterXPos.Count)],Random.Range(0.5f,-0.65f),0),Quaternion.identity) as GameObject;
		}
    void DuelTime () {
        for(int i = 0;i< GameObjectTags.Count;i++ ) {
            GameObject[] Objects =  GameObject.FindGameObjectsWithTag(GameObjectTags[i]);
            foreach (GameObject item in Objects) {
                Destroy(item);
            }
        }
    }
}
