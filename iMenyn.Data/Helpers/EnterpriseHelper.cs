using System.Linq;
using System.Text;
using iMenyn.Data.Abstract.Db;

namespace iMenyn.Data.Helpers
{
    public class EnterpriseHelper
    {
        /// <summary>
        /// Get the Id for the document
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetId(string key)
        {
            return string.Format("enterprises-{0}",key);
        }

        /// <summary>
        /// Get the KEY from the document-id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetKey(string id)
        {
            return id.Replace("enterprises-", string.Empty);
        }

        public static string GenerateEnterpriseKey(string enterpriseName, IDbEnterprises dbEnterprises)
        {
            var key = string.Empty;

            var enterpriseExist = true;

            var index = 0;
            while(enterpriseExist)
            {
                key = PossibleEnterPriseKey(enterpriseName, index);
                enterpriseExist = dbEnterprises.GetEnterpriseById(GetId(key)) != null;

                index++;
            }

            return key;
        }

        private static string PossibleEnterPriseKey(string enterpriseName,int amout)
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
            var possibleKey = sb.ToString().ToLower();

            //Increment possible key if it one with same key exists. Ex. jensens1
            if (amout > 0)
                possibleKey += amout;
            if (amout > 28)
            {
                //Extreme case! If there are over 28 documents with the keys tried, use a random string instead. Ravendb has a limit of 30 requests per client session
                possibleKey += GeneralHelper.RandomString(8);
            }
                
            return possibleKey;
        }
    }
}