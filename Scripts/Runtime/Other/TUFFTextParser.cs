using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public static class TUFFTextParser
    {
        private const int MAX_ITERATIONS = 2500;
        public enum TextTagType
        {
            LocalizedDialogueText = 0,
            LocalizedText = 1,
            MagsCount = 2,
            AutoContinue = 3,
            TextPause = 4,
        }
        public struct TagData
        {
            public TagData(TextTagType type, int index, string fullTag)
            {
                this.type = type;
                this.index = index;
                this.fullTag = fullTag;
            }
            public TextTagType type;
            public int index;
            public string fullTag;
        }
        public static string[] tags = {
            "<ld:", "<l:", "<mags:>", "<skip:>", "<wait:"
        };
        public static string ParseText(string text) => ParseText(text, null);
        public static string ParseText(string text, List<TagData> savedTags)
        {
            if (text == null) return null;
            if (text.Length <= 0) return text;
            string parse = text;

            int from = 0;
            int to = 0;
            int iterations = -1;
            while (from >= 0 && from <= parse.Length)
            {
                iterations++;
                if (iterations >= MAX_ITERATIONS) return "ERROR";
                from = parse.IndexOf('<', from); // Find first index of '<'
                if (from < 0) break; // If found no tags, exit
                to = parse.IndexOf('>', from); // Find first index of '>'
                if (to < 0) break; // If found no tag end, exit

                int length = to - from + 1;
                if (length <= 1) { from += 1; continue; }
                string substring = parse.Substring(from, length);

                bool changedString = false;
                for (int i = 0; i < tags.Length; i++)
                {
                    if (substring.StartsWith(tags[i]))
                    {
                        var tagType = (TextTagType)i;
                        //Debug.Log(tagType);
                        var tagFound = new TagData(tagType, from, substring);
                        changedString = ApplyTagBehaviour(tagType, substring, ref parse);
                        savedTags?.Add(tagFound);
                        break;
                    }
                }
                if (changedString) continue;
                from += 1;
            }
            //Debug.Log("*** Parse: " + parse);
            return parse;
        }
        public static List<TagData> GetTags(string text)
        {
            var tags = new List<TagData>();
            ParseText(text, tags);
            return tags;
        }
        private static bool ApplyTagBehaviour(TextTagType tagType, string substring, ref string parse)
        {
            switch (tagType)
            {
                // <ld:Key> Localized Dialogue Text
                case TextTagType.LocalizedDialogueText:
                    if (substring.Length > 5)
                    {
                        string key = substring.Substring(4, substring.Length - 5);
                        string localizedText = LISAUtility.GetLocalizedDialogueText(key);
                        parse = ReplaceFirst(parse, substring, localizedText);
                        return true;
                    }
                    return false;
                // <l:Table.Key> Localized Text
                case TextTagType.LocalizedText:
                    string localizationTag = substring.Substring(3, substring.Length - 4);
                    int separationIdx = localizationTag.IndexOf('.');
                    if (separationIdx >= 0 && separationIdx < localizationTag.Length - 1)
                    {
                        string table = localizationTag.Substring(0, separationIdx);
                        string key = localizationTag.Substring(separationIdx + 1);
                        string localizedText = LISAUtility.GetLocalizedText(table, key);
                        parse = ReplaceFirst(parse, substring, localizedText);
                        return true;
                    }
                    return false;
                // <mags:> Mags Count
                case TextTagType.MagsCount:
                    string magsCount = "(mags count)";
                    if (GameManager.instance != null)
                    {
                        if (GameManager.instance.playerData != null)
                        {
                            magsCount = LISAUtility.IntToString(GameManager.instance.playerData.mags);
                        }
                    }
                    parse = ReplaceFirst(parse, substring, magsCount);
                    return true;
                default:
                    parse = ReplaceFirst(parse, substring, "");
                    return true;
            }
        }
        private static string ReplaceFirst(string org, string oldValue, string newValue)
        {
            int index = org.IndexOf(oldValue);
            if (index >= 0)
            {
                return org.Substring(0, index) + newValue + org.Substring(index + oldValue.Length);
            }
            return org;
        }
    }
}

