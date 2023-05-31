using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public class TerminalExecution2 : MonoBehaviour
{
    private Process process;

    private void Start()
    {
        // ������ ��ɾ�� ���ڸ� �����մϴ�.
        string command = "node";
        string arguments = "index";

        // ���ο� ���μ����� �����մϴ�.
        process = new Process();

        // ������Ʈ ���丮�� ��� ��θ� �����մϴ�.
        string projectPath = Application.dataPath;
        string backendPath = Path.Combine(projectPath, "BlockchainServer", "backend");

        // ���μ��� ������ �����մϴ�.
        process.StartInfo.FileName = command;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.WorkingDirectory = backendPath;

        // ����� ������ ���θ� �����մϴ�. (�ʿ信 ���� ����)
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;

        // ���μ��� �̺�Ʈ �ڵ鷯�� ����մϴ�.
        process.OutputDataReceived += OnOutputDataReceived;
        process.ErrorDataReceived += OnErrorDataReceived;

        // ���μ����� �����մϴ�.
        process.Start();

        // �񵿱������� ����� �н��ϴ�.
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        // Unity ���� �� �̺�Ʈ �ڵ鷯�� ����մϴ�.
        Application.wantsToQuit += WantsToQuit;
    }

    void OnDestroy()
    {
        // ��ü�� �ı��� �� ���μ��� �̺�Ʈ �ڵ鷯�� �����մϴ�.
        process.OutputDataReceived -= OnOutputDataReceived;
        process.ErrorDataReceived -= OnErrorDataReceived;
        // ��ü�� �ı��� �� Unity ���� �̺�Ʈ �ڵ鷯�� �����մϴ�.
        Application.wantsToQuit -= WantsToQuit;
    }
    bool WantsToQuit()
    {
        StopProcess();
        return true;
    }

    void StopProcess()
    {
        // ���μ����� �����մϴ�.
        if (process != null && !process.HasExited)
        {
            process.CloseMainWindow();
            process.WaitForExit();
        }
    }

    void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        // ����� Unity �ܼ� â�� ǥ���մϴ�.
        string output = e.Data;
        UnityEngine.Debug.Log(output); // Unity �ܼ� â�� ���
    }

    void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        // ���� ����� Unity �ܼ� â�� ǥ���մϴ�.
        string error = e.Data;
        UnityEngine.Debug.LogError(error); // Unity �ܼ� â�� ��� (���� �α�)
    }
}