using System.Linq;
using System.Text;

namespace iMenyn.Data.Helpers
{
    public class EnterpriseHelper
    {
        public static string GetId(string key)
        {
            return string.Format("enterprises-{0}",key);
        }
        public static string GenerateEnterpriseKey(string enterpriseName)
        {
            var sb = new StringBuilder();
            foreach (var c in enterpriseName.Where(c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == 'å' || c == 'Å' || c == 'ä' || c == 'Ä' || c == 'ö' || c == 'Ö'))
            {
                switch (c)
                {
                    case 'å':
                    case 'ä':
                    case 'Å':
                    case 'Ä':
                        sb.Append('a');
                        break;
                    case 'ö':
                    case 'Ö':
                        sb.Append('o');
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString().ToLower();
        }
    }
}