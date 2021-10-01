using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// 正则表达式相关解析Helper
namespace RegexHelper
{
    public static class RegexHelper
    {
        /// <summary>验证字符串是否满足正则表达式</summary>
        /// <param name="regularExpression">正则表达式</param>
        /// <param name="inputStr">输入字符串</param>
        /// <returns>true为满足；false为不满足</returns>
        public static bool RegexIsOK(string regularExpression, string inputStr)
        {
            // IgnoreCase 表示不区分大小写
            Regex objRegex = new Regex(regularExpression, RegexOptions.IgnoreCase);
            if (objRegex.IsMatch(inputStr))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>提取符合正则表达式要求的所有字符串</summary>
        /// <param name="regularExpression">正则表达式</param>
        /// <param name="inputStr">输入字符串</param>
        /// <returns>返回的数据集</returns>
        public static List<string> RegexExtracts(string regularExpression, string inputStr)
        {
            List<string> list = new List<string>();
            var cols = Regex.Matches(inputStr, regularExpression);
            for (int i = 0; i < cols.Count; i++)
            {
                list.Add(cols[i].Value);
            }
            return list;
        }

        /// <summary>提取符合正则表达式要求的第一个匹配项</summary>
        /// <param name="regularExpression">正则表达式</param>
        /// <param name="inputStr">输入字符串</param>
        /// <returns>返回的数据</returns>
        public static string RegexExtractOnlyOne(string regularExpression, string inputStr)
        {
            return Regex.Match(inputStr, regularExpression).Value;
        }
    
        /// <summary>提取符合正则表达式要求的匹配项进行替换</summary>
        /// <param name="regularExpression">正则表达式</param>
        /// <param name="inputStr">输入字符串</param>
        /// <returns>返回的数据</returns>
        public static string RegexReplace(string regularExpression, string inputStr, string replaceStr)
        {
            Regex rgx = new Regex(regularExpression);
            string result = rgx.Replace(inputStr, replaceStr);
            return result;
        }

        /// <summary>提取符合正则表达式要求的匹配项进行分割</summary>
        /// <param name="regularExpression">正则表达式</param>
        /// <param name="inputStr">输入字符串</param>
        /// <returns>返回的数据</returns>
        public static string[] RegexSplit(string inputStr, string splitStr)
        {
            string[] result = Regex.Split(inputStr, splitStr);
            return result;
        }

    }
}