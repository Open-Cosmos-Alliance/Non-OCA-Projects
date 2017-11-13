using System;
using System.Collections.Generic;
using System.Text;

namespace vfslab
{
    public class file
    {
        public file()
        {
        }

        public override string ToString()
        {
            return _name + " - " + _content;
        }

        public int fileSize()
        {
            int final = 0;

            for (int i = 0; i < _content.Length; i++)
            {
                final += 7;
            }

            final = final / 8;

            return final;
        }

        public string FileNameAndSize()
        {
            string final = "";

            final = _name + " - " + fileSize().ToString() + " bytes";

            return final;
        }

        private string _name;
        private string _content;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }

    }
}
