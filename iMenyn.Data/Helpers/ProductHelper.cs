using System;
using System.Collections.Generic;
using System.Linq;

namespace iMenyn.Data.Helpers
{
    public class ProductHelper
    {
        public static string GenerateId()
        {
            return string.Format("products-{0}", Guid.NewGuid().ToString("N"));
        }
    }
}