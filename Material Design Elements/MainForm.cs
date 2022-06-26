using MaterialSkin;
using MaterialSkin.Controls;
using Steganography;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using Spire.Doc;

namespace Material_Design_Elements
{
    public partial class MainForm : MaterialForm
    {

        private Bitmap bmp = null;
        private Bitmap bmp2 = null;
        private string extractedText = string.Empty;
        private int crypto = 0;

        public MainForm()
        {
            try
            {
                InitializeComponent();
                var materialSkinManager = MaterialSkinManager.Instance;
                materialSkinManager.AddFormToManage(this);
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Red800, Primary.Red900, Primary.Red500, Accent.Red200, TextShade.WHITE);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog open_dialog = new OpenFileDialog();
                open_dialog.Filter = "Image Files (*.jpeg; *.jpg; *.png; *.bmp)|*.jpeg; *.jpg; *.png; *.bmp";

                if (open_dialog.ShowDialog() == DialogResult.OK)
                {
                    imagePictureBox.Image = Image.FromFile(open_dialog.FileName);
                    statusBox.Text = "Статус: Изображение загружено!";
                    MessageBox.Show("Изображение загружено!", "CobraStegano");
                }
                
            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
                statusBox.Text = "Статус: Ошибка загрузки изображения!";
            }
        }

