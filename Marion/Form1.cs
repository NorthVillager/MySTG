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
using PointF = System.Drawing.PointF;

namespace Marion
{
  
    public partial class GameWindow : Form
    {
        //スクロールスピード
        public int SCR = 1;

        public int NotTilte = 0;
        public float TitleX;
        public float TitleY;
        //トータルスコア
        public int TotalScore = 0;
        //トータルデスカウント
        public int TotalDeath = 0;
        public int ClearFlag = 0;
        public int ClearTimer = 0;
        public int x;
        public int y;
        public Font fnt;
        // クラス利用宣言
        STAGE STAGE = new STAGE();
        PlayerMarion Marion = new PlayerMarion();
        Enemy Enemy = new Enemy();
        //プレイヤー関数
        public bool MovetriggerUp = false;
        public bool MovetriggerDown = false;
        public bool MovetriggerRight = false;
        public bool MovetriggerLeft = false;
        public bool Movetriggershot = false;
        public bool Movetriggerpower = false;
        public bool Movetriggerspace = false;
        public int MarionNowAnimaitonNo = 0;
        public int speedshift = 0;
        public Image[] MarionDraw;
        public Image NomalBallets;
        public int AnimationTime = 0;
        public List<System.Drawing.PointF> MarionbulletList = new List<System.Drawing.PointF>();
        public int MarionDeathFlag = 0;
        public int MarionDeathTimer = 0;
        public int MarionTransferPower = 0;
        public int MarionShotDiagnol = 0;

        //WARNING
        public int WarningDraw = 0;
        public int WarningDraw2 = 0;
        //
        //敵周り画像
        public Image[] EnemyDraw;
        public Image[] EnemyBulletDraw;
        public Image[] BossDraw;
        public Image[] ChargeEff;
        //ステータス関連
        public int scores = 0;
        public string scoress;
        public PointF[][] HanteiPos;
        public int[][] HanteiNums;
        //爆発画像格納
        public Image[] EnemyBomb;
        public Image[] BulletBomb;
        //透過
        public int aiu = 0;
        //爆発構造体
        public struct Bombs
        {
            public float BombsPointX;
            public float BombsPointY;
            public int BombsMagnificationX;
            public int BombsMagnificationY;
            public int BombsTimes;
        }
        //爆発、チャージ、弾消しリスト
        public List<Bombs> EnemyBombpos = new List<Bombs>();
        public List<Bombs> EnemyBombposRemove = new List<Bombs>();
        public List<Bombs> EnemyChargepos = new List<Bombs>();
        public List<Bombs> EnemyChargeRemove = new List<Bombs>();
        public List<Bombs> BulletBombpos = new List<Bombs>();
        public List<Bombs> BulletBombRemove = new List<Bombs>();
        //弾消し時のヒットストップ時間
        public int BombWait = 4000;
        // スクロール座標格納配列.
        static int StageScroll  = 0;
        static int StageScrollAnime = 0;
        // スクロール止めスイッチ変数.
        static int StageScrollSW = 0;
        //スクロールさせる座標数
        public PointF Mainus;
        //スクロールマップパネル判定変数
        public int ScrollMapPanel = 1;
        // ステージパネル用のPictureBox配列宣言.
        public PictureBox StagePanel;

        //判定用変数の置き場//
        public float PlayerRadius = 20f;
        public float CicleHanteiRadius = 10f;

        //判定マス整列用リスト
        List<PointF> SetHanteiMass = new List<PointF>();
        //スプライト番号整列用リスト
        List<int> SetHanteiNum = new List<int>();
        //空間分割用の型
        public struct Space
        {
            public float PointX;
            public float PointY;
            public int MapNum;
        }
        //空間分割用マップマス領域格納リスト
        public Space[] SpaceMapMass = new Space[600];
        //空間分割用マップマス領域参照格納リスト
        public int[] SpaceMapMassMath = {-62 ,-61,-60,-59,-58,-32,-31, -30, -29,-28,-2, -1, 0, 1,2,28,29, 30, 31,32,58,59,60,61,62};
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
        //空間分割用弾マス格納リスト
        public List<List<System.Drawing.PointF>> BulletMass = new List<List<System.Drawing.PointF>>();
        public System.Drawing.Point[][] BulletMasss;

        //空間分割用の型
        public struct EnemyBulletRam
        {
            public float PointX;
            public float PointY;
            public int EnemyNum;
            public int BulletFlag;
            public int BulletRadius;
        }
        //敵空間分割用マス格納リスト
        public List<EnemyBulletRam>[] EnemyMass = new List<EnemyBulletRam>[600];
        //敵弾空間分割用マス格納リスト
        public List<EnemyBulletRam>[] EnemyBulletMass = new List<EnemyBulletRam>[600];
        
        public Graphics Title;
        public Image Titles;
        //敵弾空間分割用マス格納リスト　メルトダウン用
        public List<EnemyBulletRam>[] EnemyBulletMassMelt = new List<EnemyBulletRam>[600];
        public Graphics g;
        public Image mapima;

        //透過初期化
        System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix();
        System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();
        public int AlphaFlag = 0;
        public GameWindow()
        {
            //スクロールポイント初期化
            Mainus = new PointF(SCR, 0);
            //バレットマス初期化
            for (int i = 0; i <= 600; i++)
            {
                BulletMass.Add(new List<PointF>());
            }
            System.Drawing.Point[][] BulletMasss = new System.Drawing.Point[600][];
            this.DoubleBuffered = true;

            // レイアウトロジックの停止
            this.SuspendLayout();
            //ピクチャーボックス配列を初期化
            this.StagePanel = new PictureBox();
            //全ステージパネル判定格納用配列
            HanteiPos = new PointF[(int)STAGE.StagePanel.Stage1panel][];
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
                List<PointF> SetHanteiMass = new List<PointF>();
                //スプライト番号整列用リスト
                List<int> SetHanteiNum = new List<int>();
                //判定マス整列用リスト判定変数
                //地形に判定をＳＴＡＧＥから参照
                for (int Hanteii = 0; Hanteii < STAGE.HanteiMaps.Length; Hanteii++)
                {
                    if (STAGE.HanteiMaps[Hanteii] != new PointF(0, 0))
                    {
                        //判定マスリストに判定座標を加算
                        SetHanteiMass.Add(STAGE.HanteiMaps[Hanteii]);
                        //判定マス番号リストに番号を加算
                        SetHanteiNum.Add(STAGE.HanteiNum[Hanteii]);
                    }

                }
                //判定マス配列の長さを整列用リストの数で初期化
                HanteiPos[i] = new PointF[SetHanteiMass.Count];
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
            //button2.BringToFront();
            //プレイヤーの画像を読み込む
            MarionDraw = new Image[12];
            MarionDraw[0] = Marion.GetMarion(0);
            MarionDraw[1] = Marion.GetMarion(1);
            MarionDraw[2] = Marion.GetMarion(2);
            MarionDraw[3] = Marion.GetMarion(3);
            MarionDraw[4] = Marion.GetMarion(4);
            MarionDraw[5] = Marion.GetMarion(5);
            MarionDraw[6] = Marion.GetMarion(7);
            MarionDraw[7] = Marion.GetMarion(8);
            //弾画像を読み込む
            
            NomalBallets = Marion.GetMarion(6);
            //敵画像を読み込む
            EnemyDraw = new Image[12];
            EnemyDraw[0] = Enemy.GetEnemy(1,0);
            EnemyDraw[1] = Enemy.GetEnemy(1,1);
            EnemyDraw[2] = Enemy.GetEnemy(1,2);
            EnemyDraw[3] = Enemy.GetEnemy(1,3);
            EnemyDraw[4] = Enemy.GetEnemy(1,4);
            EnemyDraw[5] = Enemy.GetEnemy(1,5);
            EnemyDraw[6] = Enemy.GetEnemy(0, 9);
            //ボス画像を読み込む
            BossDraw = new Image[2];
            BossDraw[0] = Enemy.GetBoss(1, 0);
            BossDraw[1] = Enemy.GetBoss(2, 0);
            //敵の弾画像を読み込む
            EnemyBulletDraw = new Image[10];
            EnemyBulletDraw[0] = Enemy.GetEnemy(3, 4);
            EnemyBulletDraw[1] = Enemy.GetEnemy(3, 5);
            EnemyBulletDraw[2] = Enemy.GetEnemy(4, 4);
            EnemyBulletDraw[3] = Enemy.GetEnemy(4, 5);
            EnemyBulletDraw[4] = Enemy.GetEnemy(5, 4);
            EnemyBulletDraw[5] = Enemy.GetEnemy(5, 5);
            EnemyBulletDraw[6] = Enemy.GetEnemy(6, 4);
            EnemyBulletDraw[7] = Enemy.GetEnemy(6, 5);
            EnemyBulletDraw[8] = Enemy.GetEnemy(5, 6);
            EnemyBulletDraw[9] = Enemy.GetEnemy(5, 7);
            //爆発画像
            EnemyBomb = new Image[12];
            EnemyBomb[0] = Enemy.GetEnemy(2, 4);
            EnemyBomb[1] = Enemy.GetEnemy(2, 5);
            EnemyBomb[2] = Enemy.GetEnemy(2, 6);
            EnemyBomb[3] = Enemy.GetEnemy(2, 7);
            EnemyBomb[4] = Enemy.GetEnemy(2, 8);
            EnemyBomb[5] = Enemy.GetEnemy(2, 9);
            //弾消えがぞう
            BulletBomb = new Image[12];
            BulletBomb[0] = Enemy.GetEnemy(1, 6);
            BulletBomb[1] = Enemy.GetEnemy(1, 7);
            BulletBomb[2] = Enemy.GetEnemy(1, 8);
            BulletBomb[3] = Enemy.GetEnemy(1, 9);
            //透過設定
            //透過処理

            cm.Matrix00 = 1;
            cm.Matrix11 = 1;
            cm.Matrix22 = 1;
            cm.Matrix33 = 0.9F;
            cm.Matrix44 = 1;
            
            ia.SetColorMatrix(cm);

            //敵空間マスの初期化
            for (int i = 0;i<600;i++)
            {
                EnemyMass[i] = new List<EnemyBulletRam>();
                EnemyBulletMass[i] = new List<EnemyBulletRam>();
                //メルトダウン用
                EnemyBulletMassMelt[i] = new List<EnemyBulletRam>();
            }
            timer1.Stop();

            this.Text = "MARIDIUS";

            
        }

