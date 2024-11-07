using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSceneController : MonoBehaviour
{
    public GameObject moveableCam;
    private readonly float disappearSecs = 1f;
    private bool isDisappearing = false;
    private bool sceneLoaded = false;
    private CanvasGroup menuCanvas;

    public Text currentSongText;
    private int curSongIndex = -1;

    public GameObject playButton;
    public GameObject generateLevelButton;

    private string musicPath = Path.Combine(Application.streamingAssetsPath, "Music/");

    void Start()
    {
        //for (int i = SceneManager.sceneCount-1; i >= 0; i--)
        //    if (!SceneManager.GetSceneAt(i).name.Equals("MenuScene"))
        //        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
        SceneManager.SetActiveScene(gameObject.scene);
        if (SceneManager.GetSceneByName("BackgroundScene").name == null)
            SceneManager.LoadScene(2, LoadSceneMode.Additive);
        NextSongName();
    }

    private void Update()
    {
        if (isDisappearing)
        {
            menuCanvas.alpha -= disappearSecs * Time.deltaTime;
            if (menuCanvas.alpha <= 0 && sceneLoaded)
            {
                SceneManager.UnloadSceneAsync(this.gameObject.scene);
            }
                
        }
    }

    public void LoadGameScene()
    {
        // fade out the menu scene here, delete the camera there, transfer camera to game scene, start animation for camera movement??
        foreach (GameObject obj in gameObject.scene.GetRootGameObjects())
            if (obj.name.Equals("Canvas"))
                menuCanvas = obj.transform.GetChild(0).GetComponent<CanvasGroup>();
        menuCanvas.interactable = false;
        menuCanvas.blocksRaycasts = false;
        isDisappearing = true;
        var loadingScene = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        //moveableCam.GetComponent<Camera>().depth++;
        StartCoroutine(waitTilSceneLoaded(loadingScene));
    }

    private IEnumerator waitTilSceneLoaded(AsyncOperation loadingScene)
    {
        while (!loadingScene.isDone)
        {
            yield return null;
        }

        Scene gameScene = SceneManager.GetSceneByName("GameScene");



        foreach (GameObject obj in gameScene.GetRootGameObjects())
            if (obj.name.Equals("AudioController"))
                obj.GetComponent<AudioPlayer>().setSongName(currentSongText.text); // this one's the audio player
        foreach (GameObject obj in gameScene.GetRootGameObjects())
            if (obj.name.Equals("Game Camera"))
                Destroy(obj); // this one's the camera
        SceneManager.MoveGameObjectToScene(moveableCam, gameScene); // move cam over
        foreach (GameObject obj in gameScene.GetRootGameObjects())
            if (obj.name.Equals("Canvas"))
                obj.GetComponent<Canvas>().worldCamera = moveableCam.GetComponent<Camera>(); // set the pause menu's cam
        moveableCam.GetComponent<Animator>().Play("Cam-menutogame");
        foreach (GameObject obj in gameScene.GetRootGameObjects())
            if (obj.name.Equals("GameDirector"))
                obj.GetComponent<GamDirector>().moveableCam = moveableCam; // give cam to pause menu controller

        SceneManager.SetActiveScene(gameScene);
        sceneLoaded = true;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenMusicFolder()
    {
        Application.OpenURL(musicPath);
    }

    public void GenerateBeatTrack()
    {
        Debug.Log("started saving beattrack");
        BeatMapper.genAndSaveTrack(musicPath, currentSongText.text);
        Debug.Log("finished saving beattrack");
        generateLevelButton.SetActive(false);
        playButton.SetActive(true);
    }

    public void NextSongName()
    {
        string[] musicDirFiles = Directory.GetFiles(musicPath);
        int numSongs = 0;
        for (int i = 0; i < musicDirFiles.Length; i++)
        {
            musicDirFiles[i] = musicDirFiles[i].Substring(musicDirFiles[i].LastIndexOf('/') + 1);
            if (musicDirFiles[i].EndsWith(".mp3")) numSongs++;
            //Debug.Log($"filename: {filename}");
        }
            
        if (numSongs == 0)
        {
            playButton.SetActive(false);
            generateLevelButton.SetActive(false);
            currentSongText.text = "no mp3 files?!";
            return;
        }

        curSongIndex = (curSongIndex+1) % numSongs; 
        int tempIndex = 0;
        foreach (string filename in musicDirFiles)
            if (filename.EndsWith(".mp3"))
            {
                //Debug.Log($"temp index: {tempIndex}, cursongindex: {curSongIndex}, filename: {filename}");
                if (tempIndex == curSongIndex)
                {
                    currentSongText.text = filename.Substring(0, filename.Length - 4);
                    break;
                }
                else
                    tempIndex++;
            }
        foreach (string filename in musicDirFiles) {
            //Debug.Log($"text 1: {currentSongText.text.Substring(0, currentSongText.text.Length - 4)}, text 2: {filename.Substring(0, filename.Length - 10)}");
            if (currentSongText.text.Equals(filename.Substring(0, filename.Length - 10)))
            {
                playButton.SetActive(true);
                generateLevelButton.SetActive(false);
                return;
            }
        }
        playButton.SetActive(false);
        generateLevelButton.SetActive(true);
    }
}
