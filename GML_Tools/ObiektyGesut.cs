using System.Collections.Generic;

namespace GML_Tools
{
    public class ObiektyGesut : Dictionary<string, string>
    {
        public string GetIstnienie(string gmlId)
        {
            return this[gmlId];
        }
    }
}
