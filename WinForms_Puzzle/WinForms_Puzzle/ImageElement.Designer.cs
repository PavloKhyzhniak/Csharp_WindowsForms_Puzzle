namespace WinForms_Layers
{
    partial class ImageElement
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox_ImageElement = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ImageElement)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_ImageElement
            // 
            this.pictureBox_ImageElement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_ImageElement.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_ImageElement.Name = "pictureBox_ImageElement";
            this.pictureBox_ImageElement.Size = new System.Drawing.Size(150, 150);
            this.pictureBox_ImageElement.TabIndex = 0;
            this.pictureBox_ImageElement.TabStop = false;
            this.pictureBox_ImageElement.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Panel_MouseDown);
            this.pictureBox_ImageElement.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Panel_MouseMove);
            this.pictureBox_ImageElement.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Panel_MouseUp);
            // 
            // ImageElement
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox_ImageElement);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ImageElement";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ImageElement)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_ImageElement;
    }
}
