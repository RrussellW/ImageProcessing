using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCamLib;

namespace ImageProcessing
{
    public partial class Form1 : Form
    {
        Bitmap loadImage, loadImage2, colorgreen, resultImage;
        private Timer webcamTimer;
        Boolean webcam = false;
        public Form1()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.Size = pictureBox1.Size;

            comboBox1.Items.Clear();
            Device[] devices = DeviceManager.GetAllDevices();
            comboBox1.Items.AddRange(devices);
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedIndex = 0;

            webcamTimer = new Timer();
            webcamTimer.Interval = 100;
            webcamTimer.Tick += WebcamTimer_Tick;
            webcamTimer.Start();
        }

        private void WebcamTimer_Tick(object sender, EventArgs e)
        {
            CaptureWebcamFrame();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if(openFile.ShowDialog() == DialogResult.OK)
            {
                loadImage = new Bitmap(openFile.FileName);
                pictureBox1.Image = loadImage;
                
            }
        }

        private void btnGreyscale_Click(object sender, EventArgs e)
        {
            if (loadImage == null)
            {
                return;
            }
            resultImage = new Bitmap(loadImage.Width, loadImage.Height);
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            for(int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color bmpColor = bmp.GetPixel(i, j);
                    int red = bmpColor.R;
                    int green = bmpColor.G;
                    int blue = bmpColor.B;
                    int grey = (red + green + blue) / 3;
                    resultImage.SetPixel(i, j, Color.FromArgb(grey, grey, grey));
                }
            }
            pictureBox2.Image = resultImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

        }

        private void btnInvert_Click(object sender, EventArgs e)
        {
            if (loadImage == null)
            {
                return;
            }
            resultImage = new Bitmap(loadImage.Width, loadImage.Height);
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color bmpColor = bmp.GetPixel(i, j);
                    int red = bmpColor.R;
                    int green = bmpColor.G;
                    int blue = bmpColor.B;
                    resultImage.SetPixel(i, j, Color.FromArgb(255-red, 255-green, 255-blue));
                }
            }
            pictureBox2.Image = resultImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void btnHistogram_Click(object sender, EventArgs e)
        {
            if (loadImage == null)
            {
                return;
            }
            resultImage = new Bitmap(loadImage.Width, loadImage.Height);
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color bmpColor = bmp.GetPixel(i, j);
                    int red = bmpColor.R;
                    int green = bmpColor.G;
                    int blue = bmpColor.B;
                    int grey = (red + green + blue) / 3;
                    resultImage.SetPixel(i, j, Color.FromArgb(grey, grey, grey));
                }
            }
            Color sample;
            int []histData = new int[256];
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    sample = resultImage.GetPixel(i, j);
                    histData[sample.R]++;
                }
            }
            Bitmap visualGraph = new Bitmap(256,800);
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 800; j++)
                {
                    visualGraph.SetPixel(i, j, Color.White);
                }
            }

            // datas

            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < Math.Min(histData[i]/5,800); j++)
                {
                    visualGraph.SetPixel(i, 799 - j, Color.Black);
                }
            }
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = visualGraph;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            resultImage.Save(saveFileDialog1.FileName + ".png");
        }

        private void btnSubtract_Click(object sender, EventArgs e)
        {
            if(loadImage==null)
            {
                return;
            }
            if(loadImage2==null)
            {
                return;
            }
            int maxWidth = Math.Max(loadImage.Width, loadImage2.Width);
            int maxHeight = Math.Max(loadImage.Height, loadImage2.Height);

            loadImage = (Bitmap)loadImage.GetThumbnailImage(maxWidth, maxHeight, null, IntPtr.Zero);
            loadImage2 = (Bitmap)loadImage2.GetThumbnailImage(maxWidth, maxHeight, null, IntPtr.Zero);

            if (loadImage == null && loadImage2 == null)
            {
                return;
            }
            resultImage = loadImage2;
            pictureBox2.Image = resultImage;

            Color mygreen = Color.FromArgb(0, 0, 255);
            int greygreen = (mygreen.R + mygreen.G + mygreen.B) / 3;
            int threshold = 5;

            for (int x = 0; x < loadImage2.Width; x++)
            {
                for(int y = 0; y < loadImage2.Height; y++)
                {
                    Color pixel = loadImage2.GetPixel(x, y);
                    Color backpixel = loadImage.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subtractvalue = Math.Abs(grey - greygreen);
                    if (subtractvalue > threshold)
                        resultImage.SetPixel(x, y, pixel);
                    else
                        resultImage.SetPixel(x, y, backpixel);
                }
            }
            pictureBox2.Image = resultImage;
        }

        private void btnWebcam_Click(object sender, EventArgs e)
        {
            webcam = true;
        }

        private void CaptureWebcamFrame()
        {
            if(!webcam)
            {
                return;
            }
            Device[] devices = DeviceManager.GetAllDevices();

            if (devices.Length > 0)
            {
                IDataObject data;
                Image bmap;
                Device d = DeviceManager.GetDevice(comboBox1.SelectedIndex);
                d.Sendmessage();
                data = Clipboard.GetDataObject();
                bmap = (Image)(data.GetData("System.Drawing.Bitmap", true));


                if (bmap != null)
                {
                    loadImage2 = new Bitmap(bmap);
                    pictureBox3.Image = loadImage2;
                }
                else
                {
                    
                }
            }
            else
            {
                MessageBox.Show("No webcam found.");
            }
        }

        private void btnSelectWebcam_Click(object sender, EventArgs e)
        {
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            webcam = false;
        }

        private void btnSepia_Click(object sender, EventArgs e)
        {
            if (loadImage == null)
            {
                return;
            }
            resultImage = new Bitmap(loadImage.Width, loadImage.Height);
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color bmpColor = bmp.GetPixel(i, j);
                    int red = bmpColor.R;
                    int green = bmpColor.G;
                    int blue = bmpColor.B;
                    int grey = (red + green + blue) / 3;
                    resultImage.SetPixel(i, j, Color.FromArgb(grey, (int)(grey * 0.95), (int)(grey * 0.82)));
                }
            }
            pictureBox2.Image = resultImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                loadImage2 = new Bitmap(openFile.FileName);
                pictureBox3.Image = loadImage2;

            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if(loadImage==null)
            {
                return;
            }
            resultImage = new Bitmap(loadImage.Width,loadImage.Height);
            for (int i = 0; i < loadImage.Width; i++)
            {
                for (int j = 0; j < loadImage.Height; j++)
                {
                    Color bmpColor = loadImage.GetPixel(i, j);
                    resultImage.SetPixel(i, j, bmpColor);
                }
            }
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = resultImage;
            
        }
    }
}
