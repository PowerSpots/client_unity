using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gankx
{
    public class UIEditorPrefs
    {
        public static bool minimalisticLook
        {
            get { return GetBool("Gankx.UI Minimalistic", false); }
            set { SetBool("Gankx.UI Minimalistic", value); }
        }

        public static string searchField
        {
            get { return GetString("Gankx.UI Search", null); }
            set { SetString("Gankx.UI", value); }
        }

        public static string currentPath
        {
            get { return GetString("Gankx.UI Path", "Assets/"); }
            set { SetString("Gankx.UI Path", value); }
        }

        public static void SetBool(string name, bool val)
        {
            EditorPrefs.SetBool(name, val);
        }

        public static void SetInt(string name, int val)
        {
            EditorPrefs.SetInt(name, val);
        }

        public static void SetFloat(string name, float val)
        {
            EditorPrefs.SetFloat(name, val);
        }

        public static void SetString(string name, string val)
        {
            EditorPrefs.SetString(name, val);
        }

        public static void SetColor(string name, Color c)
        {
            SetString(name, c.r + " " + c.g + " " + c.b + " " + c.a);
        }

        public static void SetEnum(string name, Enum val)
        {
            SetString(name, val.ToString());
        }

        public static void Set(string name, Object obj)
        {
            if (obj == null)
            {
                EditorPrefs.DeleteKey(name);
            }
            else
            {
                var path = AssetDatabase.GetAssetPath(obj);

                if (!string.IsNullOrEmpty(path))
                {
                    EditorPrefs.SetString(name, path);
                }
                else
                {
                    EditorPrefs.SetString(name, obj.GetInstanceID().ToString());
                }
            }
        }

        public static bool GetBool(string name, bool defaultValue)
        {
            return EditorPrefs.GetBool(name, defaultValue);
        }

        public static int GetInt(string name, int defaultValue)
        {
            return EditorPrefs.GetInt(name, defaultValue);
        }

        public static float GetFloat(string name, float defaultValue)
        {
            return EditorPrefs.GetFloat(name, defaultValue);
        }

        public static string GetString(string name, string defaultValue)
        {
            return EditorPrefs.GetString(name, defaultValue);
        }

        public static Color GetColor(string name, Color c)
        {
            var strVal = GetString(name, c.r + " " + c.g + " " + c.b + " " + c.a);
            var parts = strVal.Split(' ');

            if (parts.Length == 4)
            {
                float.TryParse(parts[0], out c.r);
                float.TryParse(parts[1], out c.g);
                float.TryParse(parts[2], out c.b);
                float.TryParse(parts[3], out c.a);
            }

            return c;
        }

        public static T GetEnum<T>(string name, T defaultValue)
        {
            var val = GetString(name, defaultValue.ToString());
            var names = Enum.GetNames(typeof(T));
            var values = Enum.GetValues(typeof(T));

            for (var i = 0; i < names.Length; ++i)
            {
                if (names[i] == val)
                {
                    return (T) values.GetValue(i);
                }
            }

            return defaultValue;
        }

        public static T Get<T>(string name, T defaultValue) where T : Object
        {
            var path = EditorPrefs.GetString(name);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var retVal = UIEditorTools.LoadAsset<T>(path);

            if (retVal == null)
            {
                int id;
                if (int.TryParse(path, out id))
                {
                    return EditorUtility.InstanceIDToObject(id) as T;
                }
            }

            return retVal;
        }
    }
}