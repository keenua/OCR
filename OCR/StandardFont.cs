using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using Microsoft.Win32;
using GraphicsProcessor;

namespace OCR
{
    public class StandardFont : Font
    {
        /// <summary>Сім’я шрифта</summary>
        protected FontFamily family;
        /// <summary>Висота шрифта</summary>
        protected float fontHeight;
        /// <summary>Кількість пустих стовпців, які потрібно залишати навколо символів</summary>
        protected Dictionary<char, int> leave = new Dictionary<char, int>();

        protected FontStyle fontStyle;

        protected char[] additionalChars = new char[0];

        /// <summary>Задає список усіх можливих символів шрифта</summary>
        protected override void SetFullCharList()
        {
            fullCharList = new List<char>();

            for (int i = 33; i < 127; i++) fullCharList.Add((char)i);

            foreach (char c in additionalChars) if (!fullCharList.Contains(c)) fullCharList.Add(c);
        }

        /// <summary>Повертає зображення символа</summary>
        /// <param name="c">Символ</param>
        protected override Bitmap GetChar(char c)
        {
            Bitmap bitmap = new Bitmap((int)(fontHeight * 3), _imageHeight);

            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            g.DrawString(c + "", new System.Drawing.Font(family, fontHeight, fontStyle), new SolidBrush(Color.White), 0, 0);

            BitmapProcessor.BlackAndWhite(ref bitmap, Color.White);
            BitmapProcessor.Trim(ref bitmap, leave.ContainsKey(c) ? leave[c] : 0);

            return bitmap;
        }

        public static List<string> ListAllFonts()
        {
            List<string> result = new List<string>();
            string windowsDir = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "SystemRoot", null).ToString();

            RegistryKey fontsSubKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Fonts");

            foreach (string valuename in fontsSubKey.GetValueNames())
            {
                result.Add(valuename);
            }


            return result;
        }

        /// <summary>Встановлює сім’ю шрифта</summary>
        /// <param name="fontName">Назва шрифта</param>
        private void SetFamily(string fontName)
        {
            PrivateFontCollection fonts = new PrivateFontCollection();
            
            string filePath = "";

            string windowsDir = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "SystemRoot", null).ToString();

            RegistryKey fontsSubKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Fonts");
            fontName = fontName.ToLower();

            var fontFile = (from v in fontsSubKey.GetValueNames() where v.ToLower().Split('(')[0].Trim() == fontName select fontsSubKey.GetValue(v).ToString()).FirstOrDefault();

            if (string.IsNullOrEmpty(fontFile))
            {
                foreach (string valuename in fontsSubKey.GetValueNames())
                {
                    if (valuename.ToLower().StartsWith(fontName) && (valuename.ToLower().Contains("bold") == fontName.Contains("bold")) && (valuename.ToLower().Contains("black") == fontName.Contains("black")) && (valuename.ToLower().Contains("italic") == fontName.Contains("italic")))
                    {
                        filePath = windowsDir + "\\fonts\\" + fontsSubKey.GetValue(valuename).ToString();
                        break;
                    }
                }
            }
            else
            {
                filePath = windowsDir + "\\fonts\\" + fontFile;
            }

            fonts.AddFontFile(filePath);

            family = (FontFamily)fonts.Families.GetValue(0);
        }

        /// <summary>Конструктор шрифта</summary>
        /// <param name="fontName">Назва шрифта</param>
        /// <param name="fontHeight">Висота шрифта</param>
        public StandardFont(string fontName, float fontHeight, FontStyle fontStyle)
            :this(fontName, fontHeight, fontStyle, new char[0])
        {
         
        }

                /// <summary>Конструктор шрифта</summary>
        /// <param name="fontName">Назва шрифта</param>
        /// <param name="fontHeight">Висота шрифта</param>
        /// <param name="additionalChars">Додаткові символи</param>
        public StandardFont(string fontName, float fontHeight, FontStyle fontStyle, params char[] additionalChars)
            :this(fontName, fontHeight, fontStyle, new Dictionary<char,int>(), additionalChars)
        {
        }

        /// <summary>Конструктор шрифта</summary>
        /// <param name="fontName">Назва шрифта</param>
        /// <param name="fontHeight">Висота шрифта</param>
        /// <param name="additionalChars">Додаткові символи</param>
        public StandardFont(string fontName, float fontHeight, FontStyle fontStyle, Dictionary<char, int> leave, params char[] additionalChars)
        {
            _imageHeight = (int)(fontHeight * 2);

            SetFamily(fontName);

            _name = fontName + "#" + fontHeight;

            this.fontHeight = fontHeight;
            this.fontStyle = fontStyle;

            this.additionalChars = additionalChars;
            this.leave = leave;
        }
    }
}
