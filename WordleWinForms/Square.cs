using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordleWinForms.Enums;

namespace WordleWinForms
{
    internal class Square
    {
        private Status _status;
        private char _letter;

        public Status Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public char Letter
        {
            get { return _letter; }
            set { _letter = value; }
        }

        public Square()
        {
            _status = Status.Inactive;
            _letter = ' ';
        }
    }
}