        //public static GameWindow _GameWindow;

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            //判定ロケーションをリセット
            //button2.Location = new System.Drawing.Point(-32, -32);

            
            //空間分割配列リセット
            SpaceMapMass = new Space[600];
            
            //ステージパネルをずらす
            if (StageScrollSW == 0)
            {
                StageScroll = StageScroll - SCR;
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
                    if (StageScrollSW == 0)
                    {
                        //スクロールスイッチオン
                        StageScrollSW = 1;

                        //ボスフラグオン
                        Enemy.BossApperFlag();
                        //StageScroll = -4700;
                    }
                }
                // ステージスクロールスイッチがオフのときは
                // スクロール.
                if (StageScrollSW == 0)
                {
                    
                    //当たり判定をずらす
                    for (int hanteii = 0; hanteii < HanteiPos[i].Length; hanteii++)
                    {
                        //Ｘをずらす
                        HanteiPos[i][hanteii] = (PointF)PointF.Subtract(HanteiPos[i][hanteii], new System.Drawing.Size((int)Mainus.X,0));
                        if (HanteiPos[i][hanteii].X>16 &&  HanteiPos[i][hanteii].X <= 960)
                        {
                            
                            int berons = (int)Math.Round((HanteiPos[i][hanteii].X-32) / 32 + (HanteiPos[i][hanteii].Y-32) / 32 * 30, MidpointRounding.AwayFromZero);
                            SpaceMapMass[berons].PointX = (int)HanteiPos[i][hanteii].X;
                            SpaceMapMass[berons].PointY = (int)HanteiPos[i][hanteii].Y;
                            SpaceMapMass[berons].MapNum = HanteiNums[i][hanteii];
                        }
                    }
                    
                    // ステージパネルをスクロール.
                    StagePanel.Left =  StageScroll;
                    
                }

            }
            //アニメーションタイム
            StageScrollAnime--;

            //画面上描写時のみ
            if (MarionDeathFlag != 1)
            {
                //プレイヤー動作関数呼び出し
                MarionMove();
            }
            if (Enemy.BossList.Count == 0)
            {
                //敵の出現処理
                Enemy.EnemyMapSet(Math.Abs(StageScroll));
                //敵の動き処理
                Enemy.EnemyMoves();
                //敵の弾生成処理
                Enemy.EnemyBulletSet(Math.Abs(StageScroll));
            }
            //敵の弾動き処理
            Enemy.EnemyShotMove(Marion.MarionX + Math.Abs(StageScroll), Marion.MarionY);
            //敵の空間マス計算
            EnemySpaceMass();
            //当たり判定設定関数実行
            CollisionCheck();
            //弾の当たり判定処理
            BulletCollision();
            //爆発設定関数
            EnemyExplosionTime();
            Enemy.BossAppaerMove(Marion.MarionX, Marion.MarionY,Math.Abs(StageScroll));

            //死亡カウント
            if(MarionDeathFlag == 1)
            {
                MarionDeathTimer++;
                if(MarionDeathTimer >=50)
                {
                    Marion = new PlayerMarion();
                    //パワー引継ぎ
                    Marion.MarionPower = MarionTransferPower;
                    MarionDeathFlag = 2;
                }
            }
            //無敵時間処理
            if(MarionDeathFlag == 2)
            {
                MarionDeathTimer++;
                if(MarionDeathTimer>=120)
                {
                    MarionDeathFlag = 0;
                }
            }

            //アニメーション遷移フラグ
            if (Math.Abs(StageScrollAnime) % 30 == 0)
            {
                AnimationTime = 0;
            }
            if (Math.Abs(StageScrollAnime) % 30 == 15)
            {
                AnimationTime = 1;
            }

            //透過深度経過
            if(Math.Abs(StageScrollAnime) % 2 == 0)
            {
                ia.SetColorMatrix(cm);
                if (AlphaFlag == 0)
                {
                    cm.Matrix33 -= 0.01F;
                    if (cm.Matrix33 <= 0.7F)
                    {
                        AlphaFlag = 1;
                    }
                }

                if (AlphaFlag == 1)
                {
                    cm.Matrix33 += 0.01F;
                    if (cm.Matrix33 >= 0.95F)
                    {
                        AlphaFlag = 0;

                    }
                }
                
            }
            //ステータス
            //this.Text = "" + Marion.MarionMass + ":" +  Marion.MarionX + ":"  + Marion.MarionY + "たまかず" +StageScroll;

