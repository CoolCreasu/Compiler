using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    internal class Emitter
    {
        private string _fullPath;
        private string _header;
        private string _code;

        /// <summary>
        /// Constructor
        /// </summary>
        public Emitter(string fullPath)
        {
            _fullPath = fullPath;
            _header = "";
            _code = "";
        }

        public void Emit(string code)
        {
            _code += code;
        }

        public void EmitLine(string code)
        {
            _code += code + '\n';
        }

        public void HeaderLine(string code)
        {
            _header += code + '\n';
        }

        public void WriteFile()
        {
            int index = _fullPath.LastIndexOf("/");
            var path = _fullPath;
            if (index >= 0)
            {
                path = _fullPath.Substring(0, index);
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //File.Create(_fullPath);
            File.WriteAllText(_fullPath, _header + _code);
        }
    }
}
