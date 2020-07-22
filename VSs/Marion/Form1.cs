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
using Point = System.Windows.Point;

namespace Marion
{
  
    public partial class GameWindow : Form
    {
        public int x;
        public int y;
        //TestPlayer TestPlayer = new TestPlayer();
        // クラス利用宣言
        STAGE STAGE = new STAGE();
        PlayerMarion Marion = new PlayerMarion();
        //プレイヤー関数
        public bool MovetriggerUp = false;
        public bool MovetriggerDown = false;
        public bool MovetriggerRight = false;
        public bool MovetriggerLeft = false;
        public bool Movetriggershot = false;
        public int MarionNowAnimaitonNo = 0;
        public int speedshift = 0;
        //

        //ステータス関連
        public int scores = 0;
        public string scoress;
        public System.Windows.Point[][] HanteiPos;
        public int[][] HanteiNums;

        // 現在のステージ番号.
        static int NowStageNo = 1;
        // スクロール座標格納配列.
        static int StageScroll  = 0;
        // スクロール止めスイッチ変数.
        static int StageScrollSW = 0;
        //スクロールさせる座標数
        public Point Mainus = new System.Windows.Point(1, 0);
        //スクロールマップパネル判定変数
        public int ScrollMapPanel = 1;
        // ステージパネル用のPictureBox配列宣言.
        public PictureBox StagePanel;

        //判定用変数の置き場//
        public float PlayerRadius = 22f;
        public float CicleHanteiRadius = 12f;

        //判定マス整列用リスト
        List<Point> SetHanteiMass = new List<Point>();
        //スプライト番号整列用リスト
        List<int> SetHanteiNum = new List<int>();
        //空間分割用の型
        public struct Space
        {
            public int PointX;
            public int PointY;
            public int MapNum;
        }
        //空間分割用マップマス領域格納リスト
        public Space[] SpaceMapMass = new Space[600];
        //空間分割用マップマス領域参照格納リスト
        public int[] SpaceMapMassMath = { -31, -30, -29, -1, 0, 1, 29, 30, 31};
        //空間分割計算マス関数
        public int SpaceMath(int Marionmass,int Stagemass)
        {
            int masuman = Marionmass + Stagemass;
            if(masuman <0 || masuman >=600)
            {
                masuman = Marionmass;
            }
            return masuman;
        }
        
        //描写用格納関数
        public Graphics g;
        public Image mapima;
        //弾リスト
        List<System.Drawing.Point> bulletList = new List<System.Drawing.Point>();


