using System;
using System.Drawing;
using System.Text;

namespace GreenOnions.Gallery.Common
{
    public class VerifyCodeHelper
    {
        public static Bitmap CreateVerifyCodeImage(out string code)
        {
            Bitmap bitmap = new(200, 60);
            Graphics graph = Graphics.FromImage(bitmap);
            graph.FillRectangle(new SolidBrush(Color.White), 0, 0, 200, 60);
            Font font = new(FontFamily.GenericSerif, 48, FontStyle.Bold, GraphicsUnit.Pixel);
            Random r = new();
            code = CreatgeVerifyCodeString(5, (letter, x) =>
            {
                graph.DrawString(letter, font, new SolidBrush(Color.Black), x * 38, r.Next(0, 15));
            });

            Pen linePen = new(new SolidBrush(Color.Black), 2);
            for (int x = 0; x < 6; x++)
                graph.DrawLine(linePen, new Point(r.Next(0, 199), r.Next(0, 59)), new Point(r.Next(0, 199), r.Next(0, 59)));
            return bitmap;
        }

        public static string CreatgeVerifyCodeString(int length, Action<string, int> DrawLetter = null)
        {
            Random r = new();
            string letters = "ABCDEFGHIJKLMNPQRSTUVWXYZ0123456789";

            StringBuilder sb = new();

            for (int x = 0; x < length; x++)
            {
                string letter = letters.Substring(r.Next(0, letters.Length - 1), 1);
                sb.Append(letter);
                DrawLetter?.Invoke(letter, x);
            }

            return sb.ToString();
        }
    }
}
