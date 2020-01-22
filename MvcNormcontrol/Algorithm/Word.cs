using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Word;

namespace MvcNormcontrol.Algorithm
{
    class WordDocument
    {
        private static Application WordApp;
        private static Document WordDoc;
        private static Paragraphs Paragraphs;
        private static Microsoft.Office.Interop.Word.Range WordRange;
        private static Style StyleParagraph;
        private static Style StyleHeaderPicture;
        private static Style StyleMainHeader;
        private static Style StyleHeader1;
        private static Style StyleHeader2;
        private static Style StyleHeader3;
        private static Style StyleBeforeTables;

        enum Errors
        {
            NoTableOfContents,
            ProblemWithHeaders,
            LittleText,
            FileError
        }
        public static bool WorkWithDocument(string adress, ref string errors)
        {
            WordApp = new Application();
            WordDoc = WordApp.Documents.Open(adress);
            Paragraphs = WordDoc.Paragraphs;
            WordRange = WordDoc.Range();
            try { StyleParagraph = WordDoc.Styles["Стиль Параграфа"]; }
            catch { StyleParagraph = WordDoc.Styles.Add("Стиль параграфа", 1); }
            try { StyleMainHeader = WordDoc.Styles["Стиль основного заголовка"]; }
            catch { StyleMainHeader = WordDoc.Styles.Add("Стиль основного заголовка", 1); }
            try { StyleHeaderPicture = WordDoc.Styles["Стиль подписи"]; }
            catch { StyleHeaderPicture = WordDoc.Styles.Add("Стиль подписи", 1); }
            try { StyleHeader1 = WordDoc.Styles["Стиль заголовка 1"]; }
            catch { StyleHeader1 = WordDoc.Styles.Add("Стиль заголовка 1", 1); }
            try { StyleHeader2 = WordDoc.Styles["Стиль заголовка 2"]; }
            catch { StyleHeader2 = WordDoc.Styles.Add("Стиль заголовка 2", 1); }
            try { StyleHeader3 = WordDoc.Styles["Стиль заголовка 3"]; }
            catch { StyleHeader3 = WordDoc.Styles.Add("Стиль заголовка 3", 1); }
            try { StyleBeforeTables = WordDoc.Styles["Стиль подписи таблицы"]; }
            catch { StyleBeforeTables = WordDoc.Styles.Add("Стиль подписи таблицы", 1); }


            InitializationStyleParagraph();
            InitializationStyleHeaderPicture();
            InitializationStyleHeader1();
            InitializationStyleHeader2();
            InitializationStyleHeader3();
            InitializationStyleMainHeader();
            InitializationStyleBeforeTables();
            try
            {
                ParagraphsInDocument();
                errors = "";
                if (CheckTablesOfContents()) errors += " " + (int)Errors.NoTableOfContents;
                if (CheckHeaders()) errors += " " + (int)Errors.ProblemWithHeaders;
                if (AmountOfText()) errors += " " + (int)Errors.LittleText;
                WordApp.ActiveDocument.Close();
                WordApp.Quit();
                if (errors != "") return true;
                else return false;
            }
            catch
            {
                WordApp.ActiveDocument.Close(WdSaveOptions.wdDoNotSaveChanges);
                WordApp.Quit();
                errors += " " + (int)Errors.FileError;
                return true;
            }
        }

        public static List<string> ErrorsForOut(string inStringErrors)
        {
            var arrayInErrors = inStringErrors.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var errorsForOut = new List<string>();
            for (int i = 0; i < arrayInErrors.Length; i++)
            {
                if (int.Parse(arrayInErrors[i]) == (int)Errors.NoTableOfContents)
                    errorsForOut.Add("Нет содержания, или оно создано не автоматически.");
                if (int.Parse(arrayInErrors[i]) == (int)Errors.ProblemWithHeaders)
                    errorsForOut.Add("Нет заголовков, основанных на стиле \"Заголовок\", или нет одного из основных заголовков:\"ВВЕДЕНИЕ\",\"ЗАКЛЮЧЕНИЕ\",\"СПИСОК ИСПОЛЬЗОВАННЫХ ИСТОЧНИКОВ\".");
                if (int.Parse(arrayInErrors[i]) == (int)Errors.LittleText)
                    errorsForOut.Add("На одной из страниц отсутствует достаточное количество текста. Текст должен занимать больше половины страницы (картинки за текст не считаются).");
                if (int.Parse(arrayInErrors[i]) == (int)Errors.FileError)
                    errorsForOut.Add("Ошибка с прочтением-сохранением файла");
            }
            return errorsForOut;
        }

