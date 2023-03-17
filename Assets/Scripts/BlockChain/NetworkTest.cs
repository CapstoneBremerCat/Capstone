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

[System.Serializable]
public class Param
{
    public string from;
    public string to;
    public string tokenID;
    public string balanceOf;

}

[System.Serializable]
public class MintParam
{
    public string from;
    public string ipfs;
    public string name;
    public string ticketType;

}


[System.Serializable]
public class SaveData
{
    public SaveData(string _name, string _description, string _image )
    {
        name = _name;
        description = _description;
        image = _image;
    }

    public string name;
    public string description;
    public string image;
}
public static class SaveSystem
{
    private static string SavePath => Application.persistentDataPath + "/saves/";

    public static void Save(SaveData saveData, string saveFileName)
    {
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }

        string saveJson = JsonUtility.ToJson(saveData);
        
        string saveFilePath = SavePath + saveFileName + ".json";
        File.WriteAllText(saveFilePath, saveJson);
        Debug.Log("Save Success: " + saveFilePath);
    }

    public static SaveData Load(string saveFileName)
    {
        string saveFilePath = SavePath + saveFileName + ".json";

        if (!File.Exists(saveFilePath))
        {
            Debug.LogError("No such saveFile exists");
            return null;
        }

        string saveFile = File.ReadAllText(saveFilePath);
        SaveData saveData = JsonUtility.FromJson<SaveData>(saveFile);
        return saveData;
    }
}

public class NetworkTest : MonoBehaviour
{
    public static string cidTemp = null;

    public void TestOnClickButton()
    {
        Param param = new Param
        {
            from = "Test"
        };

        //Convert JsonString
        string json = JsonUtility.ToJson(param);

        //request Post
        StartCoroutine(Upload("http://localhost:3000/test", json));
    }

    public void OwnerOfOnClickButton()
    {
        Param param = new Param
        {
            tokenID = "0",

        };

        //Convert JsonString
        string json = JsonUtility.ToJson(param);

        //request Post
        //StartCoroutine(Upload("http://localhost:3000/ownerOf", json));
        StartCoroutine(Upload("http://localhost:3000/test1", json));

    }


    public void SafeTransferFromOnClickButton()
    {
        Param param = new Param
        {
            from = "0xd026F9247E982f087F8CcB4FD334C7d78039c37A",
            to = "0x6ef333308269711b6ACf61C7a239A626490c65F5",
            tokenID = "0",
        };

 *//*       Param param = new Param
        {
            from = "0x6ef333308269711b6ACf61C7a239A626490c65F5",
            to = "0xd026F9247E982f087F8CcB4FD334C7d78039c37A",
            tokenID = "2",
        };*//*

        //Convert JsonString
        string json = JsonUtility.ToJson(param);

        //request Post
        //StartCoroutine(Upload("http://localhost:3000/safeTransferFrom", json));
        StartCoroutine(Upload("http://localhost:3000/test2", json));

    }


    public void BalanceOfOnClickButton()
    {
        Param param = new Param
        {
            from = "0x6ef333308269711b6ACf61C7a239A626490c65F5",
        };

        //Convert JsonString
        string json = JsonUtility.ToJson(param);

        //request Post
        StartCoroutine(Upload("http://localhost:3000/balanceOf", json));
    }

    public void LoginOnClickButton()
    {

        Application.OpenURL("http://localhost:3000/");

        *//*
        Param param = new Param
        {
            from = "여기에 키를 받아야함",
        };

        //Convert JsonString
        string json = JsonUtility.ToJson(param);

        //request Post
        StartCoroutine(Upload("http://localhost:3000/login", json));
        *//*
    }

    public class packet
    {
        public int errorno;
    }

    public class req_upload
    {
        public string name;
    }

    public class res_upload : packet
    {
        public string code;
        public string message;
        public string cid;
    }



async public void RewardNFTOnClickButton2()
    {
        // 이미지 스크린샷 후 ipfs 업로드 cid 반환 
        *//* 
        byte[] _imageBytes = Screenshot.TakeScreenshot_(1920, 1080);

        var reqUploadImage = new req_upload();
        reqUploadImage.name = "imgfile";
        var json = JsonConvert.SerializeObject(reqUploadImage);

        StartCoroutine(ImageUpload("http://localhost:3000/ipfsImage", _imageBytes, json, (result) =>
        {
            //var responseResult = JsonConvert.DeserializeObject<res_upload>(result);
            Debug.Log(result);
            //Debug.Log("성공여부 : " + responseResult.message);
            //Debug.Log("cid : " + responseResult.cid);
        }));

        */

        /*
                SaveData metaData = new SaveData("test1", "test1 metadatafile", "ipfs://"+ "QmcHKBovW4hViJkqcjauuPcHFazQKgC9NDk6HEGx9LYxnH");

                SaveSystem.Save(metaData, "test");

                byte[] _fileBytes = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/saves" + "/test.json");

                var reqUploadFile = new req_upload();
                reqUploadFile.name = "jsonFile";
                var json2 = JsonConvert.SerializeObject(reqUploadFile);

                StartCoroutine(JsonUpload("http://localhost:3000/ipfsJson", _fileBytes, json2, (cidResult) =>
                {
                    Debug.Log("last check: ");
                    Debug.Log(cidResult);

                   
                }
                ));
                *//*

        MintParam mintParam = new MintParam
        {
            from = "0x6ef333308269711b6ACf61C7a239A626490c65F5",
            ipfs = "ipfs://" + "QmXVdyTE4J5i7Qiw4iLsB6jStkRyjK5bi8ZLEPBM2V7gXEt",
            name = "test1",
            ticketType = "1"
        };

        //Convert JsonString
        string json = JsonUtility.ToJson(mintParam);

        Debug.Log("ok?: ");
        //Upload("http://localhost:3000/mintNFT", json);
        StartCoroutine(Upload("http://localhost:3000/mintNFT", json));
        Debug.Log("Done: ");

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
                Debug.Log(request.downloadHandler.text);
            }

        }

    }

    IEnumerator ImageUpload(string URL, byte [] bytes, string data, System.Action<string> OnCompleteUpload)
    {
        var byteArr = Encoding.UTF8.GetBytes(data);
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("imgfile", bytes, "test.png", "image/png"));

        UnityWebRequest webRequest = UnityWebRequest.Post(URL, formData);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();

        var result = webRequest.downloadHandler.text;
        OnCompleteUpload(result);
    }

    IEnumerator JsonUpload(string URL, byte[] bytes, string data, System.Action<string> OnCompleteUpload)
    {
        var byteArr = Encoding.UTF8.GetBytes(data);
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("jsonfile", bytes, "test.json", " application/json"));

        UnityWebRequest webRequest = UnityWebRequest.Post(URL, formData);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();

        var result = webRequest.downloadHandler.text;

        res_upload responseResult = JsonConvert.DeserializeObject<res_upload>(result);
        Debug.Log(result);
        //Debug.Log("성공여부 : " + responseResult.message);
        Debug.Log("cid : " + responseResult.cid);

        string cidResult = responseResult.cid;

        OnCompleteUpload(cidResult);

    }

}
*/