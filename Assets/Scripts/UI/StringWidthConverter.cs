using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

static class StringWidthConverter
{
    public static string ConvertToFullWidth(this string self,int num = 999)
    {
        StringBuilder sb = new StringBuilder();

        foreach (char c in self)
        {
            if (c >= 33 && c <= 126)
            {
                // 半角文字の場合は全角に変換
                sb.Append((char)(c + 0xFEE0));
            }
            else
            {
                // 半角でない場合はそのまま追加
                sb.Append(c);
            }
            if(sb.Length == num)
            {
                return sb.ToString();
            }
        }

        return sb.ToString();
    }
    
    // 文字列の置換メソッド
    public static string ReplaceString(this string self, string target, string replacement)
    {
        // 対象の文字列が見つからない場合はそのまま返す
        if (!self.Contains(target))
        {
            return self;
        }

        // 文字列の置換を行う
        string result = self.Replace(target, replacement);

        return result;
    }
    
}
