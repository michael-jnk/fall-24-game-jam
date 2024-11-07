using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamDirector : MonoBehaviour
{
    public float playackSpd = 1f;
    public bool isPlaying = false;
    private bool showMenu = false;
    private bool songDone = false;

    public Animator charAnimator;
    public CanvasGroup pauseMenu;

    public GameObject moveableCam;
    public AudioSource audioSource;

    public GameObject character;
    public GameObject beathitter;
    private float offscreenx = 44;
    private readonly float sidewalkSpeed = 5.5f;

    void Start()
    {
        isPlaying = true;
        showMenu = false;
        pauseMenu.alpha = 0f;
    }

    void Update()
    {
        if (songDone)
        {
            if (character.transform.position.x < offscreenx)
                character.transform.position = new Vector3(character.transform.position.x + (1-playackSpd) * sidewalkSpeed * Time.deltaTime,
                    character.transform.position.y, character.transform.position.z);
            if (beathitter.transform.position.x >-1* offscreenx)
                beathitter.transform.position = new Vector3(beathitter.transform.position.x + (1 - playackSpd) * sidewalkSpeed * Time.deltaTime*-1,
                    beathitter.transform.position.y, beathitter.transform.position.z);
        }

        if (!isPlaying)
        {
            if (playackSpd > 0)
            {
                playackSpd -= Time.deltaTime;
                if (playackSpd < 0.05f)
                    playackSpd = 0;
            }
        }
        else
        {
            if (playackSpd < 1.0f)
            {
                playackSpd += Time.deltaTime;
                if (playackSpd > 0.95f)
                    playackSpd = 1;
            }
        }

        if (!showMenu)
        {
            if (pauseMenu.alpha > 0)
            {
                pauseMenu.alpha -= Time.deltaTime;
                if (pauseMenu.alpha < 0.05f)
                    pauseMenu.alpha = 0;
            }
        }
        else
        {
            if (pauseMenu.alpha < 1.0f)
            {
                pauseMenu.alpha += Time.deltaTime;
                if (pauseMenu.alpha > 0.95f)
                    pauseMenu.alpha = 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !songDone)
        {
            isPlaying = !isPlaying;
            if (isPlaying) Play();
            else Pause();
        }
    }

    public void Pause(bool immediately = true)
    {
        if (immediately)
        {
            playackSpd = 0f;
            charAnimator.speed = 0f;
            audioSource.Pause();
        }
            showMenu = true;
        isPlaying = false;
        pauseMenu.interactable = true;
    }

    public void Play(bool immediately = true)
    {
        if (immediately)
        {
            playackSpd = 1f;
            charAnimator.speed = 1f;
            audioSource.Play();
        }
        isPlaying = true;
        showMenu = false;
        pauseMenu.interactable = false;
    }

    public void backToMenu()
    {
        Debug.Log("back to menu");
        // fade out the menu scene here, delete the camera there, transfer camera to game scene, start animation for camera movement??
        pauseMenu.interactable = false;
        pauseMenu.blocksRaycasts = false;
        showMenu = false;
        var loadingScene = SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
        moveableCam.GetComponent<Camera>().depth++;
        StartCoroutine(waitTilSceneLoaded(loadingScene));
    }

    IEnumerator waitTilSceneLoaded(AsyncOperation loadingScene)
    {
        while (!loadingScene.isDone)
            yield return null;


        Scene menuScene = SceneManager.GetSceneByName("MenuScene");
        //gameScene.GetRootGameObjects()[1].GetComponent<AudioPlayer>().setSongName(currentSongText.text); // this one's the audio player
        foreach (GameObject obj in menuScene.GetRootGameObjects())
            if (obj.name.Equals("Main Menu Camera"))
                Destroy(obj); // this one's the camera
        SceneManager.MoveGameObjectToScene(moveableCam, menuScene); // move cam over
        foreach (GameObject obj in menuScene.GetRootGameObjects())
            if (obj.name.Equals("Canvas"))
                obj.GetComponent<Canvas>().worldCamera = moveableCam.GetComponent<Camera>(); // set the main menu's cam
        moveableCam.GetComponent<Animator>().Play("Cam-gametomenu");
        foreach (GameObject obj in menuScene.GetRootGameObjects())
            if (obj.name.Equals("MenuHandler"))
                obj.GetComponent<MenuSceneController>().moveableCam = moveableCam; // give cam to the menu controller

        SceneManager.SetActiveScene(menuScene);
        StartCoroutine(waitToUnload(Time.time + 2.5f));
    }

    IEnumerator waitToUnload(float time)
    {
        while (Time.time < time)
            yield return null;
            
        SceneManager.UnloadSceneAsync(gameObject.scene);
    }

    public int numSongBeats;
    public int goodHits;
    public int okHits;
    public int badHits;
    public int misses;

    public Text endSongText;
    public Text endSongTextHL;

    public void EndGameScene()
    {
        //Debug.Log("end scene registerd");
        songDone = true;
        Pause(false);

        endSongText.text = $"Total Notes:\t\t{numSongBeats}\r\n\r\nGood Hits:\t\t\t{goodHits}\r\nOkay Hits: \t\t\t{okHits}\r\nBad Hits:\t\t\t\t{badHits}\r\nMissed Notes: \t{misses}";
        endSongTextHL.text = endSongText.text;
    }
}