            //地獄の人口太陽画面揺れ
            if(Enemy.BossShotFlag2 == 29)
            {
                
                StagePanel.Location = Enemy.gamenyure;
            }
            //地獄の人口太陽吸引
            if(Enemy.BossShotFlag2 >=30)
            {
                StagePanel.Location = Enemy.gamenyure;
                int GladSpeed = Enemy.BossShotFlag2 % 30 + 1;
                float angle = (float)(((Math.Atan2((Marion.MarionY - Enemy.Banis.Y), (Marion.MarionX - Enemy.Banis.X)) / Math.PI)*180)+360)%360;
                float ToBossX = Enemy.MathCos[(int)angle]* GladSpeed;
                float ToBossY = Enemy.MathSin[(int)angle]* GladSpeed;
                Marion.MarionX -= ToBossX;
                Marion.MarionY -= ToBossY;
            }
            //Clear時処理
            if(ClearFlag == 1)
            {
                ClearTimer++;
                if(ClearTimer>=300)
                {
                    NotTilte = 2;
                }
            }
            //判定可視化用のボタンスクロール
            //button2.Location = new System.Drawing.Point(button2.Location.X - 1, button2.Location.Y);
            //描写リセット(Paintイベント呼び出し)
            StagePanel.Refresh();
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
                if(Marion.MarionPower <= 60 && Marion.MarionPower >=0)
                {
                    Marion.MarionPowerLimit = 1;
                    //画面上の弾数は５まで
                    if (MarionbulletList.Count < Marion.MarionBulletMaxPower1)
                    {
                        Marion.MarionShot(MarionbulletList, Marion.MarionPowerLimit, StageScroll);
                    }
                    
                }
                if(Marion.MarionPower <=150 && Marion.MarionPower >= 60)
                {
                    Marion.MarionPowerLimit = 2;
                    //画面上の弾数は１０まで
                    if (MarionbulletList.Count < Marion.MarionBulletMaxPower2)
                    {
                        Marion.MarionShot(MarionbulletList, Marion.MarionPowerLimit, StageScroll);
                    }
                    
                }
                if (Marion.MarionPower <= 300 && Marion.MarionPower >= 150)
                {
                    Marion.MarionPowerLimit = 3;
                    //画面上の弾数は１５まで
                    if (MarionbulletList.Count < Marion.MarionBulletMaxPower3)
                    {
                        Marion.MarionShot(MarionbulletList, Marion.MarionPowerLimit, StageScroll);
                    }
                    
                }
                if (Marion.MarionPower <= 500 && Marion.MarionPower >= 300)
                {
                    Marion.MarionPowerLimit = 4;
                    //画面上の弾数は２０まで
                    if (MarionbulletList.Count < Marion.MarionBulletMaxPower4)
                    {
                        Marion.MarionShot(MarionbulletList, Marion.MarionPowerLimit, StageScroll);
                    }
                    
                }
                if (Marion.MarionPower >= 500 )
                {
                    Marion.MarionPowerLimit = 5;
                    //画面上の弾数は２５まで
                    if (MarionbulletList.Count < Marion.MarionBulletMaxPower5)
                    {
                        Marion.MarionShot(MarionbulletList, Marion.MarionPowerLimit, StageScroll);
                    }
                    if (Marion.MarionPower >= 600)
                        Marion.MarionPower = 600;
                    
                }