        public GameWindow()
        {
            this.DoubleBuffered = true;

            // レイアウトロジックの停止
            this.SuspendLayout();
            //ピクチャーボックス配列を初期化
            this.StagePanel = new PictureBox();
            //全ステージパネル判定格納用配列
            HanteiPos = new System.Windows.Point[(int)STAGE.StagePanel.Stage1panel][];
            HanteiNums = new int[(int)STAGE.StagePanel.Stage1panel][];

            
            // ピクチャーボックスを生成.
            this.StagePanel = new PictureBox();
            // ステージパネルのサイズを設定.
            StagePanel.Width = STAGE.MapX * STAGE.MapChip*6;
            StagePanel.Height = STAGE.MapY * STAGE.MapChip;
            // ステージパネルの座標を設定(左詰め).
            this.StagePanel.Left = 0;// i * STAGE.MapX * STAGE.MapChip;
            // スクロールポジション.
            StageScroll = 0;//STAGE.MapX * 32 * i;
            // 画像のカプセル化用ビットマップとグラフィックを作成.
            Bitmap canvas = new Bitmap(StagePanel.Width, StagePanel.Height);
            Bitmap canvass = new Bitmap(StagePanel.Width, StagePanel.Height);
            g = Graphics.FromImage(canvas);
            Graphics gg = Graphics.FromImage(canvass);
            for (int i = 0; i <= (int)STAGE.StagePanel.Stage1panel - 1; i++)
            {
                //ステージ番号を渡し、マップを生成
                STAGE.Stage1(i + 1);
                //mapはここだけSTAGE.GetMap(i)を参照.
                using (var map = STAGE.GetMap(i))
                {
                    // 原点に画像を設定.
                    g.DrawImage(map, i * 960, 0);
                    gg.DrawImage(map, i * 960, 0);

                }
            
                //判定マス整列用リスト
                List<System.Windows.Point> SetHanteiMass = new List<System.Windows.Point>();
                //スプライト番号整列用リスト
                List<int> SetHanteiNum = new List<int>();
                //判定マス整列用リスト判定変数
                //地形に判定をＳＴＡＧＥから参照
                for (int Hanteii = 0; Hanteii < STAGE.HanteiMaps.Length; Hanteii++)
                {
                    if (STAGE.HanteiMaps[Hanteii] != new System.Windows.Point(0, 0))
                    {
                        //判定マスリストに判定座標を加算
                        SetHanteiMass.Add(STAGE.HanteiMaps[Hanteii]);
                        //判定マス番号リストに番号を加算
                        SetHanteiNum.Add(STAGE.HanteiNum[Hanteii]);
                    }

                }
                //判定マス配列の長さを整列用リストの数で初期化
                HanteiPos[i] = new System.Windows.Point[SetHanteiMass.Count];
                //同じように
                HanteiNums[i] = new int[SetHanteiNum.Count];
                //整列用リストの数だけ
                for (int HanteiMassnum = 0; HanteiMassnum < SetHanteiMass.Count; HanteiMassnum++)
                {
                    //判定マス配列に
                    HanteiPos[i][HanteiMassnum] = SetHanteiMass[HanteiMassnum];
                    //同じ用に
                    HanteiNums[i][HanteiMassnum] = SetHanteiNum[HanteiMassnum];
                }
                
                // GetMapのリソース開放.
                STAGE.GetMap(i).Dispose();
            }
            mapima = canvass;
            
            //背景を描写
            StagePanel.Image = canvas;
            // グラフィックのリソース開放.
            g.Dispose();
            gg.Dispose();

            this.StagePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.Draw);
            // ユーザーフォームへ生成したコントロールを追加.
            this.Controls.Add(this.StagePanel);
            // デザイナーで作ったやつを描写.
            InitializeComponent();
            // レイアウトロジックの再開.
            this.ResumeLayout(false);

            //判定用ボタンを前面に
            button2.BringToFront();

            //プレイヤーの親設定と透過と描写
            //pictureBox1.BackColor = Color.Transparent;
            //pictureBox1.Parent = StagePanel;
            //Bitmap canvasmarion = new Bitmap(32, 32);
            //Graphics gi = Graphics.FromImage(canvasmarion);
            //gi.DrawImage(Marion.GetMarion(0), 0, 0, 32, 32);
            //pictureBox1.Image = canvasmarion;
            //ここまで

            /*
            //ステータスバーの設定
            status.Parent = StagePanel;
            status.BackColor = Color.Transparent;
            status.Location = new System.Drawing.Point(0, 0);
            scoress= String.Format("{0:D8}", scores);
            status.Text = scoress;
            status.BringToFront();
            */
            _GameWindow = this;
        }

        public static GameWindow _GameWindow;

