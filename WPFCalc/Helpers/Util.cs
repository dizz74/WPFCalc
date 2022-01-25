using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCalc
{
    public static class Util
    {
        static bool zDEBUG = true;
        public static void Debug(string v)
        {
           if (zDEBUG) Console.WriteLine(v);
        }

    }
}
