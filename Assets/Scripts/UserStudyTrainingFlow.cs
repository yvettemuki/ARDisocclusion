using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class UserStudyTrainingFlow : MonoBehaviour
{
    public GameObject m_ToggleGroup, m_QuestionText, m_SetupCanvas, m_StudyCanvas, m_TrainCanvas;
    public Button m_NextButton, m_RedoButton, m_StartButton;
    public Toggle[] m_Toggles, m_SimToggles, m_ClosestToggles;
    public UserStudyAPIs m_api;

    private ToggleGroup choices;
    private int currentTask, currentMethod, currentTrial;
    private string[,] answers;
    private string[,] completionTime;
    private bool started, finished;
    private float timer;
    private int step = 0;

    // Start is called before the first frame update
    void Start()
    {
        choices = m_ToggleGroup.GetComponent<ToggleGroup>();
        InitValues();
    }

    public void InitValues()
    {
        currentTask = 0;
        currentMethod = 0;
        currentTrial = 0;
        choices.SetAllTogglesOff();
        answers = new string[ControllerStates.MAX_TASK_NUM, ControllerStates.MAX_METHOD_NUM * ControllerStates.MAX_TRIAL_NUM];
        completionTime = new string[ControllerStates.MAX_TASK_NUM, ControllerStates.MAX_METHOD_NUM * ControllerStates.MAX_TRIAL_NUM];
        started = false;
        finished = false;
    }

    // Update is called once per frame
    void Update()
    {
        SetVisibility();
        updateText();
    }

    private void SetVisibility()
    {
        m_QuestionText.SetActive(started);

        if (started && !finished)
        {
            for (int i = 0; i < m_Toggles.Length; i++)
            {
                m_Toggles[i].gameObject.SetActive(true);

                if (InDirectIndicateTask() 
                    || currentTask == ControllerStates.MATCH_NUM
                    || currentTask == ControllerStates.FIND_CLOSEST)
                {
                    m_Toggles[i].gameObject.SetActive(false);
                }

            }

            for (int i = 0; i < m_SimToggles.Length; i++)
                m_SimToggles[i].gameObject.SetActive(currentTask == ControllerStates.MATCH_NUM);

            for (int i = 0; i < m_ClosestToggles.Length; i++)
                m_ClosestToggles[i].gameObject.SetActive(currentTask == ControllerStates.FIND_CLOSEST);
        }
        else if (finished)
        {
            for (int i = 0; i < m_SimToggles.Length; i++)
            {
                m_SimToggles[i].gameObject.SetActive(false);
            }

            m_TrainCanvas.SetActive(false);
            m_SetupCanvas.SetActive(true);
            GetComponent<UserStudyTrainingFlow>().enabled = false;
        }

        m_NextButton.gameObject.SetActive((choices.AnyTogglesOn() || InDirectIndicateTask()) && started && !finished);
        m_RedoButton.gameObject.SetActive(started && !finished);
        //m_StartButton.gameObject.SetActive(!started);
    }

    private void updateText()
    {
        if (!finished)
        {
            for (int i = 0; i < m_Toggles.Length; i++)
            {
                m_Toggles[i].transform.GetChild(1).GetComponent<Text>().text = ControllerStates.CHOICES[currentTask, currentTrial, i];
            }

            m_QuestionText.GetComponent<Text>().text = ControllerStates.QUESTIONS[currentTask];

            if (InDirectIndicateTask())
            {
                if (step == 1)
                    m_QuestionText.GetComponent<Text>().text = "Memorize the position of the person within the side corridor";
                else if (step == 2)
                    m_QuestionText.GetComponent<Text>().text = "Indicate the direction to the person using the circle";
            }

            for (int i = 0; i < m_SimToggles.Length; i++)
                m_SimToggles[i].transform.GetChild(1).GetComponent<Text>().text = ControllerStates.CHOICES_SIMILARITY_TRAIN[currentMethod * 3 + currentTrial, i];

            for (int i = 0; i < m_ClosestToggles.Length; i++)
                m_ClosestToggles[i].transform.GetChild(1).GetComponent<Text>().text = ControllerStates.CHOICES_CLOSEST[i];
        }
        else
        {
            m_QuestionText.GetComponent<Text>().text = "You have reached the end of the study! Thank you!";
            
        }

        //m_DebugText.GetComponent<Text>().text = string.Format("The current task is {0}, method is {1}, and trial is {2}.", currentTask, currentMethod, currentTrial);
    }

    public void StartNewTrial()
    {
        started = true;
        m_api.SetUserStudyMethod((ARController.UserStudyType)currentMethod);
        m_api.SetTrainTask((UserStudyController.TaskMode)(currentTask * 3 + currentTrial));
        if (InDirectIndicateTask())
        {
            step = 1;
        }
        m_SetupCanvas.SetActive(false);
        m_StudyCanvas.SetActive(false);
        m_TrainCanvas.SetActive(true);

        timer = Time.time;
    }

    public void NextTrial()
    {
        if (currentTask >= ControllerStates.MAX_TASK_NUM)
            return;

        string answer = null;

        if (InDirectIndicateTask())
        {
            if (step == 1)
            {
                m_api.SetNoneDisocclusinWithCrosshair();
                step = 2;
                return;
            }
            if (step == 2)
            {
                answer = m_api.GetDirectIndicateAccuracy();
                step = 0;
            }
        }
        else
            answer = choices.ActiveToggles().FirstOrDefault().GetComponentInChildren<Text>().text;

        answers[currentTask, currentMethod * ControllerStates.MAX_TRIAL_NUM + currentTrial] = answer;
        completionTime[currentTask, currentMethod * ControllerStates.MAX_TRIAL_NUM + currentTrial] = (Time.time - timer).ToString("0.00");
        currentTrial++;
        if (currentTrial >= ControllerStates.MAX_TRIAL_NUM)
        {
            currentTrial = 0;
            currentMethod++;
        }

        if (currentMethod >= ControllerStates.MAX_METHOD_NUM)
        {
            currentMethod = 0;
            currentTask++;
        }

        if (currentTask >= ControllerStates.MAX_TASK_NUM)
        {
            Conclude();
        }
        else
        {
            StartNewTrial();
            choices.SetAllTogglesOff();
        }
    }

    public void RedoTrial()
    {
        if (currentTask <= 0 && currentMethod <= 0 && currentTrial <= 0)
            return;

        currentTrial--;
        if (currentTrial < 0)
        {
            currentTrial = ControllerStates.MAX_TRIAL_NUM - 1;
            currentMethod--;
        }

        if (currentMethod < 0)
        {
            currentMethod = ControllerStates.MAX_METHOD_NUM - 1;
            currentTask--;
        }

        choices.SetAllTogglesOff();
        StartNewTrial();
    }

    private void Conclude()
    {
        finished = true;
        m_api.SetUserStudyMethod(ARController.UserStudyType.TYPE_NONE);
        m_api.SetTrainTask(UserStudyController.TaskMode.NONE);

        string path = "";
        if (Application.platform == RuntimePlatform.Android)
            path = Application.persistentDataPath;

        //m_DebugText.GetComponent<Text>().text = string.Format("The path is {0}.", path);

        string fname = System.DateTime.Now.ToString("MMddHHmm") + ".csv";

        using (StreamWriter writer = new StreamWriter(Path.Combine(path, fname)))
        {
            for (int i = 0; i < ControllerStates.MAX_TASK_NUM; i++)
            {
                writer.WriteLine(string.Join(",", GetRow(answers, i)));
                writer.WriteLine(string.Join(",", GetRow(completionTime, i)));
            }
        }
    }

    // https://www.codegrepper.com/code-examples/csharp/select+a+whole+row+out+of+a+2d+array+C%23
    private string[] GetRow(string[,] matrix, int rowNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(1))
                .Select(x => matrix[rowNumber, x])
                .ToArray();
    }

    private bool InDirectIndicateTask()
    {
        return
            (UserStudyController.TaskMode)(currentTask * 3 + currentTrial) == UserStudyController.TaskMode.DIRECT_INDICATOR_EASY
        || (UserStudyController.TaskMode)(currentTask * 3 + currentTrial) == UserStudyController.TaskMode.DIRECT_INDICATOR_MEDIUM
        || (UserStudyController.TaskMode)(currentTask * 3 + currentTrial) == UserStudyController.TaskMode.DIRECT_INDICATOR_HARD;
    }
}