                if (Marion.MarionPower >= 600)
                {
                    Marion.MarionPowerLimit = 6;
                    //画面上の弾数は２５まで
                    if (MarionbulletList.Count < Marion.MarionBulletMaxPower6)
                    {
                        Marion.MarionShot(MarionbulletList, Marion.MarionPowerLimit, StageScroll);
                    }
                    if (Marion.MarionPower >= 700)
                        Marion.MarionPower = 700;

                }


            }
            //速度切り替え
            if (Movetriggerspace)
            {
                Marion.MarionSpeed = 6;
            }
            else
            {
                Marion.MarionSpeed = 12;
            }
            //プレイヤーの現在マスを格納
            Marion.MarionMass = (int)Marion.MarionX / 32 + (int)Marion.MarionY / 32 * 30;
            //ここまで//
            //移動範囲制限
            if (Marion.MarionX <= 0)
                Marion.MarionX = 0;
            if (Marion.MarionX >= 928)
                Marion.MarionX = 928;
            if (Marion.MarionY <= 0)
                Marion.MarionY = 0;
            if (Marion.MarionY >= 608)
                Marion.MarionY = 608;

        }

        //キーフラグ/キーを押しているとき
        private void Form_KeyDown(object sender, KeyEventArgs a)
        {

            //タイトル脱出
            if(a.KeyCode == Keys.Space)
            {
                if (NotTilte == 0)
                {
                    NotTilte = 1;
                    timer1.Start();
                }
                if(NotTilte == 2)
                {
                    Close();
                }
            }

            if (NotTilte == 1)
            {
                //上方向キーを押したとき
                if (a.KeyCode == Keys.W)
                {
                    Marion.MarionStatus = 4;
                    MovetriggerUp = true;
                }
                //下方向キーを押したとき
                if (a.KeyCode == Keys.S)
                {
                    Marion.MarionStatus = 2;
                    MovetriggerDown = true;
                }
                //左方向キーを押したとき
                if (a.KeyCode == Keys.A)
                {
                    MovetriggerLeft = true;

                }
                //右方向キーを押したとき
                if (a.KeyCode == Keys.D)
                {
                    MovetriggerRight = true;
                }

                //Zキーを離したとき
                if (a.KeyCode == Keys.O)
                {
                    Movetriggershot = true;
                }

                //スペースキーが離れているとき
                if (a.KeyCode == Keys.I)
                {
                    Movetriggerspace = true;
                }
            }

        }

        //キーフラグ/キーが離れたとき
        private void Form_KeyUp(object sender, KeyEventArgs a)
        {
            //上方向キーを離したとき
            if (a.KeyCode == Keys.W)
            {
                Marion.MarionStatus = 0;
                MovetriggerUp = false;
            }
            //下方向キーを離したとき
            if (a.KeyCode == Keys.S)
            {
                Marion.MarionStatus = 0;
                MovetriggerDown = false;
            }
            //左方向キーを離したとき
            if (a.KeyCode == Keys.A)
            {
                MovetriggerLeft = false;
            }
            //右方向キーを離したとき
            if (a.KeyCode == Keys.D)
            {
                MovetriggerRight = false;
            }
            //Zキーを離したとき
            if(a.KeyCode == Keys.O)
            {
                Movetriggershot = false;
            }
            //スペースキーが押されたとき
            if (a.KeyCode == Keys.I)
            {
                Movetriggerspace = false;
            }

            //Xキーが押されたとき
            if (a.KeyCode == Keys.X)
            {
                Marion.MarionPower += 1;
            }
        }

        

        //敵空間マス
        private void EnemySpaceMass()//☆
        {
            //敵がいるとき
            if (Enemy.EnemyList.Count > 0)
            {
                foreach (Enemy.Enemys enemy in Enemy.EnemyList)
                {
                    //敵マス計算
                    int EnemyMasss = (int)((enemy.EnemyPointX - 32 + StageScroll) / 32 + (enemy.EnemyPointY - 32) / 32 * 30);
                    //敵のマスに応じた配列に格納
                    if (EnemyMasss  >= 0 && EnemyMasss < 600)
                    {
                        EnemyBulletRam mass;
                        mass.PointX = enemy.EnemyPointX;
                        mass.PointY = enemy.EnemyPointY;
                        mass.EnemyNum = enemy.EnemyDraw;
                        mass.BulletRadius = Enemy.EnemyBulletRadius;
                        mass.BulletFlag = 0;
                        EnemyMass[EnemyMasss].Add(mass);
                    }
                }
            }
            //敵の弾があるとき
            if (Enemy.EnemyBulletsList.Count > 0)
            {
                foreach (Enemy.EnemyBullet enemy in Enemy.EnemyBulletsList)
                {
                    
                    //敵弾マス計算
                    int EnemyMasss = ((int)enemy.BulletX + StageScroll) / 32 + (int)enemy.BulletY / 32 * 30;
                    //敵の弾のマスを配列に格納
                    if (EnemyMasss >= 0 && EnemyMasss < 600)
                    {
                        EnemyBulletRam mass;
                        mass.PointX = enemy.BulletX + StageScroll;
                        mass.PointY = enemy.BulletY;
                        mass.EnemyNum = enemy.BulletDraw;

                        //弾の種類によって半径を切り替え
                        mass.BulletRadius = enemy.BulletVector /4;

                        mass.BulletFlag = 0;
                        //追加
                        EnemyBulletMass[EnemyMasss].Add(mass);
                        //特例処理(地獄極楽メルトダウン時巨大フレア弾の判定を周囲マスに格納
                        if (Enemy.BossArphaBullet == 1)
                        {
                            //左玉
                            if (enemy.BulletControlPoint1.X == 1)
                            {
                                mass.BulletRadius = enemy.BulletVector/5;
                                int bens = 0;
                                for (int i = 0; i < 200; i++)
                                {
                                    
                                    EnemyBulletMass[bens].Add(mass);
                                    bens++;
                                    if (bens % 10 == 0)
                                        bens += 20;
                                }
                            }
                            //右玉
                            if (enemy.BulletControlPoint1.X == 0)
                            {
                                mass.BulletRadius = enemy.BulletVector/5;
                                int bens = 19;
                                for (int i = 0; i < 200; i++)
                                {
                                    EnemyBulletMass[bens].Add(mass);
                                    bens++;
                                    if (bens % 30 == 0)
                                        bens += 20;

                                }
                            }
                            
                        }
                    }
                }
            }

        }

        
        //----------------------------------//
        //爆発座標格納関数                 //
        //引数:座標X,座標Y,拡大率X,拡大率Y//
        //-------------------------------//
        private void EnemyExplosion(float PointX, float PointY,int MagnificationX,int MagnificationY)
        {
            Bombs Bomb = new Bombs();
            Bomb.BombsPointX = PointX;
            Bomb.BombsPointY = PointY;
            Bomb.BombsMagnificationX = MagnificationX;
            Bomb.BombsMagnificationY = MagnificationY;
            Bomb.BombsTimes = 0;
            EnemyBombpos.Add(Bomb);

        }
    
        //弾消えセット
        private void BulletExplosion(float PointX, float PointY, int MagnificationX, int MagnificationY)
        {
            Bombs Bomb = new Bombs();
            Bomb.BombsPointX = PointX;
            Bomb.BombsPointY = PointY;
            Bomb.BombsMagnificationX = MagnificationX;
            Bomb.BombsMagnificationY = MagnificationY;
            Bomb.BombsTimes = 0;
            BulletBombpos.Add(Bomb);

        }


        //爆発更新関数
        private void EnemyExplosionTime()
        {
            for(int bumcnt = 0;bumcnt < EnemyBombpos.Count;bumcnt++)
            {
                Bombs bombom = EnemyBombpos[bumcnt];
                EnemyBombpos.RemoveAt(bumcnt);
                bombom.BombsPointX -= 1;
                bombom.BombsTimes += 2;
                EnemyBombpos.Insert(bumcnt, bombom);
            }

            //チャージ
            for (int bumcnt = 0; bumcnt < Enemy.EnemyChargepos.Count; bumcnt++)
            {
                Enemy.Bombs bombom = Enemy.EnemyChargepos[bumcnt];
                Enemy.EnemyChargepos.RemoveAt(bumcnt);
                if (StageScrollSW == 0)
                {
                    bombom.BombsPointX -= 1;
                }
                bombom.BombsTimes += 2;
                Enemy.EnemyChargepos.Insert(bumcnt, bombom);
            }
            //弾消え
            for (int bumcnt = 0; bumcnt < BulletBombpos.Count; bumcnt++)
            {
                Bombs bombom = BulletBombpos[bumcnt];
                BulletBombpos.RemoveAt(bumcnt);
                if (StageScrollSW == 0)
                {
                    bombom.BombsPointX -= 1;
                }
                bombom.BombsTimes += 2;
                BulletBombpos.Insert(bumcnt, bombom);
            }
        }
        
        //自機弾当たり判定
        private void BulletCollision()
        {
            if (MarionbulletList.Count > 0)
            {
                
                //弾の前進
                //MarionbulletList = MarionbulletList.Select(x => new PointF(x.X + Marion.MarionBulletSpeed , x.Y + Marion.MarionBulletSpeed)).ToList();
                for(int num = 0;num<MarionbulletList.Count;num++)
                {
                    PointF Marions = MarionbulletList[num];
                    MarionbulletList.RemoveAt(num);
                    Marions.X += Marion.MarionBulletSpeed;
                    //加算したものを反映
                    MarionbulletList.Insert(num, Marions);
                }
                //判定処理
                for(int BulletNum  = 0;BulletNum < MarionbulletList.Count;BulletNum++)
                {
                    //弾マスを計算
                    int beronss = (int)Math.Round(((double)MarionbulletList[BulletNum].X - 32) / 32 +
                        (MarionbulletList[BulletNum].Y - 32) / 32 * 30, MidpointRounding.AwayFromZero);
                    //マスが左上かつ右下以内の場合
                    if (beronss >= 0 && beronss < 600)
                    {

                        //BulletMass.Insert(beronss, new List<System.Drawing.Point>());
                        //BulletMass[beronss].Add(Bullets);

                        //周囲マス分判定
                        for(int Spaceloop = 0;Spaceloop < Marion.BulletSpaceMapMassMath.Length;Spaceloop++)
                        {
                            //弾の現在マス
                            int DetermineMass = Math.Abs(SpaceMath(beronss, Marion.BulletSpaceMapMassMath[Spaceloop]));
                            //判定処理
                            //スプライト番号1か2か91(四角形判定)
                            if (SpaceMapMass[DetermineMass].MapNum == 1
                                    || SpaceMapMass[DetermineMass].MapNum == 2
                                    || SpaceMapMass[DetermineMass].MapNum == 91)
                            {
                                //弾の座標と判定の座標の距離が３２以下
                                //(１マス分の外周と地形の外周が重なったら
                                if (Math.Abs(MarionbulletList[BulletNum].X - SpaceMapMass[DetermineMass].PointX) <= Marion.BulletRadius
                                    && Math.Abs(MarionbulletList[BulletNum].Y + 32 - SpaceMapMass[DetermineMass].PointY) <= Marion.BulletRadius)
                                {
                                    Marion.pels.Add(MarionbulletList[BulletNum]);

                                }
                            }
                            //Spacelloop(周囲マス判定終わり)//
                            //-----------------------------//
                        }

                        //敵との衝突判定

                        //敵判定
                        if (Enemy.EnemyList.Count > 0)
                        {
                            //リストの初期化
                    
                            //敵座標の格納

                            //敵の存在マスリストの作成が必要

                            for (int EnemyNum = 0; EnemyNum < Enemy.EnemyList.Count; EnemyNum++)
                            {
                                int Enemymass = (int)Math.Round((double)(Enemy.EnemyList[EnemyNum].EnemyPointX - 32) / 32 +
                                     (double)(Enemy.EnemyList[EnemyNum].EnemyPointY - 32) / 32 * 30, MidpointRounding.AwayFromZero);
                                if (Enemymass >= 0 && Enemymass < 600)
                                {
                                    //弾の座標と判定の座標の距離が３２以下
                                    //１マス分の外周と地形の外周が重なったら
                                    //また、敵の拡大率に応じて当たり判定を拡大
                                    if (Math.Abs(MarionbulletList[BulletNum].X + Math.Abs(StageScroll) - Enemy.EnemyList[EnemyNum].EnemyPointX) <= (Marion.BulletRadius-16) * ((Enemy.EnemyList[EnemyNum].MassMagnificationX / 32))
                                        && Math.Abs(MarionbulletList[BulletNum].Y - Enemy.EnemyList[EnemyNum].EnemyPointY) <= (Marion.BulletRadius-16) * ((Enemy.EnemyList[EnemyNum].MAssMagnificationY / 32)))
                                    {
                                        //マリオンの攻撃力分敵のHPを減少させる。
                                        //一度Enemys型の変数を作ってそこを変更して入れなおす必要があるわよ
                                        Enemy.Enemys OES = Enemy.EnemyList[EnemyNum];
                                        OES.EnemyHp = OES.EnemyHp - Marion.MarionAttack;
                                        Enemy.EnemyList[EnemyNum] = OES;
                                        
                                        if (Enemy.EnemyList[EnemyNum].EnemyHp <= 0)
                                        {
                                            //パワー加算
                                            Marion.MarionPower += 2;
                                            //爆発起動
                                            EnemyExplosion(Enemy.EnemyList[EnemyNum].EnemyPointX + StageScroll,
                                                Enemy.EnemyList[EnemyNum].EnemyPointY, Enemy.EnemyList[EnemyNum].MassMagnificationX,
                                                    Enemy.EnemyList[EnemyNum].MAssMagnificationY);
                                            //スコア加算
                                            TotalScore += 100;
                                            //敵削除
                                            Enemy.EnemyRemoveList.Add(Enemy.EnemyList[EnemyNum]);
                                            //弾の削除
                                            Marion.pels.Add(MarionbulletList[BulletNum]);

                                        }
                                        else
                                        {
                                            //弾の削除
                                            Marion.pels.Add(MarionbulletList[BulletNum]);
                                        }

                                    }
                                }
                            }

                        }

                        //ボスの判定
                        if (Enemy.BossList.Count > 0)
                        {

                            for (int BossPos = 0; BossPos < 5; BossPos++)
                            {
                                Enemy.BossCollision[BossPos].X = Enemy.BossList[0].BossX + Enemy.BossPosRef[BossPos].X +  Math.Abs(StageScroll);
                                Enemy.BossCollision[BossPos].Y = Enemy.BossList[0].BossY + Enemy.BossPosRef[BossPos].Y;
                                //１マス分の外周と地形の外周が重なったら
                                //また、敵の拡大率に応じて当たり判定を拡大
                                if (Math.Abs(MarionbulletList[BulletNum].X + Math.Abs(StageScroll) - Enemy.BossCollision[BossPos].X )
                                    <= Marion.BulletRadius-12
                                    && Math.Abs(MarionbulletList[BulletNum].Y - Enemy.BossCollision[BossPos].Y ) 
                                    <=  Marion.BulletRadius-12)
                                {
                                    //マリオンの攻撃力分敵のHPを減少させる。
                                    //無敵状態ではないときのみ
                                    if (Enemy.BossInvisibleTime == 0)
                                    {
                                        Enemy.BossEnemy OES = Enemy.BossList[0];
                                        OES.BossHP = OES.BossHP - Marion.MarionAttack;
                                        Enemy.BossList[0] = OES;
                                    }
                                    //HPが０以下になったら
                                    if (Enemy.BossList[0].BossHP <= 0)
                                    {
                                        //爆発起動
                                        EnemyExplosion(Enemy.BossList[0].BossX,
                                            Enemy.BossList[0].BossY, Enemy.BossList[0].MassMagnificationX,
                                                Enemy.BossList[0].MassMagnificationY);
                                        //スコア加算
                                        TotalScore += 10000;
                                        //ボスの削除
                                        Enemy.BossRemoveList.Add(Enemy.BossList[0]);
                                        //弾の削除
                                        Marion.pels.Add(MarionbulletList[BulletNum]);
                                        //クリアフラグ
                                        ClearFlag = 1;

                                    }
                                    else
                                    {
                                        //パワー加算
                                        Marion.MarionPower += 1;
                                        //爆発起動
                                        EnemyExplosion(MarionbulletList[BulletNum].X,
                                            MarionbulletList[BulletNum].Y,32,32);
                                        //スコア加算
                                        TotalScore += 100;
                                        //弾の削除
                                        Marion.pels.Add(MarionbulletList[BulletNum]);
                                    }

                                }
                            }
                        }
                       

                    }
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

        
        //--------------------//
        //当たり判定設定用関数//
        //引数:なし           //
        //--------------------//
        void CollisionCheck()
        {

            //自身の周囲9マスを参照
            for (int Spaceloop = 0; Spaceloop < SpaceMapMassMath.Length; Spaceloop++)
            {
                int DetermineMass = Math.Abs(SpaceMath(Marion.MarionMass, SpaceMapMassMath[Spaceloop]));
                if (Enemy.BossList.Count == 0)
                {
                    //スプライト番号1か2か91(四角形判定)
                    if (SpaceMapMass[DetermineMass].MapNum == 1
                            || SpaceMapMass[DetermineMass].MapNum == 2
                            || SpaceMapMass[DetermineMass].MapNum == 91)
                    {
                        //仮プレイヤーの座標と判定の座標の距離が３２以下
                        //(１マス分の外周とプレイヤーの外周が重なったら
                        if (Math.Abs(Marion.MarionX - SpaceMapMass[DetermineMass].PointX) <= Marion.MarionRadiusX
                            && Math.Abs(Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY + 16) <= Marion.MarionRadiusY)
                        {

                            //判定オン
                            //死亡
                            MarionDeath();

                        }
                    }

                    //スプライト番号の2桁目が１の場合(左上空白の三角形)
                    if (SpaceMapMass[DetermineMass].MapNum == 11)
                    {
                        //仮プレイヤーの座標と左上空白の三角形の図形が重なったら
                        //プレイヤーの座標と地形の座標のMarion.MarionX,Marion.MarionYをそれぞれかけ足したものをルート化
                        var mathposx = Marion.MarionX + 16 - SpaceMapMass[DetermineMass].PointX - 32;
                        var mathposy = Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY;
                        //それをプレイヤーの半径と地形の半径を足したものと比較し、小さかったら
                        //かつそのマスに収まる範囲内のもの
                        if (Math.Sqrt(mathposx * mathposx + mathposy * mathposy)
                            <= PlayerRadius + CicleHanteiRadius &&
                            Math.Abs(Marion.MarionX - SpaceMapMass[DetermineMass].PointX) <= Marion.MarionRadiusX
                            && Math.Abs(Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY + 16) <= Marion.MarionRadiusY)
                        {
                            //判定オン
                            //死亡
                            MarionDeath();
                        }

                    }

                    //スプライト番号の2桁目が2の場合(左下空白の三角形)
                    if (SpaceMapMass[DetermineMass].MapNum == 21)
                    {
                        //仮プレイヤーの座標と左下空白の三角形の図形が重なったら
                        //プレイヤーの座標と地形の座標のMarion.MarionX,Marion.MarionYをそれぞれかけ足したものをルート化
                        var mathposx = Marion.MarionX + 16 - SpaceMapMass[DetermineMass].PointX - 32;
                        var mathposy = Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY + 32;
                        //それをプレイヤーの半径と地形の半径を足したものと比較し、小さかったら
                        //かつそのマスに収まる範囲内のもの
                        if (Math.Sqrt(mathposx * mathposx + mathposy * mathposy)
                            <= PlayerRadius + CicleHanteiRadius &&
                            Math.Abs(Marion.MarionX - SpaceMapMass[DetermineMass].PointX) <= Marion.MarionRadiusX
                            && Math.Abs(Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY + 16) <= Marion.MarionRadiusY)
                        {
                            //判定オン
                            //死亡
                            MarionDeath();
                        }

                    }

                    //スプライト番号の2桁目が3の場合(右上空白の三角形)
                    if (SpaceMapMass[DetermineMass].MapNum == 31)
                    {
                        //仮プレイヤーの座標と右上空白の三角形の図形が重なったら
                        //プレイヤーの座標と地形の座標のMarion.MarionX,Marion.MarionYをそれぞれかけ足したものをルート化
                        var mathposx = Marion.MarionX + 16 - SpaceMapMass[DetermineMass].PointX;
                        var mathposy = Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY;
                        //それをプレイヤーの半径と地形の半径を足したものと比較し、小さかったら
                        //かつそのマスに収まる範囲内のもの
                        if (Math.Sqrt(mathposx * mathposx + mathposy * mathposy)
                            <= PlayerRadius + CicleHanteiRadius &&
                            Math.Abs(Marion.MarionX - SpaceMapMass[DetermineMass].PointX) <= Marion.MarionRadiusX
                            && Math.Abs(Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY + 16) <= Marion.MarionRadiusY)
                        {
                            //判定オン
                            //死亡
                            MarionDeath();
                        }

                    }

                    //スプライト番号の2桁目が4の場合(右下空白の三角形)
                    if (SpaceMapMass[DetermineMass].MapNum == 41)
                    {
                        //仮プレイヤーの座標と右上空白の三角形の図形が重なったら
                        //プレイヤーの座標と地形の座標のMarion.MarionX,Marion.MarionYをそれぞれかけ足したものをルート化
                        var mathposx = Marion.MarionX + 16 - SpaceMapMass[DetermineMass].PointX;
                        var mathposy = Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY + 32;
                        //それをプレイヤーの半径と地形の半径を足したものと比較し、小さかったら
                        //かつそのマスに収まる範囲内のもの
                        if (Math.Sqrt(mathposx * mathposx + mathposy * mathposy)
                            <= PlayerRadius + CicleHanteiRadius &&
                            Math.Abs(Marion.MarionX - SpaceMapMass[DetermineMass].PointX) <= Marion.MarionRadiusX
                            && Math.Abs(Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY + 16) <= Marion.MarionRadiusY)
                        {
                            //判定オン
                            //死亡
                            MarionDeath();
                        }
                    }

                    //スプライト番号の51(火山の左上空白１)
                    if (SpaceMapMass[DetermineMass].MapNum == 51)
                    {
                        //仮プレイヤーの座標と右上空白の三角形の図形が重なったら
                        //プレイヤーの座標と地形の座標のMarion.MarionX,Marion.MarionYをそれぞれかけ足したものをルート化
                        var mathposx = Marion.MarionX + 16 - SpaceMapMass[DetermineMass].PointX - 32;
                        var mathposy = Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY;
                        //それをプレイヤーの半径と地形の半径を足したものと比較し、小さかったら
                        //かつそのマスに収まる範囲内のもの
                        if (Math.Sqrt(mathposx * mathposx + mathposy * mathposy)
                            <= PlayerRadius + 5 &&
                            Math.Abs(Marion.MarionX - SpaceMapMass[DetermineMass].PointX) <= Marion.MarionRadiusX
                            && Math.Abs(Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY + 16) <= Marion.MarionRadiusY)
                        {
                            //判定オン
                            //死亡
                            MarionDeath();
                        }
                    }

                    //スプライト番号の61(火山の左上空白２)
                    if (SpaceMapMass[DetermineMass].MapNum == 61)
                    {
                        //仮プレイヤーの座標と右上空白の三角形の図形が重なったら
                        //プレイヤーの座標と地形の座標のMarion.MarionX,Marion.MarionYをそれぞれかけ足したものをルート化
                        var mathposx = Marion.MarionX + 16 - SpaceMapMass[DetermineMass].PointX - 32;
                        var mathposy = Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY;
                        //それをプレイヤーの半径と地形の半径を足したものと比較し、小さかったら
                        //かつそのマスに収まる範囲内のもの
                        if (Math.Sqrt(mathposx * mathposx + mathposy * mathposy)
                            <= PlayerRadius + 10 &&
                            Math.Abs(Marion.MarionX - SpaceMapMass[DetermineMass].PointX) <= Marion.MarionRadiusX
                            && Math.Abs(Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY + 16) <= Marion.MarionRadiusY)
                        {
                            //判定オン
                            //死亡
                            MarionDeath();
                        }
                    }

                    //スプライト番号の71(火山の右上空白１)
                    if (SpaceMapMass[DetermineMass].MapNum == 71)
                    {
                        //仮プレイヤーの座標と右上空白の三角形の図形が重なったら
                        //プレイヤーの座標と地形の座標のMarion.MarionX,Marion.MarionYをそれぞれかけ足したものをルート化
                        var mathposx = Marion.MarionX + 16 - SpaceMapMass[DetermineMass].PointX;
                        var mathposy = Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY;
                        //それをプレイヤーの半径と地形の半径を足したものと比較し、小さかったら
                        //かつそのマスに収まる範囲内のもの
                        if (Math.Sqrt(mathposx * mathposx + mathposy * mathposy)
                            <= PlayerRadius + 10 &&
                            Math.Abs(Marion.MarionX - SpaceMapMass[DetermineMass].PointX) <= Marion.MarionRadiusX
                            && Math.Abs(Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY + 16) <= Marion.MarionRadiusY)
                        {
                            //判定オン
                            //死亡
                            MarionDeath();
                        }
                    }

                    //スプライト番号の81(火山の右上空白２)
                    if (SpaceMapMass[DetermineMass].MapNum == 81)
                    {
                        //仮プレイヤーの座標と右上空白の三角形の図形が重なったら
                        //プレイヤーの座標と地形の座標のMarion.MarionX,Marion.MarionYをそれぞれかけ足したものをルート化
                        var mathposx = Marion.MarionX + 16 - SpaceMapMass[DetermineMass].PointX;
                        var mathposy = Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY;
                        //それをプレイヤーの半径と地形の半径を足したものと比較し、小さかったら
                        //かつそのマスに収まる範囲内のもの
                        if (Math.Sqrt(mathposx * mathposx + mathposy * mathposy)
                            <= PlayerRadius + 5 &&
                            Math.Abs(Marion.MarionX - SpaceMapMass[DetermineMass].PointX) <= Marion.MarionRadiusX
                            && Math.Abs(Marion.MarionY + 16 - SpaceMapMass[DetermineMass].PointY + 16) <= Marion.MarionRadiusY)
                        {
                            //判定オン
                        }
                    }
                }

                //弾の当たり判定☆
                if(Enemy.EnemyBulletsList.Count > 0)
                {
                    //プレイヤーと敵の弾の当たり判定を計算
                    //
                    for (int Bulletnum = 0; Bulletnum < EnemyBulletMass[DetermineMass].Count; Bulletnum++)
                    {
                        if (Math.Abs(Marion.MarionX - EnemyBulletMass[DetermineMass][Bulletnum].PointX+16) <= EnemyBulletMass[DetermineMass][Bulletnum].BulletRadius
                            && Math.Abs(Marion.MarionY - EnemyBulletMass[DetermineMass][Bulletnum].PointY+16) <= EnemyBulletMass[DetermineMass][Bulletnum].BulletRadius)
                        {
                           
                            //死亡
                            MarionDeath();
                        }
                       
                    }
                }

                //ボスの判定
                if (Enemy.BossList.Count > 0)
                {

                    for (int BossPos = 0; BossPos < 5; BossPos++)
                    {
                        Enemy.BossCollision[BossPos].X = Enemy.BossList[0].BossX + Enemy.BossPosRef[BossPos].X + Math.Abs(StageScroll);
                        Enemy.BossCollision[BossPos].Y = Enemy.BossList[0].BossY + Enemy.BossPosRef[BossPos].Y;
                        //１マス分の外周と地形の外周が重なったら
                        //また、敵の拡大率に応じて当たり判定を拡大
                        if (Math.Abs(Marion.MarionX + Math.Abs(StageScroll)- Enemy.BossCollision[BossPos].X)
                            <= Marion.MarionRadiusX
                            && Math.Abs(Marion.MarionY - Enemy.BossCollision[BossPos].Y)
                            <= Marion.MarionRadiusY)
                        {
                            //死亡
                            MarionDeath();
                        }
                    }
                }
                //--------------------------------------//
            }

        }

        //脂肪処理
        private void MarionDeath()
        {
            if (MarionDeathFlag == 0)
            {
                //爆発起動
                EnemyExplosion(Marion.MarionX, Marion.MarionY, Marion.MarionMagnifictionX, Marion.MarionMagnifictionY);
                //死亡フラグを立てる
                Marion.MarionX = -1000;
                Marion.MarionY = 300;
                MarionDeathFlag = 1;
                MarionDeathTimer = 0;
                //パワー少し減少
                MarionTransferPower = Marion.MarionPower - 200;
                if(MarionTransferPower < 0)
                {
                    MarionTransferPower = 0;
                }
                //スコア減少
                TotalScore -= 1000;
                //トータルデス加算
                TotalDeath++;
                if (Marion.bulletList.Count > 0)
                {
                    MarionbulletList = Marion.bulletList;
                }
            }
            
        }
        //ステータスバー
        private void StatusBar()
        {
            

        }

        //描写起動
        private void Draw(object sender, PaintEventArgs e)
        {
            if(NotTilte == 1)
                Show(e.Graphics);
            if(NotTilte == 0)
                TitleScreen(e.Graphics);
            if (NotTilte == 2)
                EndScreen(e.Graphics);
        }

        //描写
        public void Show(Graphics g)
        {
            
            //ステージの描写
            g.DrawImage(mapima, 0, 0);
            //自機の描写
            if (MarionDeathFlag == 2)
            {
                //無敵描写(点滅)
                if(MarionDeathTimer % 3 == 0)
                g.DrawImage(MarionDraw[Marion.MarionStatus + AnimationTime], Marion.MarionX + Math.Abs(StageScroll), Marion.MarionY, 32, 32);
            }
            else
            {
                g.DrawImage(MarionDraw[Marion.MarionStatus + AnimationTime], Marion.MarionX + Math.Abs(StageScroll), Marion.MarionY, 32, 32);
            }
            //ファンネルの描写

            if (Marion.MarionPowerLimit >= 2)
            {
                //ファンネル１
                g.DrawImage(MarionDraw[6+AnimationTime], Marion.MarionX + Math.Abs(StageScroll) - 26, Marion.MarionY + 26, 32, 32);
                if (Marion.MarionPowerLimit >= 3)
                {
                    //ファンネル２
                    g.DrawImage(MarionDraw[6 + AnimationTime], Marion.MarionX + Math.Abs(StageScroll) - 26, Marion.MarionY - 26, 32, 32);
                    if (Marion.MarionPowerLimit >= 4)
                    {
                        //ファンネル３
                        g.DrawImage(MarionDraw[6 + AnimationTime], Marion.MarionX + Math.Abs(StageScroll) - 26-26, Marion.MarionY +26+26, 32, 32);
                        if (Marion.MarionPowerLimit >= 5)
                        {
                            //ファンネル４
                            g.DrawImage(MarionDraw[6 + AnimationTime], Marion.MarionX + Math.Abs(StageScroll) - 26 - 26, Marion.MarionY - 26 - 26, 32, 32);
                        }
                    }
                }
            }

            
            foreach (PointF pt in MarionbulletList)
            {
                //弾の描写
                g.DrawImage(NomalBallets, pt.X+Math.Abs(StageScroll), pt.Y, 32, 32);
                if(pt.X + Math.Abs(StageScroll) >= 980 + Math.Abs(StageScroll))
                {
                    //画面外に出た場合削除リストに追加
                    Marion.pels.Add(pt);
                }
            }


            //敵の描写
            foreach (Enemy.Enemys enemy in Enemy.EnemyList)
            {
                //拡大率を大きくしても位置が変わらないように計算しつつ描写
                g.DrawImage(EnemyDraw[enemy.EnemyDraw+AnimationTime], enemy.EnemyPointX + (((enemy.MassMagnificationX / 32) - 1) * -16), 
                    enemy.EnemyPointY+(((enemy.MAssMagnificationY / 32)-1) * -16),enemy.MassMagnificationX, enemy.MAssMagnificationY);
                //画面外に出たとき削除リストへ追加
                if(enemy.EnemyPointX - Math.Abs(StageScroll) <= -64 * enemy.MassMagnificationX)
                {
                    Enemy.EnemyRemoveList.Add(enemy);
                }
            }

            //ボスの描写
            foreach (Enemy.BossEnemy Boss in Enemy.BossList)
            {
                //拡大率を大きくしても位置が変わらないように計算しつつ描写
                g.DrawImage(BossDraw[Boss.BossType + AnimationTime], Boss.BossX + Math.Abs(StageScroll) + (((Boss.MassMagnificationX / 64) - 1) * -32),
                    Boss.BossY + (((Boss.MassMagnificationY / 64) - 1) * -32), Boss.MassMagnificationX, Boss.MassMagnificationY);
            }

            //敵の弾の描写
            foreach (Enemy.EnemyBullet enemybullet in Enemy.EnemyBulletsList)
            {
                if (Enemy.BossArphaBullet == 1|| Enemy.BossArphaBullet == 2 && enemybullet.BulletDraw !=2)
                {
                    
                    //透過
                    Rectangle aiu = new Rectangle((int)enemybullet.BulletX - (enemybullet.BulletVector / 2),
                        (int)enemybullet.BulletY - (enemybullet.BulletVector / 2), enemybullet.BulletVector, enemybullet.BulletVector);
                    //拡大率を大きくしても位置が変わらないように計算しつつ描写
                    g.DrawImage(EnemyBulletDraw[enemybullet.BulletDraw + AnimationTime], aiu,0,0, 32, 32, GraphicsUnit.Pixel, ia);
                }
                else
                { 
                //拡大率を大きくしても位置が変わらないように計算しつつ描写
                g.DrawImage(EnemyBulletDraw[enemybullet.BulletDraw + AnimationTime], enemybullet.BulletX - (enemybullet.BulletVector / 2),
                    enemybullet.BulletY - (enemybullet.BulletVector / 2), enemybullet.BulletVector, enemybullet.BulletVector);
                }
                //画面外に出たとき削除リストへ追加
                if (Enemy.BossBulletDisappearEX == 0)
                {
                    if (enemybullet.BulletX - Math.Abs(StageScroll) <= -64 ||
                        enemybullet.BulletX - Math.Abs(StageScroll) >= 1000 ||
                        enemybullet.BulletY <= -50 ||
                        enemybullet.BulletY >= 630)
                    {
                        Enemy.EnemyBulletsRemoveList.Add(enemybullet);

                    } 
                }
                else
                {
                    //フィクトスター専用
                    if (enemybullet.BulletX - Math.Abs(StageScroll) <= -400 ||
                        enemybullet.BulletX - Math.Abs(StageScroll) >= 1600 ||
                        enemybullet.BulletY <= -600 ||
                        enemybullet.BulletY >= 1300)
                    {
                        Enemy.EnemyBulletsRemoveList.Add(enemybullet);

                    }
                }
            }

          

            //爆発の描写
            foreach (Bombs bom in EnemyBombpos)
            {
                //拡大率を大きくしても爆発位置が変わらないように計算しつつ描写
                g.DrawImage(EnemyBomb[bom.BombsTimes/5], bom.BombsPointX+Math.Abs(StageScroll) + (((bom.BombsMagnificationX / 32) - 1) * -16), 
                    bom.BombsPointY + (((bom.BombsMagnificationY / 32) - 1) * -16), bom.BombsMagnificationX, bom.BombsMagnificationY);
                if(bom.BombsTimes > 27)
                {
                    EnemyBombposRemove.Add(bom);
                }
            }

            //チャージの描写
            foreach (Enemy.Bombs charge in Enemy.EnemyChargepos)
            {
                //拡大率を大きくしても爆発位置が変わらないように計算しつつ描写
                g.DrawImage(Enemy.ChargeEff[charge.BombsTimes / 5], charge.BombsPointX + Math.Abs(StageScroll) + (((charge.BombsMagnificationX / 32) - 1) * -16),
                    charge.BombsPointY + (((charge.BombsMagnificationY / 32) - 1) * -16), charge.BombsMagnificationX, charge.BombsMagnificationY);
                if (charge.BombsTimes > 27)
                {
                    Enemy.EnemyChargeRemove.Add(charge);
                }
            }

            //一括弾消し
            if(Enemy.BulletBombs >= 1)
            {
                foreach (Enemy.EnemyBullet enemybullet in Enemy.EnemyBulletsList)
                {
                    BulletExplosion(enemybullet.BulletX, enemybullet.BulletY, enemybullet.BulletVector, enemybullet.BulletVector);
                }
                Enemy.EnemyBulletsList.Clear();
                Enemy.BulletBombs = 0;
                if(Enemy.BulletBombs == 2)
                {
                    BombWait = 12000;
                }
            }

            //弾消えの描写
            foreach (Bombs bombs in BulletBombpos)
            {
                //拡大率を大きくしても爆発位置が変わらないように計算しつつ描写
                g.DrawImage(BulletBomb[bombs.BombsTimes / 10], bombs.BombsPointX +(((bombs.BombsMagnificationX / 32) - 1) * -16),
                    bombs.BombsPointY + (((bombs.BombsMagnificationY / 32) - 1) * -16), bombs.BombsMagnificationX, bombs.BombsMagnificationY);
                if (bombs.BombsTimes > 30)
                {
                    BulletBombRemove.Add(bombs);
                }
                for(int i = 0;i < BombWait;i++)
                {
                    int benss = 0;
                    //空処理(意図的な処理落ち)
                }
            }

            //WARNIGアニメーション
            if(Enemy.BossFlag == 1||Enemy.BossFlag == 4)
            {
                for (int ii = 0; ii < 20; ii+=2)
                {
                    for (int i = Enemy.WarningDraw2; i < Enemy.WarningDraw; i++)
                    {
                        if (i % 2 == 0)
                        {
                            g.DrawImage(EnemyDraw[6], ((i * 32)+32) + Math.Abs(StageScroll), ii * 32, 32, 32);
                        }
                    }
                }
                for (int ii = 0; ii < 20; ii++)
                {
                    for (int i = Enemy.WarningDraw2; i < Enemy.WarningDraw; i++)
                    {
                        if (i % 2 == 0)
                        {
                            if (ii % 2 == 1)
                            {
                                g.DrawImage(EnemyDraw[6], (960 - (i * 32)) + Math.Abs(StageScroll), (ii * 32), 32, 32);
                            }
                        }
                    }
                }
                //増やす処理
                if (Enemy.WarTime1 % 1 == 0&&Enemy.WarTime1<= 80)
                {
                    Enemy.WarningDraw++;
                    if(Enemy.WarningDraw >= 31)
                    {
                        Enemy.WarningDraw = 31;
                    }

                }
                //戻す処理
                if(Enemy.WarTime1>=80&&Enemy.WarTime2 % 1 == 0)
                {
                    Enemy.WarningDraw2++;
                }

            }

            //スコア描写
            g.DrawString("SCORE:" + TotalScore.ToString() + " D:" + TotalDeath.ToString(), fnt, Brushes.White, 0 + Math.Abs(StageScroll), 0);
            //描写削除
            ShowRemove();

        }


        //描写を削除する関数
        public void ShowRemove()
        {

            //弾を削除
            foreach (PointF pt in Marion.pels)
            {
                MarionbulletList.Remove(pt);
                Marion.pels = new List<PointF>();
            }

            //敵を削除
            foreach (Enemy.Enemys enemy in Enemy.EnemyRemoveList)
            {
                Enemy.EnemyList.Remove(enemy);
                Enemy.EnemyRemoveList = new List<Enemy.Enemys>();
            }

            //敵の弾を削除
            foreach (Enemy.EnemyBullet enemybullet in Enemy.EnemyBulletsRemoveList)
            {
                Enemy.EnemyBulletsList.Remove(enemybullet);
                Enemy.EnemyBulletsRemoveList = new List<Enemy.EnemyBullet>();
            }

            //爆発の削除
            foreach (Bombs bom in EnemyBombposRemove)
            {
                EnemyBombpos.Remove(bom);
                EnemyBombposRemove = new List<Bombs>();
            }

            //チャージの削除
            foreach (Enemy.Bombs charge in Enemy.EnemyChargeRemove)
            {
                Enemy.EnemyChargepos.Remove(charge);
                Enemy.EnemyChargeRemove = new List<Enemy.Bombs>();
            }

            //弾消えの削除
            foreach (Bombs charge in BulletBombRemove)
            {
                BulletBombpos.Remove(charge);
                BulletBombRemove = new List<Bombs>();
                WarningDraw = 0;
                WarningDraw2 = 0;
            }

            //ボスの削除
            foreach (Enemy.BossEnemy bos in Enemy.BossRemoveList)
            {
                Enemy.BossList.Remove(bos);
                Enemy.BossList = new List<Enemy.BossEnemy>();
            }
            //配列のクリア
            Array.Clear(EnemyMass, 0, EnemyMass.Length);
            Array.Clear(EnemyBulletMass, 0, EnemyBulletMass.Length);
            //敵空間マスの初期化
            for (int i = 0; i < 600; i++)
            {
                EnemyMass[i] = new List<EnemyBulletRam>();
                EnemyBulletMass[i] = new List<EnemyBulletRam>();
            }
        }

        //Clear画面
        public void EndScreen(Graphics g)
        {
            Bitmap canvas = new Bitmap(this.Width, this.Height);
            Title = Graphics.FromImage(canvas);

            fnt = new Font("MS UI Gothic", 26);
            
            g.DrawString("TOTALSCORE:"+TotalScore.ToString(), fnt, Brushes.White, Math.Abs(StageScroll)+330, 150);
            fnt = new Font("MS UI Gothic", 26);
            g.DrawString("TOTAL DEATH:"+TotalDeath.ToString() , fnt, Brushes.White, Math.Abs(StageScroll)+330, 220);
            g.DrawString("MISSION COMPLETE!",fnt, Brushes.White, Math.Abs(StageScroll) +330, 290);
            g.DrawString("CLOSED PUSH SPACE", fnt, Brushes.White, Math.Abs(StageScroll) + 330, 360);
            if (TotalDeath == 0)
            {
                g.DrawString("ナカナカヤルナ！", fnt, Brushes.White, Math.Abs(StageScroll) + 330, 460);
            }
        }
        //タイトルの描写
        public void TitleScreen(Graphics g)
        {
            Bitmap canvas = new Bitmap(this.Width, this.Height);
            Title = Graphics.FromImage(canvas);
            g.FillRectangle(Brushes.Black, 0, 0, 960 + Math.Abs(StageScroll), 700);
            fnt = new Font("MS UI Gothic", 50);
            //g.FillRectangle(Brushes.Black, 0, 0, 960, 600);
            g.DrawString("MARIDIUS", fnt, Brushes.Blue, 960/2-150, 150);
            fnt = new Font("MS UI Gothic", 20);
            g.DrawString("START TO PUSH SPACE", fnt, Brushes.Blue, 960 / 2-142 , 300);
            g.DrawString("WASD KEY  MOVE O KEY SHOT I KEY SLOW", fnt, Brushes.Blue, 370 - 142, 400);
            //fnt.Dispose();
            //g.Dispose();

        }

        //classおわり
    }
    //namespaceおわり
}
//めも