        private void timer1_Tick(object sender, EventArgs e)
        {

            //判定ロケーションをリセット
            button2.Location = new System.Drawing.Point(-32, -32);

            //プレイヤー動作関数呼び出し
            MarionMove();

            //仮プレイヤー
            //this.Text = "" + MovetriggerUp + MovetriggerDown + MovetriggerRight + MovetriggerLeft + Marion.MarionSpeed
               // +":"+(Marion.MarionX/32+Marion.MarionY/32*30);
            //プレイヤーの現在マスを格納
            Marion.MarionMass = Marion.MarionX / 32 + Marion.MarionY / 32 * 30;
            //ここまで//
            //移動範囲制限
            if (Marion.MarionX <= 0)
            {
                Marion.MarionX = 0;

            }
            if (Marion.MarionX >= 928)
            {
                Marion.MarionX = 928;
            }
            if (Marion.MarionY <= 0)
            {
                Marion.MarionY = 0;
            }
            if (Marion.MarionY >= 608)
            {
                Marion.MarionY = 608;
            }
          
                //空間分割配列リセット
                SpaceMapMass = new Space[600];
            
            //ステージパネルをずらす
            if (StageScrollSW == 0)
            {
                
                StageScroll--;
                /*
                status.Location = new System.Drawing.Point(Math.Abs(StageScroll), 0);
                if(StageScroll % -10 == 0)
                {
                    scores += 10;
                }
                */
            }
            //ステージパネルの0番目の座標が1パネル分ずれる度
            if (StageScroll == (-1 * STAGE.MapX * STAGE.MapChip) * ScrollMapPanel)
            {
                //スクロールパネル判定を加算
                ScrollMapPanel++;
            }
            //ステージパネルの枚数分
            for (int i = 0; i < (int)STAGE.StagePanel.Stage1panel; i++)
            {

                // ステージパネルの0番目の座標が一定以下になったら
                // ステージスクロールスイッチをオンに(スクロール停止).
                if(StageScroll <= -1 * ((((int)STAGE.StagePanel.Stage1panel)-1) * STAGE.MapX * STAGE.MapChip))
                {
                    //スクロールスイッチオン
                    StageScrollSW = 1;
                }
                // ステージスクロールスイッチがオフのときは
                // スクロール.
                if (StageScrollSW == 0)
                {
                    
                    //当たり判定をずらす
                    for (int hanteii = 0; hanteii < HanteiPos[i].Length; hanteii++)
                    {
                        //Ｘをずらす
                        HanteiPos[i][hanteii] = (Point)Point.Subtract(HanteiPos[i][hanteii], Mainus);
                        if (HanteiPos[i][hanteii].X>0 &&  HanteiPos[i][hanteii].X < 928)
                        {
                            
                            double berons = (HanteiPos[i][hanteii].X / 32 + (HanteiPos[i][hanteii].Y-32) / 32 * 30);
                            SpaceMapMass[(int)Math.Round(berons, MidpointRounding.AwayFromZero)].PointX = (int)HanteiPos[i][hanteii].X;
                            SpaceMapMass[(int)Math.Round(berons, MidpointRounding.AwayFromZero)].PointY = (int)HanteiPos[i][hanteii].Y;
                            SpaceMapMass[(int)Math.Round(berons, MidpointRounding.AwayFromZero)].MapNum = HanteiNums[i][hanteii];
                        }
                    }
                    
                    // ステージパネルをスクロール.
                    StagePanel.Left =  StageScroll;
                    
                }

            }
            this.Text = ":" + Marion.MarionX + ":" + Marion.MarionY;
            //当たり判定設定関数実行
            CollisionCheck();
            
            //判定可視化用のボタンスクロール
            button2.Location = new System.Drawing.Point(button2.Location.X - 1, button2.Location.Y);
            //描写リセット(Paintイベント呼び出し)
            StagePanel.Refresh();
        }

        
        //当たり判定設定用関数
        void CollisionCheck()
        {
            
            //自身の周囲9マスを参照
            for (int Spaceloop = 0; Spaceloop < SpaceMapMassMath.Length; Spaceloop++)
            {
                    //スプライト番号1か2か91(四角形判定)
                    if (SpaceMapMass[Math.Abs(SpaceMath(Marion.MarionMass, SpaceMapMassMath[Spaceloop]))].MapNum == 1 
                        || SpaceMapMass[Math.Abs(SpaceMath(Marion.MarionMass, SpaceMapMassMath[Spaceloop]))].MapNum == 2
                        || SpaceMapMass[Math.Abs(SpaceMath(Marion.MarionMass, SpaceMapMassMath[Spaceloop]))].MapNum == 91)
                    {
                        //仮プレイヤーの座標と判定の座標の距離が３２以下
                        //(１マス分の外周とプレイヤーの外周が重なったら
                        if (Math.Abs(Marion.MarionX - SpaceMapMass[Math.Abs(SpaceMath(Marion.MarionMass, SpaceMapMassMath[Spaceloop]))].PointX) <= Marion.MarionRadius
                            && Math.Abs(Marion.MarionY - SpaceMapMass[Math.Abs(SpaceMath(Marion.MarionMass, SpaceMapMassMath[Spaceloop]))].PointY) <= Marion.MarionRadius)
                        {
                        this.Text = "111111111";
                            //判定オン
                            CollisionCheckOn(SpaceMapMass[Math.Abs(SpaceMath(Marion.MarionMass, SpaceMapMassMath[Spaceloop]))].PointX, SpaceMapMass[Math.Abs(SpaceMath(Marion.MarionMass, SpaceMapMassMath[Spaceloop]))].PointY);
                        }
                    }

                    //スプライト番号の2桁目が１の場合(左上空白の三角形)
                    if (SpaceMapMass[Math.Abs(SpaceMath(Marion.MarionMass, SpaceMapMassMath[Spaceloop]))].MapNum == 11)
                    {
                        //仮プレイヤーの座標と左上空白の三角形の図形が重なったら
                        //プレイヤーの座標と地形の座標のMarion.MarionX,Marion.MarionYをそれぞれかけ足したものをルート化
                        var mathposx = Marion.MarionX + 16 - SpaceMapMass[Spaceloop].PointX;
                        var mathposy = Marion.MarionY + 16 - SpaceMapMass[Spaceloop].PointY;
                        //それをプレイヤーの半径と地形の半径を足したものと比較し、小さかったら
                        //かつそのマスに収まる範囲内のもの
                        if (Math.Sqrt(mathposx * mathposx + mathposy * mathposy)
                            <= PlayerRadius + CicleHanteiRadius &&
                            Math.Abs(Marion.MarionX - SpaceMapMass[Spaceloop].PointX + 32) <= Marion.MarionRadius
                            && Math.Abs(Marion.MarionY - SpaceMapMass[Spaceloop].PointY + 32) <= Marion.MarionRadius)
                        {
                             //判定オン
                             CollisionCheckOn(SpaceMapMass[Spaceloop].PointX, SpaceMapMass[Spaceloop].PointY);
                         }

                    }

                    //スプライト番号の2桁目が2の場合(左下空白の三角形)
                    if (SpaceMapMass[Math.Abs(SpaceMath(Marion.MarionMass, SpaceMapMassMath[Spaceloop]))].MapNum == 21)
                    {
                        //仮プレイヤーの座標と左下空白の三角形の図形が重なったら
                        //プレイヤーの座標と地形の座標のMarion.MarionX,Marion.MarionYをそれぞれかけ足したものをルート化
                        var mathposx = Marion.MarionX + 16 - SpaceMapMass[Spaceloop].PointX;
                        var mathposy = Marion.MarionY + 16 - SpaceMapMass[Spaceloop].PointY + 32;
                        //それをプレイヤーの半径と地形の半径を足したものと比較し、小さかったら
                        //かつそのマスに収まる範囲内のもの
                        if (Math.Sqrt(mathposx * mathposx + mathposy * mathposy)
                            <= PlayerRadius + CicleHanteiRadius &&
                            Math.Abs(Marion.MarionX - SpaceMapMass[Spaceloop].PointX + 32) <= Marion.MarionRadius
                            && Math.Abs(Marion.MarionY - SpaceMapMass[Spaceloop].PointY + 32) <= Marion.MarionRadius)
                        {
                            //判定オン
                            CollisionCheckOn(SpaceMapMass[Spaceloop].PointX, SpaceMapMass[Spaceloop].PointY);
                        }

                    }

                    //スプライト番号の2桁目が3の場合(右上空白の三角形)
                    if (SpaceMapMass[Math.Abs(SpaceMath(Marion.MarionMass, SpaceMapMassMath[Spaceloop]))].MapNum == 31)
                    {
                        //仮プレイヤーの座標と右上空白の三角形の図形が重なったら
                        //プレイヤーの座標と地形の座標のMarion.MarionX,Marion.MarionYをそれぞれかけ足したものをルート化
                        var mathposx = Marion.MarionX + 16 - SpaceMapMass[Spaceloop].PointX + 32;
                        var mathposy = Marion.MarionY + 16 - SpaceMapMass[Spaceloop].PointY;
                        //それをプレイヤーの半径と地形の半径を足したものと比較し、小さかったら
                        //かつそのマスに収まる範囲内のもの
                        if (Math.Sqrt(mathposx * mathposx + mathposy * mathposy)
                            <= PlayerRadius + CicleHanteiRadius &&
                            Math.Abs(Marion.MarionX - SpaceMapMass[Spaceloop].PointX + 32) <= Marion.MarionRadius
                            && Math.Abs(Marion.MarionY - SpaceMapMass[Spaceloop].PointY + 32) <= Marion.MarionRadius)
                        {
                            //判定オン
                            CollisionCheckOn(SpaceMapMass[Spaceloop].PointX, SpaceMapMass[Spaceloop].PointY);
                        }

                    }

                    //スプライト番号の2桁目が4の場合(右下空白の三角形)
                    if (SpaceMapMass[Math.Abs(SpaceMath(Marion.MarionMass, SpaceMapMassMath[Spaceloop]))].MapNum == 41)
                    {
                        //仮プレイヤーの座標と右上空白の三角形の図形が重なったら
                        //プレイヤーの座標と地形の座標のMarion.MarionX,Marion.MarionYをそれぞれかけ足したものをルート化
                        var mathposx = Marion.MarionX + 16 - SpaceMapMass[Spaceloop].PointX + 32;
                        var mathposy = Marion.MarionY + 16 - SpaceMapMass[Spaceloop].PointY + 32;
                        //それをプレイヤーの半径と地形の半径を足したものと比較し、小さかったら
                        //かつそのマスに収まる範囲内のもの
                        if (Math.Sqrt(mathposx * mathposx + mathposy * mathposy)
                            <= PlayerRadius + CicleHanteiRadius &&
                            Math.Abs(Marion.MarionX - SpaceMapMass[Spaceloop].PointX + 32) <= Marion.MarionRadius
                            && Math.Abs(Marion.MarionY - SpaceMapMass[Spaceloop].PointY + 32) <= Marion.MarionRadius)
                        {
                            //判定オン
                            CollisionCheckOn(SpaceMapMass[Spaceloop].PointX, SpaceMapMass[Spaceloop].PointY);
                        }
                    }

                    //スプライト番号の51(火山の左上空白１)
                    if (SpaceMapMass[Math.Abs(SpaceMath(Marion.MarionMass, SpaceMapMassMath[Spaceloop]))].MapNum == 51)
                    {
                        //仮プレイヤーの座標と右上空白の三角形の図形が重なったら
                        //プレイヤーの座標と地形の座標のMarion.MarionX,Marion.MarionYをそれぞれかけ足したものをルート化
                        var mathposx = Marion.MarionX + 16 - SpaceMapMass[Spaceloop].PointX;
                        var mathposy = Marion.MarionY + 16 - SpaceMapMass[Spaceloop].PointY;
                        //それをプレイヤーの半径と地形の半径を足したものと比較し、小さかったら
                        //かつそのマスに収まる範囲内のもの
                        if (Math.Sqrt(mathposx * mathposx + mathposy * mathposy)
                            <= PlayerRadius + 20 &&
                            Math.Abs(Marion.MarionX - SpaceMapMass[Spaceloop].PointX + 32) <= Marion.MarionRadius
                            && Math.Abs(Marion.MarionY - SpaceMapMass[Spaceloop].PointY + 32) <= Marion.MarionRadius)
                        {
                            //判定オン
                            CollisionCheckOn(SpaceMapMass[Spaceloop].PointX, SpaceMapMass[Spaceloop].PointY);
                        }
                    }

                    //スプライト番号の61(火山の左上空白２)
                    if (SpaceMapMass[Math.Abs(SpaceMath(Marion.MarionMass, SpaceMapMassMath[Spaceloop]))].MapNum == 61)
                    {
                        //仮プレイヤーの座標と右上空白の三角形の図形が重なったら
                        //プレイヤーの座標と地形の座標のMarion.MarionX,Marion.MarionYをそれぞれかけ足したものをルート化
                        var mathposx = Marion.MarionX + 16 - SpaceMapMass[Spaceloop].PointX;
                        var mathposy = Marion.MarionY + 16 - SpaceMapMass[Spaceloop].PointY;
                        //それをプレイヤーの半径と地形の半径を足したものと比較し、小さかったら
                        //かつそのマスに収まる範囲内のもの
                        if (Math.Sqrt(mathposx * mathposx + mathposy * mathposy)
                            <= PlayerRadius + 35 &&
                            Math.Abs(Marion.MarionX - SpaceMapMass[Spaceloop].PointX + 32) <= Marion.MarionRadius
                            && Math.Abs(Marion.MarionY - SpaceMapMass[Spaceloop].PointY + 32) <= Marion.MarionRadius)
                        {
                            //判定オン
                            CollisionCheckOn(SpaceMapMass[Spaceloop].PointX, SpaceMapMass[Spaceloop].PointY);
                        }
                    }

                    //スプライト番号の71(火山の左上空白２)
                    if (SpaceMapMass[Math.Abs(SpaceMath(Marion.MarionMass, SpaceMapMassMath[Spaceloop]))].MapNum == 71)
                    {
                        //仮プレイヤーの座標と右上空白の三角形の図形が重なったら
                        //プレイヤーの座標と地形の座標のMarion.MarionX,Marion.MarionYをそれぞれかけ足したものをルート化
                        var mathposx = Marion.MarionX + 16 - SpaceMapMass[Spaceloop].PointX + 32;
                        var mathposy = Marion.MarionY + 16 - SpaceMapMass[Spaceloop].PointY;
                        //それをプレイヤーの半径と地形の半径を足したものと比較し、小さかったら
                        //かつそのマスに収まる範囲内のもの
                        if (Math.Sqrt(mathposx * mathposx + mathposy * mathposy)
                            <= PlayerRadius + 20 &&
                            Math.Abs(Marion.MarionX - SpaceMapMass[Spaceloop].PointX + 32) <= Marion.MarionRadius
                            && Math.Abs(Marion.MarionY - SpaceMapMass[Spaceloop].PointY + 32) <= Marion.MarionRadius)
                        {
                            //判定オン
                            CollisionCheckOn(SpaceMapMass[Spaceloop].PointX, SpaceMapMass[Spaceloop].PointY);
                        }
                    }

                    //スプライト番号の81(火山の左上空白２)
                    if (SpaceMapMass[Math.Abs(SpaceMath(Marion.MarionMass, SpaceMapMassMath[Spaceloop]))].MapNum == 81)
                    {
                        //仮プレイヤーの座標と右上空白の三角形の図形が重なったら
                        //プレイヤーの座標と地形の座標のMarion.MarionX,Marion.MarionYをそれぞれかけ足したものをルート化
                        var mathposx = Marion.MarionX + 16 - SpaceMapMass[Spaceloop].PointX + 32;
                        var mathposy = Marion.MarionY + 16 - SpaceMapMass[Spaceloop].PointY;
                        //それをプレイヤーの半径と地形の半径を足したものと比較し、小さかったら
                        //かつそのマスに収まる範囲内のもの
                        if (Math.Sqrt(mathposx * mathposx + mathposy * mathposy)
                            <= PlayerRadius + 35 &&
                            Math.Abs(Marion.MarionX - SpaceMapMass[Spaceloop].PointX + 32) <= Marion.MarionRadius
                            && Math.Abs(Marion.MarionY - SpaceMapMass[Spaceloop].PointY + 32) <= Marion.MarionRadius)
                        {
                            //判定オン
                            CollisionCheckOn(SpaceMapMass[Spaceloop].PointX, SpaceMapMass[Spaceloop].PointY);
                        }
                    }
                    //--------------------------------------//
                

            }
           
        }

