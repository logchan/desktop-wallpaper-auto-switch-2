using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

using MultiLang;
using System.Windows;

namespace DWAS2.Utilities
{
    class DWAS2LanguageManager
    {

        /// <summary>
        /// Load language files
        /// </summary>
        public static List<MultiLangEngine> LoadLanguages()
        {
            List<MultiLangEngine> languages = new List<MultiLangEngine>();

            /* Check if language dir exists
             * 1. If it does, scan it
             * 2. If it doesn't, skip it
             */
            string langPath = Environment.CurrentDirectory + @"\lang\";
            if (Directory.Exists(langPath))
            {
                // 1
                languages = MultiLangEngine.ScanDirectory(langPath);
            }

            /* 
             * Load built-in language files
             */
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(Properties.Resources.dwas2_en_US);
                languages.Add(new MultiLangEngine(xmldoc));
                xmldoc.LoadXml(Properties.Resources.dwas2_zh_CN);
                languages.Add(new MultiLangEngine(xmldoc));
            }
            catch (Exception ex)
            {
                // skip
            }

            return languages;
        }

        /// <summary>
        /// Select a language (MLE)
        /// </summary>
        /// <param name="languages">the list of MLEs</param>
        /// <param name="preferred">a preferred language</param>
        /// <returns>selected MLE</returns>
        public static MultiLangEngine SelectLanguage(List<MultiLangEngine> languages, string preferred)
        {
            /* Determine the UI language
             * 1. Try to use preferred language
             * 2. Try to use system language
             * 3. Use languages[0]
             */
            if (languages.Count < 1) return new MultiLangEngine();
            else
            {

                MultiLangEngine theLang = null;
                languages.ForEach(l =>
                {
                    if (string.Compare(l.Language, preferred) == 0) theLang = l;
                });
                if (theLang != null)
                {
                    // 1
                    return theLang;
                }
                else
                {
                    // 2
                    string sysLang = System.Globalization.CultureInfo.CurrentUICulture.Name;
                    languages.ForEach(l =>
                    {
                        if (l.Language.ToLower().Contains(sysLang.ToLower()))
                            theLang = l;
                    });
                    if (theLang != null) return theLang;
                    else
                    {
                        // 3
                        return languages[0];
                    }
                }
            }
        }
    }
}
