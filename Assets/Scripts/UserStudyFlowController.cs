using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class UserStudyFlowController : MonoBehaviour
{
    public GameObject m_ToggleGroup, DebugText;
    public Button Next, Redo;
    
    private ToggleGroup choices;
    private int currentTask, currentMethod, currentTrial;
    private string[,] answers;

    // Start is called before the first frame update
    void Start()
    {
        choices = m_ToggleGroup.GetComponent<ToggleGroup>();
        InitValues();
    }

    public void InitValues()
    {
        currentTask = ControllerStates.DYNAMIC_BALL;
        currentMethod = ControllerStates.P_IN_P;
        currentTrial = 0;
        choices.SetAllTogglesOff();
        answers = new string[ControllerStates.MAX_TASK_NUM, ControllerStates.MAX_METHOD_NUM * ControllerStates.MAX_TRIAL_NUM];
    }

    // Update is called once per frame
    void Update()
    {
        SetVisibility();
    }

    private void SetVisibility()
    {
        Next.gameObject.SetActive(choices.AnyTogglesOn());
    }

    public void NextTrial()
    {
        if (currentTask >= ControllerStates.MAX_TASK_NUM)
            return;
        DebugText.GetComponent<Text>().text = Application.persistentDataPath + "\n";
        DebugText.GetComponent<Text>().text += string.Format("The current task is {0}, method is {1}, and trial is {2}.", currentTask, currentMethod, currentTrial);
        string answer = choices.ActiveToggles().FirstOrDefault().gameObject.name;
        answers[currentTask, currentMethod * ControllerStates.MAX_TRIAL_NUM + currentTrial] = answer;
        currentTrial++;
        if (currentTrial >= ControllerStates.MAX_TRIAL_NUM)
        {
            currentTrial = 0;
            currentMethod++;
        }

        if (currentMethod >= ControllerStates.MAX_METHOD_NUM)
        {
            currentMethod = ControllerStates.P_IN_P;
            currentTask++;
        }

        if (currentTask >= ControllerStates.MAX_TASK_NUM)
        {
            Conclude();
        }
        
        choices.SetAllTogglesOff();
    }

    public void Conclude()
    {
        string path = "";
        if (Application.platform == RuntimePlatform.Android)
            path = Application.persistentDataPath;
        else
            path = "C:/Users/zhouy/OneDrive - purdue.edu/Desktop/VRAR/final project/Data2";

        string fname = System.DateTime.Now.ToString("HH-mm-ss") + ".csv";

        using (StreamWriter writer = new StreamWriter(Path.Combine(path, fname)))
        {
            for (int i = 0; i < ControllerStates.MAX_TASK_NUM; i++)
            {
                writer.WriteLine(string.Join(",", GetRow(answers, i)));
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
