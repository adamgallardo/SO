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
    public partial class LoginScreen : Form
    {
        int mX, mY, aux1, aux2, puerto = 9002;

        bool mouseDown = false, loginMODE = true;

        string mensaje, usuario, invitacion;

        Socket server;
        Thread atender;

        public LoginScreen()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            dataGridView1.Columns.Add("Usuarios conectados", "Usuarios conectados");
            dataGridView1.ColumnHeadersVisible = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Columns["Usuarios conectados"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void AtenderServidor()
        {
            while (true)
            {
                try
                {
                    //Recibimos mensaje del servidor.
                    byte[] msg2 = new byte[400];
                    server.Receive(msg2);

                    string[] msg = Encoding.ASCII.GetString(msg2).Split('/');

                    //MessageBox.Show(trozos[0] + "/" + trozos[1] + "/" + trozos[2]);

                    int codigo = Convert.ToInt32(msg[0]); //Tipo de mensaje.
                    string mensaje = msg[1].Split('\0')[0]; //Obtenemos información del mensaje.

                    switch (codigo)
                    {
                        case 1: //Inicio de sesión.
                            if (mensaje == "1")
                            {
                                MessageBox.Show("Conectado.");
                            }
                            else
                            {
                                MessageBox.Show("Error al iniciar sesión.");
                            }

                            break;

                        case 2: //Registrar usuario.
                            if (mensaje == "1")
                            {
                                MessageBox.Show("Conectado.");
                            }
                            else
                            {
                                MessageBox.Show("Error al registrarse.");
                            }

                            break;

                        case 3: //Rapida.
                            {
                                MessageBox.Show(mensaje);
                            }
                            break;

                        case 4: //Ganador.
                            {
                                MessageBox.Show(mensaje);
                            }
                            break;

                        case 5: //Ganadas.
                            {
                                MessageBox.Show(mensaje);
                            }
                            break;

                        case 6: //Actualiza la lista de conectados en el grid.
                            {
                                Conectados(mensaje);
                            }
                            break;

                        case 7: //Recibe invitación y prepara respuesta.
                            {
                                invitacion = mensaje;

                                DialogResult result1 = MessageBox.Show(invitacion + " te ha enviado una notificación.", "Aceptar invitación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                
                                if (result1 == DialogResult.Yes)
                                {
                                    invitacion = "7/" + usuario + "/Si/";
                                }  
                                else
                                {
                                    invitacion = "7/" + usuario + "/No/";
                                }

                                Invitacion();
                            }
                            break;

                        case 8: //Recibe respuesta.
                            {
                                MessageBox.Show(mensaje);
                            }
                            break;
                    }
                }
                catch(Exception)
                {
                } 
            }
        }

        public void Invitacion()
        {
            MessageBox.Show(invitacion);
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(invitacion);
            server.Send(msg);
        }

        public void Conectados(string mensaje)
        {
            int i = 0;

            string[] conectados = mensaje.Split(',');

            int Num = Convert.ToInt32(conectados[0]);

            dataGridView1.Rows.Clear();

            while (i < Num)
            {
                dataGridView1.Rows.Add(conectados[i + 1]);
                i++;
            }
        }

        private void Enter_Pnl_Click(object sender, EventArgs e)
        {
            ///////////////// Añadir aquí el resto de cosas que pasan cuando le damos a entrar al juego /////////////////

            //user_Tbx es la Textbox del nombre de usuario, mientras que la pass_Tbx es la de la contraseña.

            try
            {
                IPAddress direc = IPAddress.Parse("192.168.56.102");
                IPEndPoint ipep = new IPEndPoint(direc, puerto);

                //Creamos el socket.
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                server.Connect(ipep); //Intentamos conectar el socket.

                //Ponemos en marcha el Thread.
                ThreadStart ts = delegate {AtenderServidor();};
                atender = new Thread(ts);
                atender.Start();

                if (loginMODE == true)
                {
                    mensaje = "1/" + user_Tbx.Text + "/" + pass_Tbx.Text;
                    usuario = user_Tbx.Text;

                    //Enviamos al servidor el usuario y la contraseña.
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
                else
                {
                    mensaje = "2/" + user_Tbx.Text + "/" + pass_Tbx.Text;
                    usuario = user_Tbx.Text;

                    //Enviamos al servidor el usuario y la contraseña.
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
            }
            catch (Exception)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return.
                MessageBox.Show("No se ha podido conectar con el servidor.");
                return;
            }
        }

        private void consultar_Click(object sender, EventArgs e)
        {
            try
            {
                if (rapida.Checked)
                {
                    mensaje = "3/";
                }
                else if (ganador.Checked)
                {
                    mensaje = "4/" + textBox1.Text;
                }
                else if (ganadas.Checked)
                {
                    mensaje = "5/" + textBox1.Text;
                }

                //Enviamos al servidor el usuario y la contraseña.
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            catch (Exception)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("Error con la petición.");
                return;
            }
        }

        private void min_Btn_Click(object sender, EventArgs e) //Minimizamos el Form.
        {
            this.WindowState = FormWindowState.Minimized; 
        }

        private void exit_Btn_Click(object sender, EventArgs e) //Cerramos el Form.
        {
            try
            {
                byte[] msg = System.Text.Encoding.ASCII.GetBytes("0/");
                server.Send(msg);

                atender.Abort();
                server.Shutdown(SocketShutdown.Both);
                server.Close();
            }
            catch (Exception)
            {
                //Hola.
            }

            this.Dispose();
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.Value != null)
            {
                string select = dataGridView1.CurrentCell.Value.ToString(); //Recoge el nombre de la celda.

                if (select == usuario) // Solo permitimos selecionar las que no son el propio usuario.
                {
                    MessageBox.Show("No puede seleccionar tu propio usuario.");
                }
                else
                {
                    MessageBox.Show("Invitación enviada.");

                    mensaje = "6/" + select;

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
            }
        }


    }
}