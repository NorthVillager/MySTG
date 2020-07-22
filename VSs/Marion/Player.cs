using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Marion
{
    public class Player
    {
        Bitmap VipviperPic = Properties.Resources.VIPVIPER;
        public bool VipviperShow = false;
        public int VipviperX = 0;
        public int VipviperY = 0;

        public Player() { }

        public void Show(Graphics g)
        {
            if (VipviperShow)
            {
                // 80×30ピクセルで表示
                int sourceWidth = VipviperPic.Width;
                int sourceHeight = VipviperPic.Height;
                g.DrawImage(VipviperPic, new Rectangle(0, 0, 64, 32), new Rectangle(0, 0, sourceWidth, sourceHeight), GraphicsUnit.Pixel);
                VipviperPic.MakeTransparent(VipviperPic.GetPixel(0, 0));
                
               
            }
        }

        public void MoveUp()
        { 
            VipviperY -= 5;
        }
        public void MoveDown()
        {
            VipviperY += 5;
        }
        public void MoveLeft()
        {
            VipviperX -= 5;
        }
        public void MoveRight()
        {
            VipviperX += 5;
        }

        public enum PlayerDirect
        {
            None = 0,
            Up = 1,
            Down = 2,
            Left = 3,
            Right = 4,
        }

    }
}
