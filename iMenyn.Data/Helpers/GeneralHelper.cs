using System;
using System.Collections.Generic;
using System.Linq;
using iMenyn.Data.Models;

namespace iMenyn.Data.Helpers
{
    public class GeneralHelper
    {
        public static string GetGuid()
        {
            return Guid.NewGuid().ToString("N");
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

        /// <summary>
        /// Get objects that exist in list1 but not list2
        /// </summary>
        /// <param name="list1">List 1</param>
        /// <param name="list2">List 2</param>
        /// <returns>Objects that exist in list1 but not list2</returns>
        public static List<T> CompareLists<T>(List<T> list1, List<T> list2)
        {
            return list1.Where(p => !list2.Select(p1 => p1).Contains(p)).ToList();
        } 

    }
}