using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;


namespace RTS
{
    public static class ComponentsNameToConstEditorTool
    {
        public const string CommontCodePath = "Assets/Scripts/RTS/Generate/";

        [MenuItem("Tools/ECS/Generate ECS Component Name")]
        static void Generate()
        {
            Type[] allTypes = RTS.ReflectionUtils.GetChildTypes(typeof(RTS.ComponentBase));

            List<Type> userTypes = new List<Type>();

            foreach (var item in allTypes)
            {
                //if (item == typeof(RecordComponent<>))
                //    continue;
                //if (item == typeof(SingletonComponent))
                //    continue;
                //if (item == typeof(MomentComponentBase))
                //    continue;
                //if (item == typeof(MomentSingletonComponent))
                //    continue;
                //if (item == typeof(ViewComponent))
                //    continue;

                userTypes.Add(item);
            }

            string code = CreateCode(0, userTypes.ToArray());

            //FileUtils.CreateTextFile();

            ResourceIOTool.WriteStringByFile(CommontCodePath + "ComponentType.cs", code);

            AssetDatabase.Refresh();
        }


        private static string CreateCode(int startID, Type[] componentTypes)
        {
            string code = "using UnityEngine;\n\n";

            code += "//自动生成请勿更改\n\n";
            code += "namespace RTS\n";
            code += "{\n";
            code += "public partial class ComponentType : ComponentTypeBase\n";
            code += "{\n";


            List<string> tempNames = new List<string>();
            for (int i = 0; i < componentTypes.Length; i++)
            {
                Type t = componentTypes[i];
                string name = t.Name;
                if (t.IsGenericType)
                {
                    name = name.Remove(name.Length - 2);
                    Type[] tempTypes = t.GetGenericArguments();
                    for (int j = 0; j < tempTypes.Length; j++)
                    {
                        name += "_" + tempTypes[j].Name;
                    }
                }
                code += "\tpublic const int " + name + " = " + (startID + i) + ";\n";
                tempNames.Add(name);
            }

            code += "\tpublic override int Count()\n";
            code += "\t{\n";
            code += "\t\treturn " + componentTypes.Length + ";\n";
            code += "\t}\n\n";

            code += "\n\n";
            code += "\tpublic override int GetComponentIndex(string name) \n";
            code += "\t{\n";
            code += "\t\tswitch (name) \n";
            code += "\t\t{\n\n";
            foreach (var item in tempNames)
            {
                code += "\t\t\t case \"" + item + "\" : \n";
                code += "\t\t\t\t return " + item + " ; \n";
            }


            code += "\t\t}\n";
            code += "\t\tDebug.Log(\"未找到对应的组件 ：\" + name); \n";
            code += "\t\treturn -1 ; \n";
            code += "\t}\n";

            code += "}\n";
            code += "}\n";  // namespace

            return code;
        }
    }
}