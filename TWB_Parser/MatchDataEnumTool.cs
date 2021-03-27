using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWB_Parser
{
    public static class MatchDataEnumTool
    {
        public static List<string> RegionList = BuildRegionList();
        public static List<string> VersionList = BuildVersionList();
        private static Dictionary<string, List<string>> _charDico = BuildCharDico();

        /// <summary>
        /// Verify that the character string exist, and transforms it into character code if necessary. 
        /// </summary>
        /// <param name="aChar"></param>
        /// <returns></returns>
        public static string TestAndResolveCharacterName(string aChar)
        {
            string output = null;
            bool found;

            if (string.IsNullOrEmpty(aChar)) throw new FormatException("The character name cannot be empty");
            else if (aChar.Length <= 2)
            {
                if (_charDico.ContainsKey(aChar.ToUpper())) output = aChar.ToUpper();
                else throw new FormatException($"Unrecognized character code: {aChar}");
            }
            else
            {
                found = false;
                foreach(KeyValuePair<string,List<string>> kvp in _charDico)
                {
                    if(kvp.Value.Contains(aChar.ToUpper()))
                    {
                        output = kvp.Key;
                        found = true;
                        break;
                    }
                }
                if (!found) throw new FormatException($"Unrecognized character name: {aChar}");
            }

            return output;
        }

        #region Private Config Loaders

        /// <summary>
        /// Loads from config the ; separated string with the different regions.
        /// </summary>
        private static List<string> BuildRegionList()
        {
            List<string> regionList = new List<string>();
            string[] splittedString = ConfigurationManager.AppSettings.Get("RegionList").Split(';');
            foreach (string region in splittedString)
            {
                regionList.Add(region);
            }
            return regionList;
        }

        /// <summary>
        /// Loads from config the ; separated string with the different versions.
        /// </summary>
        private static List<string> BuildVersionList()
        {
            List<string> versionList = new List<string>();
            string[] splittedString = ConfigurationManager.AppSettings.Get("VersionList").Split(';');
            foreach (string region in splittedString)
            {
                versionList.Add(region);
            }
            return versionList;
        }

        /// <summary>
        /// Loads from config the ; separated string with the different versions. Format: CODE:FullName:alias1:alias2:aliasN..
        /// </summary>
        private static Dictionary<string, List<string>> BuildCharDico()
        {
            Dictionary<string, List<string>> charDico = new Dictionary<string, List<string>>();
            List<string> aliasList = new List<string>();
            string[] splittedString = ConfigurationManager.AppSettings.Get("CharacterList").Split(';');
            string[] charString;
            bool skipFirst;
            foreach (string ch in splittedString)
            {
                charString = ch.Split(':');
                aliasList = new List<string>();
                if (charString.Length < 2)
                {
                    Console.WriteLine("ERROR: Error while loading the character enum string from config file: Each member must have the format \"CODE:Fullname:alias1:alias2:aliasN\".");
                    throw new FormatException("ERROR: Error while loading the character enum string from config file: Each member must have the format \"CODE:FullnameCODE:Fullname:alias1:alias2:aliasN\".");
                }

                skipFirst = true;
                foreach(string name in charString)
                {
                    if(skipFirst) skipFirst = false;
                    else
                    {
                        aliasList.Add(name.ToUpper());
                    }

                }
                charDico.Add(charString[0].ToUpper(), aliasList);
            }
            return charDico;
        }

        #endregion

    }
}
