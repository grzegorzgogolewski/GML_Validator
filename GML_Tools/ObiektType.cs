using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GML_Tools
{
    public class ObiektType : Dictionary<string, ObiektAtrybuty>
    {
        public string GetEnumeration(string typObiektu, string atrybut)
        {
            string wartosci = "";

            Dictionary<string, List<string>> obiekt = this[typObiektu];

            if (obiekt.ContainsKey(atrybut))
            {
                List<string> wartosciAtrybutu = obiekt[atrybut];

                if (wartosciAtrybutu != null)
                {
                    foreach (string wartosc in wartosciAtrybutu)
                    {
                        wartosci = wartosci + wartosc + ", ";
                    }

                    wartosci = "[" + wartosci.Substring(0, wartosci.Length - 2) + "]";

                }
                else
                {
                    wartosci = "";
                }
            }
            else
            {
                wartosci = "";
            }




            return wartosci;
        }
    }
}