        //проверка на заголовки
        private static bool CheckHeaders()
        {
            var count1 = 0;
            var count2 = 0;
            for (int i = 1; i < WordDoc.Paragraphs.Count; i++)
            {
                var paragraph = Paragraphs[i];
                var styleParagraph = (paragraph.get_Style() as Style).NameLocal;
                if (styleParagraph == "Стиль заголовка 1" || styleParagraph == "Стиль заголовка 2" ||
                    styleParagraph == "Стиль заголовка 3" || styleParagraph == "Стиль основного заголовка")
                {
                    count1++;
                    if (paragraph.Range.Text == "ВВЕДЕНИЕ\r" || paragraph.Range.Text == "ЗАКЛЮЧЕНИЕ\r"
                        || paragraph.Range.Text == "СПИСОК ИСПОЛЬЗОВАННЫХ ИСТОЧНИКОВ\r")
                        count2++;
                }

            }
            if (count1 == 0 || count2 != 3)
                return true;
            return false;

        }


        //проверка на автоматическое содержание
        private static bool CheckTablesOfContents()
        {
            if (WordDoc.TablesOfContents.Count == 0)
                return true;
            return false;

        }


        //проверка на половину документа
        private static bool AmountOfText()
        {
            var page = 1;
            var line = 0;
            WordApp.Selection.HomeKey(WdUnits.wdStory);
            for (int i = 1; i < WordRange.ComputeStatistics(WdStatistic.wdStatisticLines); i++)
            {
                var count = (int)WordApp.Selection.Range.Information[WdInformation.wdActiveEndPageNumber];
                if (count == page)
                    line++;
                else
                {
                    if (line < 14 && page != 1) return true;
                    page++;
                    line = 0;
                }
                WordApp.Selection.MoveDown(WdUnits.wdLine);
            }
            return false;
        }

        //инициализация обычного параграфа
        private static void InitializationStyleParagraph()
        {
            StyleParagraph.set_BaseStyle(WordDoc.Styles["Обычный"]);
            StyleParagraph.Font.Name = "Times New Roman";
            StyleParagraph.Font.Size = 14;
            StyleParagraph.Font.Color = WdColor.wdColorBlack;
            StyleParagraph.Font.Italic = 0;
            StyleParagraph.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            StyleParagraph.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpace1pt5;
            StyleParagraph.ParagraphFormat.SpaceBefore = (float)0.01;
            StyleParagraph.ParagraphFormat.SpaceAfter = (float)0.01;
            StyleParagraph.Priority = 45;
            StyleParagraph.ParagraphFormat.FirstLineIndent = WordApp.CentimetersToPoints((float)1.25);
        }

        //инициализация заголовков под картинками
        private static void InitializationStyleHeaderPicture()
        {
            StyleHeaderPicture.set_BaseStyle(WordDoc.Styles[StyleParagraph]);
            StyleHeaderPicture.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            StyleHeaderPicture.Font.Size = 12;
            StyleHeaderPicture.Priority = 3;
            StyleHeaderPicture.ParagraphFormat.FirstLineIndent = (float)0.01;
        }
        private static void InitializationStyleBeforeTables()
        {
            StyleBeforeTables.set_BaseStyle(WordDoc.Styles[StyleParagraph]);
            StyleBeforeTables.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            StyleBeforeTables.ParagraphFormat.FirstLineIndent = (float)0.01;
        }

        //инициализация заголовков
        private static void InitializationStyleMainHeader()
        {
            StyleMainHeader.set_BaseStyle(WordDoc.Styles["Заголовок 1"]);
            StyleMainHeader.Font.Name = "Times New Roman";
            StyleMainHeader.Font.Size = 14;
            StyleMainHeader.Font.Color = WdColor.wdColorBlack;
            StyleMainHeader.ParagraphFormat.FirstLineIndent = (float)0.01;
            StyleMainHeader.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpace1pt5;
            StyleMainHeader.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            StyleMainHeader.Font.Bold = 1;
            StyleMainHeader.ParagraphFormat.SpaceBefore = (float)24;
            StyleMainHeader.ParagraphFormat.SpaceAfter = (float)0.01;
            StyleMainHeader.Priority = 1;
        }
        private static void InitializationStyleHeader1()
        {
            StyleHeader1.set_BaseStyle(WordDoc.Styles["Заголовок 1"]);
            StyleHeader1.Font.Name = "Times New Roman";
            StyleHeader1.Font.Size = 14;
            StyleHeader1.Font.Color = WdColor.wdColorBlack;
            StyleHeader1.ParagraphFormat.FirstLineIndent = WordApp.CentimetersToPoints((float)1.25);
            StyleHeader1.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpace1pt5;
            StyleHeader1.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            StyleHeader1.Font.Bold = 1;
            StyleHeader1.ParagraphFormat.SpaceBefore = (float)24;
            StyleHeader1.ParagraphFormat.SpaceAfter = (float)0.01;
            StyleHeader1.Priority = 1;
        }
        private static void InitializationStyleHeader2()
        {
            StyleHeader2.set_BaseStyle(WordDoc.Styles["Заголовок 2"]);
            StyleHeader2.Font.Name = "Times New Roman";
            StyleHeader2.Font.Size = 14;
            StyleHeader2.Font.Color = WdColor.wdColorBlack;
            StyleHeader2.ParagraphFormat.FirstLineIndent = WordApp.CentimetersToPoints((float)1.25);
            StyleHeader2.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpace1pt5;
            StyleHeader2.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            StyleHeader2.Font.Bold = 1;
            StyleHeader2.ParagraphFormat.SpaceBefore = (float)12;
            StyleHeader2.ParagraphFormat.SpaceAfter = (float)0.01;
            StyleHeader2.Priority = 1;
        }
        private static void InitializationStyleHeader3()
        {
            StyleHeader2.set_BaseStyle(WordDoc.Styles["Заголовок 3"]);
            StyleHeader2.Font.Name = "Times New Roman";
            StyleHeader2.Font.Size = 14;
            StyleHeader2.Font.Color = WdColor.wdColorBlack;
            StyleHeader2.ParagraphFormat.FirstLineIndent = WordApp.CentimetersToPoints((float)1.25);
            StyleHeader2.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpace1pt5;
            StyleHeader2.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            StyleHeader2.Font.Bold = 1;
            StyleHeader2.ParagraphFormat.SpaceBefore = (float)12;
            StyleHeader2.ParagraphFormat.SpaceAfter = (float)0.01;
            StyleHeader2.Priority = 1;
        }

