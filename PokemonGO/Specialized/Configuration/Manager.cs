using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGO.Specialized.Configuration
{
    internal class Manager
    {
        private Dictionary<string, string> Values;

        public Manager(string Source)
        {
            Values = new Dictionary<string, string>();

            Init(Source);
        }

        private void Init(string Source)
        {
            try
            {
                var Lines = File.ReadAllLines(Source);

                foreach (var Line in Lines)
                {
                    if (!Line.StartsWith("//"))
                    {
                        var Data = Line.Split('=');

                        if (Data.Count() == 2)
                        {
                            Values.Add(Data[0], Data[1]);
                        }
                    }
                }
            }
            catch { }
        }

        internal T GetValue<T>(string Key)
        {
            try
            {
                return (T)Convert.ChangeType(Values[Key], typeof(T));
            }
            catch { }

            return default(T);
        }
    }
}
