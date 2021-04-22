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

namespace Juego
{
    public partial class LoginScreen : Form
    {
        int mX, mY, aux1, aux2, puerto = 9042;
        bool mouseDown = false, loginMODE = true;
        string mensaje;


        public LoginScreen()
        {
            InitializeComponent();
        }

        private void min_Btn_Click(object sender, EventArgs e) //Minimizamos el Form.
        {
            this.WindowState = FormWindowState.Minimized; 
        }

        private void exit_Btn_Click(object sender, EventArgs e) //Cerramos el Form.
        {
            this.Close();
        }

        //Lo utilizamos para poder mover el Form con el ratón.

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

        /////////////////////////////////////////////////////

        private void CL_Lbl_MouseMove(object sender, MouseEventArgs e)
        {
            CL_Lbl.ForeColor = Color.FromArgb(25, 25, 25);
        }

        private void CL_Lbl_MouseLeave(object sender, EventArgs e)
        {
            CL_Lbl.ForeColor = Color.FromArgb(116, 116, 116);
        }

        private void exit_Btn_MouseMove(object sender, MouseEventArgs e)
        {
            exit_Btn.BackColor = Color.FromArgb(209, 54, 57);
            exit_Btn.Image = System.Drawing.Image.FromFile("Resources//Icons//Exit2.png");
        }

        private void exit_Btn_MouseLeave(object sender, EventArgs e)
        {
            exit_Btn.BackColor = Color.Transparent;
            exit_Btn.Image = System.Drawing.Image.FromFile("Resources//Icons//Exit1.png");
        }

        private void min_Btn_MouseMove(object sender, MouseEventArgs e)
        {
            min_Btn.BackColor = Color.FromArgb(226, 226, 226); 
        }

        private void min_Btn_MouseLeave(object sender, EventArgs e)
        {
            min_Btn.BackColor = Color.Transparent;
        }

        private void user_Tbx_Click(object sender, EventArgs e)
        {
            if (aux1 == 0)
            {
                user_Tbx.Text = "";
                aux1 = 1;
            }
        }

        private void pass_Tbx_Click(object sender, EventArgs e)
        {
            if (aux2 == 0)
            {
                pass_Tbx.Text = "";
                pass_Tbx.PasswordChar = Convert.ToChar("•");
                eyeH_Pnl.Show();
                eyeS_Pnl.Hide();
                aux2 = 1;
            }
        }

        private void eyeS_Pnl_Click(object sender, EventArgs e)
        {
            pass_Tbx.PasswordChar = Convert.ToChar("•");
            eyeS_Pnl.Hide();
            eyeH_Pnl.Show();
        }

        private void eyeH_Pnl_Click(object sender, EventArgs e)
        {
            pass_Tbx.PasswordChar = '\0';
            eyeH_Pnl.Hide();
            eyeS_Pnl.Show();
        }

        private void Enter_Pnl_Click(object sender, EventArgs e)
        {
            ///////////////// Añadir aquí el resto de cosas que pasan cuando le damos a entrar al juego /////////////////

            //user_Tbx es la Textbox del nombre de usuario, mientras que la pass_Tbx es la de la contraseña.

            Socket server;

            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, puerto);

            //Creamos el socket.
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server.Connect(ipep); //Intentamos conectar el socket.

                if (loginMODE == true)
                {
                    mensaje = "1/" + user_Tbx.Text + "/" + pass_Tbx.Text;

                    //Enviamos al servidor el usuario y la contraseña.
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    //Recibimos la respuesta del servidor.
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                    if (Convert.ToInt32(mensaje) == 1)
                    {
                        Launcher L = new Launcher();

                        this.Hide();

                        L.ShowDialog();

                        L.PonerUsuario(user_Tbx.Text);

                        server.Shutdown(SocketShutdown.Both);
                        server.Close();
                    }
                    else
                    {
                        MessageBox.Show("Error al iniciar sesión.");
                    }
                }
                else 
                {
                    mensaje = "2/" + user_Tbx.Text + "/" + pass_Tbx.Text;

                    //Enviamos al servidor el usuario y la contraseña.
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    CL_Lbl.Text = "CREAR UNA CUENTA";
                    SR_Lbl.Text = "Iniciar sesión";

                    loginMODE = true;

                    server.Shutdown(SocketShutdown.Both);
                    server.Close();
                }
 
               
            }
            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return.
                MessageBox.Show("No se ha podido conectar con el servidor.");
                return;
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }

        private void CL_Lbl_Click(object sender, EventArgs e)
        {
            if (loginMODE == true)
            {
                CL_Lbl.Text = "INICIAR SESIÓN";
                SR_Lbl.Text = "Registrarse";

                loginMODE = false;
            }
            else
            {
                CL_Lbl.Text = "CREAR UNA CUENTA";
                SR_Lbl.Text = "Iniciar sesión";

                loginMODE = true;
            }
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
    }
}