        private void materialButton4_Click(object sender, EventArgs e)
        {
            
            if (materialRadioButton1.Checked == true)
            {
                dataTextBox.Visible = true;
                OpenFileDialog open_dialog = new OpenFileDialog();
                open_dialog.Filter = "Text Files|*.txt";

                if (open_dialog.ShowDialog() == DialogResult.OK)
                {
                    dataTextBox.Text = File.ReadAllText(open_dialog.FileName);
                    statusBox.Text = "Статус: текст загружен!";
                    MessageBox.Show("Текст загружен!", "CobraStegano");
                }
               
            }
            else if (materialRadioButton2.Checked == true)
            {

                // Convert DOCX file to XML file.
                // If you need more information about UseOffice .Net email us at:
                // support@sautinsoft.com.


                dataTextBox.Visible = false;
                SautinSoft.UseOffice u = new SautinSoft.UseOffice();
                string str = "";
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Text Files|*.doc; *.docx";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    str = dialog.FileName;
                    statusBox.Text = "Статус: документ загружен!";
                    MessageBox.Show("Документ загружен!", "CobraStegano");
                }
                else
                {
                    return;
                }

                string inpFile = str;
                string outFile = Path.GetFullPath(@"./Data/Result.xml");

                // Prepare UseOffice .Net, loads MS Word in memory.
                int ret = u.InitWord();

                // Return values:
                // 0 - Loading successfully
                // 1 - Can't load MS Word library in memory 

                if (ret == 1)
                {
                    MessageBox.Show("Error! Can't load MS Word library in memory", "CobraStegano");
                    return;
                }

                // Converting
                ret = u.ConvertFile(inpFile, outFile, SautinSoft.UseOffice.eDirection.DOCX_to_XML);


                // Release MS Word from memory
                u.CloseWord();


                // 0 - Converting successfully
                // 1 - Can't open input file. Check that you are using full local path to input file, URL and relative path are not supported
                // 2 - Can't create output file. Please check that you have permissions to write by this path or probably this path already used by another application
                // 3 - Converting failed, please contact with our Support Team
                // 4 - MS Office isn't installed. The component requires that any of these versions of MS Office should be installed: 2000, XP, 2003, 2007, 2010, 2013, 2016 or 2019.
                if (ret == 0)
                {
                    // Open the result.
                   

                }
                else
                {
                    MessageBox.Show("Ошибка загрузки файла", "CobraStegano");
                    return;
                }
                // outFile
                var xml = XElement.Load(outFile);
                dataTextBox.Text = xml.Value;
                prostotest(outFile);
            }
         //MessageBox.Show("Документ загружен!");

        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (materialRadioButton1.Checked == true)
            {
                dataTextBox.Visible = true;
                try
                {
                    crypto = 1;
                    bmp = (Bitmap)imagePictureBox.Image;
                    string text = dataTextBox.Text;
                    if (text.Equals(""))
                    {
                        MessageBox.Show("Текст не найден! Введите текст!", "CobraStegano");
                        return;
                    }

                    if (encryptCheckBox.Checked)
                    {
                        if (passwordTextBox.Text.Length < 6)
                        {
                            MessageBox.Show("Для пароля необходимо минимум 6 симоволов!", "CobraStegano");
                            return;
                        }
                        else
                        {
                            text = Crypto.EncryptStringAES(text, passwordTextBox.Text);
                        }
                    }

                    bmp = SteganographyHelper.embedText(text, bmp);
                    MessageBox.Show("Зашифровано!", "Готово!");
                    statusBox.Text = "Статус: Зашифровано!";
                   
                    MessageBox.Show("Не забудьте сохранить изображение!", "CobraStegano");
                    statusBox.ForeColor = System.Drawing.Color.OrangeRed;
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.ToString());
                }
            }
            else if (materialRadioButton2.Checked == true)
            {
                dataTextBox.Visible = false;
                try
                {
                    crypto = 1;
                    bmp = (Bitmap)imagePictureBox.Image;
                    string text = dataTextBox.Text;
                    if (text.Equals(""))
                    {
                        MessageBox.Show("Текст не найден! Введите текст!", "CobraStegano");
                        return;
                    }

                    if (encryptCheckBox.Checked)
                    {
                        if (passwordTextBox.Text.Length < 6)
                        {
                            MessageBox.Show("Для пароля необходимо минимум 6 симоволов!", "CobraStegano");
                            return;
                        }
                        else
                        {
                            text = Crypto.EncryptStringAES(text, passwordTextBox.Text);
                        }
                    }

                    bmp = SteganographyHelper.embedText(text, bmp);
                    MessageBox.Show("Зашифровано!", "CobraStegano");
                    statusBox.Text = "Статус: Зашифровано!";
   
                    MessageBox.Show("Не забудьте сохранить изображение!", "CobraStegano");
                    statusBox.ForeColor = System.Drawing.Color.OrangeRed;
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.ToString());
                }
            }

               

        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            if (materialRadioButton1.Checked == true)
            {
                dataTextBox.Visible = true;
                try
                {
                    bmp = (Bitmap)imagePictureBox.Image;
                    string extractedText = SteganographyHelper.extractText(bmp);
                    if (encryptCheckBox.Checked)
                    {
                        try
                        {
                            extractedText = Crypto.DecryptStringAES(extractedText, passwordTextBox.Text);
                            MessageBox.Show("Расшифровано!", "CobraStegano");
                        }
                        catch
                        {
                            MessageBox.Show("Неверный пароль!", "CobraStegano");

                            return;
                        }
                    }

                    dataTextBox.Text = extractedText;
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.ToString());
                }
            }
            else if (materialRadioButton2.Checked == true)
            {
                dataTextBox.Visible = false;
                try
                {
                    bmp = (Bitmap)imagePictureBox.Image;
                    string extractedText = SteganographyHelper.extractText(bmp);
                    if (encryptCheckBox.Checked)
                    {
                        try
                        {
                            extractedText = Crypto.DecryptStringAES(extractedText, passwordTextBox.Text);
                            MessageBox.Show("Расшифровано! Сохраните документ!", "CobraStegano");
                        }
                        catch
                        {
                            MessageBox.Show("Неверный пароль!", "CobraStegano");

                            return;
                        }
                    }

                    dataTextBox.Text = extractedText;
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.ToString());
                }
            }

           

        }

        private void savepng()
        {
            SaveFileDialog save_dialog = new SaveFileDialog();
            save_dialog.Filter = "Png Image|*.png";
            try
            {
                if (save_dialog.ShowDialog() == DialogResult.OK)
                {
                    bmp.Save(save_dialog.FileName, ImageFormat.Png);
                }
                imagePictureBox.Image = bmp2;
                passwordTextBox.Text = "";
                dataTextBox.Text = "";
                encryptCheckBox.Checked = false;
                statusBox.Text = "Статус: Сохранено!";
                MessageBox.Show ("Статус: Сохранено!", "CobraStegano");
                statusBox.ForeColor = System.Drawing.Color.Black;
                crypto = 0;
                Application.Restart();

            }
            catch (Exception s)
            {
                MessageBox.Show(s.ToString());
            }
        }

        private void materialButton5_Click(object sender, EventArgs e)
        {
            if (crypto == 1)
            {
                savepng();
                MessageBox.Show("Сохранено!", "CobraStegano");
            }
            else
            {
                MessageBox.Show("Изображение не зашифровано!", "CobraStegano");
            }
            return;
        }


        public string a3 = @"./Data/Result.xml";

        public void prostotest(string a1)
        {
            string a2 = a1;
            a3 = a2;
        }


        private void materialButton6_Click(object sender, EventArgs e)
        {
            if (materialRadioButton1.Checked == true)
            {
                try
                {
                    SaveFileDialog save_dialog = new SaveFileDialog();
                    save_dialog.Filter = "Text Files|*.txt";

                    if (save_dialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(save_dialog.FileName, dataTextBox.Text);
                    }
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.ToString());
                }
            }
            else if (materialRadioButton2.Checked == true)
            {
                try
                {
                    deletecopy();
                    var xml = dataTextBox.Text;
                    Document doc = new Document();
                    a3 = @"./Data/Result.xml";
                    
                    doc.LoadFromFile(a3, FileFormat.Xml);

                    SaveFileDialog save_dialog = new SaveFileDialog();
                    save_dialog.Filter = "Text Files|*.doc";

                    if (save_dialog.ShowDialog() == DialogResult.OK)
                    {
                        doc.SaveToFile(save_dialog.FileName, FileFormat.Doc);
                    }
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.ToString());
                }

                MessageBox.Show("Сохранено!", "CobraStegano");
            }
        }

        private void deletecopy()
        {
            File.WriteAllText(a3, File.ReadAllText(a3).Replace("HYPERLINK", null));
            File.WriteAllText(a3, File.ReadAllText(a3).Replace("Purchase the license to remove this text.", null));
            File.WriteAllText(a3, File.ReadAllText(a3).Replace("https://sautinsoft.com/products/useoffice/order.php", null));
            File.WriteAllText(a3, File.ReadAllText(a3).Replace("Created by UseOffice .Net 5.1.2.24.", null));
        }
    }
}