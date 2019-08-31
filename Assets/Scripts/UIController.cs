using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public delegate void NotifyGridDims(int len, int hei);
    public static event NotifyGridDims OnGridDimInit;

    public delegate void NotifyBegin();
    public static event NotifyBegin OnBegin;
    public delegate void NotifyNext();
    public static event NotifyNext OnNext;
    public delegate void NotifyPause();
    public static event NotifyPause OnPause;
    public delegate void NotifyClear();
    public static event NotifyClear OnClear;
    public delegate void NotifySpeedChange(float speed);
    public static event NotifySpeedChange OnSpeedChange;

    public Camera cam;

    public InputField lenInput;
    public InputField heiInput;
    public CanvasGroup initPanel;
    public Button startButton;
    public Button nextButton;
    public Button resetButton;
    public Slider speedSlider;

    public Text genCounterTxt;
    public Text errTxt;

    private bool mouseEnabled;
    private bool errTxtUsed;
    private int genCounter;

    private void OnEnable() {
        GridController.OnPassDone += UpdateGenCnt;
    }
    private void OnDisable() {
        GridController.OnPassDone -= UpdateGenCnt;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) && mouseEnabled) {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {         
                int x, y;

                string name = hit.transform.name;
                string[] xAndY = name.Split(',');
                int.TryParse(xAndY[0], out x);
                int.TryParse(xAndY[1], out y);

                GridSetup.gridObj.grid[x, y].FlipState();
            }
        }
    }
    public void CreateButton() {
        if (lenInput.text == "") {
            if (heiInput.text == "") {
                FlashErrTxt("length and height not set.");
                return;
            } else {
                FlashErrTxt("length not set.");
                return;
            }
        } else if (heiInput.text == "") {
            FlashErrTxt("height not set.");
            return;
        }
        int len, hei;
        ParseDimensions(lenInput.text, heiInput.text, out len, out hei);

        if (OnGridDimInit != null) {
            OnGridDimInit(len, hei);
        }

        initPanel.blocksRaycasts = false;
        initPanel.interactable = false;
        initPanel.alpha = 0f;

        mouseEnabled = true;
    }
    public void StartButton() {
        if (OnBegin != null) {
            OnBegin();
        }
        startButton.interactable = false;
        nextButton.interactable = false;
        resetButton.interactable = false;
    }
    public void PauseButton() {
        if (OnPause != null) {
            OnPause();
        }

        startButton.interactable = true;
        nextButton.interactable = true;
        resetButton.interactable = true;
    }
    public void NextButton() {
        if (OnNext != null) {
            OnNext();
        }
    }
    public void ResetButton() {
        initPanel.blocksRaycasts = true;
        initPanel.interactable = true;
        initPanel.alpha = 1.0f;

        UpdateGenCnt(0);
        mouseEnabled = false;
    }
    public void ClearButton() {
        UpdateGenCnt(0);
        if (OnClear != null) {
            OnClear();
        }
    }
    private void FlashErrTxt(string text) {
        if (errTxtUsed) {
            return;
        }

        errTxt.text = text;

        StartCoroutine("FadeErrTxt");
    }
    private IEnumerator FadeErrTxt() {
        for (float alpha = 1f; alpha > 0; alpha -= .05f) {
            Color c = errTxt.color;
            c.a = alpha;
            errTxt.color = c;

            yield return new WaitForSeconds(.2f);
        }
        errTxt.text = "";
        errTxtUsed = false;
    }
    private void ParseDimensions(string lenStr, string heiStr, out int len, out int hei) {
        int.TryParse(lenStr, out len);
        int.TryParse(heiStr, out hei);

        len = len < GridController.MINDIM ? GridController.MINDIM : len;
        hei = hei < GridController.MINDIM ? GridController.MINDIM : hei;
        len = len >= GridController.MAXDIM ? GridController.MAXDIM : len;
        hei = hei >= GridController.MAXDIM ? GridController.MAXDIM : hei;
    }
    private void UpdateGenCnt() {
        genCounter++;
        genCounterTxt.text = "Generations: " + genCounter;
    }
    private void UpdateGenCnt(int amt) {
        genCounter = amt;
        genCounterTxt.text = "Generations: " + genCounter;
    }
    public void UpdateSpeed() {
        if (OnSpeedChange != null) {
            OnSpeedChange(speedSlider.value);
        }
    }
}
