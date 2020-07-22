using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace Marion
{
    public class PlayerMarion
    {
        //プレイヤーの座標
        public int MarionX = 0;
        public int MarionY = 0;
        //プレイヤーの現在マス
        public int MarionMass;
        //プレイヤーの半径
        public int MarionRadius = 32;
        //プレイヤーのライフ
        public int MarionLife = 1;
        public int MarionChip = 32;
        //プレイヤーのスピード
        public int MarionSpeed = 1;
        public Bitmap MarionImage { get; set; }
        public int MarionStatus = 0;
        //スプライト上での分割位置設定//
        public Point SpliteCell(int spno)
        {
            int MarionChipx = 0;
            int MarionChipy = spno * MarionChip;

            return new Point(MarionChipx, MarionChipy);
        }

        //チップを読み取る
        public Bitmap GetMarionChip(int spno)
        {
            this.MarionImage = (System.Drawing.Bitmap)Bitmap.FromFile(@"C:\Users\USER\source\repos\Marion\Marion\Player_Enemy_Splite.bmp");
            // 座標を読み取り.
            var mcpoint = SpliteCell(spno);
            // 座標とサイズを格納.
            Rectangle rect = new Rectangle(mcpoint.X, mcpoint.Y, MarionChip, MarionChip);
            // 座標とサイズからコピーを作成.
            Bitmap bmpNew = MarionImage.Clone(rect, MarionImage.PixelFormat);
            bmpNew.MakeTransparent();
            // コピーを返す.
            return bmpNew;
        }

        public Bitmap GetMarion(int spno)
        {
            
            // ステージ構造体を参照
            var mapdata = 1;    // 横の列数と
            var xLen = 1;
            // 縦の行数を格納
            var yLen = 1;
            // サイズを決定
            var bmp = new Bitmap(32,32);//xLen * this.MarionChip, yLen * this.MarionChip);

            // サイズでグラフィックカプセルを初期
            Graphics g = Graphics.FromImage(bmp);

            //マップ構造体を参照し該当マスの情報を読み取る
            var mapchip = this.GetMarionChip(spno);
            //マス情報からマップチップ画像を参照し該当マスに描写
            g.DrawImage(mapchip, new PointF(0,0));
            //リソースの開放
            mapchip.Dispose();
            return bmp;
        }

        public void MarionMove()
        {
           

        }
        public PlayerMarion()
        {
            
        }

        public void MarionMoveUp()
        {
            MarionY -= MarionSpeed;
        }

        public void MarionMoveDown()
        {
            MarionY += MarionSpeed;
        }

        public void MarionMoveLeft()
        {
            MarionX -= MarionSpeed;
        }

        public void MarionMoveRight()
        {
            MarionX += MarionSpeed;
        }

        public void MarionMoveShot()
        {
           // NomalBallet NomallBallet = new NomalBallet();
        }
        public enum MarionMovetrigger
        {
            None = 0,
            Up = 1,
            Down = 2,
            Left = 3,
            Right = 4,
            
        }
    }
}
