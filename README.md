# GameLog
Unity's standard logging system does not support log requests from threads other than the main thread, making it challenging to debug processes that require multi-threaded capabilities, such as multiplayer online games. GameLog is a free tool specifically designed to address this limitation, and it also enables easy debugging on the built version of the program, not just within the editor.

## Usage
* Just drag & drop GameLog-Window prefab into your scene
* After you drop the prefab into the scene, a small icon should appear at the bottom of the game screen. Click on it to enable the log window.
```c#
GameLog.Log("Kiarash Parvizi");
GameLog.Log("Running...", UnityEngine.Color.cyan);
```
---

