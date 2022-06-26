using System;
using System.Drawing;

namespace Steganography
{
    class SteganographyHelper
    {
        public enum State
        {
            Hiding,
            Filling_With_Zeros
        };

        public static Bitmap embedText(string text, Bitmap bmp)
        {
            // Вначале мы будем скрывать символы в изображении
            State state = State.Hiding;

            // содержит индекс скрываемого символа
            int charIndex = 0;

            // содержит значение символа, преобразованное в целое число
            int charValue = 0;

            // содержит индекс элемента цвета (R или G или B), который в настоящее время обрабатывается
            long pixelElementIndex = 0;

            // содержит количество нулей, добавленных при завершении процесса
            int zeros = 0;

            // пиксельные элементы
            int R = 0, G = 0, B = 0;

            // проходим через ряды
            for (int i = 0; i < bmp.Height; i++)
            {
                // проходим через каждый ряд
                for (int j = 0; j < bmp.Width; j++)
                {
                    // содержит пиксель, который в настоящее время обрабатывается
                    Color pixel = bmp.GetPixel(j, i);

                    // теперь очистим наименьший значащий бит (LSB) из каждого элемента пикселя
                    R = pixel.R - pixel.R % 2;
                    G = pixel.G - pixel.G % 2;
                    B = pixel.B - pixel.B % 2;

                    // для каждого пикселя проходим по его элементам (RGB)
                    for (int n = 0; n < 3; n++)
                    {
                        // проверим, были ли обработаны новые 8 бит
                        if (pixelElementIndex % 8 == 0)
                        {
                            // проверим, завершился ли весь процесс
                            // мы можем сказать, что она завершена, когда добавлено 8 нулей
                            if (state == State.Filling_With_Zeros && zeros == 8)
                            {
                                // накладываем последний пиксель на изображение
                                // даже если затронута только часть его элементов
                                if ((pixelElementIndex - 1) % 3 < 2)
                                {
                                    bmp.SetPixel(j, i, Color.FromArgb(R, G, B));
                                }

                                // возвращает растровое изображение со скрытым текстом
                                return bmp;
                            }

                            // проверяем, все ли символы были скрыты
                            if (charIndex >= text.Length)
                            {
                                // начинаем добавлять нули, чтобы отметить конец текста
                                state = State.Filling_With_Zeros;
                            }
                            else
                            {
                                // переходим к следующему символу и повторите процесс
                                charValue = text[charIndex++];
                            }
                        }

                        // проверим, какой элемент пикселя имеет возможность скрыть бит в своем LSB
                        switch (pixelElementIndex % 3)
                        {
                            case 0:
                                {
                                    if (state == State.Hiding)
                                    {
                                        // крайний правый бит в символе будет (charValue % 2)
                                        // чтобы поместить это значение вместо LSB пиксельного элемента
                                        // просто добавляем его в него
                                        // вспомним, что LSB элемента пикселя был очищен
                                        // перед этой операцией
                                        R += charValue % 2;

                                        // удаляем добавленный крайний правый бит символа
                                        // так, чтобы в следующий раз мы могли достичь следующего
                                        charValue /= 2;
                                    }
                                }
                                break;
                            case 1:
                                {
                                    if (state == State.Hiding)
                                    {
                                        G += charValue % 2;

                                        charValue /= 2;
                                    }
                                }
                                break;
                            case 2:
                                {
                                    if (state == State.Hiding)
                                    {
                                        B += charValue % 2;

                                        charValue /= 2;
                                    }

                                    bmp.SetPixel(j, i, Color.FromArgb(R, G, B));
                                }
                                break;
                        }

                        pixelElementIndex++;

                        if (state == State.Filling_With_Zeros)
                        {
                            // увеличиваем значение нулей до тех пор, пока оно не станет равным 8
                            zeros++;
                        }
                    }
                }
            }

            return bmp;
        }

        public static string extractText(Bitmap bmp)
        {
            int colorUnitIndex = 0;
            int charValue = 0;

            // содержит текст, который будет извлечен из изображения
            string extractedText = String.Empty;

            // проходим через ряды
            for (int i = 0; i < bmp.Height; i++)
            {
                // проходим через каждый ряд
                for (int j = 0; j < bmp.Width; j++)
                {
                    Color pixel = bmp.GetPixel(j, i);

                    // для каждого пикселя проходим по его элементам (RGB)
                    for (int n = 0; n < 3; n++)
                    {
                        switch (colorUnitIndex % 3)
                        {
                            case 0:
                                {
                                    // получим LSB из элемента пикселя (будет pixel.R % 2)
                                    // затем добавим один бит справа от текущего символа
                                    // это можно сделать следующим образом (charValue = charValue * 2)
                                    // заменить добавленный бит (значение которого по умолчанию равно 0) на
                                    // LSB элемента пикселя, простым сложением
                                    charValue = charValue * 2 + pixel.R % 2;
                                }
                                break;
                            case 1:
                                {
                                    charValue = charValue * 2 + pixel.G % 2;
                                }
                                break;
                            case 2:
                                {
                                    charValue = charValue * 2 + pixel.B % 2;
                                }
                                break;
                        }

                        colorUnitIndex++;

                        // если добавлено 8 бит, то добавим текущий символ в текст результата
                        if (colorUnitIndex % 8 == 0)
                        {
                            // обратный? конечно, поскольку каждый раз процесс происходит справа (для простоты)
                            charValue = reverseBits(charValue);

                            // может быть только 0, если это стоп-символ (8 нулей)
                            if (charValue == 0)
                            {
                                return extractedText;
                            }

                            // преобразование символьного значения из int в char
                            char c = (char)charValue;

                            // добавить текущий символ в текст результата
                            extractedText += c.ToString();
                        }
                    }
                }
            }

            return extractedText;
        }

        public static int reverseBits(int n)
        {
            int result = 0;

            for (int i = 0; i < 8; i++)
            {
                result = result * 2 + n % 2;

                n /= 2;
            }

            return result;
        }
    }
}
