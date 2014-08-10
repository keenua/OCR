using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace OCR
{
    public class MemoryFont : Font
    {
        /// <summary>Зображення усіх символів</summary>
        protected List<Bitmap> bmps;
        /// <summary>Коди усіх символів у текстовому форматі</summary>
        protected List<string> charCodes;

        /// <summary>Задає список усіх можливих символів шрифта</summary>
        protected override void SetFullCharList()
        {
            fullCharList = new List<char>();

            foreach (string c in charCodes)
            {
                int cCode = Convert.ToInt32(c);

                fullCharList.Add((char)cCode);
            }
        }

        /// <summary>Повертає зображення символа</summary>
        /// <param name="c">Символ</param>
        protected override Bitmap GetChar(char c)
        {
            for (int i = 0; i < fullCharList.Count; i++) if (fullCharList[i] == c) return bmps[i];

            return null;
        }

        /// <summary>Задає висоту зображення</summary>
        protected override void SetImageHeight()
        {
            _imageHeight = bmps[0].Height;
        }

        /// <summary>Конструктор шрифта</summary>
        /// <param name="directory">Шлях до теки із зображеннями символів</param>
        public MemoryFont(string name, List<Bitmap> bmps, List<string> charCodes)
        {
            if (bmps.Count != charCodes.Count) throw new Exception("Кількість зображень символів та кількість кодів повинна бути рівною");

            _name = name;

            this.bmps = bmps;
            this.charCodes = charCodes;
        }
    }
}
