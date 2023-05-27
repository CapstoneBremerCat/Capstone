using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public class TerminalExecution : MonoBehaviour
{
    private Process process;
    async void Awake()
    {
        // ������ ��ɾ�� ���ڸ� �����մϴ�.
        string command = "node";
        string arguments = "index";

        // ���ο� ���μ����� �����մϴ�.
        process = new Process();

        // ������Ʈ ���丮�� ��� ��θ� �����մϴ�.
        string projectPath = Application.dataPath;
        string backendPath = Path.Combine(projectPath, "BlockchainServer", "backend");
        string prontendPath = Path.Combine(projectPath, "BlockchainServer", "frontend");

        // ���μ��� ������ �����մϴ�.
        process.StartInfo.FileName = command;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.WorkingDirectory = backendPath;

        // ����� ������ ���θ� �����մϴ�. (�ʿ信 ���� ����)
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false; // UseShellExecute �Ӽ��� false�� �����մϴ�.

        // ���μ��� �̺�Ʈ �ڵ鷯�� ����մϴ�.
        process.OutputDataReceived += OnOutputDataReceived;
        process.ErrorDataReceived += OnErrorDataReceived;

        // ���μ����� �񵿱������� �����մϴ�.
        await Task.Run(() => process.Start());

        // �񵿱������� ����� �н��ϴ�.
        await Task.Run(() =>
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        });

        // Unity ���� �� �̺�Ʈ �ڵ鷯�� ����մϴ�.
        Application.quitting += StopProcess;
    }

    void OnDestroy()
    {
        // ��ü�� �ı��� �� ���μ��� �̺�Ʈ �ڵ鷯�� �����մϴ�.
        process.OutputDataReceived -= OnOutputDataReceived;
        process.ErrorDataReceived -= OnErrorDataReceived;
        // ��ü�� �ı��� �� Unity ���� �̺�Ʈ �ڵ鷯�� �����մϴ�.
        Application.quitting -= StopProcess;
    }

    void StopProcess()
    {
        // ���μ����� �����մϴ�.
        if (process != null && !process.HasExited)
        {
            // �� â�� �ݾ� ���Ḧ �����մϴ�.
            process.CloseMainWindow();
            //process.Kill();
            process.WaitForExit();
        }
    }

    void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        // ����� Unity �ܼ� â�� Node.js �͹̳ο� ǥ���մϴ�.
        string output = e.Data;
        System.Diagnostics.Debug.WriteLine(output);
        UnityEngine.Debug.Log(output); // Unity �ܼ� â�� ���
    }

    void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        // ���� ����� Unity �ܼ� â�� Node.js �͹̳ο� ǥ���մϴ�.
        string error = e.Data;
        System.Diagnostics.Debug.WriteLine(error);
        UnityEngine.Debug.LogError(error); // Unity �ܼ� â�� ��� (���� �α�)
    }
}
