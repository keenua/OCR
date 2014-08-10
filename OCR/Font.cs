using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using GraphicsProcessor;

namespace OCR
{
    /// <summary>Символ разом зі своїм зображенням</summary>
    public class Char
    {
        /// <summary>Зображення символа</summary>
        public Bitmap bmp;
        /// <summary>Символ</summary>
        public char c;

        /// <summary>Конструктор за зображенням та символом</summary>
        /// <param name="bmp">Посилання на зображення</param>
        /// <param name="c">Символ</param>
        public Char(Bitmap bmp, char c)
        {
            this.bmp = bmp;
            this.c = c;
        }
    }

    /// <summary>Список символів однакової ширини</summary>
    public class CharCollection
    {
        // Символи
        public List<Char> chars;
        // Ширина кожного символа
        public int width;

        /// <summary>Конструктор за шириною символів</summary>
        /// <param name="width">Ширина символів</param>
        public CharCollection(int width)
        {
            chars = new List<Char>();
            this.width = width;
        }
    }

    /// <summary>Шрифт</summary>
    public abstract class Font
    {
        /// <summary>Назва шрифта (деколи назва_шрифта#висота_шрифта)</summary>
        protected string _name;
        /// <summary>Висота зображення шрифта</summary>
        protected int _imageHeight = 100;
        /// <summary>Кількість пікселів ширини у пробілі</summary>
        protected int _spaceLength;

        /// <summary>Назва шрифта (деколи назва_шрифта#висота_шрифта)</summary>
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

        /// <summary>Висота зображення шрифта</summary>
        public int ImageHeight
        {
            get
            {
                return _imageHeight;
            }
        }

        public int SpaceLength
        {
            get
            {
                return _spaceLength;
            }
        }

        /// <summary>Показує, чи завантажений шрифт</summary>
        protected bool loaded = false;

        /// <summary>Список масивів символів однакової ширини разом зі своїми зображеннями</summary>
        protected List<CharCollection> chars;

        /// <summary>Список усіх можливих символів у шрифті</summary>
        protected List<char> fullCharList;

        /// <summary>Функція порівняння списків символів за шириною</summary>
        /// <param name="x">Перший список</param>
        /// <param name="y">Другий список</param>
        protected int Comparison(CharCollection x, CharCollection y)
        {
            if (x.width > y.width) return -1;
            else if (x.width == y.width) return 0;
            else return 1;
        }

        /// <summary>Сортує символи за довжинами</summary>
        /// <param name="bmps">Зображення символів</param>
        /// <param name="allowedChars">Використовувані символи</param>
        protected void SortByWidths(ref List<Char> allChars)
        {
            chars = new List<CharCollection>();

            foreach (Char c in allChars)
            {
                bool placed = false;

                foreach (CharCollection cc in chars)
                    if (cc.width == c.bmp.Width)
                    {
                        cc.chars.Add(c);
                        placed = true;
                    }

                if (!placed)
                {
                    chars.Add(new CharCollection(c.bmp.Width));
                    chars[chars.Count - 1].chars.Add(c);
                }
            }

            chars.Sort(Comparison);
        }

        /// <summary>Задає список усіх можливих символів шрифта</summary>
        protected abstract void SetFullCharList();

        /// <summary>Задає висоту зображення шрифта</summary>
        protected virtual void SetImageHeight()
        {
            Bitmap d = GetChar('(');

            BitmapProcessor.Trim(ref d);

            for (int y = d.Height - 1; y >= 0; y--)
                for (int x = 0; x < d.Width; x++)
                {
                    if (d.GetPixel(x, y).R != 255)
                    {
                        _imageHeight = y + 1;
                        return;
                    }
                }
        }

        /// <summary>Задає ширину пробіла</summary>
        protected void SetSpaceLength()
        {
            _spaceLength = (int)Math.Round((double)_imageHeight / 4.0);
            if (_spaceLength < 1) _spaceLength = 1;
        }

        /// <summary>Повертає зображення символа</summary>
        /// <param name="c">Символ</param>
        protected abstract Bitmap GetChar(char c);

        /// <summary>Повертає зображення усіх символів</summary>
        protected List<Char> GetChars()
        {
            List<Char> result = new List<Char>();

            foreach (char c in fullCharList)
            {
                Bitmap bmp = GetChar(c);
                result.Add(new Char(bmp, c));
            }

            return result;
        }

        public Size GetCharSize(char c, Color backgroundColor)
        {
            Bitmap bmp = new Bitmap(GetChar(c));

            BitmapProcessor.Trim(ref bmp);

            int minY = bmp.Height;
            int maxY = 0;
            
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color color = bmp.GetPixel(x, y);
                    if (!BitmapProcessor.ColorsEqual(color, backgroundColor))
                    {
                        if (y < minY) minY = y;
                        if (y > maxY) maxY = y;
                        break;
                    }
                }
            }

            int width = bmp.Width;

            bmp.Dispose();

