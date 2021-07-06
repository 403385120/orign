using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Globalization;
using System.Windows.Markup;

using System.Windows;
using System.IO;

namespace ZYXray
{
    public static class LangHelper
    {
        static string _langDir = System.Environment.CurrentDirectory + "\\Lang";

        public static List<string> getLangNames()
        {
            if (!Directory.Exists(_langDir)) return new List<string>();

            DirectoryInfo dirInfo = new DirectoryInfo(_langDir);
            FileInfo[] langFiles = dirInfo.GetFiles("*.xaml");

            List<string> langNames = new List<string>();
            foreach (FileInfo fi in langFiles)
            {
                langNames.Add(Path.GetFileNameWithoutExtension(fi.Name));
            }

            return langNames;
        }

        public static string getTranslation(string key)
        {
            if(null == App.Current || null == App.Current.Resources.MergedDictionaries)
            {
                return key;
            }

            int resCnt = App.Current.Resources.MergedDictionaries.Count();

            if(resCnt < 1)
            {
                return key;
            }

            ResourceDictionary i18n = Application.Current.Resources.MergedDictionaries[resCnt-1];

            foreach (object k in i18n.Keys)
            {
                if (k.ToString() == key.ToString())
                {
                    return i18n[k].ToString();
                }
            }

            return key;
        }

        public static string getTranslation(string key, int index)
        {
            if (null == App.Current || null == App.Current.Resources.MergedDictionaries)
            {
                return key;
            }

            int resCnt = App.Current.Resources.MergedDictionaries.Count();

            if (resCnt < 1)
            {
                return key;
            }

            ResourceDictionary i18n = Application.Current.Resources.MergedDictionaries[resCnt - 1];

            foreach (object k in i18n.Keys)
            {
                if (k.ToString() == key.ToString())
                {
                    if (i18n[k].GetType() == typeof(string[]))
                    {
                        string[] items = (string[]) i18n[k];

                        if (items.Count() > index)
                        {
                            return items[index].ToString();
                        }
                    }
                }
            }

            return key;
        }

        private static string _prevLang = string.Empty;
        public static bool changeLang(string langName)
        {
            if (null == App.Current.Resources.MergedDictionaries)
            {
                return false;
            }

            if (langName == _prevLang) return false;

            string langFile = _langDir + "\\" + langName + ".xaml";
            if (!File.Exists(langFile)) {
                return false;
            }

            int resCnt = App.Current.Resources.MergedDictionaries.Count();

            if (resCnt > 0)
            {
                try {
                    Application.Current.Resources.MergedDictionaries[resCnt - 1].Source = new Uri(langFile, UriKind.RelativeOrAbsolute);

                    _prevLang = langName;

                    // Set the culture information for the entire application on language loading
                    // to make the rending of the font more friendly
                    // only useble one the first call per startup
                    string newLang = getTranslation("LL_LangTag");
                    Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(newLang);
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(newLang);
                    FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(newLang)));

                    return true;
                }
                catch
                {
                    return false;
                }    
            }

            return false;
        }        
    }
}
