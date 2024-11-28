using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleWinForms
{
    internal class Banner
    {
        private string _caption = "";

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }
    }
}
