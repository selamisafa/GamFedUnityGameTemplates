using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmiralBattiGameManager : MonoBehaviour
{
    public static AmiralBattiGameManager instance;

    private void Awake()
    {
        instance = this;
    }

    public int[,] table = new int[5, 5];

    public List<int> ships;
    public List<Color> shipsColor;

    List<int> shipsBackup = new List<int>();
    List<Color> shipsColorBackup = new List<Color>();

    List<int> foundShipPartCount = new List<int>();

    public Transform seaParent;
    // Start is called before the first frame update

    public Text questionText, answer1, answer2, answer3;
    public GameObject questionPanel;

    public TileScript chosenTile;

    public int totalShipParts = 0, hitShipParts = 0;

    public GameObject endGamePanel;
    public GameObject infoPanel;

    public GameObject crossPrefab;

    public bool canChooseBlock = false;

    float timer = 0;
    public Text endGameTimer;
    public Text endGameScore;
    int wrongAnswerCount;
    int wrongGuessCount;

    public AudioClip correctSound, wrongSound, doneSound;

    void Start()
    {
        questionPanel.SetActive(false);
        endGamePanel.SetActive(false);
        infoPanel.SetActive(true);

        shipsBackup.AddRange(ships);
        foundShipPartCount.AddRange(ships);
        shipsColorBackup.AddRange(shipsColor);

        foundShipPartCount.Sort();
        foundShipPartCount.Reverse();

        ships.Clear();
        shipsColor.Clear();

        foreach (var item in shipsBackup)
        {
            totalShipParts += item;
        }

        StartCoroutine(CreateNewAssembly());
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }

    IEnumerator CreateNewAssembly()
    {
        #region Spawner

        ships.AddRange(shipsBackup);
        shipsColor.AddRange(shipsColorBackup);

        ships.Sort();
        ships.Reverse();

        while (ships.Count > 0)
        {
            int row = Random.Range(0, table.GetLength(0));
            int column = Random.Range(0, table.GetLength(1));
            bool canPlace = false;
            bool isTurned = false;

            if (!seaParent.GetChild(row).GetChild(column).GetComponent<TileScript>().haveShipPart) //if we are be able to put a ship
            {
                if (Random.Range(0, 2) > 0) //if we should put a ship
                {
                    //Debug coloring
                    //seaParent.GetChild(row).GetChild(column).GetComponent<UnityEngine.UI.Image>().color = Color.black;

                    if (ships[0] + column <= table.GetLength(0))
                    {
                        isTurned = false;

                        for (int length = 0; length < ships[0]; length++)
                        {
                            if (seaParent.GetChild(row).GetChild(column + length).GetComponent<TileScript>().haveShipPart)
                            {
                                canPlace = false;
                                break;
                            }
                            else
                            {
                                canPlace = true;
                            }
                        }
                    }
                    else if (ships[0] + row <= table.GetLength(1))
                    {
                        isTurned = true;

                        for (int length = 0; length < ships[0]; length++)
                        {
                            if (seaParent.GetChild(row + length).GetChild(column).GetComponent<TileScript>().haveShipPart)
                            {
                                canPlace = false;
                                break;
                            }
                            else
                            {
                                canPlace = true;
                            }
                        }
                    }

                    if (canPlace)
                    {
                        for (int i = 0; i < ships[0]; i++)
                        {
                            if (isTurned)
                            {
                                //Debug coloring
                                //seaParent.GetChild(row + i).GetChild(column).GetComponent<UnityEngine.UI.Image>().color = shipsColor[0];
                                seaParent.GetChild(row + i).GetChild(column).GetComponent<TileScript>().color = shipsColor[0];
                                seaParent.GetChild(row + i).GetChild(column).GetComponent<TileScript>().haveShipPart = true;
                                seaParent.GetChild(row + i).GetChild(column).GetComponent<TileScript>().length = ships[0];
                                seaParent.GetChild(row + i).GetChild(column).GetComponent<TileScript>().isTurned = isTurned;
                            }
                            else
                            {
                                //Debug coloring
                                //seaParent.GetChild(row).GetChild(column + i).GetComponent<UnityEngine.UI.Image>().color = shipsColor[0];
                                seaParent.GetChild(row).GetChild(column + i).GetComponent<TileScript>().color = shipsColor[0];
                                seaParent.GetChild(row).GetChild(column + i).GetComponent<TileScript>().haveShipPart = true;
                                seaParent.GetChild(row).GetChild(column + i).GetComponent<TileScript>().length = ships[0];
                                seaParent.GetChild(row).GetChild(column + i).GetComponent<TileScript>().isTurned = isTurned;
                            }
                        }

                        Debug.Log("Row: " + row + " - Column: " + column + " - Ships Length: " + ships[0] + " - Is Rotated: " + isTurned);

                        shipsColor.RemoveAt(0);
                        ships.RemoveAt(0);
                    }
                }
            }
        }
        #endregion

        AskQuestionReverse();
        yield return new WaitForSeconds(1);

        /*
        while (true)
        {

            yield return new WaitForSeconds(60);

            foreach (Transform item in seaParent)
            {
                foreach (Transform childs in item)
                {
                    childs.GetComponent<TileScript>().isTurned = false;
                    childs.GetComponent<TileScript>().length = 0;
                    childs.GetComponent<TileScript>().haveShipPart = false;
                    childs.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                }
            }
        }
        */
    }

    public void AskQuestion(TileScript tileScript)
    {
        if (chosenTile != null)
        {
            return;
        }
        else
        {
            chosenTile = tileScript; 
        }

        questionPanel.SetActive(true);
        infoPanel.SetActive(false);

        int firstNumber = Random.Range(1,7);
        int secondNumber = Random.Range(1,7);

        questionText.text = firstNumber + " x " + secondNumber + " = ?";

        List<Text> answers = new List<Text>();

        answers.Add(answer1);
        answers.Add(answer2);
        answers.Add(answer3);

        int rngCorrect = Random.Range(0, 3);

        foreach (var item in answers)
        {
            item.GetComponent<AnswerButtonScript>().isCorrect = false;
        }

        Debug.Log(firstNumber * secondNumber);
        answers[rngCorrect].text = (firstNumber * secondNumber).ToString();
        answers[rngCorrect].GetComponent<AnswerButtonScript>().isCorrect = true;

        answers.RemoveAt(rngCorrect);

        for (int i = 0; i < answers.Count; i++)
        {
            answers[i].text = ((firstNumber + i + 1) * secondNumber).ToString();
        }

        answer1.transform.parent.GetComponent<Button>().interactable = true;
        answer2.transform.parent.GetComponent<Button>().interactable = true;
        answer3.transform.parent.GetComponent<Button>().interactable = true;
    }

    public void AnswerGiven(AnswerButtonScript answerButtonScript)
    {
        Debug.Log(answerButtonScript.isCorrect);

        if (totalShipParts != hitShipParts)
        {
            if (answerButtonScript.isCorrect)
            {
                AudioSource.PlayClipAtPoint(correctSound, Vector3.zero, 0.5f);

                if (chosenTile.haveShipPart)
                {
                    chosenTile.GetComponent<Image>().color = chosenTile.color;

                    ColorBlock chosenBlockColor = ColorBlock.defaultColorBlock;
                    chosenBlockColor.disabledColor = Color.white;

                    chosenTile.GetComponent<Button>().colors = chosenBlockColor;

                    switch (chosenTile.length)
                    {
                        case 1:
                            foundShipPartCount[3] -= 1;

                            if (foundShipPartCount[3] == 0)
                            {
                                infoPanel.transform.GetChild(4).gameObject.SetActive(false);
                            }
                            break;
                        case 2:
                            foundShipPartCount[2] -= 1;

                            if (foundShipPartCount[2] == 0)
                            {
                                infoPanel.transform.GetChild(3).gameObject.SetActive(false);
                            }
                            break;
                        case 3:
                            foundShipPartCount[1] -= 1;

                            if (foundShipPartCount[1] == 0)
                            {
                                infoPanel.transform.GetChild(2).gameObject.SetActive(false);
                            }
                            break;
                        case 4:
                            foundShipPartCount[0] -= 1;

                            if (foundShipPartCount[0] == 0)
                            {
                                infoPanel.transform.GetChild(1).gameObject.SetActive(false);
                            }
                            break;

                        default:
                            break;
                    }

                    hitShipParts++;

                    if (totalShipParts == hitShipParts)
                    {
                        endGamePanel.SetActive(true);
                        infoPanel.SetActive(false);
                    }
                }
                else
                {
                    Instantiate(crossPrefab, chosenTile.transform);
                    chosenTile.GetComponent<Image>().color = Color.black;
                }

                chosenTile.GetComponent<Button>().interactable = false;
                questionPanel.SetActive(false);
                infoPanel.SetActive(true);

            }
            else
            {
                questionPanel.SetActive(false);
                infoPanel.SetActive(true);
            }
        }

        chosenTile = null;
    }

    public void AskQuestionReverse()
    {
        foreach (Transform item in seaParent)
        {
            foreach (Transform tiles in item)
            {
                if (tiles.childCount < 1 && tiles.GetComponent<Button>() != null)
                {
                    tiles.GetComponent<Button>().interactable = false;
                }
            }
        }

        questionPanel.SetActive(true);
        infoPanel.SetActive(false);

        int firstNumber = Random.Range(1, 7);
        int secondNumber = Random.Range(1, 7);

        questionText.text = firstNumber + " x " + secondNumber + " = ?";

        List<Text> answers = new List<Text>();

        answers.Add(answer1);
        answers.Add(answer2);
        answers.Add(answer3);

        int rngCorrect = Random.Range(0, 3);

        foreach (var item in answers)
        {
            item.GetComponent<AnswerButtonScript>().isCorrect = false;
        }

        Debug.Log(firstNumber * secondNumber);
        answers[rngCorrect].text = (firstNumber * secondNumber).ToString();
        answers[rngCorrect].GetComponent<AnswerButtonScript>().isCorrect = true;

        answers.RemoveAt(rngCorrect);

        for (int i = 0; i < answers.Count; i++)
        {
            answers[i].text = ((firstNumber + i + 1) * secondNumber).ToString();
        }

        answer1.transform.parent.GetComponent<Button>().interactable = true;
        answer2.transform.parent.GetComponent<Button>().interactable = true;
        answer3.transform.parent.GetComponent<Button>().interactable = true;
    }

    public void AnswerGivenReverse(AnswerButtonScript answerButtonScript)
    {
        Debug.Log(answerButtonScript.isCorrect);

        if (answerButtonScript.isCorrect)
        {
            AudioSource.PlayClipAtPoint(correctSound, Vector3.zero, 0.5f);

            canChooseBlock = true;

            questionPanel.SetActive(false);
            infoPanel.SetActive(true);

            foreach (Transform item in seaParent)
            {
                foreach (Transform tiles in item)
                {
                    if (tiles.childCount < 1 && tiles.GetComponent<Button>()!=null)
                    {
                        tiles.GetComponent<Button>().interactable = true;
                    }
                }
            }
        }
        else
        {
            AudioSource.PlayClipAtPoint(wrongSound, Vector3.zero, 0.5f);

            wrongAnswerCount++;
            AskQuestionReverse();
        }
    }

    public void RevealBlock(TileScript tileScript)
    {
        if (!canChooseBlock)
        {
            return;
        }

        chosenTile = tileScript;

        if (totalShipParts != hitShipParts)
        {
            if (chosenTile.haveShipPart)
            {
                AudioSource.PlayClipAtPoint(correctSound, Vector3.zero, 0.5f);

                chosenTile.GetComponent<Image>().color = chosenTile.color;

                ColorBlock chosenBlockColor = ColorBlock.defaultColorBlock;
                chosenBlockColor.disabledColor = Color.white;

                chosenTile.GetComponent<Button>().colors = chosenBlockColor;
                
                Destroy(chosenTile.GetComponent<Button>());

                switch (chosenTile.length)
                {
                    case 1:
                        foundShipPartCount[3] -= 1;

                        if (foundShipPartCount[3] == 0)
                        {
                            infoPanel.transform.GetChild(4).gameObject.SetActive(false);
                        }
                        break;
                    case 2:
                        foundShipPartCount[2] -= 1;

                        if (foundShipPartCount[2] == 0)
                        {
                            infoPanel.transform.GetChild(3).gameObject.SetActive(false);
                        }
                        break;
                    case 3:
                        foundShipPartCount[1] -= 1;

                        if (foundShipPartCount[1] == 0)
                        {
                            infoPanel.transform.GetChild(2).gameObject.SetActive(false);
                        }
                        break;
                    case 4:
                        foundShipPartCount[0] -= 1;

                        if (foundShipPartCount[0] == 0)
                        {
                            infoPanel.transform.GetChild(1).gameObject.SetActive(false);
                        }
                        break;

                    default:
                        break;
                }

                hitShipParts++;

                if (totalShipParts == hitShipParts)
                {
                    System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timer);
                    string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                    endGameTimer.text = "Süre: " + timeText;
                    endGameScore.text = "Skor: " + Mathf.RoundToInt((10000 - timer) - (wrongGuessCount * 10) - (wrongAnswerCount * 100));

                    endGamePanel.SetActive(true);
                    infoPanel.SetActive(false);

                    AudioSource.PlayClipAtPoint(doneSound, Vector3.zero, 0.5f);

                    return;
                }
            }
            else
            {
                wrongGuessCount++;
                AudioSource.PlayClipAtPoint(wrongSound, Vector3.zero, 0.5f);

                Instantiate(crossPrefab, chosenTile.transform);
                chosenTile.GetComponent<Image>().color = Color.black;
            }

            if (chosenTile.GetComponent<Button>() != null)
            {
                chosenTile.GetComponent<Button>().interactable = false;
            }
            questionPanel.SetActive(false);
            infoPanel.SetActive(true);

        }
        else
        {
            System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timer);
            string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            endGameTimer.text = "Süre: " + timeText;
            endGameScore.text = "Skor: " + Mathf.RoundToInt((10000 - timer) - (wrongGuessCount * 10) - (wrongAnswerCount * 100));

            questionPanel.SetActive(false);
            infoPanel.SetActive(true);

            AudioSource.PlayClipAtPoint(doneSound, Vector3.zero, 0.5f);
        }

        canChooseBlock = false;

        AskQuestionReverse();
    }
}