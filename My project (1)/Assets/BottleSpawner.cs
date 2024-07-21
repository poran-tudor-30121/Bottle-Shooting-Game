using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BottleSpawner : MonoBehaviour
{
    public GameObject bottlePrefab;
    public Transform spawnPoint;
    public float minDistance = 1f;
    public float maxDistance = 5f;
    private float spawnInterval = 2.5f;
    private float minSpawnInterval = 0.5f;
    private float spawnIntervalDecreaseRate = 0.05f;
    private bool gameStarted = false;
    private float bottleLifetime = 2f;
    private List<GameObject> activeBottles = new List<GameObject>();
    public TextMeshProUGUI elapsedTimeText;
    public TextMeshProUGUI spawnRate;
    public TextMeshProUGUI youLostText;
    public TextMeshProUGUI finalScoreText;

    public ScoreManager scoreManager;

    void Start()
    {
        SpawnFirstBottle();
    }

    void SpawnFirstBottle()
    {
        Vector3 spawnDirection = Random.insideUnitCircle.normalized;
        spawnDirection.y = 0f;
        float distance = Random.Range(minDistance, maxDistance);
        Vector3 spawnPosition = spawnPoint.position + spawnDirection * distance;
        GameObject newBottle = Instantiate(bottlePrefab, spawnPosition, Quaternion.identity);
    }

    void StartGame()
    {
        StartCoroutine(SpawnBottlesContinuously());
        gameStarted = true;
    }

    IEnumerator SpawnBottlesContinuously()
    {
        yield return new WaitForSeconds(spawnInterval);

        while (true)
        {
            Vector3 spawnDirection = Random.insideUnitCircle.normalized;
            spawnDirection.y = 0f;
            float distance = Random.Range(minDistance, maxDistance);
            Vector3 spawnPosition = spawnPoint.position + spawnDirection * distance;
            GameObject newBottle = Instantiate(bottlePrefab, spawnPosition, Quaternion.identity);
            activeBottles.Add(newBottle);
            StartCoroutine(CheckBottleLifetime(newBottle));
            spawnRate.text = "Spawn rate:" + spawnInterval;
            spawnInterval -= spawnIntervalDecreaseRate;
            spawnInterval = Mathf.Max(spawnInterval, minSpawnInterval);
            yield return new WaitForSeconds(spawnInterval);

        }
    }

    IEnumerator CheckBottleLifetime(GameObject bottle)
    {
        float elapsedTime = 0f;

        while (true)
        {
            if (!activeBottles.Contains(bottle))
            {
                print("S A OPRIT RUTINA");
                break;
            }

            elapsedTime += Time.deltaTime;

            if (elapsedTimeText != null)
            {
                elapsedTimeText.text = "Elapsed Time: " + elapsedTime.ToString("F1");
            }
           

            yield return null;

            if (bottle != null && activeBottles.Contains(bottle) && elapsedTime > bottleLifetime )
            {
                LoseGame();
                break;
            }
        }
    }

    public void OnFirstBottleHit(GameObject firstBottle)
    {
        if (!gameStarted)
        {
            StartGame();
        }
    }

    public void OnBottleDestroyed(GameObject bottle)
    {    
        if (activeBottles.Contains(bottle))
        {
            activeBottles.Remove(bottle);
            Debug.Log("Bottle removed from list. Active Bottles Count: " + activeBottles.Count);
        }
        else
        {
            Debug.LogError("Trying to remove a bottle that is not in the activeBottles list!");
        }
    }
    void LoseGame()
    {
        StopAllCoroutines();

        foreach (GameObject bottle in activeBottles)
        {
            if (bottle != null)
            {
                Destroy(bottle);
            }
        }
        spawnRate.gameObject.SetActive(false);
        elapsedTimeText.gameObject.SetActive(false);
        scoreManager.SwitchScoreVisibility();
        activeBottles.Clear();


        // Show "YOU LOST" and "PRESS R TO RESTART" text
        youLostText.gameObject.SetActive(true);
        finalScoreText.text = "FINAL SCORE: " + scoreManager.getScore();
        finalScoreText.gameObject.SetActive(true);

        // Wait for player input to restart the game
        StartCoroutine(WaitForRestart());
    }
    IEnumerator WaitForRestart()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                // Restart the game
                youLostText.gameObject.SetActive(false);
                finalScoreText.gameObject.SetActive(false);
                StartCoroutine(RestartGame());
                if (scoreManager != null)
                {
                    scoreManager.ResetScore();
                }
                yield break;
            }

            yield return null;
        }
    }


    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(2f);
        spawnInterval = 2f;
        spawnRate.gameObject.SetActive(true);
        elapsedTimeText.gameObject.SetActive(true);
        scoreManager.SwitchScoreVisibility();
        SpawnFirstBottle();
        gameStarted = false;
    }
}
