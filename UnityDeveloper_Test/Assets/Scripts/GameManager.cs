using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text timerText;
    [SerializeField] private float timerInSeconds = 121f;
    [SerializeField] private Transform endGameCanvas;
    [SerializeField] private Text messageText;
    private bool startTimer;
    [SerializeField] private Transform pointCubesParent;
    [SerializeField] private PlayerController playerController;
    void Start()
    {
        startTimer = true;
    }

    void Update()
    {
        if (startTimer)
        {
            timerInSeconds -= Time.deltaTime;
            DisplayTimeLeft(timerInSeconds);
            if(timerInSeconds <= 0)
            {
                timerInSeconds = 0;
                startTimer = false;
                DisplayTimeLeft(timerInSeconds);
                DisplayEndCanvas("Game Over!!!");
            }
            else
            {
                CheckGameOver();
            }
        }
    }

    private void DisplayTimeLeft(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void DisplayEndCanvas(string message)
    {
        if(startTimer)
        {
            startTimer = false;
        }
        endGameCanvas.gameObject.SetActive(true);
        messageText.text = message;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void CheckGameOver()
    {
        bool gameOver = true;
        //Checking cubes collected
        foreach(Transform pointCube in pointCubesParent)
        {
            if(pointCube.gameObject.activeInHierarchy)
            {
                gameOver = false;
                break;
            }
        }
        if (gameOver)
        {
            DisplayEndCanvas("Game Complete!!!");
        }
        if (!gameOver)
        {
            //Checking if player is falling continuously
            gameOver = playerController.CheckGameOver();
            if (gameOver)
            {
                DisplayEndCanvas("Game Over!!!");
            }
        }
        
    }
}
