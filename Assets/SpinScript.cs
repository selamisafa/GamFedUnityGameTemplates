using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// her oyun sonu menü ekle

public class SpinScript : MonoBehaviour
{
    public int minPower = 0, maxPower, power;

    bool questionShown = false, rotating = false;
    Rigidbody2D rb;

    [System.Serializable]
    public struct Question
    {
        public string question;
        public string correctAnswer;
        public string wrongAnswer;
    }

    public List<Question> Questions;

    public Text questionText, answer1, answer2;

    public GameObject questionPanel;

    public List<Animator> thingToRescue;

    public GameObject endGamePanel;

    int pickedQuestionIndex;

    public GameObject emojiImage;

    public AudioClip correctSound, wrongSound, doneSound;

    private void Start()
    {
        ControlSpinWheelLook(questionPanel, 0);
        questionPanel.SetActive(false);

        rb = gameObject.GetComponent<Rigidbody2D>();

        Transform[] container = new Transform[gameObject.transform.childCount];
        
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            container[i] = gameObject.transform.GetChild(i);
        }

        for (int i = 0; i < container.Length; i++)
        {
            if (Questions.Count > i)
            {
                //container[i].transform.GetChild(0).GetComponent<Text>().text = Questions[i].question;
                container[i].transform.GetChild(0).GetComponent<Text>().text = "?";
            }
        }
    }

    public void SpinTheWheel()
    {
        power = Random.Range(minPower, maxPower);

        rb.angularVelocity = power;

        rotating = true;
    }

    private void Update()
    {
        if (!questionShown && rb.angularVelocity == 0 && rotating)
        {
            questionShown = true;

            int questionCount = Mathf.RoundToInt((Mathf.Abs(rb.rotation + 90) / 20)) % 18;

            PickQuestion(questionCount);
        }
    }

    void PickQuestion(int questionNumber)
    {
        if (gameObject.transform.GetChild(questionNumber).GetComponent<Image>().color == Color.grey)
        {
            power = -20;

            rb.angularVelocity = power;

            rotating = true;
            questionShown = false;

            return;
        }

        ControlSpinWheelLook(questionPanel, 1);

        gameObject.transform.GetChild(questionNumber).GetChild(0).GetComponent<Text>().text = Questions[questionNumber].question;
        questionText.text = Questions[questionNumber].question;
        pickedQuestionIndex = questionNumber;

        List<Text> answers = new List<Text>();

        answer1.text = "A) ";
        answer2.text = "B) ";

        answers.Add(answer1);
        answers.Add(answer2);

        int rngCorrect = Random.Range(0, 2);

        foreach (var item in answers)
        {
            item.GetComponent<AnswerButtonScript>().isCorrect = false;
        }

        Debug.Log(Questions[questionNumber].correctAnswer);
        answers[rngCorrect].text += Questions[questionNumber].correctAnswer;
        answers[rngCorrect].GetComponent<AnswerButtonScript>().isCorrect = true;

        answers.RemoveAt(rngCorrect);

        answers[0].text += Questions[questionNumber].wrongAnswer;

        answer1.transform.parent.GetComponent<Button>().interactable = true;
        answer2.transform.parent.GetComponent<Button>().interactable = true;

        rotating = false;

        ControlSpinWheelLook(gameObject.transform.parent.gameObject, 0);
    }

    public void AnswerGiven(AnswerButtonScript answerButtonScript)
    {
        Debug.Log(answerButtonScript.isCorrect);

        if (thingToRescue.Count > 0)
        {
            if (answerButtonScript.isCorrect)
            {
                AudioSource.PlayClipAtPoint(correctSound, Vector3.zero, 0.5f);

                thingToRescue[0].SetTrigger("Correct");
                GameObject go = thingToRescue[0].gameObject;

                thingToRescue.RemoveAt(0);

                gameObject.transform.GetChild(pickedQuestionIndex).GetComponent<Image>().color = Color.grey;

                StartCoroutine(NewQuestion(go));
            }
            else
            {
                AudioSource.PlayClipAtPoint(wrongSound, Vector3.zero, 0.5f);

                //questionPanel.SetActive(false);
                ControlSpinWheelLook(questionPanel, 0);

                questionShown = false;

                ControlSpinWheelLook(gameObject.transform.parent.gameObject ,1);
            }
        }
    }

    public void ControlSpinWheelLook(GameObject GO, int visible)
    {
        //GameObject[] objects = gameObject.transform.parent.childCount;

        //gameObject.transform.parent.localScale = Vector3.one * visible;

        StartCoroutine(InOutAnimation(GO, visible));
    }

    IEnumerator InOutAnimation(GameObject GO, int destinationScale)
    {
        float percent = 0;
        GO.SetActive(true);

        Vector3 startingScale = GO.transform.localScale;
        Vector3 destination = Vector3.one * destinationScale;

        while (percent < 1)
        {
            GO.transform.localScale = Vector3.Lerp(startingScale, destination, percent);

            percent += Time.deltaTime;
            yield return null;
        }

        GO.transform.localScale = destination;
    }

    IEnumerator NewQuestion(GameObject animal)
    {
        answer1.transform.parent.GetComponent<Button>().interactable = false;
        answer2.transform.parent.GetComponent<Button>().interactable = false;

        yield return new WaitForSeconds(5);

        ControlSpinWheelLook(questionPanel, 0);

        questionShown = false;

        //animal.transform.GetChild(0).gameObject.SetActive(false);
        //animal.transform.GetChild(1).gameObject.SetActive(false);

        ControlSpinWheelLook(animal.transform.GetChild(0).gameObject, 0);
        ControlSpinWheelLook(animal.transform.GetChild(1).gameObject, 0);

        Instantiate(emojiImage, animal.transform);

        if (thingToRescue.Count > 0)
        {
            ControlSpinWheelLook(gameObject.transform.parent.gameObject, 1);
        }
        else
        {
            AudioSource.PlayClipAtPoint(doneSound, Vector3.zero, 0.5f);

            endGamePanel.SetActive(true);
        }
    }
}
