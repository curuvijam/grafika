using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpGL.SceneGraph;
using SharpGL;
using System.Reflection;
using Microsoft.Win32;
using System.Globalization;

namespace RGProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window

    {
        World m_world = null;
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                m_world = new World(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "C:\\Users\\Marko\\source\\repos\\RGProject\\RGProject\\Model"), "castle.obj", System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "C:\\Users\\Marko\\source\\repos\\RGProject\\RGProject\\Model"), "BOW AND ARROW.obj",(int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta", "GRESKA", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void OpenGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F4: this.Close(); break;
                case Key.K:
                    if (!m_world.AnimationRunning)
                    {
                        if (m_world.RotationX >= 5.0f)
                        {
                            m_world.RotationX -= 5.0f;
                        }
                    }
                    break;
                case Key.I:
                    if (!m_world.AnimationRunning)
                    {
                        if (m_world.RotationX <= 45.0f)
                        {
                            m_world.RotationX += 5.0f;
                        }
                    }
                    break;
                case Key.J:
                    if (!m_world.AnimationRunning) { 
                        if (m_world.RotationY >= -85.0f)
                        {
                            m_world.RotationY -= 5.0f;
                        }
                    }
                    break;
                case Key.L:
                    if (!m_world.AnimationRunning)
                    {
                        if (m_world.RotationY <= 85.0f)
                        {
                            m_world.RotationY += 5.0f;
                        }
                    }
                    break;
                case Key.Add:
                    if (!m_world.AnimationRunning)
                    {
                        m_world.SceneDistance -= 700.0f;
                    }
                    break;
                case Key.Subtract:
                    if (!m_world.AnimationRunning)
                    {
                        m_world.SceneDistance += 700.0f;
                    }
                    break;
                case Key.V:
                    if (!m_world.AnimationRunning)
                    {
                        m_world.startAnimation();
                    }
                    break;
                
            }
        }

        private void increaseTrans_Click(object sender, RoutedEventArgs e)
        {
            if (!m_world.AnimationRunning)
            {
                m_world.TranslateLeftWallX += 100.0f;
            }
        }

        private void decreaseTrans_Click(object sender, RoutedEventArgs e)
        {
            if (!m_world.AnimationRunning)
            {
                m_world.TranslateLeftWallX -= 100.0f;
            }
        }

        private void increaseRotate_Click(object sender, RoutedEventArgs e)
        {
            if (!m_world.AnimationRunning)
            {
                m_world.RightWallRotateY += 10.0f;
            }
        }

        private void decreaseRotate_Click(object sender, RoutedEventArgs e)
        {
            if (!m_world.AnimationRunning)
            {
                m_world.RightWallRotateY -= 10.0f;
            }
        }

        private void increaseScale_Click(object sender, RoutedEventArgs e)
        {
            if (!m_world.AnimationRunning)
            {
                m_world.ScaleArrow += 1;
            }
        }

        private void decreaseScale_Click(object sender, RoutedEventArgs e)
        {
            if (!m_world.AnimationRunning)
            {
                m_world.ScaleArrow -= 1;
            }
        }
    }
}