        //当たり判定可視化用関数
        void CollisionCheckOn(int hantei1,int hantei2)
        {
            //当たり判定可視化用のボタンを当たったマスに表示
            button2.Location = new System.Drawing.Point((int)hantei1-32,
                (int)hantei2 - 32);

        }
        //プレイヤーの動作
        void MarionMove()
        {
            //上移動
            if (MovetriggerUp)
            {
                Marion.MarionMoveUp();
            }
            //下移動
            if (MovetriggerDown)
            {
                Marion.MarionMoveDown();
            }
            //左移動
            if (MovetriggerLeft)
            {
                Marion.MarionMoveLeft();
            }
            //右移動
            if (MovetriggerRight)
            {
                Marion.MarionMoveRight();
            }
            //ショット
            if (Movetriggershot)
            {
                //画面上の弾が10個以下の場合ショットを打つ
                if (bulletList.Count < 4)
                {
                    MarionShot(bulletList);
                }
            }

        }

        //キーフラグ/キーを押しているとき
        private void Form_KeyDown(object sender, KeyEventArgs a)
        {
            //上方向キーを押したとき
            if (a.KeyCode == Keys.Up)
            {
                Marion.MarionStatus = 4;
                MovetriggerUp = true;
            }
            //下方向キーを押したとき
            if (a.KeyCode == Keys.Down)
            {
                Marion.MarionStatus = 2;
                MovetriggerDown = true;
            }
            //左方向キーを押したとき
            if (a.KeyCode == Keys.Left)
            {
                MovetriggerLeft = true;
                
            }
            //右方向キーを押したとき
            if (a.KeyCode == Keys.Right)
            {
                MovetriggerRight = true;
            }

            //Zキーを離したとき
            if (a.KeyCode == Keys.Z)
            {
                Movetriggershot = true;
            }

        }