        //проверка на заголовки
        static bool Headers(Paragraph paragraph, ref int i)
        {

            var styleParagraph = (paragraph.get_Style() as Style).NameLocal;
            if (styleParagraph == "Заголовок 1" || styleParagraph == "Заголовок оглавления")
            {
                var text = paragraph.Range.Text;
                if (paragraph.Range.Text == "ВВЕДЕНИЕ\r" || paragraph.Range.Text == "ЗАКЛЮЧЕНИЕ\r" || paragraph.Range.Text == "ОГЛАВЛЕНИЕ\r" ||
                    paragraph.Range.Text == "СПИСОК ИСПОЛЬЗУЕМЫХ ИСТОЧНИКОВ\r" || paragraph.Range.Text.Contains("ПРИЛОЖЕНИЕ"))
                    paragraph.set_Style(StyleMainHeader);
                else paragraph.set_Style(StyleHeader1);

                if (i - 1 > 0 && paragraph.Previous().Range.Text != "\f\r")
                {
                    Paragraphs.Add(paragraph.Range).Range.InsertBreak(WdBreakType.wdPageBreak);
                    i++;
                }

                return true;
            }
            else if (styleParagraph == "Заголовок 2")
            {
                paragraph.set_Style(StyleHeader2);
                return true;
            }
            else if (styleParagraph == "Заголовок 3")
            {
                paragraph.set_Style(StyleHeader3);
                return true;
            }
            return false;
        }

        //проверка на картинку
        static bool Images(Paragraph paragraph, ref int i)
        {
            if (paragraph.Range.InlineShapes.Count > 0)
            {
                paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                if (i < Paragraphs.Count - 1)
                {
                    paragraph.Next().set_Style(StyleHeaderPicture);
                    i++;
                    return true;
                }
            }
            if (paragraph.Range.OMaths.Count > 0)
                return true;
            return false;


        }
        static void ParagraphsInDocument()
        {
            WordRange.Underline = WdUnderline.wdUnderlineNone;
            WordRange.PageSetup.LeftMargin = WordApp.CentimetersToPoints((int)3);
            WordRange.PageSetup.RightMargin = WordApp.CentimetersToPoints((float)1.5);
            WordRange.PageSetup.TopMargin = WordApp.CentimetersToPoints((int)2);
            WordRange.PageSetup.BottomMargin = WordApp.CentimetersToPoints((int)2);
            WordRange.PageSetup.HeaderDistance = (float)0.01;

            for (int i = 1; i < WordDoc.Paragraphs.Count; i++)
            {
                var paragraph = Paragraphs[i];
                Style style = (Style)paragraph.get_Style();
                if (style.NameLocal.Contains("Оглавление"))
                {
                    paragraph.Range.Font.Name = "Times New Roman";
                    paragraph.Range.Font.Size = 14;
                    paragraph.Range.Font.Color = WdColor.wdColorBlack;
                    paragraph.Range.Font.Italic = 0;
                    continue;
                }
                if (Headers(paragraph, ref i))
                    continue;
                if (Table(paragraph))
                    continue;
                if (Images(paragraph, ref i))
                {
                    continue;
                }
                if (paragraph.Range.Text == "\r" && !((Style)paragraph.Previous().get_Style()).NameLocal.Contains("Оглавление"))
                {
                    paragraph.Range.Text = "";
                    i--;
                    continue;
                }
                Text(paragraph, i);

            }
        }

        static bool Table(Paragraph paragraph)
        {
            if (paragraph.Range.Tables.Count != 0)
            {
                if (paragraph.Previous().Range.Tables.Count == 0)
                    paragraph.Previous().set_Style(StyleBeforeTables);
                return true;
            }
            return false;
        }

        //основное форматирование
        static void Text(Paragraph paragraph, int i)
        {
            paragraph.Range.Font.Reset();
            paragraph.Range.set_Style(StyleParagraph);
        }

    }
}
