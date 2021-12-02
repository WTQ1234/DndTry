﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;
using RegexHelper;
namespace ET
{
    public struct CellInfo
    {
        public string Type;
        public string Name;
        public string Desc;
    }

    public class ExcelMD5Info
    {
        public Dictionary<string, string> fileMD5 = new Dictionary<string, string>();

        public string Get(string fileName)
        {
            string md5 = "";
            this.fileMD5.TryGetValue(fileName, out md5);
            return md5;
        }

        public void Add(string fileName, string md5)
        {
            this.fileMD5[fileName] = md5;
        }
    }

    public class ExcelExporterEditor: EditorWindow
    {
        [MenuItem("Tools/导出配置")]
        private static void ShowWindow()
        {
            GetWindow(typeof (ExcelExporterEditor));
        }

        private void OnEnable()
        {
            ExcelPath = Application.dataPath + "/Other/Excel";
        }

        private string ExcelPath = "/Other/Excel";
        private string clientPath = "./Assets/Resources/Config";
        private string clientClassPath = @"./Assets/Scripts/Config/Config_ET";

        private bool isClient;

        private ExcelMD5Info md5Info;

        private void OnGUI()
        {
            try
            {
                if (GUILayout.Button("导出客户端配置"))
                {
                    this.isClient = true;
                    Log.Debug($"{ExcelPath}");

                    // 删除类下的文件
                    DeleteDir(clientPath);
                    DeleteDir(clientClassPath);

                    ExportAll(clientPath);
                    ExportAllClass(clientClassPath, "namespace ET\n{\n");//using MongoDB.Bson.Serialization.Attributes;\n\n

                    Log.Info($"导出客户端配置完成!");
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        // 导出所有类
        private void ExportAllClass(string exportDir, string csHead)
        {
            foreach (string filePath in Directory.GetFiles(ExcelPath))
            {
                if (Path.GetExtension(filePath) != ".xlsx")
                {
                    continue;
                }

                if (Path.GetFileName(filePath).StartsWith("~"))
                {
                    continue;
                }

                ExportClass(filePath, exportDir, csHead);
                Log.Info($"生成{Path.GetFileName(filePath)}类");
            }

            AssetDatabase.Refresh();
        }
        // 导出单个类
        private void ExportClass(string fileName, string exportDir, string csHead)
        {
            XSSFWorkbook xssfWorkbook;
            using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                xssfWorkbook = new XSSFWorkbook(file);
            }

            string protoName = Path.GetFileNameWithoutExtension(fileName);

            string exportPath = Path.Combine(exportDir, $"{protoName}.cs");
            using (FileStream txt = new FileStream(exportPath, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(txt))
            {
                StringBuilder sb = new StringBuilder();
                // todo 以 # 打头的sheet才导出
                ISheet sheet = xssfWorkbook.GetSheetAt(0);
                sb.Append(csHead);

                // sb.Append($"\t[Config]\n");  // 属性禁用
                sb.Append($"\tpublic partial class {protoName}Category : ACategory<{protoName}>\n");
                sb.Append("\t{\n");
                sb.Append($"\t\tpublic static {protoName}Category Instance;\n");
                sb.Append($"\t\tpublic {protoName}Category()\n");
                sb.Append("\t\t{\n");
                sb.Append($"\t\t\tInstance = this;\n");
                sb.Append("\t\t}\n");
                sb.Append("\t}\n\n");

                sb.Append($"\tpublic partial class {protoName}: IConfig\n");
                sb.Append("\t{\n");
                //sb.Append("\t\t[BsonId]\n");
                sb.Append("\t\tpublic int Id { get; set; }\n");

                int cellCount = sheet.GetRow(3).LastCellNum;

                for (int i = 2; i < cellCount; i++)
                {
                    string fieldDesc = GetCellString(sheet, 2, i);

                    if (fieldDesc.StartsWith("#"))
                    {
                        continue;
                    }

                    // s开头表示这个字段是服务端专用
                    if (fieldDesc.StartsWith("s") && this.isClient)
                    {
                        continue;
                    }

                    string fieldName = GetCellString(sheet, 3, i);

                    if (fieldName == "Id" || fieldName == "_id")
                    {
                        continue;
                    }

                    string fieldType = GetCellString(sheet, 4, i);
                    if (fieldType == GlobalDefine.str_Empty || fieldName == GlobalDefine.str_Empty)
                    {
                        continue;
                    }
                    if (RegexHelper.RegexHelper.RegexIsOK("enum_.*", fieldType))
                    {
                        fieldType = RegexHelper.RegexHelper.RegexReplace("enum_", fieldType, GlobalDefine.str_Empty);
                    }
                    if (fieldType == "stringArray")
                    {
                        fieldType = "string";
                    }
                    Debug.Log($"\t\tpublic {fieldType} {fieldName};\n");
                    sb.Append($"\t\tpublic {fieldType} {fieldName};\n");
                }

                sb.Append("\t}\n");
                sb.Append("}\n");

                sw.Write(sb.ToString());
            }
            Debug.Log(fileName + protoName);
        }
        // 导出所有json
        private void ExportAll(string exportDir)
        {
            string md5File = Path.Combine(ExcelPath, "md5.txt");
            if (!File.Exists(md5File))
            {
                this.md5Info = new ExcelMD5Info();
            }
            else
            {
                this.md5Info = JsonHelper.FromJson<ExcelMD5Info>(File.ReadAllText(md5File));
            }
            Debug.Log(ExcelPath);
            foreach (string filePath in Directory.GetFiles(ExcelPath))
            {
                if (Path.GetExtension(filePath) != ".xlsx")
                {
                    continue;
                }

                if (Path.GetFileName(filePath).StartsWith("~"))
                {
                    continue;
                }

                string fileName = Path.GetFileName(filePath);
                string oldMD5 = this.md5Info.Get(fileName);
                string md5 = MD5Helper.FileMD5(filePath);
                this.md5Info.Add(fileName, md5);
                // if (md5 == oldMD5)
                // {
                // 	continue;
                // }

                Export(filePath, exportDir);
            }

            File.WriteAllText(md5File, JsonHelper.ToJson(this.md5Info));

            Log.Info("所有表导表完成");
            AssetDatabase.Refresh();
        }

        private void Export(string fileName, string exportDir)
        {
            Log.Debug($"Export {fileName}");
            XSSFWorkbook xssfWorkbook;
            using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                xssfWorkbook = new XSSFWorkbook(file);
            }

            string protoName = Path.GetFileNameWithoutExtension(fileName);
            Log.Info($"{protoName}导表开始");
            for (int i = 0; i < xssfWorkbook.NumberOfSheets; ++i)
            {
                ISheet sheet = xssfWorkbook.GetSheetAt(i);
                // 以 # 打头的sheet才导出     
                if (sheet.SheetName.StartsWith("#"))
                {
                    string exportPath = Path.Combine(exportDir, $"{protoName}_{sheet.SheetName}.json");
                    using (FileStream txt = new FileStream(exportPath, FileMode.Create))
                    using (StreamWriter sw = new StreamWriter(txt))
                    {
                        sw.WriteLine("{");
                        // todo 加json注释
                        ExportSheet(sheet, sw);
                        sw.WriteLine("}");
                    }
                }
            }


            Log.Info($"{protoName}导表完成");
        }

        // todo 考虑把不同页签分别存放，定义一个题头来判断是否要输出新类   var configType = assembly.GetType(configTypeName);
        private void ExportSheet(ISheet sheet, StreamWriter sw)
        {
            int cellCount = sheet.GetRow(3).LastCellNum;

            CellInfo[] cellInfos = new CellInfo[cellCount];

            for (int i = 2; i < cellCount; i++)
            {
                string fieldDesc = GetCellString(sheet, 2, i);
                string fieldName = GetCellString(sheet, 3, i);
                string fieldType = GetCellString(sheet, 4, i);
                cellInfos[i] = new CellInfo() { Name = fieldName, Type = fieldType, Desc = fieldDesc };
            }

            for (int i = 5; i <= sheet.LastRowNum; ++i)
            {
                if (GetCellString(sheet, i, 2) == "")
                {
                    continue;
                }

                StringBuilder sb = new StringBuilder();
                IRow row = sheet.GetRow(i);
                for (int j = 2; j < cellCount; ++j)
                {
                    string desc = cellInfos[j].Desc.ToLower();
                    if (desc.StartsWith("#"))
                    {
                        continue;
                    }

                    // s开头表示这个字段是服务端专用
                    if (desc.StartsWith("s") && this.isClient)
                    {
                        continue;
                    }

                    // c开头表示这个字段是客户端专用
                    if (desc.StartsWith("c") && !this.isClient)
                    {
                        continue;
                    }

                    string fieldValue = GetCellString(row, j);
                    if (fieldValue == "")
                    {
                        continue;
                    }

                    if (j > 2)
                    {
                        sb.Append(",");
                    }

                    string fieldName = cellInfos[j].Name;

                    if (fieldName == "Id" || fieldName == "_id")
                    {
                        fieldName = "Id";
                        sb.Append("\"" + fieldValue + "\":{");
                    }

                    string fieldType = cellInfos[j].Type;
                    sb.Append($"\"{fieldName}\":{Convert(fieldType, fieldValue)}");
                }

                if (i < sheet.LastRowNum) sb.Append("},");
                else sb.Append("}");
                sw.WriteLine(sb.ToString());
            }
        }

        private static string Convert(string type, string value)
        {
            if (RegexHelper.RegexHelper.RegexIsOK("enum_.*", type))
            {
                // 是枚举
                return $"\"{value}\"";
            }
            if (type == "stringArray")
            {
                return $"\"{RegexHelper.RegexHelper.RegexReplace("\n", value, ";")}\"";
                //return $"[\"{RegexHelper.RegexHelper.RegexReplace("\n", value, "\",\"")}\"]";
            }
            switch (type)
            {
                case "int[]":
                case "int32[]":
                case "long[]":
                    return $"[{value}]";
                case "string[]":
                    return $"[{value}]";
                case "int":
                case "int32":
                case "int64":
                case "long":
                case "float":
                case "double":
                case "bool":
                    return value;
                    //return $"\"{value}\"";
                case "string":
                    return $"\"{value}\"";
                default:
                    throw new Exception($"不支持此类型: {type}");
            }
        }

        private static string GetCellString(ISheet sheet, int i, int j)
        {
            IRow _irow = sheet.GetRow(i);
            if(_irow != null)
            {
               return GetCellString(_irow, j);
            }
            return "";
        }

        private static string GetCellString(IRow row, int i)
        {
            ICell _icell = row.GetCell(i);
            if (_icell != null)
            {
                return GetCellString(_icell); ;
            }
            return "";
        }

        private static string GetCellString(ICell cell)
        {
            if (cell != null)
            {
                if(cell.CellType == CellType.Numeric || (cell.CellType == CellType.Formula && cell.CachedFormulaResultType == CellType.Numeric))
                {
                    return cell.NumericCellValue.ToString();
                }
                else if (cell.CellType == CellType.String || (cell.CellType == CellType.Formula && cell.CachedFormulaResultType == CellType.String))
                {
                    return cell.StringCellValue.ToString();
                }
                else if (cell.CellType == CellType.Boolean || (cell.CellType == CellType.Formula && cell.CachedFormulaResultType == CellType.Boolean))
                {
                    return cell.BooleanCellValue.ToString();
                }
                else
                {
                    return cell.ToString();
                }
            }
            return "";
        }
    
        // 删除文件夹下所有文件
        private void DeleteDir(string dirPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(dirPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    } 
                    else
                    {
                        File.Delete(i.FullName);      //删除指定文件
                    }
                }                
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}