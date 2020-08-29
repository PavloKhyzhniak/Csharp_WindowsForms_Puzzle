using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Globalization;

namespace WinForms_Layers
{
    public partial class Puzzle : Form
    {      
        public Puzzle()
        {
            InitializeComponent();
            PrepareImage();
            PrepareTableLayoutPanel();
        }
        private void PrepareTableLayoutPanel()
        {
            this.Controls.SetChildIndex(tableLayoutPanel_ImageElement, this.Controls.Count - 1);

            tableLayoutPanel_ImageElement.RowCount = row;
            tableLayoutPanel_ImageElement.ColumnCount = column;

            for (int i = 0; i < row; i++)
            {
                tableLayoutPanel_ImageElement.RowStyles[i].SizeType = SizeType.Absolute;
                tableLayoutPanel_ImageElement.RowStyles[i].Height = source.Height/row;
            }
            for (int j = 0; j < row; j++)
            {
                tableLayoutPanel_ImageElement.ColumnStyles[j].SizeType = SizeType.Absolute;
                tableLayoutPanel_ImageElement.ColumnStyles[j].Width = source.Width/column;
            }

            tableLayoutPanel_ImageElement.Width = source.Width;
            tableLayoutPanel_ImageElement.Height = source.Height;

            tableLayoutPanel_ImageElement.Refresh();

            // результирующая картинка
            Bitmap res = new Bitmap(source.Width, source.Height);

            // graphics для фона картинки
            Graphics background_gr = Graphics.FromImage(res);
         
            // создание атрибутов изображения
            ImageAttributes attr = new ImageAttributes();

            // белый цвет делаем прозрачным
            attr.SetColorKey(Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255));

            // матрица цветов задаёт прозрачноть для каждого слоя
            ColorMatrix myColorMatrix = new ColorMatrix();
            myColorMatrix.Matrix00 = 1.00f;
            myColorMatrix.Matrix11 = 1.00f;
            myColorMatrix.Matrix22 = 1.00f;
            myColorMatrix.Matrix33 = 0.1f;

            // применение матрицы
            attr.SetColorMatrix(myColorMatrix);

            // отображение слоя
            background_gr.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attr);

            // выбор результирующей картинки для фона
            tableLayoutPanel_ImageElement.BackgroundImage = res;

