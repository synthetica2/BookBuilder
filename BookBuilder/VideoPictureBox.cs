﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace BookBuilder
{
    class VideoPictureBox : PictureBox
    {
        private MainForm mainForm;
        private bool isMoving;
        private int xPos;
        private int yPos;
        private ImagePictureBox imagePictureBox;
        private TableLayoutPanel mainLayoutPanel;

        public bool MainFormResize { get; set; }

        /// <summary>
        /// Initializes Video Picture Box and sets up mouse handlers
        /// </summary>
        /// <param name="mainForm"></param>
        public VideoPictureBox(MainForm mainForm)
        {
            this.mainForm = mainForm;
            this.MouseUp += MouseUpHandler;
            this.MouseDown += MouseDownHandler;
            this.MouseMove += MouseMoveHandler;
            this.isMoving = false;
            this.Resize += ResizeHandler;
            /*
            Size minSize = this.DefaultMinimumSize;
            Console.WriteLine("width is {0} and height is {1}", minSize.Width, minSize.Height);
            */
            MainFormResize = false;
        }

        /// <summary>
        /// Sets Video Pictures Box's imagePictureBox to the same one in MainForm
        /// </summary>
        /// <param name="imagePictureBox"></param>
        public void setImagePictureBox(ImagePictureBox imagePictureBox)
        {
            this.imagePictureBox = imagePictureBox;
        }

        /// <summary>
        /// When video placeholder is being resized update the size and location display on Mainform
        /// and if it is resized as a result of the user clicking an dragging it (not from mainform expanding or shrinking)
        /// update the Ratios struct as well.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ResizeHandler(object sender, System.EventArgs e)
        {
            if (mainForm.currentPage != null)
            {
                mainForm.DisplayVideoSizeAndLocation();
                if (!MainFormResize)
                {
                    mainForm.UpdateCurrentRatio();
                }
            }
        }

        /// <summary>
        /// Sets Video Picture Box's tableLayoutPanel to the same one in MainForm
        /// </summary>
        /// <param name="tableLayoutPanel"></param>
        public void setTableLayoutPanel(TableLayoutPanel tableLayoutPanel)
        {
            this.mainLayoutPanel = tableLayoutPanel;
        }
        

        /// <summary>
        /// Updates the text fields in the main form based on video box's current location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseUpHandler(object sender, MouseEventArgs e)
        {
            this.isMoving = false;
            mainForm.changeMade = true;
            mainForm.DisplayVideoSizeAndLocation();  //Re-update size and location of video when user is done resizing.
            mainForm.UpdateCurrentRatio();
        }

        /// <summary>
        /// Keeps track of where the user pressed down the mouse
        /// Sets isMoving to true.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseDownHandler(object sender, MouseEventArgs e)
        {
            this.isMoving = true;
            //e.X and e.Y are relatively to the coordinates of this VideoPictureBox
            //(Top left = 0,0)
            //Console.WriteLine("e.X=" + e.X + " e.Y=" + e.Y);
            xPos = e.X;
            yPos = e.Y;
        }

        /// <summary>
        /// Moves the video picture box across the main form according to user's mouse movement
        /// Bounds the video picture box to the image incase user tries to drag out of that area.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseMoveHandler(object sender, MouseEventArgs e)
        { 
            if (this.isMoving)
            {
             
                /* this.Top = distance between top of placeholder and top of picturebox
                 * this.Left = distance between left edge of placeholder and left edge of picturebox
                 * xPos = distance between mouse click and left edge of placeholder
                 * yPos = distance between mouse click and top edge of placeholder
                 * e.Y = distance of mouse between top of placeholder and top of picturebox after mouse move
                 * e.X = distance of mouse between left edge of placeholder and left edge of placeholder after move
                 * e.Y - yPos = distance between new mouse Y pos and old one
                 * e.X - xPos = distance between new mouse X pos and old one
                 */
                   
                int newTop = this.Top + e.Y - yPos;
                int newLeft = this.Left + e.X - xPos;
                
                int imageRight = imagePictureBox.ImageRectangle.X + imagePictureBox.ImageRectangle.Width;
                double imageTop = (mainLayoutPanel.Size.Height - imagePictureBox.ImageRectangle.Size.Height) / 2.0;
                double imageBottom = imagePictureBox.ImageRectangle.Size.Height + imageTop;
               
                //Bound video picture box to the images container
                if (newTop - imageTop < 0)
                    newTop = (int)imageTop;
                if (newLeft - imagePictureBox.ImageRectangle.Left < 0)
                    newLeft = imagePictureBox.ImageRectangle.Left;
                if (newTop + this.Size.Height > imageBottom)
                    newTop = (int)imageBottom - this.Size.Height;
                if (newLeft + this.Size.Width > imageRight)
                    newLeft = imageRight - this.Size.Width;
                
                this.Top = newTop+ MainForm.yVideoOffset;
                this.Left = newLeft+ MainForm.xVideoOffset;
            }
        }

        /// <summary>
        /// Makes Video Picture Box resizable
        /// </summary>
        //http://stackoverflow.com/questions/17264225/how-can-user-resize-control-at-runtime-in-winforms
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style |= 0x840000;  //WS_BORDER + WS_THICKFRAME
                return cp;
            }
        }

    }
}
