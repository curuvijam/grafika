using SharpGL;
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

namespace RGProject
{
    public class World : IDisposable
    {
        private float m_xRotation = 0.0f;
        private float m_yRotation = 0.0f;
        private AssimpScene m_scene;
        private AssimpScene m_scene2;
        private float m_sceneDistance = 4000.0f;
        private int m_height;
        private int m_width;

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
        }


        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);

            m_scene.LoadScene();
            m_scene2.LoadScene();
            m_scene.Initialize();
            m_scene2.Initialize();

        }

        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(50f, (double)width / height, 1f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Viewport(0, 0, m_width, m_height);
            gl.LoadIdentity();

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
                //gl.LookAt(0f, 150.0f, -200.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);

                //Iscrtavanje podloge
                gl.PushMatrix();
                {
                    gl.Translate(0.0f, -5.0f, 0.0f);
                    gl.Color(1f, 1f, 1f);
                    gl.Begin(OpenGL.GL_QUADS);
                    gl.Vertex(-1500.0f, 0.0f, 1500.0f);
                    gl.Vertex(1500.0f, 0.0f, 1500.0f);
                    gl.Vertex(1500.0f, 0.0f, -1500.0f);
                    gl.Vertex(-1500.0f, 0.0f, -1500.0f);
                    gl.End();                   
                }
                gl.PopMatrix();

                //Iscrtavanje staze
                gl.PushMatrix();
                {
                    gl.Color(0f, 0f, 1f);
                    gl.Begin(OpenGL.GL_QUADS);
                    gl.Vertex(-150.0f, 0.0f, 1500.0f);//gore levo
                    gl.Vertex(150.0f, 0.0f, 1500.0f);//gore desno
                    gl.Vertex(150.0f, 0.0f, -500.0f);//dole desno
                    gl.Vertex(-150.0f, 0.0f, -500.0f);//dole levo
                    gl.End();
                }
                gl.PopMatrix();

                //Iscrtavanje modela zamka
                gl.PushMatrix();
                {
                    m_scene.Draw();
                }
                gl.PopMatrix();

                //iscrtavanje modela strele
                gl.PushMatrix();
                {
                    gl.Translate(-500.0f, 250f, -1250.0f);
                    gl.Scale(50, 50, 50);
                    m_scene2.Draw();
                }
                gl.PopMatrix();
                
                //iscrtavanje zida
                gl.PushMatrix();
                {
                    gl.Translate(1200.0f, 300f, 0.0f);
                    gl.Scale(100, 300, 1500);
                    gl.Color(1f, 0f, 1f);
                    Cube cube = new Cube();
                    cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                }
                gl.PopMatrix();

                //iscrtavanje drugog zida
                gl.PushMatrix();
                {
                    gl.Translate(-1200.0f, 300f, 0.0f);
                    gl.Scale(100, 300, 1500);
                    gl.Color(1f, 0f, 1f);
                    Cube cube = new Cube();
                    cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                }
                gl.PopMatrix();
            }           
            gl.PopMatrix();

            //Iscrtavanje teksta
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
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
            gl.PopMatrix();
            gl.Flush();
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
