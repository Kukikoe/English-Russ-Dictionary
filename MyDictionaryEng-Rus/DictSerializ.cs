using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using MyDictionaryEng_Rus;

namespace MyDictionaryEng_Rus
{
    class DictSerializ
    {
        DictionaryEng_Rus<string, string> dict;
        #region ctors
        /// <summary>
        /// Constructor
        /// </summary>
        public DictSerializ()
        {
            dict = new DictionaryEng_Rus<string, string>();
        }

        public DictSerializ(string path)
        {
            if (File.Exists(path))
            {
                string str = null;
                using (StreamReader sr = new StreamReader(path, Encoding.Default))
                {
                    str = sr.ReadToEnd();
                }
                if (str != null && str != "")
                {
                    var list = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(str);
                    dict = new DictionaryEng_Rus<string, string>(list.ToArray());
                }
                else
                    dict = new DictionaryEng_Rus<string, string>();
            }
            else
            {
                dict = new DictionaryEng_Rus<string, string>();
            }
        }
        #endregion
        #region public methods
        /// <summary>
        /// Add new pair into dictionary
        /// </summary>
        /// <param name="Rus"></param>
        /// <param name="Eng"></param>
        public void Add(string Rus, string Eng)
        {
            dict.Add(Rus, Eng); 
        }

        /// <summary>
        /// Keyword validation
        /// </summary>
        /// <param name="Rus">Searching word</param>
        /// <returns>True if dictionary contains specified key, else false</returns>
        public bool ContainsKey(string Rus)
        {
            if (dict == null) return false;
            return dict.ContainsKey(Rus);
        }

        /// <summary>
        /// Return translate of the Englist keyword
        /// </summary>
        /// <param name="Rus">Russian KeyWord to translating</param>
        /// <returns>Return English translate</returns>
        public string GetTranslate(string Rus)
        {
            if (ContainsKey(Rus))
                return dict[Rus];
            else return null;
        }

        /// <summary>
        /// Serialize all items of current dictionary in string using Json format
        /// </summary>
        /// <returns>Serialize of current object</returns>
        public string SerializeToString()
        {
            var list = dict.GetListItemsWithQueue();
            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// Serializr all items of current dictionary in file, using Json format
        /// </summary>
        /// <param name="path">Path to file destination</param>
        public void SerializeToFile(string path)
        {
            string str = JsonConvert.SerializeObject(dict.GetListItemsWithQueue());

            if (!File.Exists(path))
                File.Create(path);

            using (StreamWriter sw = new StreamWriter(path, false, Encoding.Default))
            {
                sw.WriteLine(str);
            }
        }
        #endregion
    }
}
