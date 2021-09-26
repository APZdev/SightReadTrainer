using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(MenuManager))]
public class GameManager : MonoBehaviour
{
    private MenuManager menuManager;

    public List<GameObject> notes;

    [Space(10)]
    [BoxGroup("UI Essentials")] [SerializeField] private GameObject noteHolder;
    [BoxGroup("UI Essentials")] [SerializeField] private GameObject noteTemplate;
    [BoxGroup("UI Essentials")] [SerializeField] private GameObject staffLimit;
    [BoxGroup("UI Essentials")] [SerializeField] private GameObject staffHolder;
    [BoxGroup("UI Essentials")] [SerializeField] private GameObject noteSpawnPoint;
    [Space(10)]
    [BoxGroup("UI Essentials")] [SerializeField] private Button sharpButton;
    [BoxGroup("UI Essentials")] [SerializeField] private Button flatButton;
    [BoxGroup("UI Essentials")] public NoteID.AccidentalNotes accidentalNote;

    [Space(10)]
    //Bottom to Top when counting lines
    [Tooltip("First line of the treble clef staff")]
    [BoxGroup("UI Essentials")] [SerializeField] private GameObject firstSampleLine;

    [Tooltip("Second line of the treble clef staff")]
    [BoxGroup("UI Essentials")] [SerializeField] private GameObject secondSampleLine;

    [Tooltip("Fifth line of the bass clef staff")]
    [BoxGroup("UI Essentials")] [SerializeField] private GameObject bassSampleLine;

    [Space(10)]
    [BoxGroup("Settings")] [Range(0f, 10f)] public float minGlidingSpeed;
    [BoxGroup("Settings")] [Range(30f, 100f)] public float maxGlidingSpeed;
    [BoxGroup("Settings")] [Range(250, 900f)] public float spaceBetweenNotes;

    //Put the value on the X axis at where notes has to spawn
    private float noteSpawnerY;

    //All the treble clef spawn points Y axis coordinates
    private float[] spawnPoints;
    private const int SPAWN_POINT_NUMBER = 42;
    private float trebleNoteOffset;
    private float noteStartingPosY;
    private float staffLineSpace;
    private bool isCalculated;

    private int minTrebleInterval;
    private int maxTrebleInterval;
    private int minBassInterval;
    private int maxBassInterval;

    private float glidingSpeed;
    private float practiceTimer;

    private void Start()
    {
        menuManager = GetComponent<MenuManager>();

        //So the notes only spawn when you have loaded the sight read page for the first time
        isCalculated = false;
    }

    private void Update()
    {
        SetupAndManageStates();

        MenuToGameValuesChanger();
        SpawnNotes();
        MoveNotes();
        UpdateAccidentalNotesButtonSates();
    }

    private void SetupAndManageStates()
    {
        noteSpawnerY = bassSampleLine.transform.position.y + (Mathf.Abs(bassSampleLine.transform.position.y - secondSampleLine.transform.position.y) / 2);

        //We do this on Update() because if the object is disabled at the launch of the app, It skips the Start()
        if (!isCalculated)
        {
            StaffSetup();
            isCalculated = true;
        }

        //Only generate notes when we are in playmode (not in the menu)
        if (!menuManager.isPlaying)
        {
            //Assign the menu timer values to the practice session
            practiceTimer = menuManager.inputSeconds + menuManager.inputMinutes * 60f;

            //Clear notes in the scene
            for (int i = 0; i < notes.Count; i++)
            {
                Destroy(notes[i]);
                notes.RemoveAt(i);
            }
            //Return so nothing from this script gets executed when not playing
            return;
        }

        //Substract 1 second from the timer
        practiceTimer -= Time.deltaTime;

        //If the practice time is finished
        if (practiceTimer <= 0.0f)
        {
            menuManager.SceneSetup();
        }
    }

    private void SpawnNotes()
    {
        if (CheckIfNoteCanSpawn())
        {
            //Check if it's treble clef
            if (menuManager.trebleSelected && !menuManager.bassSelected)
            {
                CreateKeys(minTrebleInterval, maxTrebleInterval);
            }
            //Check if it's bass clef
            else if(!menuManager.trebleSelected && menuManager.bassSelected)
            {
                CreateKeys(minBassInterval, maxBassInterval);
            }
            //Check if it's both clefs together (the order to play is bass then treble)
            else if(menuManager.trebleSelected && menuManager.bassSelected)
            {
                CreateKeys(minBassInterval, maxBassInterval);
                CreateKeys(minTrebleInterval, maxTrebleInterval);
            }
        }
    }