            return new Size(width, maxY - minY + 1);
        }

        /// <summary>Завантажує шрифт</summary>
        public void Load()
        {
            SetFullCharList();

            SetImageHeight();

            SetSpaceLength();

            List<Char> allChars = GetChars();

            SortByWidths(ref allChars);

            loaded = true;
        }

        /// <summary>Експортує усі символи щрифта у виді bmp файлів у задану теку</summary>
        /// <param name="exportDir">Тека для експорту</param>
        public void ExportChars(string exportDir)
        {
            if (!Directory.Exists(exportDir)) return;

            foreach (CharCollection cc in chars)
                foreach (Char c in cc.chars)
                {
                    c.bmp.Save(exportDir + "\\" + (int)c.c + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                }
        }

        /// <summary>Розпінає текст</summary>
        /// <param name="bmp">Зображення для розпізнавання</param>
        /// <param name="allowedChars">Список дозволених символів</param>
        /// <param name="textColor">Колір, яким написаний текст</param>
        public string RecognizeText(ref Bitmap bmp, char[] allowedChars, Color textColor, object maxDiff, int maxColorDiff, Size expand)
        {
            if (!loaded) Load();

            if (textColor != Color.Empty) 
            {
                BitmapProcessor.BlackAndWhite(ref bmp, textColor, maxColorDiff);
            }

            Bitmap expanded = BitmapProcessor.Expand(bmp, expand);
            bmp.Dispose();
            bmp = expanded;

            BitmapProcessor.Trim(ref bmp, 2);

            string result = new string(' ', bmp.Width);

            if (BitmapProcessor.IsEmpty(bmp)) return "";

            foreach (CharCollection cc in chars)
                foreach (Char c in cc.chars)
                {
                    if (allowedChars != null && !allowedChars.Contains(c.c)) continue;

                    int maxDiffInt = 0;

                    if (maxDiff is int) maxDiffInt = (int)maxDiff;
                    else if (maxDiff is double) maxDiffInt = (int)((double)c.bmp.Size.Width * (double)c.bmp.Size.Height * (double)maxDiff);

                    List<int> pos = BitmapProcessor.FindSmallerSubimage(bmp, c.bmp, maxDiffInt, int.MaxValue, false);

                    foreach (int p in pos)
                    {
                        bool ok = true;

                        for (int i = 0; i < c.bmp.Width; i++)
                        {
                            if (result[p + i] != ' ')
                            {
                                ok = false;
                                break;
                            }
                        }

                        if (ok)
                        {
                            result = result.Remove(p, 1);
                            result = result.Insert(p, c.c + "");

                            for (int i = 1; i < c.bmp.Width; i++)
                            {
                                if (result[p + i] == ' ')
                                {
                                    result = result.Remove(p + i, 1);
                                    result = result.Insert(p + i, "^");
                                }
                            }
                        }
                    }
                }

            result = result.Replace("^", "");
            result = result.Replace(new string(' ', _spaceLength), "`");
            result = result.Replace(" ", "");
            result = result.Replace("`", " ");
            if (allowedChars != null && !allowedChars.Contains(' ')) result = result.Replace(" ", "");
            result = result.Trim();

            return result;
        }

        public string RecognizeText(ref Bitmap bmp, char[] allowedChars, Color textColor, object maxDiff, int maxColorDiff)
        {
            return RecognizeText(ref bmp, allowedChars, textColor, maxDiff, maxColorDiff, new Size(0, 0));
        }

        /// <summary>Розпінає текст</summary>
        /// <param name="bmp">Зображення для розпізнавання</param>
        /// <param name="allowedChars">Список дозволених символів</param>
        public string RecognizeText(ref Bitmap bmp, char[] allowedChars)
        {
            return RecognizeText(ref bmp, allowedChars, Color.Empty, 0, 0);
        }

        /// <summary>Розпінає текст</summary>
        /// <param name="bmp">Зображення для розпізнавання</param>
        /// <param name="textColor">Колір, яким написаний текст</param>
        public string RecognizeText(ref Bitmap bmp, Color textColor)
        {
            return RecognizeText(ref bmp, null, textColor, 0, 0);
        }

        /// <summary>Розпінає текст</summary>
        /// <param name="bmp">Зображення для розпізнавання</param>
        public string RecognizeText(ref Bitmap bmp)
        {
            return RecognizeText(ref bmp, null);
        }

        public void TrimBy(char c, int addToTop, int addToBottom)
        {
            if (!loaded) Load();

            Bitmap image = GetChar(c);

            int top = -1;
            int bottom = image.Height - 1;

            for (int y = 0; y < image.Height; y++)
            {
                bool free = true;

                for (int x = 0; x < image.Width; x++)
                {
                    if (BitmapProcessor.ColorsEqual(image.GetPixel(x, y), Color.Black))
                    {
                        free = false;
                        break;
                    }
                }

                if (free)
                {
                    if (top == y - 1) top = y;
                }
                else
                {
                    bottom = y;
                }
            }

            foreach (CharCollection cc in chars)
                foreach (Char ch in cc.chars)
                {
                    Bitmap bmp = ch.bmp;
                    BitmapProcessor.Copy(ref bmp, new Rectangle(0, Math.Max(top + 1 - addToTop, 0), bmp.Width, Math.Min(bottom - top + addToBottom, bmp.Height - 1)));
                    ch.bmp = bmp;
                }
        }
    }
}
