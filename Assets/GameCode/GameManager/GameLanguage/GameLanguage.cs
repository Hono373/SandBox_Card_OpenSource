using System;
using ProjectLumiyaDB;
using UnityEngine;
public static class GameLanguage
{
    internal static void Create()
    {

    }
    public static string GetText(string key)
    {
        return "未定义的设计";
    }
    public static string GetText(Text info)
    {
        switch (GameSerialize.Config.language)
        {
            case LanguageEnum.CN:
                return info.CN;
            case LanguageEnum.EN:
                return info.EN;
            case LanguageEnum.JP:
                return info.JP;
            default:
                GameDebug.Log("Undefine Type");
                return info.CN;
        }
    }
    public static string GetText(TextInfo info)
    {
        switch (GameSerialize.Config.language)
        {
            case LanguageEnum.CN:
                return info.cn;
            case LanguageEnum.EN:
                return info.en;
            case LanguageEnum.JP:
                return info.jp;
            default:
                GameDebug.Log("Undefine Type");
                return info.cn;
        }
    }


}
