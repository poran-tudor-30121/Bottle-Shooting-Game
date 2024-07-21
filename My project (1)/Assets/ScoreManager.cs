using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    private int score = 0;

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score.ToString();
    }
   public int getScore()
    {
        return score;
    }
    public void SwitchScoreVisibility()
    {
        scoreText.gameObject.SetActive(!scoreText.gameObject.activeSelf);
    }
    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }
}
