using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using UnityEngine;

public class GameLog_Core : MonoBehaviour {

    public Text Txt_Title;
    public GameObject TextOBJ;
    public Text MainLogs, logCount;
    public RectTransform Content;

    void Awake() {
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
        if (a == 12) {
            GameLog.CULLING();
            GameLog.setContentRectSize();
            GameLog.setLogCount();

            // Responsive Design :)
            GameLog.Recalculate_RectPos();
            if ((float)Screen.width/(float)Screen.height < 1) {
                Txt_Title.enabled = false;
                logCount.enabled = false;
            } else { Txt_Title.enabled = true; logCount.enabled = true; }

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
    int i;
    public void CLS() {
        for (i = 1; i < GameLog.AllTextOBJ.Count; i++) { Destroy(GameLog.AllTextOBJ[i].gameObject); }
        Content.sizeDelta = new Vector2(Content.sizeDelta.x, 0); Content.anchoredPosition = new Vector2(Content.anchoredPosition.x, 0);
        GameLog.INIT();
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
        RectTrans = data.TextOBJ.GetComponent<RectTransform>();
        data.MainLogs = data.TextOBJ.GetComponent<Text>(); data.MainLogs.text = "";
        ContentRect = data.Content;
        cullingException = -1;
        AllTextOBJ.Clear(); AllTextOBJ.Add(RectTrans);

        //Side
        lineCounter = 0; textIndex = 0; counter = 0; lastIndex = 0; hadBadByte = false; firstCloneCommand = true;
        messages.Clear();
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
    static int lineCounter = 0, textIndex = 0;
    static int counter = 0, lastIndex = 0;
    public static void Core() {
        for (int i = lastIndex; i < messages.Count; i++) {
            // Creating A New Text GameObject
            if (lineCounter > 10)
            {
                if (firstCloneCommand)
                {
                    GameLog_Core.wait = 1;
                    firstCloneCommand = false;
                    return;
                }
                textIndex++;
                cullingException = textIndex;
                //
                //
                //
                var obj = GameObject.Instantiate(data.TextOBJ, data.Content); obj.SetActive(true);
                RectTrans = obj.GetComponent<RectTransform>();
                AllTextOBJ.Add(RectTrans);
                RectTrans.anchoredPosition = new Vector2(RectTrans.anchoredPosition.x, calculateNewRectPos());
                data.MainLogs = obj.GetComponent<Text>();

                data.MainLogs.text = "";
                lineCounter = 0;
                firstCloneCommand = true;
            }
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
        }
        
    }

    //-------------------------------------------------------------

    static int cullingException = -1; static int i;
    public static void CULLING() {
        for (i = 0; i < AllTextOBJ.Count; i++) {
            if (i == cullingException)
                continue;
            if (Mathf.Abs(-AllTextOBJ[i].anchoredPosition.y - ContentRect.anchoredPosition.y) < 3000)
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

    public static void Recalculate_RectPos() {
        float v = 0;
        foreach (RectTransform r in AllTextOBJ) {
            r.anchoredPosition = new Vector2(r.anchoredPosition.x, v);
            v = (r.anchoredPosition.y) - r.sizeDelta.y;
        }
    }

    //-------------------------------------------------------------

    public static void setContentRectSize() {
        var temp = AllTextOBJ[AllTextOBJ.Count - 1];
        ContentRect.sizeDelta = new Vector2(ContentRect.sizeDelta.x, -(temp.anchoredPosition.y) + temp.sizeDelta.y + 80);
    }

    static int logCount = 0;
    public static void setLogCount() {
        if (logCount != messages.Count) {
            logCount = messages.Count;
            data.logCount.text = "LogCount: " + logCount;
        }
    }

    static float calculateNewRectPos() {
        var temp = AllTextOBJ[AllTextOBJ.Count - 2];
        return (temp.anchoredPosition.y) - temp.sizeDelta.y;
    }

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
