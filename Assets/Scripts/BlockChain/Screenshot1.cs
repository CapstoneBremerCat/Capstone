/*using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Game;
using BlockChain;
namespace BlockChain
{
    public class Screenshot : MonoBehaviour
    {

        private static Screenshot instance;
        private byte[] bytesImage;
        private Camera GCamera;
        private bool takeScreenshot;
        private bool isPostRenderComplete; // OnPostRender �޼��尡 �Ϸ�Ǿ����� ����

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
                GameManager.Instance.SetPause(true);
                takeScreenshot = false;
                RenderTexture renderTexture = GCamera.targetTexture;

                Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
                Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
                renderResult.ReadPixels(rect, 0, 0);

                byte[] byteArray = renderResult.EncodeToPNG();
                string folderPath = Application.persistentDataPath + "/saves/";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string filePath = folderPath + imageName + ".png";
                System.IO.File.WriteAllBytes(filePath, byteArray);
                Debug.Log(Application.persistentDataPath + "/saves/" + imageName + ".png");
                Debug.Log("Saved Screenshot");

                RenderTexture.ReleaseTemporary(renderTexture);
                GCamera.targetTexture = null;
                bytesImage = System.IO.File.ReadAllBytes(filePath);
                GameManager.Instance.SetPause(false);
                isPostRenderComplete = true; // OnPostRender �޼��尡 �Ϸ���� ǥ��
            }
        }
        private IEnumerator TakeScreenshot(int _width, int _height, string _name)
        {
            isPostRenderComplete = false; // OnPostRender �޼��尡 ���� �Ϸ���� �ʾ����� ǥ��
            GCamera.targetTexture = RenderTexture.GetTemporary(_width, _height, 16);
            imageName = _name;
            takeScreenshot = true;


            while (!isPostRenderComplete) // OnPostRender �޼��尡 �Ϸ�� ������ ���
            {
                yield return null;
            }
        }

        public static byte[] TakeScreenshot_(int _width, int _height, string _name)
        {
            // ������� ����ǵ��� �ڷ�ƾ���� ����
            instance.StartCoroutine(instance.TakeScreenshot(_width, _height, _name));

            return instance.bytesImage;
        }
    }
}*/