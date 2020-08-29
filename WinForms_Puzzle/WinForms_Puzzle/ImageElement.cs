using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace WinForms_Layers
{
    public enum RegionType
    {
        none,
        Union,
        Exclude
    }

    public partial class ImageElement : UserControl
    {
        public Point Region_Top { get; set; }
        public Point Region_Bottom { get; set; }
        public Point Region_Left { get; set; }
        public Point Region_Right { get; set; }

        public RegionType RType_Top { get; set; }
        public RegionType RType_Bottom { get; set; }
        public RegionType RType_Left { get; set; }
        public RegionType RType_Right { get; set; }

        public Bitmap Picture { get; set; }
        public int delta { get; set; }
        public void CreateRegion()
        {
            // создание контура
            GraphicsPath path;
            Region tmp_region;

            Rectangle base_rect = new Rectangle(delta, delta, Picture.Width - 2 * delta, Picture.Height - 2 * delta);
            // создание региона на основе контура
            Region new_region = new Region(base_rect);

            if (RType_Top != RegionType.none)
            {
                path = new GraphicsPath();
                path.StartFigure();
                path.AddEllipse(delta + (Picture.Width - 2 * delta) / 2, delta / 2, delta, delta);
                path.CloseFigure();
                tmp_region = new Region(path);

                if (RType_Top == RegionType.Union)
                    new_region.Union(tmp_region);
                else if (RType_Top == RegionType.Exclude)
                    new_region.Exclude(tmp_region);
            }

            if (RType_Bottom != RegionType.none)
            {
                path = new GraphicsPath();
                path.StartFigure();
                path.AddEllipse(delta + (Picture.Width - 2 * delta) / 2, Picture.Height - 3*delta / 2, delta, delta);
                path.CloseFigure();
                tmp_region = new Region(path);

                if (RType_Bottom == RegionType.Union)
                    new_region.Union(tmp_region);
                else if (RType_Bottom == RegionType.Exclude)
                    new_region.Exclude(tmp_region);
            }

            if (RType_Left != RegionType.none)
            {
                path = new GraphicsPath();
                path.StartFigure();
                path.AddEllipse(delta / 2, delta + (Picture.Height - 2 * delta) / 2, delta, delta);
                path.CloseFigure();
                tmp_region = new Region(path);

                if (RType_Left == RegionType.Union)
                    new_region.Union(tmp_region);
                else if (RType_Left == RegionType.Exclude)
                    new_region.Exclude(tmp_region);
            }

            if (RType_Right != RegionType.none)
            {
                path = new GraphicsPath();
                path.StartFigure();
                path.AddEllipse(Picture.Width - 3*delta / 2, delta + (Picture.Height - 2 * delta) / 2, delta, delta);
                path.CloseFigure();
                tmp_region = new Region(path);

                if (RType_Right == RegionType.Union)
                    new_region.Union(tmp_region);
                else if (RType_Right == RegionType.Exclude)
                    new_region.Exclude(tmp_region);
            }

            this.Region = new_region;
        }
        public void ShowElement()
        {
            this.pictureBox_ImageElement.Image = Picture;
        }

        public void SendMouseUp(MouseEventHandler mouseMoveEvent)
        {
            this.pictureBox_ImageElement.MouseUp += mouseMoveEvent;
        }

        public ImageElement()
        {
            InitializeComponent();

        //    this.pictureBox_ImageElement.MouseUp += Element_MouseUp;
        //    this.pictureBox_ImageElement.MouseDown += Element_MouseDown;
        //    this.pictureBox_ImageElement.MouseMove += Element_MouseMove;

         
            this.pictureBox_ImageElement.Parent = this;
        }

        private Point mouseOffset;
        private Point elementOffset;
        public static bool isMouseDown = false;

        private void Panel_MouseUp(object sender, MouseEventArgs e)
        {
            // Changes the isMouseDown field so that the form does
            // not move unless the user is pressing the left mouse button.
            if (e.Button == MouseButtons.Left)
            {
                data = null;
                isMouseDown = false;
            }
        }

        static public DataObject data;
        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            int xOffset;
            int yOffset;

            if (e.Button == MouseButtons.Left)
            {
            //    // Создать контейнер для хранения данных
            //    data = new DataObject();
            //
            //    // Добавить признак пользовательского формата в контейнер
            //    data.SetData("MyAppFormat", this);


                xOffset = -e.X;
                yOffset = -e.Y;
                mouseOffset = new Point(xOffset, yOffset);

                if (sender is Control control)
                    elementOffset = control.Parent.Location;

                isMouseDown = true;
            }
        }

        private void Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isMouseDown)
            {     
                if (sender is Control control)
                {
                    Point mousePos = e.Location;
                    mousePos.Offset(mouseOffset.X, mouseOffset.Y);

                    control.Parent.Left = elementOffset.X + mousePos.X;
                    control.Parent.Top = elementOffset.Y + mousePos.Y;

                    elementOffset = control.Parent.Location;

                    //попробуем сделать нашу панель выезжающую за пределы нашей формы
                    //Set to Foreground
                    if (this.ParentForm.Controls.Contains(control.Parent))
                        this.ParentForm.Controls.SetChildIndex(control.Parent, 0);
                }
            }
        }

//        bool isMouseElementDown = false;
//        private void Element_MouseDown(object sender, MouseEventArgs e)
//        {
//            if (e.Button == MouseButtons.Left)
//            {
//                ((PictureBox)sender).Tag = this;
//                isMouseElementDown = true;
//            }
//        }

//        private void Element_MouseMove(object sender, MouseEventArgs e)
//        {
//            if (e.Button == MouseButtons.Left && isMouseElementDown)
//            {
//                var element = (PictureBox)sender;
//                if (element.Tag != null)
//                {
//                    // Создать контейнер для хранения данных
//                    DataObject data = new DataObject();
//
//                    //                // Положить содержимое выделенной в списке строки
//                    //                StringCollection col = new StringCollection();
//                    //                foreach (ListViewItem item in listView_panel.SelectedItems)
//                    //                {
//                    //                    if (item.Tag is DirectoryInfo dinfo)
//                    //                        col.Add(dinfo.FullName);
//                    //                    if (item.Tag is FileInfo finfo)
//                    //                        col.Add(finfo.FullName);
//                    //                }
//                    //                data.SetFileDropList(col);
//
//                    //                // Если выделено имя файла картинки - положить картинку в контейнер
//                    //                string ext = Path.GetExtension(str);
//                    //                if (ext == ".bmp" || ext == ".jpg" || ext == ".gif" || ext == ".png")
//                    //                {
//                    //                    Image img = Bitmap.FromFile(str);
//                    //                    data.SetImage(img);
//                    //                }
//
//                    // Добавить признак пользовательского формата в контейнер
//                    data.SetData("MyAppFormat", this);
//
//                    element.DoDragDrop(data, DragDropEffects.Move);
//                }
//            }
//        }

//        private void Element_MouseUp(object sender, MouseEventArgs e)
//        {
//            if (e.Button == MouseButtons.Left && isMouseElementDown)
//            {
//                isMouseElementDown = true;
//                ((PictureBox)sender).Tag = null;
//            }
//        }

      
    }
}
