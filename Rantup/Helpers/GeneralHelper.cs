using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Rantup.Data.Models;
using Rantup.Web.Models;

namespace Rantup.Web.Helpers
{
    public class GeneralHelper
    {
        static readonly Random _random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[_random.Next(s.Length)])
                          .ToArray());
            return result;
        }

        public static List<ValueAndText> GetCountyNameAndCodes()
        {
            var countyNameAndCodes = new List<ValueAndText>
                                         {
                                             new ValueAndText{Value = "AB",Text = "Stockholm"},
                                             new ValueAndText{Value = "AC",Text = "Västerbotten"},
                                             new ValueAndText{Value = "BD",Text = "Norrbotten"},
                                             new ValueAndText{Value = "C",Text = "Uppsala"},
                                             new ValueAndText{Value = "D",Text = "Södermanland"},
                                             new ValueAndText{Value = "E",Text = "Östergötland"},
                                             new ValueAndText{Value = "F",Text = "Jönköping"},
                                             new ValueAndText{Value = "G",Text = "Kronoberg"},
                                             new ValueAndText{Value = "H",Text = "Kalmar"},
                                             new ValueAndText{Value = "I",Text = "Gotland"},
                                             new ValueAndText{Value = "K",Text = "Blekinge"},
                                             new ValueAndText{ Value = "M",Text = "Skåne"},
                                             new ValueAndText{ Value = "N",Text = "Halland"},
                                             new ValueAndText{ Value = "O",Text = "Västra Götaland"},
                                             new ValueAndText{ Value = "S",Text = "Värmland"},
                                             new ValueAndText{ Value = "T",Text = "Örebro"},
                                             new ValueAndText{ Value = "U",Text = "Västmanland"},
                                             new ValueAndText{ Value = "W",Text = "Dalarna"},
                                             new ValueAndText{ Value = "X",Text = "Gävleborg"},
                                             new ValueAndText{ Value = "Y",Text = "Västernorrland"},
                                             new ValueAndText{ Value = "Z",Text = "Jämtland"}
                                         };

            return countyNameAndCodes.OrderBy(c => c.Text).ToList();
        }

        public static List<ValueAndText> GetCategories()
        {
            var categories = new List<ValueAndText>
                                 {
                                             new ValueAndText{Value = "African",Text = "Afrikansk"},
                                             new ValueAndText{Value = "American",Text = "Amerikansk"},
                                             new ValueAndText{Value = "AsianFusion",Text = "Asiatisk, fusion"},
                                             new ValueAndText{Value = "Bar",Text = "Bar"},
                                             new ValueAndText{Value = "Cafe",Text = "Café"},
                                             new ValueAndText{Value = "Chinese",Text = "Kinesisk"},
                                             new ValueAndText{Value = "European",Text = "Europeisk"},
                                             new ValueAndText{Value = "FastFood",Text = "Snabbmat"},
                                             new ValueAndText{Value = "French",Text = "Fransk"},
                                             new ValueAndText{Value = "German",Text = "Tysk"},
                                             new ValueAndText{Value = "HotDog",Text = "Varmkorv"},
                                             new ValueAndText{Value = "Halal",Text = "Halal"},
                                             new ValueAndText{Value = "Indian",Text = "Indisk"},
                                             new ValueAndText{Value = "Indonesian",Text = "Indonesisk"},
                                             new ValueAndText{Value = "Irish",Text = "Irländskt"},
                                             new ValueAndText{Value = "Italian",Text = "Italiensk"},
                                             new ValueAndText{Value = "Japanese",Text = "Japansk"},
                                             new ValueAndText{Value = "Kebab",Text = "Kebab"},
                                             new ValueAndText{Value = "Korean",Text = "Koreansk"},
                                             new ValueAndText{Value = "Lebanese",Text = "Libanesisk"},
                                             new ValueAndText{Value = "Mexican",Text = "Mexikansk"},
                                             new ValueAndText{Value = "Mongolian",Text = "Mongolisk"},
                                             new ValueAndText{Value = "Moroccan",Text = "Marockansk"},
                                             new ValueAndText{Value = "Pakistani",Text = "Pakistansk"},
                                             new ValueAndText{Value = "Pizza",Text = "Pizza"},
                                             new ValueAndText{Value = "Pasta",Text = "Pasta"},
                                             new ValueAndText{Value = "Salad",Text = "Sallad"},
                                             new ValueAndText{Value = "Sandwiches",Text = "Smörgåsar"},
                                             new ValueAndText{Value = "Scandinavian",Text = "Skandinavisk"},
                                             new ValueAndText{Value = "Seafood",Text = "Skaldjur"},
                                             new ValueAndText{Value = "Steakhouse",Text = "Steakhouse"},
                                             new ValueAndText{Value = "Sushi",Text = "Sushi"},
                                             new ValueAndText{Value = "Taiwanese",Text = "Taiwanesisk"},
                                             new ValueAndText{Value = "Swedish",Text = "Svensk"},
                                             new ValueAndText{Value = "Tapas",Text = "Tapas"},
                                             new ValueAndText{Value = "Thai",Text = "Thai"},
                                             new ValueAndText{Value = "Turkish",Text = "Turkisk"},
                                             new ValueAndText{Value = "Vegetarian",Text = "Vegetarisk"},
                                             new ValueAndText{Value = "Vietnamese",Text = "Vietnamesisk"},
                                             new ValueAndText{Value = "Wraps",Text = "Wraps"}
                                 };
            return categories.OrderBy(c => c.Text).ToList();
        }
    }
}