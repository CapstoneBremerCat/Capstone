using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Data;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;	// UnityWebRequest����� ���ؼ� �����ش�.
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
using BlockChain;
namespace BlockChain
{
    [System.Serializable]
    public class MessageParam
    {
        public string message;

    }

    public class loginPacket
    {
        public int errorno;
    }

    public class login_res_upload : loginPacket
    {
        public string code;
        public string message;
        public string addr;
    }

    public class LoginManager : MonoBehaviour
    {
        public static string loginAddr;
        #region instance
        private static LoginManager instance = null;
        public static LoginManager Instance { get { return instance; } }

        private void Awake()
        {
            // Scene�� �̹� �ν��Ͻ��� ���� �ϴ��� Ȯ�� �� ó��
            if (instance)
            {
                Destroy(this.gameObject);
                return;
            }
            // instance�� ���� ������Ʈ�� �����
            instance = this;

            // Scene �̵� �� ���� ���� �ʵ��� ó��
            DontDestroyOnLoad(this.gameObject);

            loginAddr = "---";
        }
        #endregion

        /*
            // Start is called before the first frame update
            void Start()
            {

            }*/


        public void SetAddr(string addr)
        {
            loginAddr = addr;
        }

        public string GetAddr()
        {
            return loginAddr;
        }

        public void LoginOnClickButton()
        {

            Application.OpenURL("http://localhost:3000/");
            //Application.OpenURL("https://aeong-psi.vercel.app/");

        }


        public void LoginCheckOnClickButton()
        {

            MessageParam param = new MessageParam
            {
                message = "login addr request",
            };

            //Convert JsonString
            string json = JsonUtility.ToJson(param);

            //request Post
            StartCoroutine(Upload("http://localhost:5000/login-result-unity", json));

        }

        IEnumerator Upload(string URL, string json)
        {
            using (UnityWebRequest request = UnityWebRequest.Post(URL, json))
            {
                try
                {
                    byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
                    request.uploadHandler.Dispose();
                    request.uploadHandler = new UploadHandlerRaw(jsonToSend);
                    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");

                    yield return request.SendWebRequest();
                    
                    //if (request.isNetworkError || request.isHttpError)
                    if (request.result == UnityWebRequest.Result.ConnectionError)
                    {
                        Debug.Log(request.error);
                    }
                    else
                    {
                        var result = request.downloadHandler.text;
                        Debug.Log(result);
                        login_res_upload responseResult = JsonConvert.DeserializeObject<login_res_upload>(result);
                        Debug.Log("addr : " + responseResult.addr);
                        SetAddr(responseResult.addr);
                        //string temp = LoginManager.Instance.GetAddr();
                        //Debug.Log("loginAddr : " + temp);
                        //resourceText.text = temp;
                        yield return new WaitForSeconds(2.0f);
                        NFTManager.Instance.InitNFT();
                    }
                }
                finally
                {
                    if (request != null)
                    {
                        request.Dispose();
                    }
                }
            }

        }

        IEnumerator WaitForIt()
        {
            yield return new WaitForSeconds(10.0f);
        }
    }
}