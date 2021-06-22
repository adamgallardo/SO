using System;
using System.Net;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace DAN
{
    public partial class DAN : Form
    {
        Socket server; Thread atender;

        int mX, mY, aux1, aux2, aux3;

        int puerto = 9002; string ip = "192.168.56.102";

        //int puerto = 50070; string ip = "147.83.117.22";

        string usuario, rival; 

        int numConectados, jugarINV, jugarAUX;

        int[] numForms = new int[100];
        List<Form2> formularios = new List<Form2>();

        delegate void DelegadoParaMostrarElementos();
        delegate void DelegadorParaEscribir(string mensaje);

        bool mouseDown = false, login = true, consulta = true, invitacion = true, host = true, conectado = false;

        public DAN()
        {
            InitializeComponent();
        }

        //ATENCIÓN AL CLIENTE.
        private void AtenderServidor()
        {
            while (true)
            {
                    ////Recibimos mensaje del servidor.
                    byte[] msg2 = new byte[400];
                    server.Receive(msg2);

                    string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');

                    int codigo = Convert.ToInt32(trozos[0]); //Tipo de mensaje.
                    int idPartida = Convert.ToInt32(trozos[1]); //Obtenemos la id del Form.
                    string mensaje = trozos[2].Split('\0')[0]; //Obtenemos información del mensaje.

                    switch (codigo)
                    {
                        case 1: //Inicio de sesión.

                            if (mensaje == "0") { usuario = usu_tbx.Text; DelegadoParaMostrarElementos delegado = new DelegadoParaMostrarElementos(MostrarContenido); this.Invoke(delegado); }

                            else if (mensaje == "1") { mensaje = "Datos incorrectos."; DelegadorParaEscribir delegado = new DelegadorParaEscribir(ErrorIniciarSesion); this.Invoke(delegado, new object[] { mensaje }); }

                            else if (mensaje == "2") { mensaje = "La lista de conectados esta llena."; DelegadorParaEscribir delegado = new DelegadorParaEscribir(ErrorIniciarSesion); this.Invoke(delegado, new object[] { mensaje }); }
                           
                            else if (mensaje == "3") { mensaje = "El usuario ya tiene una sesion activa."; DelegadorParaEscribir delegado = new DelegadorParaEscribir(ErrorIniciarSesion); this.Invoke(delegado, new object[] { mensaje }); }
                            
                            else { mensaje = "Ha habido un error en la autentificación."; DelegadorParaEscribir delegado = new DelegadorParaEscribir(ErrorIniciarSesion); this.Invoke(delegado, new object[] { mensaje }); }
                            
                            break;

                        case 2: //Registrar usuario.

                            if (mensaje == "0") { usuario = usu_tbx.Text; DelegadoParaMostrarElementos delegado = new DelegadoParaMostrarElementos(MostrarContenido); this.Invoke(delegado); }

                            else if (mensaje == "1") { mensaje = "El usuario ya existe."; DelegadorParaEscribir delegado = new DelegadorParaEscribir(ErrorIniciarSesion); this.Invoke(delegado, new object[] { mensaje }); }

                            else if (mensaje == "2") { mensaje = "La lista de conectados esta llena."; DelegadorParaEscribir delegado = new DelegadorParaEscribir(ErrorIniciarSesion); this.Invoke(delegado, new object[] { mensaje }); }

                            break;

                        case 3: //Rapida.
                            {
                                if (mensaje == "0") { DelegadorParaEscribir delegado = new DelegadorParaEscribir(ErrorConsultas); this.Invoke(delegado, new object[] { "Datos inválidos." }); }

                                else { DelegadorParaEscribir delegado = new DelegadorParaEscribir(ErrorConsultas); this.Invoke(delegado, new object[] { mensaje }); }
                            }
                            break;

                        case 4: //Ganador.
                            {
                                if (mensaje == "0") { DelegadorParaEscribir delegado = new DelegadorParaEscribir(ErrorConsultas); this.Invoke(delegado, new object[] { "Datos inválidos." }); }
                                
                                else { DelegadorParaEscribir delegado = new DelegadorParaEscribir(ErrorConsultas); this.Invoke(delegado, new object[] { mensaje }); }
                            }
                            break;

                        case 5: //Ganadas.
                            {
                                if (mensaje == "0") { DelegadorParaEscribir delegado = new DelegadorParaEscribir(ErrorConsultas); this.Invoke(delegado, new object[] { "Datos inválidos." }); }

                                else { DelegadorParaEscribir delegado = new DelegadorParaEscribir(ErrorConsultas); this.Invoke(delegado, new object[] { mensaje }); }
                            }
                            break;

                        case 6: //Actualiza la lista de conectados en el grid.
                            {
                                DelegadorParaEscribir delegado = new DelegadorParaEscribir(ActualizarLista); this.Invoke(delegado, new object[] { mensaje });
                            }
                            break;

                        case 7: //Recibe invitación y prepara respuesta.
                            {
                                DialogResult result1 = MessageBox.Show(mensaje + " te ha enviado una notificación.", "Aceptar invitación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                if (result1 == DialogResult.Yes)
                                {
                                    rival = mensaje;

                                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("7/" + mensaje + "/SI");
                                    server.Send(msg);

                                    host = false;
                                }
                                else
                                {
                                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("7/" + mensaje + "/NO");
                                    server.Send(msg);
                                }
                            }
                            break;
                        
                        case 8: //Recibe respuesta de la invitación.
                            {
                                if (mensaje == "0")
                                {
                                    ThreadStart ts = delegate { AbrirPartidas(idPartida); };
                                    Thread T = new Thread(ts);
                                    T.Start();
                                }
                                else
                                {
                                    DelegadorParaEscribir delegado = new DelegadorParaEscribir(PartidaRechaza); this.Invoke(delegado, new object[] { "Se ha cancelado la partida." });
                                }
                            }
                            break;

                        case 9: //Escribe mensaje en chat de un Form específico.
                            {
                                formularios[numForms[idPartida]].chat(mensaje);
                            }
                            break;

                        case 10: //Actualiza el Form acorde a la información entregada por el servidor.
                            {
                                formularios[numForms[idPartida]].jugada(mensaje);
                            }
                            break;

                        case 11: //Actualiza el Form acorde a la información entregada por el servidor.
                            {
                                formularios[numForms[idPartida]].conclusion(mensaje);
                            }
                            break;
                    }
                }
            }
        
         ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void PartidaRechaza(string mensaje)
        {
            invitacion_lbl.Text = mensaje;
        }

         private void entrar_btn_MouseUp(object sender, MouseEventArgs e) //BOTÓN PARA INICIAR SESIÓN O REGISTRARSE.
         {
             entrar_btn.Location = new Point(entrar_btn.Location.X, entrar_btn.Location.Y + 3);

             try
             {
                 if ((usu_tbx.Text != "") && (contra_tbx.Text != ""))
                 {
                     IPAddress direc = IPAddress.Parse(ip); IPEndPoint ipep = new IPEndPoint(direc, puerto);

                     server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); server.Connect(ipep);

                     //Pongo en marcha el thread que atenderá los mensajes del servidor.
                     ThreadStart ts = delegate { AtenderServidor(); };
                     atender = new Thread(ts); atender.Start();

                     if (login == true) //INICIAR SESIÓN.
                     {
                         //Enviamos al servidor el nombre del usuario y su contraseña.
                         usuario = usu_tbx.Text;
                         byte[] msg = System.Text.Encoding.ASCII.GetBytes("1/" + usu_tbx.Text + "/" + contra_tbx.Text); server.Send(msg);
                     }
                     else //REGISTRARSE.
                     {
                         //Enviamos al servidor el nombre del usuario y su contraseña.
                         usuario = usu_tbx.Text;
                         byte[] msg = System.Text.Encoding.ASCII.GetBytes("2/" + usu_tbx.Text + "/" + contra_tbx.Text); server.Send(msg);
                     }
                 }
                 else
                 {
                     error_lbl.Show(); error_lbl.Text = "Por favor, introduce un usuario válido.";
                 }
             }
             catch (Exception)
             {
                 MessageBox.Show("Error al conectar con el servidor.");
                 return;
             }
         }

        private void consulta_btn_MouseUp(object sender, MouseEventArgs e)
         {
             consulta_btn.Location = new Point(consulta_btn.Location.X, consulta_btn.Location.Y + 3);

             try
             {
                 if (rapida_rbn.Checked) { byte[] msg = System.Text.Encoding.ASCII.GetBytes("3/"); server.Send(msg); }
                 
                 else if (ganador_rbn.Checked) { byte[] msg = System.Text.Encoding.ASCII.GetBytes("4/" + consulta_tbx.Text); server.Send(msg); }
                
                 else if (ganadas_rbn.Checked) { byte[] msg = System.Text.Encoding.ASCII.GetBytes("5/" + consulta_tbx.Text); server.Send(msg); }
             }
             catch (Exception)
             {
                 MessageBox.Show("Error al conectar con el servidor.");
                 return;
             }
         }

        private void disc_btn_MouseUp(object sender, MouseEventArgs e) //BOTÓN DE DESCONEXIÓN.
         {
             disc_btn.Location = new Point(disc_btn.Location.X, disc_btn.Location.Y + 3);

             byte[] msg = System.Text.Encoding.ASCII.GetBytes("0/");
             server.Send(msg);

             atender.Abort();
             server.Shutdown(SocketShutdown.Both);
             server.Close();

             for (int i = 0; i < formularios.Count(); i++)
             {
                 formularios[i].Close();
             }

             OcultarContenido();
             conectado = false;
         }

        private void jugar_btn_MouseUp(object sender, MouseEventArgs e) //BOTÓN DE JUGAR.
         {
             jugar_btn.Location = new Point(jugar_btn.Location.X, jugar_btn.Location.Y + 3);

             if (jugarAUX == 1)
             {
                 host = true;

                 jugarAUX = 0;
                 jugarINV = 0;

                 invitacion_lbl.Text = "";

                 byte[] msg = System.Text.Encoding.ASCII.GetBytes("6/" + rival);
                 server.Send(msg);
             }
             else
             {
                 invitacion_lbl.Show();

                 invitacion_lbl.Text = "Selecciona a un usuario válido para poder jugar.";
             }
         }

        private void invite_btn_MouseUp(object sender, MouseEventArgs e)
         {
            invite_btn.Location = new Point(invite_btn.Location.X, invite_btn.Location.Y + 3);

            invitacion_lbl.Show();

            if (jugarINV == 1)
            {
                invitacion_lbl.Text = "Dale 'click' a JUGAR para invitar a " + rival + ".";

                jugarAUX = 1;
            }
            else
            {
                invitacion_lbl.Text = "Selecciona a un usuario válido para poder jugar.";
            }
         }

        private void data_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            jugarINV = 0;

            if ((data.CurrentCell.Value != null) && (data.CurrentCell.Value.ToString() != "") && (data.CurrentCell.Value.ToString() != usuario))
            {
                rival = data.CurrentCell.Value.ToString(); //Recoge el nombre de la celda.
                invitacion_lbl.Show(); invitacion_lbl.Text = "El usuario seleccionado es: " + rival + ".";
                jugarINV = 1;
            }
            else
            {
                invitacion_lbl.Show(); invitacion_lbl.Text = "Selecciona a un usuario válido para poder jugar.";
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        //FUNCIONES DE APOYO.

        private void ActualizarLista(string mensaje)
        {
            int i = 0;

            ayudita_pnl.Show();

            string[] vector = mensaje.Split(','); numConectados = Convert.ToInt32(vector[0]); data.Rows.Clear();

            if ((numConectados > 3) && (ayudita_pnl.Location.X == 1179))
            {
                ayudita_pnl.Location = new Point(ayudita_pnl.Location.X - 17, ayudita_pnl.Location.Y);
            }
            else if ((numConectados <= 3) && (ayudita_pnl.Location.X == 1162))
            {
                ayudita_pnl.Location = new Point(ayudita_pnl.Location.X + 17, ayudita_pnl.Location.Y);
            }
            
            while(i < numConectados)
            {
                data.Rows.Add(vector[i + 1]);
                i++;
            }
        }

        private void ErrorConsultas(string mensaje)
        {
            respuesta_lbl.Show();

            if (mensaje == "Datos inválidos.")
            {
                respuesta_lbl.Text = "Datos inválidos.";
            }
            else
            {
                respuesta_lbl.Text = "Respuesta: " + mensaje; 
            }
        }

        public void AbrirPartidas(int idPartida) //Abre un nuevo formulario y le envia los parametros necesarios para su correcta operacion.
        {
            numForms[idPartida] = formularios.Count(); //Almacenamos el número del Form en función de la ID de la partida.
            
            //Si ID = 3 [][][][número de FORM][][][][][][][]...

            Form2 f2 = new Form2(numForms[idPartida], idPartida, server, usuario, rival, host); //SI ATHAX INVITA A MRCAPITAN Y ESTE ES EL CLIENTE DE ATHAX, RIVAL SERÁ MRCAPITAN.
                                                                                                //SI ATHAX INVITA A MRCAPITAN Y ESTE ES EL CLIENTE DE MRCAPITAN, EL RIVAL SERÁ ATHAX. 
            formularios.Add(f2); 
            
            f2.ShowDialog();
        }

        private void ErrorIniciarSesion(string mensaje)
        {
            error_lbl.Text = mensaje; error_lbl.Show();
        }

        private void MostrarContenido() //MUESTRA EL CONTENIDO NECESARIO PARA CONSULTAS E INVITACIONES.
         {
            this.BackgroundImage = System.Drawing.Image.FromFile("Recursos//B5.png");

            conectado = true;

            panel1_pnl.Location = new Point(480, 291);
            panel2_pnl.Location = new Point(751, 92);

            panel1_pnl.Show(); panel2_pnl.Show(); consulta_btn.Show(); consulta_tbx.Show(); ganadas_rbn.Show(); ganador_rbn.Show(); rapida_rbn.Show(); error_lbl.Hide(); invite_btn.Show(); disc_btn.Show(); data.Show();

            /*data.Columns.Add("Usuarios conectados", "Usuarios conectados"); data.ColumnHeadersVisible = true; data.RowHeadersVisible = false;*/
         }

        private void OcultarContenido() //OCULTA EL CONTENIDO NECESARIO PARA CONSULTAS E INVITACIONES.
        {
             this.BackgroundImage = System.Drawing.Image.FromFile("Recursos//B1.png");

             panel1_pnl.Hide(); panel2_pnl.Hide(); consulta_btn.Hide(); consulta_tbx.Hide(); ganadas_rbn.Hide(); ganador_rbn.Hide(); rapida_rbn.Hide(); error_lbl.Hide(); invite_btn.Hide(); disc_btn.Hide(); data.Hide(); respuesta_lbl.Hide(); ayudita_pnl.Hide(); invitacion_lbl.Hide();

             usu_tbx.Text = "USUARIO";
             contra_tbx.Text = "CONTRASEÑA";

             contra_tbx.PasswordChar = Convert.ToChar('\0');

             entrar_btn.Hide();

             aux1 = 0; aux2 = 0; aux3 = 0;

             login = true;

             consulta_tbx.Text = "CONSULTAR";
        }

        private void PonerEnMarchaFormulario()
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2(numForms[0], 0, server, usuario, rival, host);

            f2.ShowDialog();
        }











































































        private void entrar_btn_MouseDown(object sender, MouseEventArgs e)
        {
            entrar_btn.Location = new Point(entrar_btn.Location.X, entrar_btn.Location.Y - 3);
        }

        private void usu_tbx_MouseClick(object sender, MouseEventArgs e)
        {
            error_lbl.Hide();

            if (aux1 == 0)
            {
                usu_tbx.Text = "";
                aux1 = 1;
            }

            if ((aux1 == 1) && (aux2 == 1))
            {
                entrar_btn.Show();
            }
        }

        private void contra_tbx_MouseClick(object sender, MouseEventArgs e)
        {
            error_lbl.Hide();

            if (aux2 == 0)
            {
                contra_tbx.Text = "";
                contra_tbx.PasswordChar = Convert.ToChar("•");
                aux2 = 1;
            }

            if ((aux1 == 1) && (aux2 == 1))
            {
                entrar_btn.Show();
            }
        }

        private void ISR2_lbl_Click(object sender, EventArgs e)
        {
            if (login == true)
            {
                ISR1_lbl.Text = "REGISTRARSE";
                ISR2_lbl.Text = "INICIAR SESIÓN";
                login = false;
            }
            else
            {
                ISR1_lbl.Text = "INICIAR SESIÓN";
                ISR2_lbl.Text = "REGISTRARSE";
                login = true;
            }
        }

        private void ISR2_lbl_MouseMove(object sender, MouseEventArgs e)
        {
            ISR2_lbl.ForeColor = Color.FromArgb(25, 25, 25);
        }

        private void ISR2_lbl_MouseLeave(object sender, EventArgs e)
        {
            ISR2_lbl.ForeColor = Color.FromArgb(166, 166, 166);
        }

        private void jugar_btn_MouseDown(object sender, MouseEventArgs e)
        {
            jugar_btn.Location = new Point(jugar_btn.Location.X, jugar_btn.Location.Y - 3);
        }

        private void invite_btn_MouseDown(object sender, MouseEventArgs e)
        {
            invite_btn.Location = new Point(invite_btn.Location.X, invite_btn.Location.Y - 3);
        }

        private void consulta_tbx_MouseClick(object sender, MouseEventArgs e)
        {
            respuesta_lbl.Hide();

            if (aux3 == 0)
            {
                consulta_tbx.Text = "";
                aux3 = 1;
            }
        }

        private void consulta_btn_MouseDown(object sender, MouseEventArgs e)
        {
            consulta_btn.Location = new Point(consulta_btn.Location.X, consulta_btn.Location.Y - 3);
        }

        private void oc_consulta_btn_MouseDown(object sender, MouseEventArgs e)
        {
            oc_consulta_btn.Location = new Point(oc_consulta_btn.Location.X, oc_consulta_btn.Location.Y - 3);
        }

        private void oc_consulta_btn_MouseUp(object sender, MouseEventArgs e)
        {
            if ((consulta == true) && (invitacion == true))
            {
                oc_consulta_btn.Location = new Point(oc_consulta_btn.Location.X, oc_consulta_btn.Location.Y + 3);
                this.BackgroundImage = System.Drawing.Image.FromFile("Recursos//B4.png");

                oc_consulta_btn.Image = System.Drawing.Image.FromFile("Recursos//show.png");
                consulta_tbx.Hide();
                consulta_btn.Hide();

                rapida_rbn.Hide();
                ganadas_rbn.Hide();
                ganador_rbn.Hide();

                respuesta_lbl.Hide();

                consulta_tbx.Text = "CONSULTAR";
                aux3 = 0;

                consulta = false;
            }
            else if ((consulta == true) && (invitacion == false))
            {
                oc_consulta_btn.Location = new Point(oc_consulta_btn.Location.X, oc_consulta_btn.Location.Y + 3);
                this.BackgroundImage = System.Drawing.Image.FromFile("Recursos//B2.png");

                oc_consulta_btn.Image = System.Drawing.Image.FromFile("Recursos//show.png");
                consulta_tbx.Hide();
                consulta_btn.Hide();

                rapida_rbn.Hide();
                ganadas_rbn.Hide();
                ganador_rbn.Hide();

                respuesta_lbl.Hide();

                consulta_tbx.Text = "CONSULTAR";
                aux3 = 0;

                consulta = false;
            }
            else if (((consulta == false)) && ((invitacion == true)))
            {
                oc_consulta_btn.Location = new Point(oc_consulta_btn.Location.X, oc_consulta_btn.Location.Y + 3);
                this.BackgroundImage = System.Drawing.Image.FromFile("Recursos//B5.png");

                oc_consulta_btn.Image = System.Drawing.Image.FromFile("Recursos//hide.png");
                consulta_tbx.Show();
                consulta_btn.Show();

                rapida_rbn.Show();
                ganadas_rbn.Show();
                ganador_rbn.Show();

                consulta = true;
            }
            else
            {
                oc_consulta_btn.Location = new Point(oc_consulta_btn.Location.X, oc_consulta_btn.Location.Y + 3);
                this.BackgroundImage = System.Drawing.Image.FromFile("Recursos//B3.png");

                oc_consulta_btn.Image = System.Drawing.Image.FromFile("Recursos//hide.png");
                consulta_tbx.Show();
                consulta_btn.Show();

                rapida_rbn.Show();
                ganadas_rbn.Show();
                ganador_rbn.Show();

                consulta = true;
            }

        }

        private void oc_invitar_btn_MouseDown(object sender, MouseEventArgs e)
        {
            oc_invitar_btn.Location = new Point(oc_invitar_btn.Location.X, oc_invitar_btn.Location.Y - 3);
        }

        private void oc_invitar_btn_MouseUp(object sender, MouseEventArgs e)
        {
            if ((consulta == true) && (invitacion == true))
            {
                oc_invitar_btn.Location = new Point(oc_invitar_btn.Location.X, oc_invitar_btn.Location.Y + 3);
                this.BackgroundImage = System.Drawing.Image.FromFile("Recursos//B3.png");

                oc_invitar_btn.Image = System.Drawing.Image.FromFile("Recursos//show.png");
                invite_btn.Hide();
                ayudita_pnl.Hide();
                data.Hide();
                invitacion = false;
            }
            else if ((consulta == true) && (invitacion == false))
            {
                oc_invitar_btn.Location = new Point(oc_invitar_btn.Location.X, oc_invitar_btn.Location.Y + 3);
                this.BackgroundImage = System.Drawing.Image.FromFile("Recursos//B5.png");

                oc_invitar_btn.Image = System.Drawing.Image.FromFile("Recursos//hide.png");
                invite_btn.Show();
                data.Show();
                ayudita_pnl.Show();
                invitacion = true;
            }
            else if (((consulta == false)) && ((invitacion == true)))
            {
                oc_invitar_btn.Location = new Point(oc_invitar_btn.Location.X, oc_invitar_btn.Location.Y + 3);
                this.BackgroundImage = System.Drawing.Image.FromFile("Recursos//B2.png");

                oc_invitar_btn.Image = System.Drawing.Image.FromFile("Recursos//show.png");
                invite_btn.Hide();
                data.Hide();
                ayudita_pnl.Hide();
                invitacion = false;
            }
            else
            {
                oc_invitar_btn.Location = new Point(oc_invitar_btn.Location.X, oc_invitar_btn.Location.Y + 3);
                this.BackgroundImage = System.Drawing.Image.FromFile("Recursos//B4.png");

                oc_invitar_btn.Image = System.Drawing.Image.FromFile("Recursos//hide.png");
                invite_btn.Show();
                data.Show();
                ayudita_pnl.Show();
                invitacion = true;
            }
        }

        private void disc_btn_MouseDown(object sender, MouseEventArgs e)
        {
            disc_btn.Location = new Point(disc_btn.Location.X, disc_btn.Location.Y - 3);
        }

        //FUNCIONES PARA MOVER EL FORM Y DEL BOTÓN DE CERRAR Y MINIMIZAR.

        private void move_pnl_MouseDown(object sender, MouseEventArgs e)
        {
            mX = e.X;
            mY = e.Y;
            mouseDown = true;
        }

        private void move_pnl_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown == true)
            {
                this.SetDesktopLocation(MousePosition.X - mX, MousePosition.Y - mY);
            }
        }

        private void move_pnl_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void cerrar_btn_Click(object sender, EventArgs e)
        {
            if (conectado == true)
            {
                byte[] msg = System.Text.Encoding.ASCII.GetBytes("0/");
                server.Send(msg);

                atender.Abort();
                server.Shutdown(SocketShutdown.Both);
                server.Close();

                for (int i = 0; i < formularios.Count(); i++)
                {
                    formularios[i].Close();
                }

                OcultarContenido();

                this.Close();
                this.Dispose();
            }
            else
            {
                this.Close();
                this.Dispose();
            }
        }

        private void cerrar_btn_MouseEnter(object sender, EventArgs e)
        {
            cerrar_btn.BackColor = Color.FromArgb(209, 54, 57);
            cerrar_btn.Image = System.Drawing.Image.FromFile("Recursos//Exit2.png");
        }

        private void cerrar_btn_MouseLeave(object sender, EventArgs e)
        {
            cerrar_btn.BackColor = Color.Transparent;
            cerrar_btn.Image = System.Drawing.Image.FromFile("Recursos//Exit1.png");
        }

        private void min_btn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void min_btn_MouseEnter(object sender, EventArgs e)
        {
            min_btn.BackColor = Color.FromArgb(226, 226, 226);
        }

        private void min_btn_MouseLeave(object sender, EventArgs e)
        {
            min_btn.BackColor = Color.Transparent;
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
