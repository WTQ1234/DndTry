using System;
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
        private string clientEnumPath = @"./Assets/Scripts/Config";

        private bool isClient;

        private ExcelMD5Info md5Info;

        private void OnGUI()
        {
            try
            {
                if (GUILayout.Button("导出客户端配置"))
                {
                    this.isClient = true;
                    DeleteDir(clientPath);
                    ExportAll(clientPath);
                    Log.Info($"导出客户端配置完成!");
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            try
            {
                if (GUILayout.Button("导出类"))
                {
                    this.isClient = true;
                    // 删除类下的文件
                    DeleteDir(clientClassPath);
                    ExportAllClass(clientClassPath);//using MongoDB.Bson.Serialization.Attributes;\n\n
                    Log.Info($"导出类完成!");
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            try
            {
                if (GUILayout.Button("导出枚举"))
                {
                    this.isClient = true;
                    ExportAllEnum(clientEnumPath);
                    Log.Info($"导出枚举完成!");
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        // 导出所有类
        private void ExportAllClass(string exportDir)
        {
            foreach (string filePath in Directory.GetFiles(ExcelPath))
            {
                if (Path.GetExtension(filePath) != ".xlsx")
                {
                    continue;
                }
                ExportClass(filePath, exportDir);
            }
            AssetDatabase.Refresh();
        }
        private void ExportClass(string fileName, string exportDir)
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
                sb.Append($"// 自动导出 {ExcelPath} {fileName}\n");
                sb.Append("using ET;\n");
                for (int sheetNum = 0; sheetNum < xssfWorkbook.NumberOfSheets; ++sheetNum)
                {
                    ISheet sheet = xssfWorkbook.GetSheetAt(sheetNum);
                    // 以 # 打头的sheet  且  第二行有 ExportClass 标识的才导出     
                    if (sheet.SheetName.StartsWith("#"))
                    {
                        IRow row = sheet.GetRow(1);
                        string isExportClass = GetCellString(row.GetCell(0));
                        if (isExportClass == "ExportClass")
                        {
                            string ClassName = GetCellString(row.GetCell(1));
                            sb.Append("\n");
                            // sb.Append($"\t[Config]\n");  // 属性禁用
                            sb.Append($"public partial class {ClassName}Category : ACategory<{ClassName}>\n");
                            sb.Append("{\n");
                            sb.Append($"\tpublic static {ClassName}Category Instance;\n");
                            sb.Append($"\tpublic {ClassName}Category()\n");
                            sb.Append("\t{\n");
                            sb.Append($"\t\tInstance = this;\n");
                            sb.Append("\t}\n");
                            sb.Append("}\n\n");
                            sb.Append($"public partial class {ClassName}: IConfig\n");
                            sb.Append("{\n");
                            //sb.Append("\t\t[BsonId]\n");  // 属性禁用
                            sb.Append("\tpublic int Id { get; set; }\n");
                            int cellCount = sheet.GetRow(3).LastCellNum;

                            for (int i = 2; i < cellCount; i++)
                            {
                                string fieldDesc = GetCellString(sheet, 2, i);
                                if (fieldDesc.StartsWith("#"))
                                {
                                    continue;
                                }
                                if (fieldDesc == "break")
                                {
                                    break;
                                }
                                // s开头表示这个字段是服务端专用
                                if (fieldDesc.StartsWith("s") && this.isClient)
                                {
                                    continue;
                                }
                                string fieldName = GetCellString(sheet, 3, i);
                                // todo 后面添加KEY的时候，应该也是这样略过
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
                                sb.Append($"\tpublic {fieldType} {fieldName};\n");
                            }
                            sb.Append("}\n");
                        }
                        else
                        {
                            Debug.LogError($"No ExportClass Found! in {fileName} —— {isExportClass}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"No # in Sheet! in {fileName} —— {sheet.SheetName}");
                    }
                }
                sw.Write(sb.ToString());
            }
            Debug.Log($"导出类文件 {fileName} - {protoName}");
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
            foreach (string filePath in Directory.GetFiles(ExcelPath))
            {
                if (Path.GetExtension(filePath) != ".xlsx")
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
                        // 添加json注释, 约定 $ 开头的json题头为注释  ACategory.cs
                        sw.WriteLine($"\"$_Note\":{{\"path\":\"路径 {protoName}.xlsx {sheet.SheetName}\"}},");
                        ExportSheet(sheet, sw);
                        sw.WriteLine("}");
                    }
                }
            }


            Log.Info($"{protoName}导表完成");
        }
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
                    if (desc == "break")
                    {
                        break;
                    }
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

        // 导出所有枚举
        private void ExportAllEnum(string exportDir)
        {
            //string fileName = "EnumDefine_Config.cs";
            //string protoName = Path.GetFileNameWithoutExtension(fileName);


            string exportPath = Path.Combine(exportDir, $"EnumDefine_Config1.cs");
            using (FileStream txt = new FileStream(exportPath, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(txt))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"// 自动导出枚举 {ExcelPath}\n");
                sb.Append("using ET;\n");
                sb.Append("using System;\n");
                sb.Append("using Sirenix.OdinInspector;\n");

                foreach (string filePath in Directory.GetFiles(ExcelPath))
                {
                    if (Path.GetExtension(filePath) != ".xlsx")
                    {
                        continue;
                    }
                    XSSFWorkbook xssfWorkbook;
                    using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        xssfWorkbook = new XSSFWorkbook(file);
                    }
                    for (int sheetNum = 0; sheetNum < xssfWorkbook.NumberOfSheets; ++sheetNum)
                    {
                        ISheet sheet = xssfWorkbook.GetSheetAt(sheetNum);
                        // 以 & 打头的sheet  且  第二行有 ExportEnum 标识的才导出     
                        if (sheet.SheetName.StartsWith("&"))
                        {
                            IRow row = sheet.GetRow(1);
                            string isExportEnum = GetCellString(row.GetCell(0));
                            if (isExportEnum == "ExportEnum")
                            {
                                string EnumName = GetCellString(row.GetCell(1));
                                string EnumDesc = GetCellString(row.GetCell(2));
                                sb.Append("\n");
                                sb.Append($"[LabelText(\"{EnumDesc}\")]\n");
                                sb.Append($"public enum {EnumName}\n");
                                sb.Append($"{{\n");

                                //    // 由等级决定
                                //    [LabelText("理智")]
                                //    Sanity = 1000,

                                for (int i = 3; i <= sheet.LastRowNum; i++)
                                {
                                    string id = GetCellString(sheet, i, 2);
                                    string key = GetCellString(sheet, i, 3);
                                    string name = GetCellString(sheet, i, 4);
                                    string desc = GetCellString(sheet, i, 5);
                                    sb.Append($"\t[LabelText(\"{name}\")]\n");
                                    sb.Append($"\t{key} = {id},\n");
                                    if (desc != null && desc != "")
                                    {
                                        sb.Append($"\n");
                                        sb.Append($"\t//{desc}\n");
                                    }
                                }
                                sb.Append($"}}\n");
                            }
                            else
                            {
                                //Debug.LogError($"No ExportClass Found! in {fileName} —— {isExportClass}");
                            }
                        }
                        else
                        {
                            //Debug.LogWarning($"No # in Sheet! in {fileName} —— {sheet.SheetName}");
                        }
                    }
                }
                sw.Write(sb.ToString());
            }
            //Debug.Log($"导出类文件 {fileName} - {protoName}");
            AssetDatabase.Refresh();
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