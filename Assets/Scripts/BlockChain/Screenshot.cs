using System.Collections;
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
        private void Awake()
        {
            instance = this;
        }

        private byte[] bytesImage;
        private bool isPostRenderComplete; // OnPostRender �޼��尡 �Ϸ�Ǿ����� ����

        private IEnumerator TakeScreenshot(int _width, int _height, string _name)
        {
            string fileName = _name + ".png";
            string folderPath = Application.persistentDataPath + "/screenshots/";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filePath = folderPath + fileName;
            ScreenCapture.CaptureScreenshot(filePath);

            while (!File.Exists(filePath)) // ��ũ���� ������ ������ ������ ���
            {
                yield return null;
            }

            var bytesImage = File.ReadAllBytes(filePath);
            //OnCompleteCapture(bytesImage);

            // ���� ����
            File.Delete(filePath);
        }

        public static byte[] GetScreenshot(int _width, int _height, string _name)
        {
            instance.StartCoroutine(instance.TakeScreenshot(_width, _height, _name));
            return instance.bytesImage;
        }
    }
}