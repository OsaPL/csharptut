using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Threading;

namespace client_sharptut
{ 
        public partial class Form1 : Form
        {
            System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
            NetworkStream serverStream = default(NetworkStream);
            string readData = null;

            public Form1()
            {
                InitializeComponent();
            }

            private void button1_Click(object sender, EventArgs e)
            {
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(textBox2.Text + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
            }

            private void button2_Click(object sender, EventArgs e)
            {
                
                readData = "Conected to Chat Server ...";
                msg();
                clientSocket.Connect("127.0.0.1", 8888);
                serverStream = clientSocket.GetStream();

                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(textBox3.Text + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                Thread ctThread = new Thread(getMessage);
                ctThread.Start();
                textBox3.Enabled = false;
        }

            private void getMessage()
            {
                while (true)
                {
                    serverStream = clientSocket.GetStream();
                    //Int32 buffSize = clientSocket.ReceiveBufferSize;
                    byte[] inStream = new byte[2048];
                    //buffSize = (int)clientSocket.ReceiveBufferSize;
                    serverStream.Read(inStream, 0, inStream.Length);
                    string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                    readData = "" + returndata;
                    msg();
                }
            }

            private void msg()
            {
                if (this.InvokeRequired)
                    this.Invoke(new MethodInvoker(msg));
                else
                    textBox1.Text = textBox1.Text + Environment.NewLine + " >> " + readData;
            }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Lines.Count() > 11)
                if (!textBox3.Enabled)
                {
                    textBox1.Text = textBox1.Text.Substring(textBox1.Lines[0].Length + Environment.NewLine.Length);
                }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if(e.KeyChar == Enter)
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            textBox3.Text = "";
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(textBox2.Text + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
                textBox2.Text = "";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes("Disconnect" + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
            */
        }
    }
    }

