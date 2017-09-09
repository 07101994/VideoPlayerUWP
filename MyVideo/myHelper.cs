using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyVideo
{
    class myHelper
    {
        public static StorageFile file;
        public static string abc="No Change";
        public static void setFile(StorageFile abc)
        {
            file = abc;
        }
    }
}
