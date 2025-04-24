using MDM.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MDM.Helpers
{
    public static class TextHelper
    {
        public static List<char> markers = new List<char>() { '-', '>', '*', '▣', '▷' };
        public static List<char> linebreakMarkers = new List<char>() { '※', '☞', '→', '+' };
        public static List<char> NumberDividerMarks = new List<char>() { '.', ')' };
        static List<char> openingBrachets = new List<char>() { '(', '（', '【', '「', '『', '《' };
        static List<char> closingBrackets = new List<char>() { ')', '）', '】', '」', '』', '》' };

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
        public static double CalculateSimilary2(string origin, string target) => JaroWinklerSimilarity(origin, target) * 100;
        public static string CleansingForXML(string value)
        {
            string output = string.Empty;

            foreach (char c in value)
            {
                if (c.IsCharZeroWidthSpace()) continue;
                if (c.IsCharHorizontalTab()) continue;

                output += c;
            }

            return output;
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
        private static double JaroWinklerSimilarity(string s1, string s2)
        {
            if (s1 == s2)
                return 1.0;

            int len1 = s1.Length;
            int len2 = s2.Length;

            if (len1 == 0 || len2 == 0)
                return 0.0;

            // 각 문자열의 유사도 비교 범위 설정
            int matchDistance = Math.Max(len1, len2) / 2 - 1;

            bool[] s1Matches = new bool[len1];
            bool[] s2Matches = new bool[len2];

            int matches = 0;
            for (int i = 0; i < len1; i++)
            {
                int start = Math.Max(0, i - matchDistance);
                int end = Math.Min(i + matchDistance + 1, len2);

                for (int j = start; j < end; j++)
                {
                    if (s2Matches[j]) continue;
                    if (s1[i] != s2[j]) continue;
                    s1Matches[i] = true;
                    s2Matches[j] = true;
                    matches++;
                    break;
                }
            }

            if (matches == 0) return 0.0;

            // 문자열 간 변환 작업 횟수 계산
            double t = 0;
            int k = 0;
            for (int i = 0; i < len1; i++)
            {
                if (!s1Matches[i]) continue;
                while (!s2Matches[k]) k++;
                if (s1[i] != s2[k]) t++;
                k++;
            }

            t /= 2;

            // Jaro 유사도 계산
            double jaro = ((matches / (double)len1) +
                          (matches / (double)len2) +
                          ((matches - t) / matches)) / 3.0;

            // Jaro-Winkler 보정
            int prefix = 0;
            for (int i = 0; i < Math.Min(4, Math.Min(len1, len2)); i++)
            {
                if (s1[i] == s2[i])
                    prefix++;
                else
                    break;
            }

            return jaro + (prefix * 0.1 * (1 - jaro));
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
        public static string RemoveNoTextLine(string input)
        {
            string output = string.Empty;

            string[] lines = TextHelper.SplitText(input);
            foreach (string ln in lines)
            {
                if (TextHelper.IsNoText(ln)) continue;
                if (ln != lines.First()) output += "\n";
                output += ln;

            }

            return output;
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
        public static bool IsNoText(string input)
        {
            foreach (char c in input)
            {
                if (c == '\u200B') continue;
                if (!char.IsWhiteSpace(c)) return false;
            }

            return true;
        }
        public static Match IsImageMarkdown(string input)
        {
            string pattern = @"^!\[([^\]]+)\]\(([\w-]+)\.png\)";
            return Regex.Match(input, pattern);
        }
        public static bool IsTableMarkdownValid(string input)
        {
            string[] lines = SplitText(input);

            // Divider가 있는지 여부?
            string divider = lines.Where(x => IsTableDivider(x)).FirstOrDefault();
            if (string.IsNullOrEmpty(divider)) return false;

            int columnCnt = GetCellValueInRowString(divider.Replace("＋", "+").Replace("+", "|")).Length;
            foreach (string ln in lines)
            {
                if (ln == divider) continue;
                if (IsNoText(ln)) continue;
                int cnt = GetCellValueInRowString(ln).Length;
                if (columnCnt != cnt) return false;
            }

            return true;
        }
        public static bool IsTableDivider(string input)
        {
            string line = CleansingForXML(input).RemoveEmtpy();
            line = line.Replace("＋", "+");

            string pattern = @"^(\|[-]+|\+[-]+)+\|$";
            bool isDivider = Regex.IsMatch(line, pattern);
            return isDivider;
        }
        public static bool IsEnClosedNumbers(char input)
        {
            return '\u2460' <= input && input <= '\u2471';
        }
        public static bool IsRomanNumbers(char input) => input >= '\u2160' && input <= '\u217F';
        public static bool IsFirstNumericListMark(string input)
        {
            char firstChar = input.Trim().First();

            if(char.IsDigit(firstChar)) return true;
            if (IsEnClosedNumbers(firstChar)) return true;
            if (IsRomanNumbers(firstChar)) return true;

            
            if (openingBrachets.Contains(firstChar))
            {
                int index = openingBrachets.IndexOf(firstChar);
                char closeBracket = closingBrackets[index];
                if(input.Contains(closeBracket))
                {
                    string number = input.Substring(1).Split(closeBracket)[0];
                    return int.TryParse(number, out int result);
                }
            }
            return false;
        }
        public static bool IsFirstLinebreakMark(string input)
        {
            return linebreakMarkers.Contains(input.Trim().First());
        }
        private static bool IsCharZeroWidthSpace(this char c) => c == '\u200B';
        private static bool IsCharHorizontalTab(this char c) => c == '\u000B';
        public static bool IsFirstCharUnorderMark(this string input)
        {
            string text = input.Trim();
            //output = text.Substring(1).Trim();
            return markers.Contains(text.First());
        }


        public static string[] GetCellValueInRowString(string input)
        {
            string[] parts = CleansingForXML(input.Trim()).Split('|', (char)StringSplitOptions.RemoveEmptyEntries);
            if (string.IsNullOrEmpty(parts.First())) parts = parts.Skip(1).ToArray();
            if (string.IsNullOrEmpty(parts.Last())) parts = parts.Take(parts.Length - 1).ToArray();

            return parts;
        }
        public static int GetRowHeaderCount(string divider)
        {
            int output = 0;

            foreach (char c in divider)
            {
                if (c == '|') output++;
                else if (c == '+' || c == '＋') return output;
            }

            return 0;
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
            Match match = Regex.Match(input, pattern, RegexOptions.Singleline); // 여기 추가됨

            if (match.Success) return match.Groups[1].Value;

            return null;
        }



        public static string Preprocessing(this string originText)
        {
            string[] textSplits = SplitText(originText);

            string output = string.Empty;
            foreach (string origin in textSplits)
            {
                if (IsNoText(origin)) continue;

                string line = origin;

                int levelCnt = 0;

                int total = line.Length;
                for (int i = 0; i < total; i++)
                {
                    char c = line[i];
                    if (c == '\t')
                    {
                        levelCnt++;
                        line = line.Substring(1);
                        continue;
                    }
                    if (char.IsWhiteSpace(c))
                    {
                        string gap = line.Substring(0, 1);
                        if(gap == "  ")
                        {
                            levelCnt++;
                            line = line.Substring(2);
                            continue;
                        }
                    }
                    break;
                }

                string mark = "-";
                if (TextHelper.IsFirstCharUnorderMark(line))
                {
                    mark = "-";
                    line = line.Trim().Substring(1).TrimStart();
                }
                else if (TextHelper.IsFirstNumericListMark(line))
                {
                    mark = string.Empty;
                    line = line.Trim();
                }
                else if (TextHelper.IsFirstLinebreakMark(line))
                {
                    mark = line.First() == '+' ? string.Empty : "+";
                    line = line.Trim();
                }
                else
                {
                    line = line.Trim();
                }

                string indent = string.Empty;
                for (int i = 0; i < levelCnt; i++) indent += "  ";

                line = string.Format("{0}{1} {2}", indent, mark, line);
                output += line;
                output += "\n";
            }

            return output.TrimEnd();
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
                if (c.IsCharZeroWidthSpace()) continue;

                output += c;
            }

            return output;
        }
        public static string RemoveHorizontalTab(string value)
        {
            string output = string.Empty;

            foreach (char c in value)
            {
                //(char)8203
                if (c.IsCharHorizontalTab()) continue;

                output += c;
            }

            return output;
        }
        public static string RemoveLevelInText(string value)
        {
            if (IsNoText(value)) return string.Empty;

            string[] lines = SplitText(value);

            int num = 0;
            Dictionary<int, string> tempLines = new Dictionary<int, string>();
            foreach (string ln in lines) if (!IsNoText(ln)) tempLines.Add(num++, ln);

            bool hasLetterFirst = tempLines.Values.Where(x => !char.IsWhiteSpace(x.First())).Count() > 0;
            while (!hasLetterFirst)
            {
                foreach (int key in tempLines.Keys.ToList())
                {
                    string text = tempLines[key];
                    if (char.IsWhiteSpace(text.First())) text = text.Substring(1);
                    tempLines[key] = text;
                }

                hasLetterFirst = tempLines.Values.Where(x => !char.IsWhiteSpace(x.First())).Count() > 0;
            }


            string output = string.Empty;
            foreach (string item in tempLines.Values)
            {
                output += item;
                output += "\n";
            }

            return output.Trim();
        }
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
        public static Dictionary<int, string> ToDictionary(this string[] lines)
        {
            Dictionary<int, string> output = new Dictionary<int, string>();
            int num = 0;
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                if (line.Trim() == string.Empty) continue;
                output.Add(num++, line);
            }
            return output;

        }
        public static List<mTextLine> ToLineList(this string[] textLines)
        {
            List<mTextLine> output = new List<mTextLine>();
            int num = 0;
            foreach (string line in textLines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                if (line.Trim() == string.Empty) continue;
                mTextLine textLine = new mTextLine();
                textLine.LineNumber = num++;
                textLine.LineText = line;
                output.Add(textLine);
            }
            return output;
        }

        public static string GetEmptyCharFromHead(string prevLine)
        {
            string output = string.Empty;

            string text = prevLine;
            foreach (char c in prevLine)
            {
                if(c == '\t')
                {
                    output += "  ";
                    continue;
                }
                if (char.IsWhiteSpace(c))
                {
                    output += c;
                    continue;
                }
                break;
            }

            return output;
        }
        public static int GetLineLevel(string line)
        {
            string empty = GetEmptyCharFromHead(line);
            return empty.Length / 2;
        }
    }
}