        //キーフラグ/キーが離れたとき
        private void Form_KeyUp(object sender, KeyEventArgs a)
        {
            //上方向キーを離したとき
            if (a.KeyCode == Keys.Up)
            {
                Marion.MarionStatus = 0;
                MovetriggerUp = false;
            }
            //下方向キーを離したとき
            if (a.KeyCode == Keys.Down)
            {
                Marion.MarionStatus = 0;
                MovetriggerDown = false;
            }
            //左方向キーを離したとき
            if (a.KeyCode == Keys.Left)
            {
                MovetriggerLeft = false;
            }
            //右方向キーを離したとき
            if (a.KeyCode == Keys.Right)
            {
                MovetriggerRight = false;
            }
            //Zキーを離したとき
            if(a.KeyCode == Keys.Z)
            {
                Movetriggershot = false;
            }
            //スペースキーが押されたとき
            if (a.KeyCode == Keys.Space)
            {
                Marion.MarionSpeed += 2;
                //スピードが14を超えたら
                if (Marion.MarionSpeed >= 14)
                {
                    Marion.MarionSpeed = 1;
                }
            }
            

        }


        //スペースマップマス変換
        private int SpaceMass(int x , int y)
        {
            int Mass;
  
            Mass = x / 32 + y / 32 * 30;
            return Mass;
        }

