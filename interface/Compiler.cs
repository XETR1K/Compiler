using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace @interface
{
    public partial class Compiler : Form
    {
        public Compiler()
        {
            InitializeComponent();

            //параметры saveFileDialog1
            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.Filter = "Текстовые файлы (*.txt)|*.txt|All files (*.*)|*.*";

            this.KeyDown += new KeyEventHandler(hotkeySave);
            this.KeyDown += new KeyEventHandler(hotkeySaveAs);
            this.KeyDown += new KeyEventHandler(hotkeyCreate);
            this.KeyDown += new KeyEventHandler(hotkeyOpen);
            this.KeyDown += new KeyEventHandler(hotkeyReference);

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

        //Справка
        private void showHelp()
        {
            Help.ShowHelp(this, "https://github.com/XETR1K/interface");
        }

        //сохранение
        private void save()
        {
            if (saveFileDialog1.FileName != string.Empty)
                File.WriteAllText(saveFileDialog1.FileName, richTextBox1.Text);
            else
                saveAs();
            richTextBox1.Modified = false;
        }

        private void saveAs()
        {
            string file;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;
                richTextBox1.SaveFile(filename, RichTextBoxStreamType.PlainText);
                file = Path.GetFileName(filename);
                MessageBox.Show("Файл " + file + " успешно сохранен.", "Сохранение успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            richTextBox1.Modified = false;
        }

        private void savingResult()
        {
            if (richTextBox1.Modified == true)
            //if (richTextBox1.Text != string.Empty)
            {
                DialogResult result = MessageBox.Show("Хотите ли вы сохранить свои изменения?", "Сохранить изменения?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    save();
                    saveFileDialog1.FileName = string.Empty;
                    richTextBox1.ResetText();
                    richTextBox1.Focus();
                    richTextBox1.Modified = false;
                }
                else
                {
                    saveFileDialog1.FileName = string.Empty;
                    richTextBox1.ResetText();
                    richTextBox1.Focus();
                    richTextBox1.Modified = false;
                }
            }
            else
            {
                saveFileDialog1.FileName = string.Empty;
                richTextBox1.ResetText();
                richTextBox1.Focus();
            }
        }

        //открытие
        private void open()
        {
            savingResult();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.LoadFile(openFileDialog1.FileName, RichTextBoxStreamType.PlainText);
                saveFileDialog1.FileName = openFileDialog1.FileName;
                richTextBox1.Modified = false;
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
            richTextBox1.Undo();
        }

        private void вернутьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Redo();
        }

        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = string.Empty;
        }

        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
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
            richTextBox1.Undo();
        }

        private void returnToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Redo();
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void вызовСправкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showHelp();
        }
    }
}
