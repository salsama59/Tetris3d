using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public GameObject pointTextGameObject;
    public GameObject scoreTextGameObject;
    private Dictionary<int, int> playersScrore = new Dictionary<int, int>();
    private Dictionary<int, Text> playersScoreText = new Dictionary<int, Text>();
    private Dictionary<int, Text> playersPointText = new Dictionary<int, Text>();

    // Use this for initialization
    void Start ()
    {
        for (int playerId = 0; playerId < ApplicationUtils.playerNumber; playerId++)
        {
            this.SetUpScoreElements(playerId);
        }
    }

    private void SetUpScoreElements(int playerId)
    {
        if(!ApplicationUtils.IsInMultiPlayerMode())
        {
            this.SetUpScoreElementsForOnePlayerMode(playerId);
        }
        else
        {
            this.SetUpScoreElementsForTwoPlayerMode(playerId);
        }
        this.PlayersScoreText[playerId].gameObject.SetActive(true);
    }

    private void SetUpScoreElementsForTwoPlayerMode(int playerId)
    {
        float scoreXposition = 0f;
        float pointsXposition = 0f;

        String scoreTextTagName = null;
        String pointTextTagName = null;
        String fieldTagName = null;

        if (playerId == (int)PlayerEnum.PlayerId.PLAYER_1)
        {
            scoreTextTagName = TagConstants.TAG_NAME_PLAYER_1_SCORE_TEXT;
            pointTextTagName = TagConstants.TAG_NAME_PLAYER_1_POINTS_TEXT;
            fieldTagName = TagConstants.TAG_NAME_PLAYER_1_FIELD;
        }
        else if (playerId == (int)PlayerEnum.PlayerId.PLAYER_2)
        {
            scoreTextTagName = TagConstants.TAG_NAME_PLAYER_2_SCORE_TEXT;
            pointTextTagName = TagConstants.TAG_NAME_PLAYER_2_POINTS_TEXT;
            fieldTagName = TagConstants.TAG_NAME_PLAYER_2_FIELD;
        }

        Vector3 fieldsize = ElementType.CalculateGameObjectMaxRange(GameObject.FindGameObjectWithTag(fieldTagName).transform.GetChild(0).gameObject);

        Vector3 foreseeWindowSize = ElementType.CalculateGameObjectMaxRange(GameObject.FindGameObjectWithTag(TagConstants.TAG_NAME_FORESEE_WINDOW).transform.GetChild(0).gameObject);

        if (playerId == (int)PlayerEnum.PlayerId.PLAYER_1)
        {
            scoreXposition = (foreseeWindowSize.x / 2 + fieldsize.x + GameFieldUtils.FIELD_MARGIN) * -1;
            pointsXposition = (GameFieldUtils.FIELD_MARGIN + fieldsize.x / 2) * -1;
        }
        else if (playerId == (int)PlayerEnum.PlayerId.PLAYER_2)
        {
            scoreXposition = foreseeWindowSize.x / 2 + GameFieldUtils.FIELD_MARGIN;
            pointsXposition = GameFieldUtils.FIELD_MARGIN + foreseeWindowSize.x + fieldsize.x / 2;
        }

        Vector3 scorePosition = new Vector3(
                scoreXposition
            , 0.5f
            , fieldsize.z / 2
            );

        Vector3 pointsPosition = new Vector3(
              pointsXposition
            , 0.5f
            , Vector3.zero.z
            );

        this.InstantiateScoreElementsOnField(playerId, scoreTextTagName, pointTextTagName, scorePosition, pointsPosition);
    }

    private void SetUpScoreElementsForOnePlayerMode(int playerId)
    {
        float scoreXposition = 0f;
        float pointsXposition = 0f;

        String scoreTextTagName = null;
        String pointTextTagName = null;
        String fieldTagName = null;

        scoreTextTagName = TagConstants.TAG_NAME_PLAYER_1_SCORE_TEXT;
        pointTextTagName = TagConstants.TAG_NAME_PLAYER_1_POINTS_TEXT;
        fieldTagName = TagConstants.TAG_NAME_PLAYER_1_FIELD;

        Vector3 fieldsize = ElementType.CalculateGameObjectMaxRange(GameObject.FindGameObjectWithTag(fieldTagName).transform.GetChild(0).gameObject);

        Vector3 foreseeWindowSize = ElementType.CalculateGameObjectMaxRange(GameObject.FindGameObjectWithTag(TagConstants.TAG_NAME_FORESEE_WINDOW).transform.GetChild(0).gameObject);

       
        scoreXposition = (foreseeWindowSize.x / 2) * -1;
        pointsXposition = GameObject.FindGameObjectWithTag(fieldTagName).transform.position.x + (fieldsize.x / 2);
       

        Vector3 scorePosition = new Vector3(
                scoreXposition
            , 0.5f
            , fieldsize.z / 2
            );

        Vector3 pointsPosition = new Vector3(
              pointsXposition
            , 0.5f
            , Vector3.zero.z
            );
        this.InstantiateScoreElementsOnField(playerId, scoreTextTagName, pointTextTagName, scorePosition, pointsPosition);
    }

    private void InstantiateScoreElementsOnField(int playerId, string scoreTextTagName, string pointTextTagName, Vector3 scorePosition, Vector3 pointsPosition)
    {
        //Calculate the text position
        Vector3 scoreTextPosition = Camera.main.WorldToScreenPoint(scorePosition);
        Vector3 pointsTextPosition = Camera.main.WorldToScreenPoint(pointsPosition);

        GameObject instantiatedPointText = Instantiate(pointTextGameObject);
        GameObject instantiatedScoreText = Instantiate(scoreTextGameObject, Vector3.zero, Quaternion.identity);

        GameObject instantiatedPointTextGameObjectChild = instantiatedPointText.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        GameObject instantiatedScoreTextGameObjectChild = instantiatedScoreText.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;

        instantiatedPointTextGameObjectChild.tag = pointTextTagName;
        instantiatedScoreTextGameObjectChild.tag = scoreTextTagName;

        Text pointText = instantiatedPointTextGameObjectChild.GetComponent<Text>();
        Text scoreText = instantiatedScoreTextGameObjectChild.GetComponent<Text>();

        this.PlayersScoreText.Add(playerId, scoreText);
        this.PlayersPointText.Add(playerId, pointText);
        this.playersScrore.Add(playerId, 0);

        this.PlayersScoreText[playerId].text = "Score \n" + this.playersScrore[playerId];
        this.PlayersPointText[playerId].text = "";

        RectTransform scoreTextRectTransform = this.PlayersScoreText[playerId].GetComponent<RectTransform>();
        scoreTextRectTransform.position = scoreTextPosition;

        RectTransform pointsTextRectTransform = this.PlayersPointText[playerId].GetComponent<RectTransform>();
        pointsTextRectTransform.position = pointsTextPosition;

        int scoreTextfontSize = 0;
        int pointTextfontSize = 0;

        if (!ApplicationUtils.IsInMultiPlayerMode())
        {
            scoreTextfontSize = 30;
            pointTextfontSize = 23;
        }
        else
        {
            scoreTextfontSize = 24;
            pointTextfontSize = 22;
        }

        this.PlayersScoreText[playerId].fontSize = scoreTextfontSize;
        this.PlayersPointText[playerId].fontSize = pointTextfontSize;

    }

    // Update is called once per frame
    void Update ()
    {
        for (int playerId = 0; playerId < ApplicationUtils.playerNumber; playerId++)
        {
            this.PlayersScoreText[playerId].text = "Score \n" + this.playersScrore[playerId];
        }
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
            points = 50 * nbLine;
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

    public void AddPlayerPointAmountToScore(int nbLine, int playerId)
    {
        this.playersScrore[playerId] += this.GetTotalEarnedPoint(nbLine);
    }

    public void DisplayEarnedPoints(int nbLine, int lineId, int playerId)
    {
        float pointsXposition = 0f;
        String fieldTagName = null;

        if (playerId == (int)PlayerEnum.PlayerId.PLAYER_1)
        {
            fieldTagName = TagConstants.TAG_NAME_PLAYER_1_FIELD;
        }
        else if (playerId == (int)PlayerEnum.PlayerId.PLAYER_2)
        {
            fieldTagName = TagConstants.TAG_NAME_PLAYER_2_FIELD;
        }

        Vector3 fieldsize = ElementType.CalculateGameObjectMaxRange(GameObject.FindGameObjectWithTag(fieldTagName).transform.GetChild(0).gameObject);

        Vector3 foreseeWindowSize = ElementType.CalculateGameObjectMaxRange(GameObject.FindGameObjectWithTag(TagConstants.TAG_NAME_FORESEE_WINDOW).transform.GetChild(0).gameObject);

        if (playerId == (int)PlayerEnum.PlayerId.PLAYER_1)
        {
            pointsXposition = (GameFieldUtils.FIELD_MARGIN + fieldsize.x / 2) * -1;
        }
        else if (playerId == (int)PlayerEnum.PlayerId.PLAYER_2)
        {
            pointsXposition = GameFieldUtils.FIELD_MARGIN + foreseeWindowSize.x + fieldsize.x / 2;
        }

        //Pieces line destroyed position calculation
        Vector3 lineWorldPosition = new Vector3(pointsXposition, 0.5f, lineId + 0.5f);
        //Calculate the text position depending on the destroyed line position
        Vector3 textPosition = Camera.main.WorldToScreenPoint(lineWorldPosition);
        RectTransform textRectTransform = this.PlayersPointText[playerId].GetComponent<RectTransform>();
        textRectTransform.position = textPosition;
        this.PlayersPointText[playerId].gameObject.SetActive(true);
        this.PlayersPointText[playerId].text = "+ " + this.GetTotalEarnedPoint(nbLine);
        
    }

    public Dictionary<int, Text> PlayersPointText
    {
        get
        {
            return playersPointText;
        }

        set
        {
            playersPointText = value;
        }
    }

    public Dictionary<int, Text> PlayersScoreText
    {
        get
        {
            return playersScoreText;
        }

        set
        {
            playersScoreText = value;
        }
    }

}