            background_gr.Dispose();
        }

        Bitmap Crop(Bitmap source, Rectangle crop)
        {
            Bitmap target = new Bitmap(crop.Width, crop.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(source, new Rectangle(0, 0, target.Width, target.Height), crop, GraphicsUnit.Pixel);
            }

            return target;
        }
        ImageElement[][] img_elements;
        string ImageFilename = ".\\main_picture.png";

      
        int delta = 20; //перекрытие элементов
        int row = 4;    //количество строк
        int column = 4; //количество столбцов
        Image source;
        private void PrepareImage()
        {
            //исходная картинка
            source = Bitmap.FromFile(ImageFilename);
            pictureBox_Source.Image = source;

            // результирующая картинка
            Bitmap source_bitmap = new Bitmap(source.Width + delta, source.Height + delta);

            // контекст для рисования на результирующей картинке
            Graphics resgr = Graphics.FromImage(source_bitmap);

            // покрасить фон в прозрачный цвет
            resgr.Clear(Color.Transparent);

            //отрисуем исходную картинку
            resgr.DrawImage(source, delta, delta);

            Rectangle rectSource;
            img_elements = new ImageElement[row][];
            for (int i = 0; i < row; i++)
            {
                img_elements[i] = new ImageElement[column];
                for (int j = 0; j < column; j++)
                    img_elements[i][j] = new ImageElement();
            }
            
            int width_delta = source.Width / row;
            int height_delta = source.Height / column;

            int top_element = height_delta;
            int left_element = -width_delta / 2;

            Random rand = new Random();
            for (int i = 0; i < row; i++)
            {
                left_element += width_delta;
                if (left_element > source.Width)
                    left_element = width_delta + delta ;

                for (int j = 0; j < column; j++)
                {
                    img_elements[i][j].Top = top_element;
                    img_elements[i][j].Left = left_element;

                    top_element += height_delta / 2;
                    left_element += (j % 2 * 2 * delta);
                    left_element -= ((j + 1) % 2 * 2 * delta);
                    if (top_element > source.Height)
                        top_element = height_delta;

                    //сделаем все элементы
                    rectSource = new Rectangle(j * width_delta, i * height_delta, width_delta + (2 * delta), height_delta + (2 * delta));
                    img_elements[i][j].Picture = Crop(source_bitmap, rectSource);
                    img_elements[i][j].delta = delta;
                    //top
                    if (i == 0)
                        img_elements[i][j].RType_Top = RegionType.none;
                    //bottom
                    if (i < row - 1)
                    {
                        if (rand.Next(0, 2) == 1) 
                        {
                            img_elements[i][j].RType_Bottom = RegionType.Union;
                            img_elements[i + 1][j].RType_Top = RegionType.Exclude;
                        }
                        else
                        {
                            img_elements[i][j].RType_Bottom = RegionType.Exclude;
                            img_elements[i + 1][j].RType_Top = RegionType.Union;
                        }
                    }
                    else
                        img_elements[i][j].RType_Bottom = RegionType.none;
                    //left
                    if (j == 0)
                        img_elements[i][j].RType_Left = RegionType.none;
                    //right
                    if (j < column - 1)
                    {
                        if (rand.Next(0, 2) == 1)
                        {
                            img_elements[i][j].RType_Right = RegionType.Union;
                            img_elements[i][j + 1].RType_Left = RegionType.Exclude;
                        }
                        else
                        {
                            img_elements[i][j].RType_Right = RegionType.Exclude;
                            img_elements[i][j + 1].RType_Left = RegionType.Union;
                        }
                    }
                    else
                        img_elements[i][j].RType_Right = RegionType.none;

                    img_elements[i][j].CreateRegion();
                    img_elements[i][j].ShowElement();
                    img_elements[i][j].SendMouseUp(tableLayoutPanel_ImageElement_MouseUp);
                    this.Controls.Add(img_elements[i][j]);
                }
            }

            resgr.Dispose();
        }

        private void ClearForm()
        {
            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                    this.Controls.Remove(img_elements[i][j]);

            Bitmap bitmap = new Bitmap(source.Width, source.Height);
            Graphics gr = Graphics.FromImage(bitmap);
            gr.Clear(Color.White);
            pictureBox_Result.Image = bitmap;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size.Width;
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size.Height;
            this.WindowState = FormWindowState.Maximized;
        }

        private void CheckImage()
        {
//            Rectangle rectSource = new Rectangle(tableLayoutPanel_ImageElement.Location,source.Size);
//            pictureBox_Result.Image = Crop(source_bitmap, rectSource);

            Bitmap bitmap = new Bitmap(source.Width, source.Height);
            Graphics gr = Graphics.FromImage(bitmap);

            //int xPosition = this.Location.X + tableLayoutPanel_ImageElement.Location.X + SystemInformation.FrameBorderSize.Width;
            //int yPosition = this.Location.Y + tableLayoutPanel_ImageElement.Location.Y + SystemInformation.CaptionHeight + SystemInformation.FrameBorderSize.Height;

            var pt = tableLayoutPanel_ImageElement.PointToScreen(new Point(0, 0));
            int xPosition = pt.X;
            int yPosition = pt.Y;
           
            gr.CopyFromScreen(new Point(xPosition,yPosition), new Point(0, 0), source.Size);

            pictureBox_Result.Image = bitmap;

            if (Equality(pictureBox_Result.Image, pictureBox_Source.Image))
                MessageBox.Show("Congratulation!!! Image is Equal");
            if (CompareBitmaps((Bitmap)pictureBox_Result.Image, (Bitmap)pictureBox_Source.Image))
                MessageBox.Show("Congratulation!!! Image is Correct");
            gr.Dispose();
        }

        bool Equality(Image Img1, Image Img2)
        {
            Bitmap Bmp1 = (Bitmap)Img1;
            Bitmap Bmp2 = (Bitmap)Img2;
            if (Bmp1.Size == Bmp2.Size)
            {
                for (int i = 0; i < Bmp1.Width; i++)
                    for (int j = 0; j < Bmp1.Height; j++)
                        if (Bmp1.GetPixel(i, j) != Bmp2.GetPixel(i, j))
                            return false;
                return true;
            }
            else return false;
        }

        public bool CompareBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Width != bmp2.Width || bmp1.Height != bmp2.Height)
                return false;
            System.Drawing.Imaging.ImageLockMode Mode = System.Drawing.Imaging.ImageLockMode.ReadWrite;
            Rectangle Range = new Rectangle(0, 0, bmp1.Width, bmp1.Height);
            System.Drawing.Imaging.BitmapData BMPD1 = bmp1.LockBits(Range, Mode, bmp1.PixelFormat);
            System.Drawing.Imaging.BitmapData BMPD2 = bmp2.LockBits(Range, Mode, bmp2.PixelFormat);
            try
            {
                int cnt_equal = 0;
                int cnt_falsh = 0;
                int cnt = 0;

                int c = Range.Height * BMPD1.Stride;
                unsafe
                {
                    byte* p1 = (byte*)(void*)BMPD1.Scan0;
                    byte* p2 = (byte*)(void*)BMPD2.Scan0;
                    try
                    {

                        for (int i = 0; i < c; i++)
                        {
                            //                        if (*p1 != *p2)
                            //                            return false;
                            if (*p1 == *p2)
                                cnt_equal++;
                            else if (Math.Abs(*p1 - *p2) < 10)
                                cnt++;
                            else if (*p1 != *p2)
                                cnt_falsh++;
                            p1++;
                            p2++;
                        }
                    }
                    catch (AccessViolationException ex)
                    {
                        Console.WriteLine($"Исключение: {ex.Message}");
                    }
                }

                if (cnt_equal > cnt_falsh//хороших битов больше плохих
                    && cnt_falsh < 3 * (c / (row * column))//плохих битов меньше чем на 3 элемент
                    && cnt_falsh < (c * 20 / 100)//плохих битов меньше 20 процентов
                    && Math.Abs(cnt_falsh - cnt) < (c * 25 / 100))//количество реально плохих и несовпадающих отличаеться не более чем на 25 процентов
                    return true;
                else
                    return false;
            
            }
            catch (AccessViolationException ex)
            {
                Console.WriteLine($"Исключение: {ex.Message}");
            }
            finally
            {
                bmp1.UnlockBits(BMPD1);
                bmp2.UnlockBits(BMPD2);
            }

            return true;
        }

        private void tableLayoutPanel_ImageElement_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !ImageElement.isMouseDown)
            {
                if (sender is PictureBox picture)
                {
               //     if (ImageElement.data == null)
               //         return;
               //     if (!ImageElement.data.GetDataPresent("MyAppFormat"))
               //         return;

                    int CurrentColumn = -1;
                    int CurrentRow = -1;

                    int height = (int)tableLayoutPanel_ImageElement.RowStyles[0].Height;
                    int width = (int)tableLayoutPanel_ImageElement.ColumnStyles[0].Width;

//                    var pt = tableLayoutPanel_ImageElement.PointToClient(new Point(e.X + picture.Parent.Location.X, e.Y + picture.Parent.Location.Y));
  
                    var pt = picture.PointToScreen(e.Location);
                    pt.X -= (this.Location.X + this.tableLayoutPanel_ImageElement.Location.X);
                    pt.Y -= (this.Location.Y + this.tableLayoutPanel_ImageElement.Location.Y);
                   

                    if (pt.Y > 0 && pt.X > 0)
                    {
                        CurrentColumn = pt.X / width;
                        if (CurrentColumn < 0 || CurrentColumn >= column) CurrentColumn = -1;
                        CurrentRow = pt.Y / height;
                        if (CurrentRow < 0 || CurrentRow >= row) CurrentRow = -1;
                    }

                    if (CurrentColumn != -1 && CurrentRow != -1)
                    {
//                        ImageElement obj = (ImageElement)ImageElement.data.GetData("MyAppFormat");
                        picture.Parent.Location = new Point(tableLayoutPanel_ImageElement.Location.X + CurrentColumn * width - delta + 1, tableLayoutPanel_ImageElement.Location.Y + CurrentRow * height - delta + 1);
                        CheckImage();
//                        tableLayoutPanel_ImageElement.Controls.Add(obj, CurrentColumn, CurrentRow);
                    }
                }
            }
        }

        private void button_NewImage_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.SelectedPath == null)
            {

                // Задание начальной папки для диалога
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    //                MessageBox.Show(folderBrowserDialog1.SelectedPath);
                }
            }

            openFileDialog.InitialDirectory = folderBrowserDialog.SelectedPath;
            // Заголовок диалогового окна
            openFileDialog.Title = openFileDialog.InitialDirectory;

            // Фильтры файлов в диалоге
            openFileDialog.Filter = "Image|*.jpg;*.bmp;*.png;*.ico;*.gif" + "|All Files|*.*";

            // Номер выбранного по умолчанию фильтра
            openFileDialog.FilterIndex = 1;

            // Проверка существования выбранного файла
            openFileDialog.CheckFileExists = true;

            // Разрешить выбор нескольких файлов
            openFileDialog.Multiselect = false;

            // Открытие диалога
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ImageFilename = openFileDialog.FileName;

                // Короткое имя выбранного файла
                MessageBox.Show(openFileDialog.SafeFileName, "Файл открыт");
            }
            ClearForm();
            PrepareImage();
            PrepareTableLayoutPanel();
        }
    }
}
