using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;

namespace Marion
{
    class NomalBallet:PlayerMarion 
    {
       
        public PictureBox Ballet;
        int BalletSpeed = 10;
        public Bitmap canvasballet;
        public NomalBallet()
        {
            
        }

        public void BalletMove(int BalletX,int BalletY)
        {
            Ballet = new PictureBox();
            Ballet.Size = new System.Drawing.Size(32, 32);
            Ballet.BackColor = Color.Transparent;
            canvasballet = new Bitmap(32, 32);
            Graphics gi = Graphics.FromImage(canvasballet);
            gi.DrawImage(GetMarion(6), 0, 0, 32, 32);
            

            //Controls.Add(this.Ballet);
            Ballet.Location = new System.Drawing.Point(BalletX+16, BalletY+16);
        }
    }

    
}
