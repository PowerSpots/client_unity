using UnityEngine;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace Gankx.PAL
{
    public class UniClipboard
    {
        private static IBoard MyBoard;

        private static IBoard board
        {
            get
            {
                if (MyBoard == null)
                {
#if UNITY_EDITOR || UNITY_STANDALONE
                    MyBoard = new EditorBoard();
#elif UNITY_ANDROID
                _board = new AndroidBoard();
#elif UNITY_IOS
                _board = new IOSBoard();
#endif
                }

                return MyBoard;
            }
        }

        public static void SetText(string str)
        {
            board.SetText(str);
        }

        public static string GetText()
        {
            return board.GetText();
        }
    }

    internal interface IBoard
    {
        void SetText(string str);
        string GetText();
    }

    internal class EditorBoard : IBoard
    {
        public void SetText(string str)
        {
            GUIUtility.systemCopyBuffer = str;
        }

        public string GetText()
        {
            return GUIUtility.systemCopyBuffer;
        }
    }


#if UNITY_IOS
    internal class IOSBoard : IBoard
    {
        public void SetText(string str)
        {
            if (Application.platform != RuntimePlatform.OSXEditor)
            {
                SetText_(str);
            }
        }

        public string GetText()
        {
            return GetText_();
        }

        [DllImport("__Internal")]
        private static extern void SetText_(string str);

        [DllImport("__Internal")]
        private static extern string GetText_();
    }
#endif


#if UNITY_ANDROID
    internal class AndroidBoard : IBoard
    {
        private readonly AndroidJavaClass myCb = new AndroidJavaClass("jp.ne.donuts.uniclipboard.Clipboard");

        public void SetText(string str)
        {
            myCb.CallStatic("setText", str);
        }

        public string GetText()
        {
            return myCb.CallStatic<string>("getText");
        }
    }

#endif
}