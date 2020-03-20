using System.Collections.Generic;
using UnityEngine;

namespace Gankx
{
    public class SimplePlayerPrefs
    {
        private static Dictionary<string, object> SimpleCachedPrefs = new Dictionary<string, object>();

        public static void SetInt(string key, int value)
        {
            SetValue(key, value);
            PlayerPrefs.SetInt(key, value);
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            return GetValue(key, defaultValue);
        }

        public static void SetFloat(string key, float value)
        {
            SetValue(key, value);
            PlayerPrefs.SetFloat(key, value);
        }

        public static float GetFloat(string key, float defaultValue = 0.0f)
        {
            return GetValue(key, defaultValue);
        }

        public static void SetString(string key, string value)
        {
            SetValue(key, value);
            PlayerPrefs.SetString(key, value);
        }

        public static string GetString(string key, string defaultValue = "")
        {
            return GetValue(key, defaultValue);
        }

        public static T GetValue<T>(string key, T defaultValue)
        {
            var res = GetValue(key);
            if (res != null)
            {
                return (T) res;
            }

            var itemType = typeof(T);
            if (itemType == typeof(int))
            {
                res = PlayerPrefs.GetInt(key, (int) (object) defaultValue);
            }
            else if (itemType == typeof(float))
            {
                res = PlayerPrefs.GetFloat(key, (float) (object) defaultValue);
            }
            else if (itemType == typeof(string))
            {
                res = PlayerPrefs.GetString(key, (string) (object) defaultValue);
            }

            SetValue(key, res);
            return (T) res;
        }

        private static void SetValue(string key, object value)
        {
            if (SimpleCachedPrefs.ContainsKey(key))
            {
                SimpleCachedPrefs[key] = value;
            }
            else
            {
                SimpleCachedPrefs.Add(key, value);
            }
        }

        private static object GetValue(string key)
        {
            if (SimpleCachedPrefs.ContainsKey(key))
            {
                return SimpleCachedPrefs[key];
            }

            return null;
        }

        public static void DeleteKey(string key)
        {
            if (SimpleCachedPrefs.ContainsKey(key))
            {
                SimpleCachedPrefs.Remove(key);
            }

            PlayerPrefs.DeleteKey(key);
        }

        public static bool HasKey(string key)
        {
            if (SimpleCachedPrefs.ContainsKey(key))
            {
                return true;
            }

            return PlayerPrefs.HasKey(key);
        }
    }
}