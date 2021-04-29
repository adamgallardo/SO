using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Juego
{
    public partial class Launcher : Form
    {
        int mX, mY;
        bool mouseDown;
        string mensaje, usuario;

        Socket server;

        public void PrepararForm(string usuario, Socket server)
        {
            this.usuario = usuario;
            this.server = server;
        }

        public Launcher()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (mayor.Checked)
                {
                    mensaje = "5/" + textBox1.Text;
                }
                else if (ganador.Checked)
                {
                    mensaje = "4/" + textBox1.Text;
                }
                else if (rapida.Checked)
                {
                    mensaje = "3/";
                }
                else if (lista.Checked)
                {
                    mensaje = "6/" + usuario;
                }

                //Enviamos al servidor el usuario y la contraseña.
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor.
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                MessageBox.Show(mensaje);
            }
            catch (Exception)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return.
                MessageBox.Show("No se ha podido conectar con el servidor.");
                return;
            }
        }

        private void Drag_Pnl_MouseDown(object sender, MouseEventArgs e)
        {
            mX = e.X;
            mY = e.Y;
            mouseDown = true;
        }

        private void Drag_Pnl_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown == true)
            {
                this.SetDesktopLocation(MousePosition.X - mX, MousePosition.Y - mY);
            }
        }

        private void Drag_Pnl_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        protected override CreateParams CreateParams //Cosas raras de Windows para poner sombreado al Form.
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= 0x20000;
                return cp;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Enviamos al servidor el nombre tecleado para realizar la desconexion
                byte[] msg = System.Text.Encoding.ASCII.GetBytes("0/");
                server.Send(msg);

                // Nos desconectamos
                server.Shutdown(SocketShutdown.Both);
                server.Close();

                this.Close();
            }
            catch (Exception)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("Error al cerrar conexion");
                return;
            } 
        }
    }
}
