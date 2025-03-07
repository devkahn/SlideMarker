using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MDM.Helpers
{
    public static class TextHelper
    {
        public static string RemoveHeadingSymbol(this string value)
        {
            string output = value;
            while (output.First() == '#')
            {
                output = output.Substring(1);
            }
            return output;
        }
        public static string RemoveEmtpy(this string value)
        {
            string output = string.Empty;
            foreach (char item in value)
            {
                if (char.IsWhiteSpace(item)) continue;
                output += item;
            }
            return output;
        }
        public static string RemoveSpecialChar(this string value)
        {
            return Regex.Replace(value, @"[^a-zA-Z0-9\s\uac00-\ud7af]", "");
        }
        public static string GetLineMark(string lineText)
        {
            string output = string.Empty;

            foreach (char c in lineText)
            {
                if (char.IsLetter(c)) break;
                output += c;
            }

            return output.Trim();
        }
        public static double CalculateSimilary(string origin, string target)
        {
            int distance = LevenshteinDistance(origin, target);
            int maxLength = Math.Max(origin.Length, target.Length);
            return (1.0 - (double)distance / maxLength) * 100; // 유사도를 퍼센트로 계산
        }
        private static int LevenshteinDistance(string origin, string target)
        {
            int n = origin.Length;
            int m = target.Length;
            int[,] d = new int[n + 1, m + 1];

            // 초기화
            for (int i = 0; i <= n; i++) d[i, 0] = i;
            for (int j = 0; j <= m; j++) d[0, j] = j;

            // 편집 거리 계산
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (origin[i - 1] == target[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
        public static bool IsTextNuberic(string text)
        {
            foreach (char c in text)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }
        public static string HighlightMarkdown(string value)
        {

            return value;
        }
        public static string[] SplitText(string input)
        {
            // 정규 표현식으로 \v, \r, \n 등을 기준으로 분리
            return Regex.Split(input, @"[\v\r\n]");
        }

        public static bool IsNoText(string input)
        {
            return string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input);
        }
        public static Match IsImageMarkdown(string input)
        {
            string pattern = @"^!\[([^\]]+)\]\(([\w-]+)\.png\)";
            return Regex.Match(input, pattern);
        }
        public static bool IsEnClosedNumbers(char input)
        {
            return '\u2460' <= input && input <= '\u2471';
        }
        public static string GetImageFileNameFromMarkdown(string input)
        {
            string pattern = @"\(([^)]+)\)[^\(]*$";
            Match match = Regex.Match(input, pattern);

            if (match.Success) return match.Groups[1].Value; // 첫 번째 캡처 그룹

            return null;
        }
        public static string GetImageTitleFromMarkdown(string input)
        {
            string pattern = @"\[(.*?)\]";
            Match match = Regex.Match(input, pattern);

            if (match.Success) return match.Groups[1].Value; // 첫 번째 캡처 그룹

            return null;
        }
            
        public static bool IsFirstNumericListMark(string input)
        {
            string pattern = @"^\d+\.\s";
            return Regex.IsMatch(input, pattern);
        }

        public static string Preprocessing(this string originText)
        {
            string[] textSplits = SplitText(originText);

            string output = string.Empty;
            foreach (string line in textSplits)
            {
                if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line)) continue;

                string newLine = line;
                if (newLine.StartsWith("\t\t\t\t")) newLine = newLine.Replace("\t\t\t\t", "        "+"- ");
                else if (newLine.StartsWith("\t\t\t")) newLine = newLine.Replace("\t\t\t", "      "+"- ");
                else if (newLine.StartsWith("\t\t")) newLine = newLine.Replace("\t\t", "    "+"- ");
                else if (newLine.StartsWith("\t")) newLine = newLine.Replace("\t", "  "+"- ");
                else if (textSplits.Length > 1)
                {
                    if (newLine.First() == '▣') newLine = newLine.Replace("▣", "        "+"- ");
                    else if (newLine.First() == '▷') newLine = newLine.Replace("▷", "      "+"- ");
                    else if (newLine.First() == '*') newLine = newLine.Replace("*", "    "+"- ");
                    else if (newLine.First() == '>') newLine = newLine.Replace(">", "  "+"- ");
                    else if (newLine.First() != '-') newLine = newLine.Insert(0, "- ");
                    //else newLine = newLine.Insert(0, "- ");
                }

                output += newLine;
                if (line != textSplits.Last()) output += "\n";
            }

            return output;
        }
        public static string Numbering(this string originText)
        {
            string[] textSplits = SplitText(originText);

            string output = string.Empty;
            foreach (string line in textSplits)
            {
                if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line)) continue;

                string newLine = line;
                if (newLine.StartsWith("\t\t\t\t")) newLine = newLine.Replace("\t\t\t\t", "\t\t\t\t▣ ");
                else if (newLine.StartsWith("\t\t\t")) newLine = newLine.Replace("\t\t\t", "\t\t\t▷ ");
                else if (newLine.StartsWith("\t\t")) newLine = newLine.Replace("\t\t", "\t\t* ");
                else if (newLine.StartsWith("\t")) newLine = newLine.Replace("\t", "\t> ");
                else if (textSplits.Length > 1)
                {
                    if (newLine.First() == '▣') newLine = newLine.Replace("▣", "\t\t\t\t▣ ");
                    else if (newLine.First() == '▷') newLine = newLine.Replace("▷", "\t\t\t▷ ");
                    else if (newLine.First() == '*') newLine = newLine.Replace("*", "\t\t* ");
                    else if (newLine.First() == '>') newLine = newLine.Replace(">", "\t> ");
                    else newLine = newLine.Insert(0, "- ");
                }

                output += newLine;
                if (line != textSplits.Last()) output += "\n";
            }

            return output;
        }

        public static string RemoveZeroWidthSpace(string value)
        {
            string output = string.Empty;

            foreach (char c in value)
            {
                //(char)8203
                if (c == '\u200B')
                {
                    continue;
                }
                output += c;
            }

            return output;
        }
    }
}
