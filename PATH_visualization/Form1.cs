using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace PATH_visualization
{
    public partial class PATH : Form
    {
        public PATH()
        {
            InitializeComponent();
        }
        //LAI2000
        //private double total_gap_fraction;
        //private double LAIe;
        private string[] file_LAI2000;


        //TRAC
        private string[] file_trac;

        //imgae


        //TSL
        private string file_envelop, out_folder;
        private string[] file_ptx;
        private static Process p = new System.Diagnostics.Process();
        private bool exited;
        private double prog = 0.0;
        //private bool flash;

        //ASL
        private string file_ALS;
        private string file_name_ALS;
        private string file_chm;


        private void PATH_Load(object sender, EventArgs e)
        {
            zenithTextBox1.Text = "0";
            GTextBox1.Text = "0.5";
            TRACFilePathTextBox.ReadOnly = true;

            pointCloudDataPathTextBox.ReadOnly = true;
            envelopePathTextBox.ReadOnly = true;
            outTextBox.ReadOnly = true;
            ASLFilePathTextBox.ReadOnly = true;
            chmTextBox.ReadOnly = true;
            skinEngine1.SkinFile = Application.StartupPath + @"\EmeraldColor1.ssk";
            intensityThresholdNumericUpDown.Value = 0.4M;
            zenithRangeMaxNumericUpDown.Value = 180M;
        }

        //窗口闪烁
        [DllImport("user32.dll")]
        public static extern bool FlashWindow(
        IntPtr hWnd,     // handle to window
        bool bInvert   // flash status
         );

        //LAI2000
        private void gapFractionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            gapFractionTextBox.Visible = true;
            LAIeTextBox.Visible = false;
            zenithTextBox1.Visible = true;
            GTextBox1.Visible = true;
        }

        private void LAIeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            LAIeTextBox.Visible = true;
            gapFractionTextBox.Visible = false;
            zenithTextBox1.Visible = true;
            GTextBox1.Visible = true;

        }

        //TRAC
        private void TRACFilePathButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "TRAC Data(*.trc)|*.trc";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                file_trac = fileDialog.FileNames;  //文件绝对路径（包括文件名）
                string display = "";
                foreach (string file in file_trac)
                {
                    display += String.Concat(file, Environment.NewLine);
                }
                TRACFilePathTextBox.Text = display;
            }
        }

        //image
        private void inputImagButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            //folderDialog.Description = "请选择txt所在文件夹";
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(folderDialog.SelectedPath))
                {
                    MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return;
                }
                else
                {
                    string path = folderDialog.SelectedPath;
                    //out_folder = path;
                    imageFolderTextBox.Text = path;
                }
            }
        }
        private void typicalLADRadioButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void fixedGradioButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void statisticgRadioButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void statisticgButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "dat(*.dat)|*.dat";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {

                string file = fileDialog.FileName;  //文件绝对路径（包括文件名）
                file_envelop = file;
                statisticgTextBox.Text = file;
            }
        }


        //TLS
        private void TLSCloudPathButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "Cloud Point Data(*.ptx)|*.ptx";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                file_ptx = fileDialog.FileNames;  //文件绝对路径（包括文件名）
                string display = "";
                foreach (string file in file_ptx)
                {
                    display += String.Concat(file, Environment.NewLine);
                }
                pointCloudDataPathTextBox.Text = display;

            }
        }
        private void zenithTextBox_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void EnvelopePathButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "Envelope Data(*.obj)|*.obj";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {

                string file = fileDialog.FileName;  //文件绝对路径（包括文件名）
                file_envelop = file;
                envelopePathTextBox.Text = file;
            }

        }

        //ALS
        private void ALSfilePathButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "LiDAR Data(*.laz)|*.laz";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;  //文件绝对路径（包括文件名）
                file_name_ALS = Path.GetFileNameWithoutExtension(file);
                file_envelop = file;
                ASLFilePathTextBox.Text = file;
                file_ALS = file;
            }
        }

        private void chmPathButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "CHM Data(*.tif)|*.tif";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;  //文件绝对路径（包括文件名）
                chmTextBox.Text = file;
                file_chm = file;
            }
        }

        private void outputpathButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "请选择txt所在文件夹";
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(folderDialog.SelectedPath))
                {
                    MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return;
                }
                else
                {
                    string path = folderDialog.SelectedPath;
                    out_folder = path;
                    outTextBox.Text = path;
                }

            }
        }

        

        private void PATH_MouseEnter(object sender, EventArgs e)
        {
            //flash = false;
        }

        private void PATH_MouseLeave(object sender, EventArgs e)
        {
            //flash = true;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {

        }

        

        private void LAIeTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void zenithTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void imageTabPage_Click(object sender, EventArgs e)
        {

        }

        private void inputDataTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LAI2000TabPage_Click(object sender, EventArgs e)
        {

        }

        private void TransTabPage_Click(object sender, EventArgs e)
        {

        }

        private void LAI2000FilePathButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "text(*.txt)|*.txt";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                file_LAI2000 = fileDialog.FileNames;  //文件绝对路径（包括文件名）
                string display = "";
                foreach (string file in file_LAI2000)
                {
                    display += String.Concat(file, Environment.NewLine);
                }
                LAI2000FilePathTextBox.Text = display;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String copyright = 
