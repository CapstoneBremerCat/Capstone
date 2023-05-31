using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BlockChain;

public class TerminalExecution : MonoBehaviour
{
    private static Process externalProcess;
    public void Awake()
    {
        if (externalProcess == null)
        {
            // ������Ʈ ���丮�� ��� ��θ� �����մϴ�.
            string projectPath = Application.streamingAssetsPath;
            string exePath = Path.Combine(projectPath, "caver.exe");
             externalProcess = Process.Start(exePath);

            // Unity ���� �� �̺�Ʈ �ڵ鷯�� ����մϴ�.
            Application.wantsToQuit += WantsToQuit;
        }

    }
    public void OnApplicationQuit()
    {
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
        if (externalProcess != null && !externalProcess.HasExited)
        {
            externalProcess.Kill();
            externalProcess.Dispose();
            //// �� â�� �ݾ� ���Ḧ �����մϴ�.
            //externalProcess.CloseMainWindow();
            //// ������ ����� ������ ����մϴ�.
            //externalProcess.WaitForExit();
            //externalProcess.Dispose();
        }
    }

/*    void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
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
    }*/
}
