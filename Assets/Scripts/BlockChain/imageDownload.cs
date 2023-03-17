using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using BlockChain;
namespace BlockChain
{
    public class imageDownload : MonoBehaviour
    {
        public RawImage img;

        // Start is called before the first frame update
        void getImg(string url)
        {
            StartCoroutine(GetTexture(url, img));
        }


        IEnumerator GetTexture(string _url, RawImage img)
        {
            var url = $"{_url}";
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                img.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            }
        }
    }
}
