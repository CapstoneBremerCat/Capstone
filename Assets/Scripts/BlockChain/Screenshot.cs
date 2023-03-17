using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockChain;
namespace BlockChain
{
    public class Screenshot : MonoBehaviour
    {

        private static Screenshot instance;

        private Camera GCamera;
        private bool takeScreenshot;
        private string imageName;

        private void Awake()
        {
            instance = this;
            GCamera = gameObject.GetComponent<Camera>();
        }

        private void OnPostRender()
        {
            if (takeScreenshot)
            {
                takeScreenshot = false;
                RenderTexture renderTexture = GCamera.targetTexture;

                Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
                Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
                renderResult.ReadPixels(rect, 0, 0);

                byte[] byteArray = renderResult.EncodeToPNG();
                System.IO.File.WriteAllBytes(Application.persistentDataPath + "/saves/" + imageName + ".png", byteArray);
                Debug.Log(Application.persistentDataPath + "/saves/" + imageName + ".png");
                Debug.Log("Saved Screenshot");

                RenderTexture.ReleaseTemporary(renderTexture);
                GCamera.targetTexture = null;
            }
        }

        private void TakeScreenshot(int _width, int _height, string _name)
        {
            GCamera.targetTexture = RenderTexture.GetTemporary(_width, _height, 16);
            imageName = _name;
            takeScreenshot = true;

        }

        public static byte[] TakeScreenshot_(int _width, int _height, string _name)
        {
            instance.TakeScreenshot(_width, _height, _name);

            byte[] bytesImage = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/saves/" + _name + ".png");

            Debug.Log("bytesImage");
            Debug.Log(bytesImage);

            return bytesImage;
        }
    }
}