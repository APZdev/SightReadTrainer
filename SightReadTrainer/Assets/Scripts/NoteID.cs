using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NoteID : MonoBehaviour
{
    private MenuManager menuManager;

    [HideInInspector] public int noteIndex;

    [Header("Note Essentials")]
    public GameObject noteObject;
    [SerializeField] private GameObject noteUp;
    [SerializeField] private GameObject noteDown;

    [Space(10), Header("Treble Clef")]
    [SerializeField] private GameObject trebleThirdTopLine;
    [SerializeField] private GameObject trebleSecondTopLine;
    [SerializeField] private GameObject trebleFirstTopLine;
    [SerializeField] private GameObject trebleFirstBottomLine;
    [SerializeField] private GameObject trebleSecondBottomLine;
    [SerializeField] private GameObject trebleThirdBottomLine;

    [Space(10), Header("Bass Clef")]
    [SerializeField] private GameObject bassThirdTopLine; 
    [SerializeField] private GameObject bassSecondTopLine;
    [SerializeField] private GameObject bassFirstTopLine;
    [SerializeField] private GameObject bassFirstBottomLine;
    [SerializeField] private GameObject bassSecondBottomLine;
    [SerializeField] private GameObject bassThirdBottomLine;

    [Space(10), Header("Accidental Notes")]
    [SerializeField] private GameObject sharpObject;
    [SerializeField] private GameObject flatObject;

    [SerializeField] private float fadeOutTime;
    [SerializeField] private float flyingNoteSpeed;

    private List<GameObject> noteObjects = new List<GameObject>();

    public enum Key
    {
        C,
        D,
        E,
        F,
        G,
        A,
        B
    }
    [HideInInspector] public Key key;

    public enum AccidentalNotes
    {
        Natural = 0,
        Sharp = 1,
        Flat = 2,
    }
    public AccidentalNotes accidentalNotes;

    public enum KeyState
    {
        Waiting,
        Correct,
        Incorrect,
    }
    [HideInInspector] public KeyState keyState;

    private void Start()
    {
        menuManager = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<MenuManager>();

        //Set to key to waiting because KeyState is controlled by GameManager script
        keyState = KeyState.Waiting;

        SetupNoteObjects();
        AssignPitchToKey();
        CheckForLedgerLines();
    }

    private void LateUpdate()
    {
        PlayFadeAnimation(keyState);
    }

    private void SetupNoteObjects()
    {
        noteObjects.Add(noteDown);
        noteObjects.Add(noteUp);
        noteObjects.Add(sharpObject);
        noteObjects.Add(flatObject);
        noteObjects.Add(trebleThirdTopLine);
        noteObjects.Add(trebleSecondTopLine);
        noteObjects.Add(trebleFirstTopLine);
        noteObjects.Add(trebleFirstBottomLine);
        noteObjects.Add(trebleSecondBottomLine);
        noteObjects.Add(trebleThirdBottomLine);
        noteObjects.Add(bassThirdTopLine);
        noteObjects.Add(bassSecondTopLine);
        noteObjects.Add(bassFirstTopLine);
        noteObjects.Add(bassFirstBottomLine);
        noteObjects.Add(bassSecondBottomLine);
        noteObjects.Add(bassThirdBottomLine);
    }

    private void AssignPitchToKey()
    {
        //Assign a key to the generated treble clefs notes
        if (noteIndex == 16 || noteIndex == 9 || noteIndex == 2 || noteIndex == 25 || noteIndex == 32 || noteIndex == 39)
            key = NoteID.Key.C;
        if (noteIndex == 15 || noteIndex == 8 || noteIndex == 1 || noteIndex == 24 || noteIndex == 31 || noteIndex == 38)
            key = NoteID.Key.B;
        if (noteIndex == 14 || noteIndex == 7 || noteIndex == 0 || noteIndex == 23 || noteIndex == 30 || noteIndex == 37)
            key = NoteID.Key.A;
        if (noteIndex == 20 || noteIndex == 13 || noteIndex == 6 || noteIndex == 22 || noteIndex == 29 || noteIndex == 36)
            key = NoteID.Key.G;
        if (noteIndex == 19 || noteIndex == 12 || noteIndex == 5 || noteIndex == 21 || noteIndex == 28 || noteIndex == 35)
            key = NoteID.Key.F;
        if (noteIndex == 18 || noteIndex == 11 || noteIndex == 4 || noteIndex == 41 || noteIndex == 27 || noteIndex == 34)
            key = NoteID.Key.E;
        if (noteIndex == 17 || noteIndex == 10 || noteIndex == 3 || noteIndex == 40 || noteIndex == 26 || noteIndex == 33)
            key = NoteID.Key.D;

        //Assign the accidental notes randomly if they are checked on the menu
        if (menuManager.sharpNotesToggle.isOn && !menuManager.flatNotesToggle.isOn)
            accidentalNotes = (AccidentalNotes)Random.Range(0, 2);
        else if (!menuManager.sharpNotesToggle.isOn && menuManager.flatNotesToggle.isOn)
            accidentalNotes = (AccidentalNotes)RandomRangeExcept(0, 3, 1);
        else if (menuManager.sharpNotesToggle.isOn && menuManager.flatNotesToggle.isOn)
            accidentalNotes = (AccidentalNotes)Random.Range(0, 3);

        //Activate sharp or flat objects based on the random selection
        if (accidentalNotes == AccidentalNotes.Flat)
            flatObject.SetActive(true);
        if (accidentalNotes == AccidentalNotes.Sharp)
            sharpObject.SetActive(true);
    }

    private void CheckForLedgerLines()
    {
        //Check if a treble key needs ledger lines
        if (noteIndex == 0)
        {
            bassFirstBottomLine.SetActive(true);
            bassSecondBottomLine.SetActive(true);
            bassThirdBottomLine.SetActive(true);
        }
        if (noteIndex == 1 || noteIndex == 2)
        {
            bassFirstBottomLine.SetActive(true);
            bassSecondBottomLine.SetActive(true);
        }
        if (noteIndex == 3 || noteIndex == 4)
            bassFirstBottomLine.SetActive(true);

        if (noteIndex == 16 || noteIndex == 17)
            bassFirstTopLine.SetActive(true);
        if (noteIndex == 18 || noteIndex == 19)
        {
            bassFirstTopLine.SetActive(true);
            bassSecondTopLine.SetActive(true);
        }
        if (noteIndex == 20)
        {
            bassFirstTopLine.SetActive(true);
            bassSecondTopLine.SetActive(true);
            bassThirdTopLine.SetActive(true);
        }

        //Check if a treble key needs ledger lines
        if (noteIndex == 21)
        {
            trebleFirstBottomLine.SetActive(true);
            trebleSecondBottomLine.SetActive(true);
            trebleThirdBottomLine.SetActive(true);
        }
        if (noteIndex == 22 || noteIndex == 23)
        {
            trebleFirstBottomLine.SetActive(true);
            trebleSecondBottomLine.SetActive(true);
        }
        if (noteIndex == 24 || noteIndex == 25)
            trebleFirstBottomLine.SetActive(true);

        if (noteIndex == 37 || noteIndex == 38)
            trebleFirstTopLine.SetActive(true);
        if (noteIndex == 39 || noteIndex == 40)
        {
            trebleFirstTopLine.SetActive(true);
            trebleSecondTopLine.SetActive(true);
            noteUp.SetActive(false);
            noteDown.SetActive(true);
        }
        if (noteIndex == 41)
        {
            trebleFirstTopLine.SetActive(true);
            trebleSecondTopLine.SetActive(true);
            trebleThirdTopLine.SetActive(true);
            noteUp.SetActive(false);
            noteDown.SetActive(true);
        }
    }

    private void PlayFadeAnimation(KeyState guessedState)
    {
        if(guessedState == KeyState.Correct)
        {
            foreach(GameObject go in noteObjects)
            {
                go.GetComponent<Image>().color = Color.Lerp(go.GetComponent<Image>().color, new Color32(0, 0, 0, 0), fadeOutTime * Time.deltaTime);
                go.transform.Translate(Vector3.up * flyingNoteSpeed * Time.deltaTime);
                go.transform.Translate(Vector3.left * (flyingNoteSpeed / 2) * Time.deltaTime);
            }
        }
        else if(guessedState == KeyState.Incorrect)
        {
            foreach (GameObject go in noteObjects)
            {
                go.GetComponent<Image>().color = Color.Lerp(go.GetComponent<Image>().color, new Color32(240, 0, 0, 0), fadeOutTime * Time.deltaTime);
                go.transform.Translate(Vector3.down * flyingNoteSpeed * Time.deltaTime);
                go.transform.Translate(Vector3.left * (flyingNoteSpeed / 2) * Time.deltaTime);
            }

            Destroy(transform.gameObject, 3);
        }
        else
        {
            return;
        }
    }

    private int RandomRangeExcept(int min, int max, int except)
    {
        int number;
        do
        {
            number = Random.Range(min, max);
        } while (number == except);
        return number;
    }
}