@"Path Length Distribution Method
Contact: Ronghai HU(sea@mail.bnu.edu.cn) 
Reference: 
1.Hu, R.et al. (2014).Indirect Measurement of Leaf Area Index on the Basis of Path Length Distribution.REMOTE SENS ENVIRON, 155, 239 - 247. 
2.Yan, G.et al. (2016).Scale Effect in Indirect Measurement of Leaf Area Index. IEEE T GEOSCI REMOTE, 54, 3475 - 3484.";
            MessageBox.Show(this, copyright, "copyright");
        }

        private void lAI2000ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("LAI200 help");
        }

        private void ranButton_Click(object sender, EventArgs e)
        {
            switch(inputDataTabControl.SelectedIndex)
            {
                #region LAI Transform
                case 0:
                    {
                        float LAIe;
                        float gap;
                        string out_file = out_folder + "\\result.txt";
                        float zenith;
                        float G;
                        
                        if (zenithTextBox1.Text.Trim() == String.Empty)
                        {
                            MessageBox.Show(this, "Please enter zenith angle.", "Notice");
                            return;
                        }   
                        else
                        {
                            zenith = Convert.ToSingle(zenithTextBox1.Text);
                        }

                        if (GTextBox1.Text.Trim() == String.Empty)
                        {
                            MessageBox.Show(this, "Please enter G.", "Notice");
                            return;
                        }
                        else
                        {
                            G = Convert.ToSingle(GTextBox1.Text);
                        }

                        if (LAIeRadioButton.Checked)
                        {
                            if (LAIeTextBox.Text.Trim() == String.Empty)
                            {
                                MessageBox.Show(this, "Please enter LAIe.", "Notice");
                                return;
                            }
                            else
                            {
                                LAIe = Convert.ToSingle(LAIeTextBox.Text);
                                indicatorLabel.Text = "Calculating...";
                                string config = "-LAIe " + LAIe + " -z " + zenith + " -g " + G + " -o " + out_file;
                                //MessageBox.Show(this, config, "Notice");
                                Console.WriteLine(config);
                                p.StartInfo = new ProcessStartInfo("LAI_PATH_transform.exe", config);
                                p.EnableRaisingEvents = true; ;
                                p.StartInfo.UseShellExecute = false;
                                //p.StartInfo.RedirectStandardInput = true;
                                //p.StartInfo.RedirectStandardOutput = true;
                                p.StartInfo.CreateNoWindow = true;
                                p.Start();
                                p.WaitForExit();
                                p.Close();
                            }
                        }

                        else if(gapFractionRadioButton.Checked)
                        {
                            if (gapFractionTextBox.Text.Trim() == String.Empty)
                            {
                                MessageBox.Show(this, "Please enter gap fraction.", "Notice");
                                return;
                            }
                            else
                            {
                                gap = Convert.ToSingle(gapFractionTextBox.Text);
                                indicatorLabel.Text = "Calculating...";
                                string config = "-gap " + gap + " -z " + zenith + " -g " + G + " -o " + out_file;
                                //MessageBox.Show(this, config, "Notice");
                                Console.WriteLine(config);
                                p.StartInfo = new ProcessStartInfo("LAI_PATH_example.exe", config);
                                p.EnableRaisingEvents = true; ;
                                p.StartInfo.UseShellExecute = false;
                                //p.StartInfo.RedirectStandardInput = true;
                                //p.StartInfo.RedirectStandardOutput = true;
                                p.StartInfo.CreateNoWindow = true;
                                p.Start();
                                p.WaitForExit();
                                p.Close();
                            }
                        }
                        progressBar1.Value = progressBar1.Maximum;
                        indicatorLabel.Text = "Free";
                        System.Diagnostics.Process.Start("notepad.exe", out_folder + "\\result.txt");
                        break;
                    }
                #endregion

                #region LAI2000
                case 1:
                    {
                        //MessageBox.Show(this, "TRAC", "提示");

                        if (file_LAI2000.Length == 0)
                        {
                            MessageBox.Show(this, "输入不能为空", "提示");
                            return;
                        }
                        double g = 0.5;
                        if (LAI2000GValue.Text.Trim() != String.Empty)
                        {
                            g = Convert.ToSingle(LAI2000GValue.Text);
                        }
                        int i = 0;

                        foreach (string file in file_LAI2000)
                        {
                            i++;
                            progressBar1.Value = (int)(progressBar1.Maximum * (i / (float)file_LAI2000.Length));
                            indicatorLabel.Text = "Calculating...";
                            int index = file.LastIndexOf('\\');
                            string file_name = file.Substring(index + 1);
                            string out_file = out_folder + "\\" + file_name;
                            string config = "-i " + file + " -o " + out_file + " -g " + g;
                            Console.WriteLine(config);
                            p.StartInfo = new ProcessStartInfo("LAI_PATH_2000.exe", config);
                            p.EnableRaisingEvents = true; ;
                            p.StartInfo.UseShellExecute = false;
                            //p.StartInfo.RedirectStandardInput = true;
                            //p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.CreateNoWindow = true;
                            p.Start();
                            p.WaitForExit();
                            p.Close();

                        }
                        progressBar1.Value = progressBar1.Maximum;
                        indicatorLabel.Text = "Free";
                        break;
                    }
                #endregion

                #region TRAC
                case 2:
                    {
                        //MessageBox.Show(this, "TRAC", "提示");
                        
                        if (file_trac.Length == 0 || zenithTextBox.Text.Trim() == String.Empty || wTextBox.Text.Trim() == String.Empty || dTextBox.Text.Trim() == String.Empty)
                        {
                            MessageBox.Show(this, "输入不能为空", "提示");
                            return;
                        }
                        float zenith = Convert.ToSingle(zenithTextBox.Text);
                        float w = Convert.ToSingle(wTextBox.Text);
                        float d = Convert.ToSingle(dTextBox.Text);
                        float i = 0;
                        string out_file = out_folder + "\\trac_result.txt";
                        foreach (string file in file_trac)
                        {
                            i++;
                            progressBar1.Value = (int)(progressBar1.Maximum * (i / (float)file_trac.Length));
                            indicatorLabel.Text = "Calculating...";
                            string config = "-i " + file + " -o " + out_file +" -z " + zenith+ " -w " + w + " -d " +d + " -g 0.5";
                            Console.WriteLine(config);
                            p.StartInfo = new ProcessStartInfo("CalcTRACData.exe", config);
                            p.EnableRaisingEvents = true; ;
                            p.StartInfo.UseShellExecute = false;
                            //p.StartInfo.RedirectStandardInput = true;
                            //p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.CreateNoWindow = true;

                            p.Start();
                            p.WaitForExit();
                            p.Close();

                        }
                        progressBar1.Value = progressBar1.Maximum;
                        indicatorLabel.Text = "Free";
                        System.Diagnostics.Process.Start("notepad.exe", out_folder + "\\trac_result.txt");
                        break;
                    }
                #endregion

                #region image
                case 3:
                    {
                        MessageBox.Show(this, "Image", "提示");
                        break;
                    }
                #endregion

                #region TLS
                //TLS
                case 4:
                    {
                        progressBar1.Value = progressBar1.Minimum;
                        if (file_ptx.Length == 0 || file_envelop.Length == 0)
                        {
                            MessageBox.Show(this, "输入不能为空", "提示");
                            return;
                        }
                        string imageDebug = "";
                        if (imageDebugCheckBox.Checked)
                        {
                            imageDebug = " -img_debug";
                        }
                        string refine = "";
                        if (refineCheckBox.Checked)
                        {
                            refine = " -refine";
                        }
                        string zenithRange = "";
                        //string zenithRange = " -zenith_ranges ";
                        if(zenithLayeringCheckBox.Checked)
                        {
                            zenithRange = " -zenith_ranges ";
                            zenithRange = zenithRange + zenithRangeMinNumericUpDown.Value.ToString() + " " + zenithRangeMaxNumericUpDown.Value.ToString() + " 10";
                        }
                        string intensityThreshold = " -intensity_threshold ";
                        intensityThreshold = intensityThreshold + intensityThresholdNumericUpDown.Value.ToString();
                        float i = 0;
                        foreach (string file in file_ptx)
                        {
                            i++;
                            //progressBar1.Value = (int)(progressBar1.Maximum * (i / (float)file_ptx.Length));
                            indicatorLabel.Text = "Calculating...";
                            string config = "-ptx " + file + " -envelope " + file_envelop + " -otxt -o " + out_folder + zenithRange + intensityThreshold + imageDebug + refine;
                            Console.WriteLine(config);
                            p.StartInfo = new ProcessStartInfo("LAIpathTLS.exe", config);
                            p.EnableRaisingEvents = true; ;
                            p.StartInfo.UseShellExecute = false;
                            //p.StartInfo.RedirectStandardInput = true;
                            //p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.CreateNoWindow = true;

                            p.Start();
                            p.WaitForExit();
                            p.Close();
                            progressBar1.Value = (int)(progressBar1.Maximum * (i / (float)file_ptx.Length));
                            //int test = (int)(progressBar1.Maximum * (i / (float)file_ptx.Length));
                            //MessageBox.Show(this, test.ToString(), "提示");  
                        }
                        progressBar1.Value = progressBar1.Maximum;
                        indicatorLabel.Text = "Free";
                        System.Diagnostics.Process.Start("notepad.exe", out_folder+"\\out.txt");
                        break;
                    }
                #endregion

                #region ALS
                case 5:
                    {
                        float h;
                        float stepGap;
                        float step;
                        MessageBox.Show(this, "ALS", "提示");
                        if(ASLFilePathTextBox.Text.Trim() == String.Empty || chmTextBox.Text.Trim() == String.Empty)
                        {
                            MessageBox.Show(this, "Please choose input file.", "Notice");
                        }

                        if (heightTextBox.Text.Trim() == String.Empty)
                        {
                            MessageBox.Show(this, "Please enter G.", "Notice");
                            return;
                        }
                        else
                        {
                            h = Convert.ToSingle(heightTextBox.Text);
                        }

                        if (stepGapTextBox.Text.Trim() == String.Empty)
                        {
                            MessageBox.Show(this, "Please enter G.", "Notice");
                            return;
                        }
                        else
                        {
                            stepGap = Convert.ToSingle(stepGapTextBox.Text);
                        }

                        if (stepTextBox.Text.Trim() == String.Empty)
                        {
                            MessageBox.Show(this, "Please enter G.", "Notice");
                            return;
                        }
                        else
                        {
                            step = Convert.ToSingle(stepTextBox.Text);
                        }
                        indicatorLabel.Text = "Calculating...";
                        string outPath = out_folder + "\\" + file_name_ALS + ".txt";
                        //MessageBox.Show(this, outPath, "Notice");
                        string config = "-i " + file_ALS + " -od " + outPath + " -step_gap " + stepGap + " -step " + step + " -chm " + file_chm + " -height " + h;
                        MessageBox.Show(this, config, "Notice");
                        Console.WriteLine(config);
                        p.StartInfo = new ProcessStartInfo("LiDAR Processing.exe", config);
                        p.EnableRaisingEvents = true; ;
                        p.StartInfo.UseShellExecute = false;
                        //p.StartInfo.RedirectStandardInput = true;
                        //p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.CreateNoWindow = false;
                        p.Start();
                        p.WaitForExit();
                        p.Close();
                        progressBar1.Value = progressBar1.Maximum;
                        indicatorLabel.Text = "Free";
                        File.Delete(outPath);
                        MessageBox.Show(this, "Done!", "Notice");

                        break;
                    }
                #endregion
            }

            //窗口闪烁
            FlashWindow(this.Handle, true);
            //FlashWindow(this.Handle, true);
            //FlashWindow(this.Handle, true);
        }
    }
}
