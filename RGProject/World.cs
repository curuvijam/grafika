﻿using SharpGL;
using System;
using System.Collections;
using SharpGL.SceneGraph.Primitives;
using System.Collections.Generic;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace RGProject
{

    public enum TextureFilterMode
    {
        Nearest,
        Linear,
        NearestMipmapNearest,
        NearestMipmapLinear,
        LinearMipmapNearest,
        LinearMipmapLinear
    };
    public class World : IDisposable
    {
        private float m_xRotation = 0.0f;
        private float m_yRotation = 0.0f;
        private float translateLeftWallX = 0.0f;
        private float rightWallRotateY = 0.0f;
        private float scaleArrow = 1.0f;
        private float arrowXTranslate = 0.0f;
        private float arrowYTranslate = 0.0f;
        private float arrowZTranslate = 0.0f;
        private float arrowXRotation = 0.0f;
        private int arrowCounter = 0;
        private enum TextureObjects { Staza = 0, Podloga, Zid };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;
        private uint[] m_textures = null;
        private string[] m_textureFiles = { "..//..//images//staza.jpg", "..//..//images//Podloga.jpg", "..//..//images//metal.jpg" };
        private AssimpScene m_scene;
        private AssimpScene m_scene2;
        private float m_sceneDistance = 4000.0f;
        private int m_height;
        private int m_width;
        private TextureFilterMode m_selectedMode = TextureFilterMode.Nearest;
        private bool animationRunning = false;
        private DispatcherTimer timer1;
        private DispatcherTimer timer2;
        private DispatcherTimer timer3;
        private DispatcherTimer timer4;

        public bool AnimationRunning
        {
            get
            {
                return animationRunning;
            }
            set
            {
                animationRunning = value;
            }
        }

        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        public float TranslateLeftWallX
        {
            get { return translateLeftWallX; }
            set { translateLeftWallX = value; }
        }

        public float RightWallRotateY
        {
            get { return rightWallRotateY; }
            set { rightWallRotateY = value; }
        }

        public float ScaleArrow
        {
            get { return scaleArrow; }
            set { scaleArrow = value; }
        }

        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        public AssimpScene Scene2
        {
            get { return m_scene2; }
            set { m_scene2 = value; }
        }

        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public World(String scenePath, String sceneFileName, String scenePath2, String sceneFileName2, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_scene2 = new AssimpScene(scenePath2, sceneFileName2, gl);
            m_textures = new uint[m_textureCount];
        }


        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            //gl.ShadeModel(OpenGL.GL_SMOOTH);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            //gl.FrontFace(OpenGL.GL_CW);
            gl.Enable(OpenGL.GL_LIGHTING);

            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            LoadTextures(gl);
            

            m_scene.LoadScene();
            m_scene2.LoadScene();
            m_scene.Initialize();
            m_scene2.Initialize();


        }

        private void LoadTextures(OpenGL gl)
        {


            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL);
            gl.GenTextures(m_textureCount, m_textures);


            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);		// Linear Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);      // Linear Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
                image.UnlockBits(imageData);
                image.Dispose();
            }
        }

        private void SetupLighting(OpenGL gl)
        {
            float[] ambijentalnaKomponenta = { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] difuznaKomponenta = { 1.0f, 1.0f, 0.0f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, ambijentalnaKomponenta);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, difuznaKomponenta);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
            gl.Enable(OpenGL.GL_LIGHT0);
            float[] pozicija = { -2000.0f, 1500.0f, 0.0f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, pozicija);

            gl.Enable(OpenGL.GL_NORMALIZE);
        }

        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);//preko celog ekrana
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width / (double)height, 0.5f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

        }
        public void PomeranjeKamereUZamak(object sender, EventArgs e)
        {
            this.SceneDistance -= 50;
            if(this.SceneDistance==-100)
            {
                timer1.Stop();
                this.timer2 = new DispatcherTimer();
                timer2.Interval = TimeSpan.FromMilliseconds(20);
                timer2.Tick += new EventHandler(PrelazakUPticiju);
                timer2.Start();
            }

        }

        public void Udaljavanje(object sender, EventArgs e)
        {
            this.SceneDistance += 50;
            if(this.m_sceneDistance==3200)
            {
                timer3.Stop();
                this.timer4 = new DispatcherTimer();
                timer4.Interval = TimeSpan.FromMilliseconds(20);
                timer4.Tick += new EventHandler(IspaljivanjeStrele);
                timer4.Start();
                
            }
        }

        public void IspaljivanjeStrele(object sender, EventArgs e)
        {
            
            arrowYTranslate += 200;
            if(arrowYTranslate>4000)
            {
                arrowYTranslate = -250.0f;
                arrowCounter+=1;
            }
            if (arrowCounter == 10)
            {
                timer4.Stop();
                ResetParameters();
                animationRunning = false;
            }
        }

        public void ResetParameters()
        {
            m_xRotation = 0.0f;
            m_yRotation = 0.0f;
            m_sceneDistance = 4000.0f;
            arrowCounter = 0;
            arrowXRotation = 0;
            arrowYTranslate = 0;
            arrowZTranslate = 0;
        }

        public void PrelazakUPticiju(object sender, EventArgs e)
        {
            this.m_xRotation += 5;
            if(m_xRotation == 90)
            {
                timer2.Stop();
                this.timer3 = new DispatcherTimer();
                timer3.Interval = TimeSpan.FromMilliseconds(20);
                timer3.Tick += new EventHandler(Udaljavanje);
                timer3.Start();
            }
        }

        public void startAnimation()
        {
            animationRunning = true;
            arrowXTranslate = 500.0f;
            arrowYTranslate = -250.0f;
            arrowZTranslate = 1000.0f;
            arrowXRotation = 90.0f;
            this.timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(20);
            timer1.Tick += new EventHandler(PomeranjeKamereUZamak);
            timer1.Start();
            animationRunning = true;
        }

        public void Draw(OpenGL gl)
        {

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Viewport(0, 0, m_width, m_height);
            gl.PushMatrix();
            {
                gl.Translate(0.0f, 0.0f, -m_sceneDistance);
                gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
                gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
                gl.LookAt(0f, 50f, 200f, 0f, 0f, 0f, 0f, 1.0f, 0f);

                //Iscrtavanje modela zamka
                gl.PushMatrix();
                {
                    gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
                    m_scene.Draw();
                }
                gl.PopMatrix();

                //iscrtavanje modela strele
                gl.PushMatrix();
                {
                    
                    gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
                    gl.Color(1, 0, 0);
                    gl.Translate(-500.0f+arrowXTranslate, 250f+arrowYTranslate, -1250.0f+arrowZTranslate);
                    gl.Rotate(0.0f , 0.0f , 0.0f + arrowXRotation);
                    
                    gl.Scale(50 * scaleArrow, 50 * scaleArrow, 50 * scaleArrow);
                    m_scene2.Draw();
                }
                gl.PopMatrix();
                //Iscrtavanje podloge
                gl.PushMatrix();
                {

                    gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL);
                    gl.Translate(0.0f, -5.0f, 0.0f);
                    gl.MatrixMode(OpenGL.GL_TEXTURE);
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Podloga]);
                    gl.PushMatrix();
                    gl.MatrixMode(OpenGL.GL_MODELVIEW);
                    gl.Begin(OpenGL.GL_QUADS);
                    gl.Normal(0.0f, 1.0f, 0.0f);
                    gl.TexCoord(0.0f, 0.0f);
                    gl.Vertex(-1500.0f, 0.0f, 1500.0f);
                    gl.TexCoord(0.0f, 1.0f);
                    gl.Vertex(1500.0f, 0.0f, 1500.0f);
                    gl.TexCoord(1.0f, 1.0f);
                    gl.Vertex(1500.0f, 0.0f, -1500.0f);
                    gl.TexCoord(1.0f, 0.0f);
                    gl.Vertex(-1500.0f, 0.0f, -1500.0f);
                    gl.End();
                    gl.MatrixMode(OpenGL.GL_TEXTURE);
                }
                gl.PopMatrix();
                gl.MatrixMode(OpenGL.GL_MODELVIEW);
                gl.PopMatrix();

                //Iscrtavanje staze
                gl.PushMatrix();
                {
                    //gl.Color(0f, 0f, 1f);
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Staza]);
                    gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL);
                    gl.Begin(OpenGL.GL_QUADS);
                    gl.Normal(0.0f, -1.0f, 0.0f);
                    gl.TexCoord(0.0f, 0.0f);
                    gl.Vertex(-300.0f, 0.0f, 1500.0f);//gore levo
                    gl.TexCoord(1.0f, 0.0f);
                    gl.Vertex(300.0f, 0.0f, 1500.0f);//gore desno
                    gl.TexCoord(1.0f, 1.0f);
                    gl.Vertex(300.0f, 0.0f, -500.0f);//dole desno
                    gl.TexCoord(0.0f, 1.0f);
                    gl.Vertex(-300.0f, 0.0f, -500.0f);//dole levo
                    gl.End();
                }
                gl.PopMatrix();

                //iscrtavanje desnog zida
                gl.PushMatrix();
                {
                    Cube cube = new Cube();
                    gl.Rotate(0, 0 + rightWallRotateY, 0);
                    gl.Translate(1200.0f, 300f, 0.0f);
                    gl.Scale(1, 300, 1500);
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Zid]);

                    gl.Color(0f, 0f, 1f);

                    cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                }
                gl.PopMatrix();

                //iscrtavanje levog zida
                gl.PushMatrix();
                {
                    Cube cube = new Cube();
                    gl.Translate(-1200.0f + translateLeftWallX, 300f, 0.0f);
                    gl.Scale(1, 300, 1500);
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Zid]);
                    gl.Color(1f, 0f, 1f);

                    cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                }
                gl.PopMatrix();

                /*gl.PushMatrix();
                {
                    Cube cube = new Cube();
                    gl.Translate(0.0f, 1200.0f,0.0f);
                    gl.Scale(10, 10, 10);
                    gl.Color(1, 0, 0);
                    cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                }
                gl.PopMatrix();*/
                
            }
            gl.PopMatrix();

            //Iscrtavanje teksta
            
            gl.Viewport(m_width / 2, 0, m_width / 2, m_height / 2);
            {
                gl.PushMatrix();
                gl.DrawText(300, 150, 0.0f, 255.0f, 255.0f, "Arial", 14, "Predmet: Racunarska grafika");
                gl.DrawText(300, 150, 0.0f, 255.0f, 255.0f, "Arial", 14, "_______________________");
                gl.DrawText(300, 120, 0.0f, 255.0f, 255.0f, "Arial", 14, "Sk.god: 2017/18.");
                gl.DrawText(300, 120, 0.0f, 255.0f, 255.0f, "Arial", 14, "_____________");
                gl.DrawText(300, 90, 0.0f, 255.0f, 255.0f, "Arial", 14, "Ime: Marko");
                gl.DrawText(300, 90, 0.0f, 255.0f, 255.0f, "Arial", 14, "_________");
                gl.DrawText(300, 60, 0.0f, 255.0f, 255.0f, "Arial", 14, "Prezime: Curuvija");
                gl.DrawText(300, 60, 0.0f, 255.0f, 255.0f, "Arial", 14, "______________");
                gl.DrawText(300, 30, 0.0f, 255.0f, 255.0f, "Arial", 14, "Sifra zad: 20.1 ");
                gl.DrawText(300, 30, 0.0f, 255.0f, 255.0f, "Arial", 14, "___________");

            }
            reflektor(gl);
            SetupLighting(gl);
            gl.PopMatrix();
            gl.PopMatrix();
            gl.Flush();
        }

        public void reflektor(OpenGL gl)
        {

            float[] ambijentalnaKomponenta = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] difuznaKomponenta = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] smer = { 0f, -1f, 0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, ambijentalnaKomponenta);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, difuznaKomponenta);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 45.0f);
            gl.Enable(OpenGL.GL_LIGHT1);
            float[] pozicija = { 0f, 1200f, 0f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, pozicija);

        }


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~World()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }

    }


}
