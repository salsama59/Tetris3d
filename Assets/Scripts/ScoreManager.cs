using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public Text scoreText;
    private int playerScore;
    public Text pointsText;
    public GameObject pointsTextGameObject;
    public GameObject scoreTextGameObject;

    // Use this for initialization
    void Start ()
    {
        this.playerScore = 0;
        this.ScoreText.text = "Score \n" + this.playerScore;
        this.PointsText.text = "";
    }
	
	// Update is called once per frame
	void Update ()
    {
        this.ScoreText.text = "Score \n" + this.playerScore;
    }

    private int GetLineDestroyPoints(int nbLine)
    {
        return nbLine * 100;
    }

    private int GetChainBonusPoints(int nbLine)
    {
        int points = 0;

        if(nbLine > 1)
        {
            int counter = 0;

            while(counter < nbLine)
            {
                points += 50;
                counter++;
            }
        }

        return points;
    }

    private int GetTotalEarnedPoint(int nbLine)
    {
        int total = 0;

        total += this.GetLineDestroyPoints(nbLine);
        total += this.GetChainBonusPoints(nbLine);

        return total;
    }

    public void AddPlayerPointAmountToScore(int nbLine)
    {
        this.playerScore += this.GetTotalEarnedPoint(nbLine);
    }

    public void DisplayEarnedPoints(int nbLine, int lineId)
    {
        //Pieces line destroyed position calculation
        Vector3 lineWorldPosition = new Vector3(10, 2, lineId + 0.5f);
        //Calculate the text position depending on the destroyed line position
        Vector3 textPosition = Camera.main.WorldToScreenPoint(lineWorldPosition);
        RectTransform textRectTransform = this.PointsText.GetComponent<RectTransform>();
        textRectTransform.position = textPosition;
        this.PointsText.gameObject.SetActive(true);
        this.PointsText.text = "+ " + this.GetTotalEarnedPoint(nbLine);
        
    }

    public Text ScoreText
    {
        get
        {
            return scoreText;
        }

        set
        {
            scoreText = value;
        }
    }

    public Text PointsText
    {
        get
        {
            return pointsText;
        }

        set
        {
            pointsText = value;
        }
    }

}
