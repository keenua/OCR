using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace OCR
{
    public class TextRecognition
    {
        List<Font> fonts;

        public void AddFont(Font font)
        {
            font.Load();
            fonts.Add(font);
        }

        Font GetFont(string fontName)
        {
            foreach (Font f in fonts)
            {
                if (f.Name == fontName) return f;
            }            

            return null;
        }

        public void ExportChars(string fontName, string exportDir)
        {
            Font font = GetFont(fontName);

            if (font == null) return;

            font.ExportChars(exportDir);
        }

        public void TrimBy(string fontName, char trimBy)
        {
            Font font = GetFont(fontName);

            if (font == null) return;

            font.TrimBy(trimBy, 0, 0);
        }

        public void TrimAllBy(char trimBy, int addToTop, int addToBottom)
        {
            foreach (Font f in fonts)
            {
                f.TrimBy(trimBy, addToTop, addToBottom);
            }
        }

        public void TrimAllBy(char trimBy)
        {
            TrimAllBy(trimBy, 0, 0);
        }

        public string Recognize(string[] fontNames, ref Bitmap bmp, char[] allowedChars, Color textColor, object maxDiff, int maxColorDiff, Size expand)
        {
            Color color = textColor;

            foreach (string font in fontNames)
            {
                Bitmap newBmp = new Bitmap(bmp);
                string res = Recognize(font, ref newBmp, allowedChars, color, maxDiff, maxColorDiff, expand);
                newBmp.Dispose();

                if (res != "") return res;
            }

            return "";
        }

        public string Recognize(string[] fontNames, ref Bitmap bmp, char[] allowedChars, Color textColor, object maxDiff, int maxColorDiff)
        {
            return Recognize(fontNames, ref bmp, allowedChars, textColor, maxDiff, maxColorDiff, new Size(0, 0));
        }

        public string Recognize(string fontName, ref Bitmap bmp, char[] allowedChars, Color textColor, object maxDiff, int maxColorDiff, Size expand)
        {
            Font font = GetFont(fontName);

            if (font == null) return "";

            return font.RecognizeText(ref bmp, allowedChars, textColor, maxDiff, maxColorDiff, expand);
        }

        public string Recognize(string fontName, ref Bitmap bmp, char[] allowedChars, Color textColor, object maxDiff, int maxColorDiff)
        {
            return Recognize(fontName, ref bmp, allowedChars, textColor, maxDiff, maxColorDiff, new Size(0, 0));
        }

        public string Recognize(string[] fontNames, ref Bitmap bmp, char[] allowedChars)
        {
            return Recognize(fontNames, ref bmp, allowedChars, Color.Empty, 0, 0);
        }

        public string Recognize(string fontName, ref Bitmap bmp, char[] allowedChars)
        {
            return Recognize(fontName, ref bmp, allowedChars, Color.Empty, 0, 0);
        }

        public string Recognize(string[] fontNames, ref Bitmap bmp, Color textColor)
        {
            return Recognize(fontNames, ref bmp, null, textColor, 0, 0);
        }

        public string Recognize(string fontName, ref Bitmap bmp, Color textColor)
        {
            return Recognize(fontName, ref bmp, null, textColor, 0, 0);
        }

        public string Recognize(string[] fontNames, ref Bitmap bmp)
        {
            return Recognize(fontNames, ref bmp, null, Color.Empty, 0, 0);
        }

        public string Recognize(string fontName, ref Bitmap bmp)
        {
            return Recognize(fontName, ref bmp, null, Color.Empty, 0, 0);
        }

        public TextRecognition()
        {
            fonts = new List<Font>();
        }
    }
}
