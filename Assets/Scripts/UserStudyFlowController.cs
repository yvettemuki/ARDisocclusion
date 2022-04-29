using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class UserStudyFlowController : MonoBehaviour
{
    public GameObject m_ToggleGroup, m_DebugText, m_QuestionText, m_SetupCanvas, m_StudyCanvas;
    public Button m_NextButton, m_RedoButton, m_StartButton;
    public Toggle[] m_Toggles;
    public UserStudyAPIs m_api;
    
    private ToggleGroup choices;
    private int currentTask, currentMethod, currentTrial;
    private string[,] answers;
    private string[,] completionTime;
    private bool started, finished;
    private float timer;

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
        for (int i = 0; i < m_Toggles.Length; i++)
        {
            m_Toggles[i].gameObject.SetActive(started && !finished);
            if (i == 0 || i == 2 || i == 4)
            {
                m_Toggles[i].gameObject.SetActive(currentMethod != 2);
            }
        }
        m_NextButton.gameObject.SetActive(choices.AnyTogglesOn() && started && !finished);
        m_RedoButton.gameObject.SetActive(started && !finished);
        m_StartButton.gameObject.SetActive(!started);
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
        } else
        {
            m_QuestionText.GetComponent<Text>().text = "You have reached the end of the study";
        }

        //m_DebugText.GetComponent<Text>().text = string.Format("The current task is {0}, method is {1}, and trial is {2}.", currentTask, currentMethod, currentTrial);
    }

    public void StartNewTrial()
    {
        started = true;
        m_api.SetUserStudyMethod((ARContorller.UserStudyType)currentMethod);
        m_api.SetUserStudyTask((UserStudyController.TaskMode) (currentTask * 3 + currentTrial));
        m_SetupCanvas.SetActive(false);
        m_StudyCanvas.SetActive(true);
        timer = Time.time;
    }

    public void NextTrial()
    {
        if (currentTask >= ControllerStates.MAX_TASK_NUM)
            return;
        Debug.Log(1);
        string answer = choices.ActiveToggles().FirstOrDefault().gameObject.name;
        Debug.Log(choices.ActiveToggles().FirstOrDefault());
        Debug.Log("name" + choices.ActiveToggles().FirstOrDefault().gameObject.name);
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
        } else
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
        string path = "";
        if (Application.platform == RuntimePlatform.Android)
            path = Application.persistentDataPath;
        else
            path = "C:/Users/zhouy/OneDrive - purdue.edu/Desktop/VRAR/final project/Data2";

        m_DebugText.GetComponent<Text>().text = string.Format("The path is {0}.", path);


        string fname = System.DateTime.Now.ToString("HH-mm-ss") + ".csv";

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
}
