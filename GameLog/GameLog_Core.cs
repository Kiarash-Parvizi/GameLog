﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using UnityEngine;

public class GameLog_Core : MonoBehaviour {

    public GameObject TextOBJ;
    public GameObject parentOBJ;
    public Text MainLogs;
    public RectTransform Content;

    void Start() {
        DontDestroyOnLoad(this.gameObject);
        GameLog.data = this;
        GameLog.INIT();
    }
    int a = 0;
    public static int wait = 0;
    private void FixedUpdate() {
        if (wait > 0) {
            wait--;
            return;
        }
        GameLog.Core();
        a++;
        if (a > 10) {
            GameLog.CULLING();
            a = 0;
        }
    }


    //--------------------------------
    public InputField page2Input;
    public InputField String2Find_Input;
    public Text page2Text;
    public void GETBytes() {
        int index = int.Parse(page2Input.text, System.Globalization.NumberStyles.HexNumber);
        if (index >= GameLog.messages.Count || index < 0) {
            page2Text.text = "Not Found";
            return;
        }
        page2Text.text = "";
        for (int i = 0; i < GameLog.messages[index].b.Length; i++) {
            page2Text.text += ""+GameLog.messages[index].b[i] + ", ";
        }
        page2Text.text += "\nlen: " + GameLog.messages[index].b.Length;
    }

    public void FindString() {
        Search.Find();
    }
    //--------------------------------

    bool b;
    public GameObject Page2;
    public void open_InputPage() {
        if (b) {
            Page2.SetActive(false);
            b = false;
        } else {
            Page2.SetActive(true);
            b = true;
        }
    }
}

public static class GameLog {
    public static GameLog_Core data;
    public static List<NLogMessage> messages = new List<NLogMessage>();
    static RectTransform RectTrans;
    static RectTransform ContentRect;
    public static void INIT() {
        RectTrans = data.MainLogs.GetComponent<RectTransform>();
        ContentRect = data.Content;
        AllTextOBJ.Add(RectTrans);
    }

    public static List<RectTransform> AllTextOBJ = new List<RectTransform>();

    public static void Log(string t, Color c) {
        messages.Add(new NLogMessage { message = t, color = c, b = Encoding.ASCII.GetBytes(t) });
    }
    public static void Log(string t) {
        messages.Add(new NLogMessage { message = t, color = Color.white, b = Encoding.ASCII.GetBytes(t) });
    }

    static bool firstCloneCommand = true;
    static bool hadBadByte = false;
    static float yPluss = 0;
    static int lineCounter = 0, textIndex = 0;
    static int counter = 0, lastIndex = 0;
    public static void Core() {
        for (int i = lastIndex; i < messages.Count; i++) {
            //LOG
            string message = "";
            for (int a = 0; a < messages[i].b.Length; a++) {
                if (messages[i].b[a] < 33 || messages[i].b[a] > 126) {
                    if (!hadBadByte)
                    {
                        message += "<color=yellow>";
                        hadBadByte = true;
                    }
                    message += " '" + messages[i].b[a] + "' ";
                    continue;
                }
                else {
                    if (hadBadByte)
                    {
                        message += "</color>";
                        hadBadByte = false;
                    }
                    message += (char)messages[i].b[a];
                }
            }
            if (hadBadByte)
            {
                message += "</color>";
                hadBadByte = false;
            }
            data.MainLogs.text += "<color="+ ToRGBHex(messages[i].color) + "><color=#557799>" + counter.ToString("X") + " : </color>" + message+"</color>\n\n";
            counter++;
            lastIndex++;
            lineCounter++;
            if (lineCounter > 10)
            {
                if (firstCloneCommand) {
                    GameLog_Core.wait = 5;
                    firstCloneCommand = false;
                    return;
                }
                textIndex++;
                cullingException = textIndex;
                yPluss += RectTrans.sizeDelta.y+10;
                //if (firstSec) {
                //    yPluss += RectTrans.sizeDelta.y / 2;
                //    firstSec = false;
                //}
                var obj = GameObject.Instantiate(data.TextOBJ, data.parentOBJ.transform); obj.SetActive(true);
                ContentRect.sizeDelta = new Vector2(ContentRect.sizeDelta.x, yPluss*2.5f);
                RectTrans = obj.GetComponent<RectTransform>();
                AllTextOBJ.Add(RectTrans);
                RectTrans.anchoredPosition = new Vector2(RectTrans.anchoredPosition.x, RectTrans.anchoredPosition.y - yPluss*1.4f);
                data.MainLogs = obj.GetComponent<Text>();
                data.MainLogs.text = "";
                lineCounter = 0;
                firstCloneCommand = true;
            }
        }
        
    }

    //-------------------------------------------------------------

    static int cullingException = -1;
    public static void CULLING() {
        for (int i = 0; i < AllTextOBJ.Count; i++) {
            if (i == cullingException)
                continue;
            if (Mathf.Abs(-AllTextOBJ[i].anchoredPosition.y - ContentRect.anchoredPosition.y) < 1100)
            {
                //ENABLE
                AllTextOBJ[i].gameObject.SetActive(true);
                continue;
            }
            //DISABLE
            AllTextOBJ[i].gameObject.SetActive(false);
        }
    }

    //-------------------------------------------------------------

    public static string ToRGBHex(Color c) {
        return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
    }
    private static byte ToByte(float f) {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }
}


public struct NLogMessage {
    public string message;
    public Color color;
    public byte[] b;
}



//--------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------


public static class Search {

    static
    string text;


    public static void Find() {
        string s = "";
        text = GameLog.data.String2Find_Input.text;
        for (int i = 0; i < GameLog.messages.Count; i++) {
            if (GameLog.messages[i].message.Contains(text)) {
                s += i.ToString("X") + " / ";
            }
        }
        GameLog.data.page2Text.text = s;
    }
}