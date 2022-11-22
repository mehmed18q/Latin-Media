using System;
using System.Collections.Generic;
using System.Text;

namespace LatinMedia.Core.Genertors
{
   public class GeneratorName
    {
        public static string GenrateUniqeCode()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
