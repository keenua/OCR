using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace OCR
{
    public class CustomFont : Font
    {
        /// <summary>Шлях до теки із зображеннями символів шрифта</summary>
        protected string directory;

        /// <summary>Усі дозволені розширення файлів зображень</summary>
        static string[] allowedExtensions = new string[] { ".bmp" };

        /// <summary>Повертає шляхи до усіх файлів зображень у теці шрифта</summary>
        protected List<string> GetAllImageFiles()
        {
            List<string> result = new List<string>();

            string[] files = Directory.GetFiles(directory);

            foreach (string f in files)
            {
                string ext = Path.GetExtension(f).ToLower();

                if (allowedExtensions.Contains(ext))
                {
                    result.Add(f);
                }
            }

            return result;
        }

        /// <summary>Задає список усіх можливих символів шрифта</summary>
        protected override void SetFullCharList()
        {
            fullCharList = new List<char>();

            foreach (string f in GetAllImageFiles())
            {
                string c = Path.GetFileNameWithoutExtension(f);

                int cCode = Convert.ToInt32(c);

                fullCharList.Add((char)cCode);
            }
        }

        /// <summary>Повертає зображення символа</summary>
        /// <param name="c">Символ</param>
        protected override Bitmap GetChar(char c)
        {
            int code = (int)c;

            string path = directory + code + ".bmp";

            return new Bitmap(path);
        }

        /// <summary>Задає висоту зображення</summary>
        protected override void SetImageHeight()
        {
            List<string> allImageFiles = GetAllImageFiles();

            if (allImageFiles.Count < 0) return;

            string f = allImageFiles[0];
         
            Bitmap bmp = new Bitmap(f);

            _imageHeight = bmp.Height;

            bmp.Dispose();
        }

        /// <summary>Конструктор шрифта</summary>
        /// <param name="directory">Шлях до теки із зображеннями символів</param>
        public CustomFont(string name, string directory)
        {
            _name = name;

            if (!directory.EndsWith("\\")) directory = directory + '\\';

            this.directory = directory;
        }
    }
}
