using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace @interface
{
    public partial class Compiler : Form
    {
        public Compiler()
        {
            InitializeComponent();

            //параметры saveFileDialog1 и openFileDialog1
            openFileDialog1.FileName = string.Empty;
            saveFileDialog1.FileName= string.Empty;
            saveFileDialog1.DefaultExt = openFileDialog1.DefaultExt = "txt";
            saveFileDialog1.Filter = openFileDialog1.Filter = "Текстовые файлы(*.txt)|*.txt|Microsoft Word(*.doc)|*.doc|Заголовочный файл(*.h)|*.h" +
                "|Файл реализации(*.cpp)|*.cpp|Класс C#(*.cs)|*.cs|C(*.c)|*.c|Все файлы (*.*)|*.*";

            this.KeyDown += new KeyEventHandler(hotkeySave);
            this.KeyDown += new KeyEventHandler(hotkeySaveAs);
            this.KeyDown += new KeyEventHandler(hotkeyCreate);
            this.KeyDown += new KeyEventHandler(hotkeyOpen);
            this.KeyDown += new KeyEventHandler(hotkeyReference);
            this.KeyDown += new KeyEventHandler(hotkeyLaunch);

        }

        private void Launch()
        {
            richTextBoxOutput.Clear();
            richTextBoxOutput.Text = LexicalAnalyzer.RunScanner(richTextBoxInput.Text);
        }

        //хоткеи
        private void hotkeySave(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)//Ctrl+S
            {
                save();
            }
        }
        private void hotkeySaveAs(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Shift && e.KeyCode == Keys.S)//Ctrl+Shift+S
            {
                saveAs();
            }
        }
        private void hotkeyCreate(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.N)//Ctrl+N
            {
                savingResult();
            }
        }
        private void hotkeyOpen(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.O)//Ctrl+O
            {
                open();
            }
        }
        private void hotkeyReference(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F1)//Ctrl+F1
            {
                showHelp();
            }
        }
        private void hotkeyLaunch(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F9)//Ctrl+F9
            {
                Launch();
            }
        }

        //Справка
        private void showHelp()
        {
            Help.ShowHelp(this, "https://github.com/XETR1K/interface");
        }

        //сохранение
        private void save()
        {
            if (saveFileDialog1.FileName != string.Empty)
                File.WriteAllText(saveFileDialog1.FileName, richTextBoxInput.Text);
            else
                saveAs();
            richTextBoxInput.Modified = false;
        }

        private void saveAs()
        {
            string file;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;
                richTextBoxInput.SaveFile(filename, RichTextBoxStreamType.PlainText);
                file = Path.GetFileName(filename);
                MessageBox.Show("Файл " + file + " успешно сохранен.", "Сохранение успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            richTextBoxInput.Modified = false;
        }

        private void savingResult()
        {
            if (richTextBoxInput.Modified == true)
            //if (richTextBox1.Text != string.Empty)
            {
                DialogResult result = MessageBox.Show("Хотите ли вы сохранить свои изменения?", "Сохранить изменения?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    save();
                    saveFileDialog1.FileName = string.Empty;
                    richTextBoxInput.ResetText();
                    richTextBoxInput.Focus();
                    richTextBoxInput.Modified = false;
                }
                else
                {
                    saveFileDialog1.FileName = string.Empty;
                    richTextBoxInput.ResetText();
                    richTextBoxInput.Focus();
                    richTextBoxInput.Modified = false;
                }
            }
            else
            {
                saveFileDialog1.FileName = string.Empty;
                richTextBoxInput.ResetText();
                richTextBoxInput.Focus();
            }
        }

        //открытие
        private void open()
        {
            savingResult();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBoxInput.LoadFile(openFileDialog1.FileName, RichTextBoxStreamType.PlainText);
                saveFileDialog1.FileName = openFileDialog1.FileName;
                richTextBoxInput.Modified = false;
            }
        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            savingResult();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            open();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save();
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveAs();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            savingResult();
            this.Close();
        }

        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBoxInput.Undo();
        }

        private void вернутьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBoxInput.Redo();
        }

        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBoxInput.Cut();
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBoxInput.Copy();
        }

        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBoxInput.Paste();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBoxInput.SelectedText = string.Empty;
        }

        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBoxInput.SelectAll();
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            save();
        }

        private void createToolStripButton_Click(object sender, EventArgs e)
        {
            savingResult();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            open();
        }

        private void cancelToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBoxInput.Undo();
        }

        private void returnToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBoxInput.Redo();
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBoxInput.Copy();
        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBoxInput.Cut();
        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBoxInput.Paste();
        }

        private void вызовСправкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showHelp();
        }

        private void пускToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Launch();
        }                
        
        //Номера строк
        private void UpdateLineNumbers()
        {
            int firstVisibleCharIndex = richTextBoxInput.GetCharIndexFromPosition(new Point(0, 0));
            int firstVisibleLine = richTextBoxInput.GetLineFromCharIndex(firstVisibleCharIndex);

            richTextBoxLineNumber.Clear();

            int lineCount = richTextBoxInput.Lines.Length;
            int lastVisibleLine = firstVisibleLine + richTextBoxInput.Height / richTextBoxInput.Font.Height;

            for (int i = firstVisibleLine; i <= lastVisibleLine && i < lineCount; i++)
            {
                richTextBoxLineNumber.SelectionIndent = 5;
                richTextBoxLineNumber.AppendText((i + 1) + Environment.NewLine);
            }
        }

        //Подсветка синтаксиса
        private void HighlightSyntax(RichTextBox richTextBox)
        {
            // Определение цветов
            Color keywordColor = Color.Purple;
            Color stringColor = Color.Red;
            Color commentColor = Color.Green;
            Color typeOfDataColor = Color.DarkBlue;

            // Определение регулярных выражений для ключевых слов, строк и комментариев
            string keywordPattern = @"\b(if|else|for|while|switch)\b";
            string typeOfDataPattern = @"\b(int|float|long|short|double|string|char|bool|true|false)\b";
            string stringPattern = @"""(\\.|[^""\\])*""";
            string commentPattern = @"//.*?$|/\*.*?\*/";

            // Создание объекта регулярного выражения
            Regex keywordRegex = new Regex(keywordPattern);
            Regex typeOfDataRegex = new Regex(typeOfDataPattern);
            Regex stringRegex = new Regex(stringPattern);
            Regex commentRegex = new Regex(commentPattern, RegexOptions.Multiline);

            // Сохранение текущей позиции курсора
            int originalSelectionStart = richTextBox.SelectionStart;
            int originalSelectionLength = richTextBox.SelectionLength;

            // Сохранение текущего цвета выделенного текста
            Color selectionColor = richTextBox.ForeColor;

            // Отключение перерисовки RichTextBox
            richTextBox.SuspendLayout();

            // Итерация по всем строкам текста
            for (int i = 0; i < richTextBox.Lines.Length; i++)
            {
                string line = richTextBox.Lines[i];

                // Поиск ключевых слов
                foreach (Match keywordMatch in keywordRegex.Matches(line))
                {
                    richTextBox.Select(keywordMatch.Index + richTextBox.GetFirstCharIndexFromLine(i), keywordMatch.Length);
                    richTextBox.SelectionColor = keywordColor;
                }

                foreach (Match typeOfDataMatch in typeOfDataRegex.Matches(line))
                {
                    richTextBox.Select(typeOfDataMatch.Index + richTextBox.GetFirstCharIndexFromLine(i), typeOfDataMatch.Length);
                    richTextBox.SelectionColor = typeOfDataColor;
                }

                // Поиск строк
                foreach (Match stringMatch in stringRegex.Matches(line))
                {
                    richTextBox.Select(stringMatch.Index + richTextBox.GetFirstCharIndexFromLine(i), stringMatch.Length);
                    richTextBox.SelectionColor = stringColor;
                }

                // Поиск комментариев
                foreach (Match commentMatch in commentRegex.Matches(line))
                {
                    richTextBox.Select(commentMatch.Index + richTextBox.GetFirstCharIndexFromLine(i), commentMatch.Length);
                    richTextBox.SelectionColor = commentColor;
                }
            }

            // Восстановление выделения текста и позиции курсора
            richTextBox.Select(originalSelectionStart, originalSelectionLength);

            // Восстановление цвета выделенного текста
            richTextBox.SelectionColor = selectionColor;

            // Включение перерисовки RichTextBox
            richTextBox.ResumeLayout();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateLineNumbers();
            HighlightSyntax(richTextBoxInput);
        }

        private void richTextBox1_VScroll(object sender, EventArgs e)
        {
            UpdateLineNumbers();
        }

        private void splitContainer_Panel1_Resize(object sender, EventArgs e)
        {

        }

        private void лаб3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "https://github.com/XETR1K/interface");
        }

        private void постановкаЗадачиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Environment.CurrentDirectory + "\\html-documents\\Постановка задачи.html";
            System.Diagnostics.Process.Start(path);
        }

        private void грамматикаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Environment.CurrentDirectory + "\\html-documents\\Разработка грамматики.html";
            System.Diagnostics.Process.Start(path);
        }

        private void классификацияГрамматикиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Environment.CurrentDirectory + "\\html-documents\\Классификация грамматики.html";
            System.Diagnostics.Process.Start(path);
        }

        private void методАнализаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Environment.CurrentDirectory + "\\html-documents\\Метод анализа.html";
            System.Diagnostics.Process.Start(path);
        }

        private void диагностикаИНейтрализацияОшибкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Environment.CurrentDirectory + "\\html-documents\\Диагностика и нейтрализация синтаксических ошибок.html";
            System.Diagnostics.Process.Start(path);
        }

        private void тестовыйПримерToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Environment.CurrentDirectory + "\\html-documents\\Тестовый пример.html";
            System.Diagnostics.Process.Start(path);
        }

        private void списокЛитературыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Environment.CurrentDirectory + "\\html-documents\\Список литературы.html";
            System.Diagnostics.Process.Start(path);
        }

        private void исходныйКодПрограммыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Environment.CurrentDirectory + "\\html-documents\\Исходный код программы.html";
            System.Diagnostics.Process.Start(path);
        }
    }
}
