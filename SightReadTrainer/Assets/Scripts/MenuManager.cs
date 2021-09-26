using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [HideInInspector] public bool trebleSelected;
    [HideInInspector] public bool bassSelected;
    [HideInInspector] public int inputMinutes;
    [HideInInspector] public int inputSeconds;
    [HideInInspector] public bool isPlaying;

    [Space(10)]
    [BoxGroup("UI Pannels")] public GameObject mainMenuPannel;
    [BoxGroup("UI Pannels")] public GameObject menuPannel;
    [BoxGroup("UI Pannels")] public GameObject settingsPannel;
    [BoxGroup("UI Pannels")] public GameObject sightReadPannel;

    [Space(10)]
    [BoxGroup("UI Elements")] [SerializeField] private Toggle trebleClefToggle;
    [BoxGroup("UI Elements")] [SerializeField] private Toggle bassClefToggle;
    [BoxGroup("UI Elements")] [SerializeField] private TMP_InputField minutesInputField;
    [BoxGroup("UI Elements")] [SerializeField] private TMP_InputField secondsInputField;
    [BoxGroup("UI Elements")] public Toggle sharpNotesToggle;
    [BoxGroup("UI Elements")] public Toggle flatNotesToggle;

    [BoxGroup("Settings")] public Slider minGlidingSpeedSlider;
    [BoxGroup("Settings")] public Slider maxGlidingSpeedSlider;
    [BoxGroup("Settings")] public Slider spaceBetweenNotesSlider;

    [Space(10)]
    [BoxGroup("IntervalSelector")] [SerializeField] private GameObject minLimiter;
    [BoxGroup("IntervalSelector")] [SerializeField] private GameObject maxLimiter;

    [Space(10)]
    [BoxGroup("IntervalSelector")] [SerializeField] private GameObject trebleClefIntervalSection;
    [BoxGroup("IntervalSelector")] [SerializeField] private GameObject trebleVisualizerPannel;
    [Space(10)]
    [BoxGroup("IntervalSelector")] [SerializeField] private GameObject trebleLeftCursor;
    [BoxGroup("IntervalSelector")] [SerializeField] private GameObject invisibleTrebleLeftCursor;
    [HideInInspector] public int trebleLeftIndex;
    [Space(10)]
    [BoxGroup("IntervalSelector")] [SerializeField] private GameObject trebleRightCursor;
    [BoxGroup("IntervalSelector")] [SerializeField] private GameObject invisibleTrebleRightCursor;
    [HideInInspector] public int trebleRightIndex;
    [Space(10)]

    [Space(10)]
    [BoxGroup("IntervalSelector")] [SerializeField] private GameObject bassClefIntervalSection;
    [BoxGroup("IntervalSelector")] [SerializeField] private GameObject bassVisualizerPannel;
    [Space(10)]
    [BoxGroup("IntervalSelector")] [SerializeField] private GameObject bassLeftCursor;
    [BoxGroup("IntervalSelector")] [SerializeField] private GameObject invisibleBassLeftCursor;
    [HideInInspector] public int bassLeftIndex;
    [Space(10)]
    [BoxGroup("IntervalSelector")] [SerializeField] private GameObject bassRightCursor;
    [BoxGroup("IntervalSelector")] [SerializeField] private GameObject invisibleBassRightCursor;
    [HideInInspector] public int bassRightIndex;

    const int WHITE_KEY_AMOUNT = 33;

    private float trebleStartDist;
    private float bassStartDist;
    private float keyLength;

    private List<float> notesLimit = new List<float>();

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        InitializeSettingValues();
    }

    private void Start()
    {
        SceneSetup();
        SetTheKeyLimiterValues();
    }

    private void Update()
    {
        //Get the input time as int values
        if (minutesInputField.text != "" && secondsInputField.text != "")
        {
            inputMinutes = int.Parse(minutesInputField.text);
            inputSeconds = int.Parse(secondsInputField.text);
        }

        trebleSelected = trebleClefToggle.isOn;
        bassSelected = bassClefToggle.isOn;

        //Disable the interval selector based on the checkbox states
        trebleClefIntervalSection.SetActive(trebleSelected);
        bassClefIntervalSection.SetActive(bassSelected);

        SetupTheIntervalSelector();
    }

    //When clicking the 'Practice' button
    public void OnClick_PlayButton()
    {
        //Change the state so the GameManager script can do his job
        isPlaying = true;
    }

    //When clicking the 'Exit' button in the playmode
    public void OnClick_ExitButton()
    {
        //Change the state so the GameManager script can do his job
        isPlaying = false;  
    }

    public void OnClick_SettingsButton()
    {
        mainMenuPannel.SetActive(true);
        menuPannel.SetActive(false);
        settingsPannel.SetActive(true);
        sightReadPannel.SetActive(false);
    }

    public void OnClick_ScoreButton()
    {
        //Display the score pannel
    }
    public void OnSettingsUpdate()
    {
        gameManager.minGlidingSpeed = minGlidingSpeedSlider.value;
        gameManager.maxGlidingSpeed = maxGlidingSpeedSlider.value;
        gameManager.spaceBetweenNotes = spaceBetweenNotesSlider.value;
    }

    private void SetupTheIntervalSelector()
    {
        //Assign the selected note based on coordinates
        trebleLeftIndex = CurrentNoteSelector(invisibleTrebleLeftCursor);
        trebleRightIndex = CurrentNoteSelector(invisibleTrebleRightCursor);
        bassLeftIndex = CurrentNoteSelector(invisibleBassLeftCursor);
        bassRightIndex = CurrentNoteSelector(invisibleBassRightCursor);

        //Clamp the intervals
        trebleLeftIndex = Mathf.Clamp(trebleLeftIndex, 12, 32);
        trebleRightIndex = Mathf.Clamp(trebleRightIndex, 13, 33);
        bassLeftIndex = Mathf.Clamp(bassLeftIndex, 0, 20);
        bassRightIndex = Mathf.Clamp(bassRightIndex, 1, 21);


        //Put the cursor position to specific intervals
        AdjustCursorPosition(trebleLeftCursor, invisibleTrebleLeftCursor, trebleLeftIndex);
        AdjustCursorPosition(trebleRightCursor, invisibleTrebleRightCursor, trebleRightIndex);
        AdjustCursorPosition(bassLeftCursor, invisibleBassLeftCursor, bassLeftIndex);
        AdjustCursorPosition(bassRightCursor, invisibleBassRightCursor, bassRightIndex);

        //Put the trebleVisualizer anchor point in between the two cursor
        trebleVisualizerPannel.transform.position = new Vector2(trebleLeftCursor.transform.position.x + ((trebleRightCursor.transform.position.x - trebleLeftCursor.transform.position.x) / 2),
                                                                trebleVisualizerPannel.transform.position.y);
        //Scale trebleVisualizer with the ratio of the distance 
        trebleVisualizerPannel.transform.localScale = new Vector2((trebleRightCursor.transform.position.x - trebleLeftCursor.transform.position.x) / trebleStartDist,
                                                                  trebleVisualizerPannel.transform.localScale.y);

        bassVisualizerPannel.transform.position = new Vector2(bassLeftCursor.transform.position.x + ((bassRightCursor.transform.position.x - bassLeftCursor.transform.position.x) / 2),
                                                                bassVisualizerPannel.transform.position.y);
        bassVisualizerPannel.transform.localScale = new Vector2((bassRightCursor.transform.position.x - bassLeftCursor.transform.position.x) / bassStartDist,
                                                                  bassVisualizerPannel.transform.localScale.y);


        //NOT WORKING
        //Make the left treble cursor always on the left of the right one
/*        if (trebleLeftCursor.transform.position.x >= trebleRightCursor.transform.position.x)
        {
            trebleLeftCursor.transform.position = new Vector2(trebleLeftCursor.transform.position.x - keyLength, trebleLeftCursor.transform.position.y);
            trebleLeftIndex--;
        }
        if (trebleRightCursor.transform.position.x <= trebleLeftCursor.transform.position.x)
        {
            trebleRightCursor.transform.position = new Vector2(trebleRightCursor.transform.position.x + keyLength, trebleRightCursor.transform.position.y);
            trebleRightIndex++;
        }*/
    }

    private void AdjustCursorPosition(GameObject visibleCursor,GameObject invisibleCursor, int noteIndex)
    {
        //Snap the visible cursor to the notes intervals

        IntervalSlider intervalSlider = invisibleCursor.GetComponent<IntervalSlider>();
        //Snap back the position of the invisible cursor only when we are not changing the position of it
        if(!intervalSlider.isMoving)
        {
            invisibleCursor.transform.position = new Vector2(visibleCursor.transform.position.x + 0.001f, invisibleCursor.transform.position.y);
        }

        visibleCursor.transform.position = new Vector2(minLimiter.transform.position.x + (keyLength * (noteIndex)),
                                                            visibleCursor.transform.position.y);
    }

    private void SetTheKeyLimiterValues()
    {
        float keyboardLength;
        //Get the length of the keyboard
        keyboardLength = maxLimiter.transform.position.x - minLimiter.transform.position.x;
        //Divide the keyboard length by the number of keys to get the length of one key

        keyLength = keyboardLength / WHITE_KEY_AMOUNT;
        //Cycle through every key in the list to add each one
        for (int i = 0; i < WHITE_KEY_AMOUNT; i++)
        {
            //Add the limit x coordinate of each key to delimitate them
            notesLimit.Add(minLimiter.transform.position.x + (keyLength * i));
        }
        //Add one empty element to the list to prevent loop errors
        notesLimit.Add(minLimiter.transform.position.x + (keyLength * (WHITE_KEY_AMOUNT + 1)));
    }

    private int CurrentNoteSelector(GameObject usedCursor)
    {
        int index = 0;

        //If the cursor is under the limit
        if (usedCursor.transform.position.x < minLimiter.transform.position.x)
            return 0;
        //If the cursor is above the limit
        else if (usedCursor.transform.position.x > maxLimiter.transform.position.x)
            return WHITE_KEY_AMOUNT;

        //Cycle through each limit positions to know determine where the cursor is located and in what key it is
        for (int i = 1; i <= WHITE_KEY_AMOUNT; i++)
        {
            //If a key is between the current x coordinate and the next one at the same time, it means the cursor is on a specific note
            if (usedCursor.transform.position.x > notesLimit[i] && usedCursor.transform.position.x < notesLimit[i + 1])
            {
                index = i;
                //Return the index of the note
                return i;
            }
        }
        //Get the value after all 'else if' statement in case the 'if' doesn't occur  -> if not we get an error
        return index;
    }

    public void SceneSetup()
    {
        isPlaying = false;
        mainMenuPannel.SetActive(true);
        sightReadPannel.SetActive(false);
        settingsPannel.SetActive(false);

        //Cache this values to use them later in the code
        trebleStartDist = invisibleTrebleRightCursor.transform.position.x - invisibleTrebleLeftCursor.transform.position.x;
        bassStartDist = invisibleBassRightCursor.transform.position.x - invisibleBassLeftCursor.transform.position.x;
    }

    private void InitializeSettingValues()
    {
        minGlidingSpeedSlider.value = PlayerPrefs.GetFloat("MinGlidingSpeed", 3.5f);
        maxGlidingSpeedSlider.value = PlayerPrefs.GetFloat("MaxGlidingSpeed", 60f);
        spaceBetweenNotesSlider.value = PlayerPrefs.GetFloat("SpaceBetweenNotes", 600f);

        trebleClefToggle.isOn = Constants.IntToBool(PlayerPrefs.GetInt("TrebleToggle", 1));
        bassClefToggle.isOn = Constants.IntToBool(PlayerPrefs.GetInt("BassToggle", 1));
        sharpNotesToggle.isOn = Constants.IntToBool(PlayerPrefs.GetInt("SharpToggle", 1));
        flatNotesToggle.isOn = Constants.IntToBool(PlayerPrefs.GetInt("FlatToggle", 1));
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("MinGlidingSpeed", minGlidingSpeedSlider.value);
        PlayerPrefs.SetFloat("MaxGlidingSpeed", maxGlidingSpeedSlider.value);
        PlayerPrefs.SetFloat("SpaceBetweenNotes", spaceBetweenNotesSlider.value);

        PlayerPrefs.SetInt("TrebleToggle", Constants.BoolToInt(trebleClefToggle.isOn));
        PlayerPrefs.SetInt("BassToggle", Constants.BoolToInt(bassClefToggle.isOn));
        PlayerPrefs.SetInt("SharpToggle", Constants.BoolToInt(sharpNotesToggle.isOn));
        PlayerPrefs.SetInt("FlatToggle", Constants.BoolToInt(flatNotesToggle.isOn));
    }
}
