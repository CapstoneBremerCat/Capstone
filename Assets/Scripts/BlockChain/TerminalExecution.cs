using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public class TerminalExecution : MonoBehaviour
{
    private Process process;
    public void Awake()
    {
        if (process == null)
        {
            // 실행할 명령어와 인자를 설정합니다.
            string command = "node";
            string arguments = "index";

            // 새로운 프로세스를 생성합니다.
            process = new Process();

            // 프로젝트 디렉토리의 상대 경로를 설정합니다.
            //string projectPath = Application.dataPath;
            string backendPath = Path.Combine(Application.streamingAssetsPath, "backend");

            // 프로세스 정보를 설정합니다.
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WorkingDirectory = backendPath;

            // 출력을 받을지 여부를 설정합니다. (필요에 따라 설정)
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // 프로세스 이벤트 핸들러를 등록합니다.
            process.OutputDataReceived += OnOutputDataReceived;
            process.ErrorDataReceived += OnErrorDataReceived;

            // 프로세스를 시작합니다.
            process.Start();

            // 비동기적으로 출력을 읽습니다.
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Unity 종료 시 이벤트 핸들러를 등록합니다.
            Application.wantsToQuit += WantsToQuit;
        }

    }

    public void OnApplicationQuit()
    {
        process.OutputDataReceived -= OnOutputDataReceived;
        process.ErrorDataReceived -= OnErrorDataReceived;
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
        if (process != null && !process.HasExited)
        {
            process.Kill();
            process.Dispose();
        }
    }
    void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        // 출력을 Unity 콘솔 창에 표시합니다.
        string output = e.Data;
        UnityEngine.Debug.Log(output); // Unity 콘솔 창에 출력
    }
    void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        // 에러 출력을 Unity 콘솔 창에 표시합니다.
        string error = e.Data;
        UnityEngine.Debug.LogError(error); // Unity 콘솔 창에 출력 (에러 로그)
    }
}