    private void CreateKeys(int minInterval, int maxInterval)
    {
        //Generate notes randomly based on the chosen level in the menu
        var index = Random.Range(minInterval, maxInterval);
        var go = Instantiate(noteTemplate, Vector3.zero, Quaternion.identity);
        go.transform.SetParent(noteHolder.transform);
        //Resize the object because somehow, the size change when I parent it
        go.transform.localScale = new Vector3(staffHolder.transform.localScale.x, staffHolder.transform.localScale.y, staffHolder.transform.localScale.z);
        //Spawn the note out off the screen
        go.transform.position = new Vector3(noteSpawnPoint.transform.position.x, noteSpawnerY, 0f);
        //Set the note to the right line
        go.GetComponent<NoteID>().noteObject.transform.position = new Vector3(go.GetComponent<NoteID>().noteObject.transform.position.x, 
                                                                              spawnPoints[index], 0f);
        //Set the index of the note to manage ledger lines and key
        go.GetComponent<NoteID>().noteIndex = index;
        //Add the note to a list so we can keep track of what is happening
        notes.Add(go);
    }

    private void MenuToGameValuesChanger()
    {
        //This since some notes are the same but in different keys, and we can choose the same notes but in different keys, this is useful to offset index values
        minTrebleInterval = menuManager.trebleLeftIndex + 9;
        maxTrebleInterval = menuManager.trebleRightIndex + 9;

        minBassInterval = menuManager.bassLeftIndex;
        maxBassInterval = menuManager.bassRightIndex;
    }

    private void MoveNotes()
    {
        //Prevent errors if there is no notes
        if(notes.Count > 0)
        {
            //Calculate the speed of notes based on the distance to the limit point
            glidingSpeed = maxGlidingSpeed * ((Mathf.Abs(staffLimit.transform.position.x - notes[0].transform.position.x)) / 100);
            //Clamp the speed
            glidingSpeed = Mathf.Clamp(glidingSpeed, minGlidingSpeed, maxGlidingSpeed);

            //Move all the notes
            foreach(GameObject note in notes)
            {
                //IF the limit is reached -> Stop the notes (float added at the end of the "if" is an offset so the note stop before it crosses the limit line)
                if(notes[0].transform.position.x <= staffLimit.transform.position.x + 0.6f)
                {
                    return;
                }
            
                //Move notes to the left
                note.transform.Translate(-Vector3.right * Mathf.Abs(glidingSpeed) * Time.deltaTime);
            }
        }
    }

    private bool CheckIfNoteCanSpawn()
    {
        if(notes.Count > 0)
        {
            //Check if the distance between the spawner and the last generated note is greater than the threshold
            if ((Mathf.Abs(noteSpawnPoint.transform.position.x - notes[notes.Count - 1].transform.position.x) > spaceBetweenNotes / 250f))
                return true;
            else
                return false;
        }
        //Default return to prevent error
        else
        {
            return true;
        }
    }

    private void StaffSetup()
    {
        spawnPoints = new float[SPAWN_POINT_NUMBER];
        //Get the distance between first and second line
        staffLineSpace = Mathf.Abs(secondSampleLine.transform.position.y - firstSampleLine.transform.position.y) / 2;
        //Get the distance between the last note of the bass clef and the first note of the treble clef (read from bottom to top)
        trebleNoteOffset = Mathf.Abs((bassSampleLine.transform.position.y + (staffLineSpace * 6)) - (secondSampleLine.transform.position.y - (staffLineSpace * 6)));
        //This value is the starting point from where to start calculations
        noteStartingPosY = secondSampleLine.transform.position.y - trebleNoteOffset - (staffLineSpace * 26);

        //Generate spawn point based on the distance between two staff lines
        for (int i = 0; i < SPAWN_POINT_NUMBER; i++)
        {
            if (i <= 20)
                spawnPoints[i] = noteStartingPosY + (staffLineSpace * i);
            else
                spawnPoints[i] = noteStartingPosY + (staffLineSpace * i) + trebleNoteOffset - staffLineSpace;
        }
    }

    private void UpdateAccidentalNotesButtonSates()
    {
        bool sharpButtonValue = sharpButton.GetComponent<OnButtonEvent>().isClicked;
        bool flatButtonValue = flatButton.GetComponent<OnButtonEvent>().isClicked;

        //Each case of the sharp/flat buttons to determine which accidental note is pressed
        if (sharpButtonValue && flatButtonValue)
            accidentalNote = NoteID.AccidentalNotes.Sharp;
        else if (sharpButtonValue && !flatButtonValue)
            accidentalNote = NoteID.AccidentalNotes.Sharp;
        else if (!sharpButtonValue && flatButtonValue)
            accidentalNote = NoteID.AccidentalNotes.Flat;
        else if (!sharpButtonValue && !flatButtonValue)
            accidentalNote = NoteID.AccidentalNotes.Natural;
    }

    public void OnClick_GetKeyPressed(string noteName)
    {
        NoteID noteID = notes[0].GetComponent<NoteID>();

        //If button pressed correspond to the waiting note key
        if (noteID.key.ToString() == noteName)
        {
            //If the sharp/flat/natural matches
            if(noteID.accidentalNotes == accidentalNote)
            {
                //Remove the first note to let the next be evaluated
                noteID.keyState = NoteID.KeyState.Correct;
                notes.RemoveAt(0);
            }
            else
            {
                noteID.keyState = NoteID.KeyState.Incorrect;
                notes.RemoveAt(0);
            }
        }
        else
        {
            noteID.keyState = NoteID.KeyState.Incorrect;
            notes.RemoveAt(0);
        }
    }
}
