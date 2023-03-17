/*using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Data;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;	// UnityWebRequest사용을 위해서 적어준다.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;


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



public class Login : MonoBehaviour
{
    public TextMeshProUGUI resourceText;



    public void LoginOnClickButton()
    {

        Application.OpenURL("http://localhost:3000/");

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
        StartCoroutine(Upload("http://localhost:5000/resultUnity", json));

    }


    IEnumerator Upload(string URL, string json)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(URL, json))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                var result = request.downloadHandler.text;
                Debug.Log(result);
                login_res_upload responseResult = JsonConvert.DeserializeObject<login_res_upload>(result);
                Debug.Log("addr : " + responseResult.addr);
                LoginManager.Instance.SetAddr(responseResult.addr);
                //string temp = LoginManager.Instance.GetAddr();
                //Debug.Log("loginAddr : " + temp);
                //resourceText.text = temp;

                SceneManager.LoadScene("main");


            }

        }

    }

    IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(10.0f);
    }
}
*/