        //ショットした時の挙動
        public void MarionShot(List<System.Drawing.Point> bulletList)
        {
            bulletList.Add(new System.Drawing.Point(Marion.MarionX - 16 +  Math.Abs(StageScroll), Marion.MarionY));
        }

        //ステータスバー
        private void StatusBar()
        {
            

        }

        private void Draw(object sender, PaintEventArgs e)
        {
            Show(e.Graphics);
        }

        public void Show(Graphics g)
        {
            //ステージの描写
            g.DrawImage(mapima, 0, 0);
            //自機の描写
            g.DrawImage(Marion.GetMarion(Marion.MarionStatus), Marion.MarionX + Math.Abs(StageScroll), Marion.MarionY, 32, 32);
            //弾の前進
            bulletList = bulletList.Select(x => new System.Drawing.Point(x.X + 30, x.Y)).ToList();
            //弾の描写
            //消す弾を指定するリスト
            var pels = new List<System.Drawing.Point>();
            foreach (System.Drawing.Point pt in bulletList)
            {
                g.DrawImage(Marion.GetMarion(6), pt.X, pt.Y, 32, 32);
                if(pt.X - Math.Abs(StageScroll) >= 980)
                {
                    pels.Add(pt);
                }
            }
            //弾が画面外に出た場合削除
            foreach (System.Drawing.Point pt in pels)
            {
                bulletList.Remove(pt);
            }


        }
        
        //classおわり
    }
    //namespaceおわり
}
//めも
//判定の取り方がおかしいので調整必要
//パネルの参照もあってるか確認すること
