using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public class TerminalExecution : MonoBehaviour
{
    private Process externalProcess;
    private void Awake()
    {
        // 프로젝트 디렉토리의 상대 경로를 설정합니다.
        string projectPath = Application.streamingAssetsPath;
        string exePath = Path.Combine(projectPath, "caver-win.exe");
        externalProcess = Process.Start(exePath);

        // Unity 종료 시 이벤트 핸들러를 등록합니다.
        Application.wantsToQuit += WantsToQuit;
    }

    void OnDestroy()
    {
        // 객체가 파괴될 때 Unity 종료 이벤트 핸들러를 제거합니다.
        Application.wantsToQuit -= WantsToQuit;
    }
    bool WantsToQuit()
    {
        StopProcess();
        return true;
    }

    void StopProcess()
    {
        // 프로세스를 종료합니다.
        if (externalProcess != null && !externalProcess.HasExited)
        {
            externalProcess.Kill();
            externalProcess.Dispose();
            //// 주 창을 닫아 종료를 유도합니다.
            //externalProcess.CloseMainWindow();
            //// 완전히 종료될 때까지 대기합니다.
            //externalProcess.WaitForExit();
            //externalProcess.Dispose();
        }
    }

    void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        // 출력을 Unity 콘솔 창과 Node.js 터미널에 표시합니다.
        string output = e.Data;
        System.Diagnostics.Debug.WriteLine(output);
        UnityEngine.Debug.Log(output); // Unity 콘솔 창에 출력
    }

    void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        // 에러 출력을 Unity 콘솔 창과 Node.js 터미널에 표시합니다.
        string error = e.Data;
        System.Diagnostics.Debug.WriteLine(error);
        UnityEngine.Debug.LogError(error); // Unity 콘솔 창에 출력 (에러 로그)
    }
}
