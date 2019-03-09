using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;


namespace Community_Finder
{
    public partial class Form1 : Form
    {
        Graph graph = new Graph();
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }
        private const int cGrip = 16;
        private const int cCaption = 32;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {
                Point pos = new Point(m.LParam.ToInt32());
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17;
                    return;
                }
            }
            base.WndProc(ref m);
        }
        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            if(WindowState== FormWindowState.Maximized)
                WindowState = FormWindowState.Normal;
            else
                WindowState = FormWindowState.Maximized;
        }
        private void bunifuImageButton3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        private void bunifuToggleSwitch1_OnValuechange(object sender, EventArgs e)
        {
            graph.Cindex = 0;
            graph.help = 0;
            graph.Working_clusters.Clear();
            graph.Finall_clusters.Clear();
            graph.components.Clear();
            clustershow.Hide();
            clustershow.Hide();
            clustershow.Items.Clear();
            pictureBox2.Hide();
            pictureBox2.ImageLocation = @"pic.PNG";
            pictureBox2.Load();
            File.Delete("pic_generator.png");
            graph.Overlap = bunifuToggleSwitch1.Value;
        }
        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "text file |*.txt";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                graph.A.Clear();
                graph.Working_clusters.Clear();
                graph.Finall_clusters.Clear();
                graph.components.Clear();
                graph.N = 0;
                graph.m = 0;
                graph.Cindex = 0;
                graph.help = 0;
                graph.path = "";
                graph.orionted = false;
                bunifuMaterialTextbox1.Text = ofd.FileName;
                graph.path = bunifuMaterialTextbox1.Text;
                graph.Preparing_graph();
                if (graph.orionted == false)
                    label5.Text = "Undirected";
                else
                    label5.Text = "Directed";
                pictureBox2.Hide();
                pictureBox2.Image = null;
                clustershow.Hide();
                clustershow.Items.Clear();
                pictureBox2.ImageLocation = @"pic.PNG";
                pictureBox2.Load();
                File.Delete("pic_generator.png");
            }
        } 
    
        private void bunifuThinButton22_Click(object sender, EventArgs e)
        {
            if (graph.path.Length != 0)
            {
                Stopwatch sw = Stopwatch.StartNew();
                graph.Get_connected_components();
                sw.Stop();
                modularity.Text = "Q=" + graph.Q.ToString();
                runtime.Text = "Time=" + sw.Elapsed.TotalSeconds.ToString();
                label4.Text = "S";
                if (pictureBox2.Visible == false && clustershow.Visible == false)
                {
                    for (int i = 0; i < graph.Finall_clusters.Count; i++)
                        clustershow.Items.Add("Cluster[" + (i + 1) + "]=[" + string.Join(",", graph.Finall_clusters[i].c) + "];");
                    clustershow.Show();
                }
               
            }
            else
                MessageBox.Show("add file path first please !");
        }
        private void bunifuImageButton4_Click(object sender, EventArgs e)
        {
            if (graph.Finall_clusters.Count != 0) 
            {
                graph.Generate_a_dot_script();
                //System.Threading.Thread.Sleep(5000);
                pictureBox2.ImageLocation = @"pic_generator.PNG";
                pictureBox2.Load();
                clustershow.Hide();
                pictureBox2.Show();
            }
            else
            {
                MessageBox.Show("Error there is no Clusters !");
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            
            
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_2(object sender, EventArgs e)
        {

        }

        private void clusterss_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}
