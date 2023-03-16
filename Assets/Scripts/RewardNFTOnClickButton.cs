using System.Collections;
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
using BlockChain;
namespace BlockChain
{

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

    [System.Serializable]
    public class MintParam
    {
        public string from;
        public string ipfs;
        public string name;
        public string description;
        public string nftType;

    }


    [System.Serializable]
    public class SaveData
    {
        public SaveData(string _name, string _description, string _image)
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

    public class RewardNFTOnClickButton : MonoBehaviour
    {
        async public void mintOnClickButton()
        {
            var name = "rewardNFTName";
            var description = "rewardNFTDescription";

            // 1. 이미지 스크린샷 후 ipfs 업로드 cid 반환 

            byte[] _imageBytes = Screenshot.TakeScreenshot_(1920, 1080, name);

            var reqUploadImage = new req_upload();
            reqUploadImage.name = "imgfile";
            var json = JsonConvert.SerializeObject(reqUploadImage);

            StartCoroutine(ImageUpload("http://localhost:5000/ipfsImage", _imageBytes, json, name, (result) =>
            {
                var responseResult = JsonConvert.DeserializeObject<res_upload>(result);
                Debug.Log("result: " + responseResult);
                Debug.Log("image cid : " + responseResult.cid);

            // 2. 이미지 cid를 이용하여 metadata.json 생성한 후 cid 받아오기
            SaveData metaData = new SaveData(name, description, "ipfs://" + responseResult.cid);

                SaveSystem.Save(metaData, name);

                byte[] _fileBytes = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/saves/" + name + ".json");

                var reqUploadFile = new req_upload();
                reqUploadFile.name = "jsonFile";
                var json2 = JsonConvert.SerializeObject(reqUploadFile);

                StartCoroutine(JsonUpload("http://localhost:5000/ipfsJson", _fileBytes, json2, name, (cidResult) =>
                {
                    Debug.Log("last json cid check: " + cidResult);

                    MintParam mintParam = new MintParam
                    {
                        from = LoginManager.Instance.GetAddr(),
                        ipfs = cidResult,
                        name = name,
                        description = description,
                        nftType = "2"
                    };

                    Application.OpenURL($"http://localhost:3000/mint/{mintParam.ipfs}/{mintParam.name}/{mintParam.description}/{mintParam.nftType}/{mintParam.from}");
                }
                ));

            }));
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
                request.Dispose();
            }

        }

        IEnumerator ImageUpload(string URL, byte[] bytes, string data, string _name, System.Action<string> OnCompleteUpload)
        {
            var byteArr = Encoding.UTF8.GetBytes(data);
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormFileSection("imgfile", bytes, _name + ".png", "image/png"));

            UnityWebRequest webRequest = UnityWebRequest.Post(URL, formData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return webRequest.SendWebRequest();

            var result = webRequest.downloadHandler.text;
            OnCompleteUpload(result);
            webRequest.Dispose();
        }

        /*    // Coroutine function for uploading a JSON file to a server using UnityWebRequest
            IEnumerator JsonUpload(string URL, byte[] bytes, string data, string _name, System.Action<string> OnCompleteUpload)
            {
                // Convert additional data string to byte array using UTF8 encoding
                var byteArr = Encoding.UTF8.GetBytes(data);

                // Create list of multipart form sections with JSON file data as one of the sections
                List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
                formData.Add(new MultipartFormFileSection("jsonfile", bytes, _name + ".json", " application/json"));

                // Create UnityWebRequest object with POST method and URL and multipart form data
                UnityWebRequest webRequest = UnityWebRequest.Post(URL, formData);

                // Set download handler to buffer
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                // Send request and wait for response
                yield return webRequest.SendWebRequest();

                // Read result as text
                var result = webRequest.downloadHandler.text;

                // Deserialize JSON response using JsonConvert.DeserializeObject() method
                res_upload responseResult = JsonConvert.DeserializeObject<res_upload>(result);

                // Log response and extract CID value from response
                Debug.Log(result);
                Debug.Log("cid : " + responseResult.cid);

                string cidResult = responseResult.cid;

                // Pass CID value to callback function using OnCompleteUpload() method
                OnCompleteUpload(cidResult);

                // Dispose of UnityWebRequest object
                webRequest.Dispose();
            }*/

        IEnumerator JsonUpload(string URL, byte[] bytes, string data, string _name, System.Action<string> OnCompleteUpload)
        {
            var byteArr = Encoding.UTF8.GetBytes(data);
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormFileSection("jsonfile", bytes, _name + ".json", " application/json"));

            UnityWebRequest webRequest = UnityWebRequest.Post(URL, formData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            try
            {
                yield return webRequest.SendWebRequest();
                var result = webRequest.downloadHandler.text;
                res_upload responseResult = JsonConvert.DeserializeObject<res_upload>(result);
                Debug.Log(result);
                Debug.Log("cid : " + responseResult.cid);
                string cidResult = responseResult.cid;
                OnCompleteUpload(cidResult);
            }
            finally
            {
                if (webRequest != null)
                {
                    webRequest.Dispose();
                }
            }
        }

    }
}