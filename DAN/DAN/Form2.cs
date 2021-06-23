using System;
using System.Net;
using System.Data;
using System.Linq;
using System.Text;
using System.Media;
using System.Drawing;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace DAN
{
    public partial class Form2 : Form
    {
        Socket server;

        string usuario, rival;

        int numForm, idPartida;

        int mX, mY, aux1, aux2, tuMOV;

        bool mouseDown = false, host = false, onCard = true, pFin = false, cartaSeleccionada = false;
        
        SoundPlayer media; //para .wav.

        //INICIAMOS EL FORM CON LAS VARIABLES NECESARIAS.

        public Form2(int numForm, int idPartida, Socket server, string usuario, string rival, bool host)
        {
            InitializeComponent();

            this.numForm = numForm; this.idPartida = idPartida; this.server = server; this.usuario = usuario; this.rival = rival; this.host = host;

            //FI_lbl.Text = "FORM: " + numForm + " ID: " + idPartida;

            if(host == true) { jug1_lbl.Text = usuario; jug2_lbl.Text = rival; }
            
            else { jug2_lbl.Text = usuario; jug1_lbl.Text = rival; }

            CheckForIllegalCrossThreadCalls = false;
        }



        private void c1_MouseClick(object sender, MouseEventArgs e) //Si le damos a la carta 1, envíamos al servidor que hemos seleccionado dicha carta.
        {
            if (cartaSeleccionada == false)
            {
                if (Convert.ToInt32(muncion_lbl.Text) == 0)
                {
                    nobalas_lbl.Show();
                }
                else
                {
                    tuMOV = 1;
                    onCard = false;
                    nobalas_lbl.Hide();

                    c1.Location = new Point(c1.Location.X + 221, c1.Location.Y - 359);

                    cartaSeleccionada = true;

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("9/" + idPartida + "/1");
                    server.Send(msg);
                }
            }
        }

        private void c2_MouseClick(object sender, MouseEventArgs e) //Si le damos a la carta 1, envíamos al servidor que hemos seleccionado dicha carta.
        {
            if (cartaSeleccionada == false)
            {
                tuMOV = 2;
                onCard = false;
                nobalas_lbl.Hide();

                c2.Location = new Point(c2.Location.X, c2.Location.Y - 359);

                cartaSeleccionada = true;

                byte[] msg = System.Text.Encoding.ASCII.GetBytes("9/" + idPartida + "/2");
                server.Send(msg);
            }
        }

        private void c3_MouseClick(object sender, MouseEventArgs e) //Si le damos a la carta 1, envíamos al servidor que hemos seleccionado dicha carta.
        {
            if (cartaSeleccionada == false)
            {
                tuMOV = 3;
                onCard = false;
                nobalas_lbl.Hide();

                c3.Location = new Point(c3.Location.X - 221, c3.Location.Y - 359);

                cartaSeleccionada = true;

                byte[] msg = System.Text.Encoding.ASCII.GetBytes("9/" + idPartida + "/3");
                server.Send(msg);
            }
        }

        public void jugada(string mensaje) //Comparamos ambos movimiento para que suene un sonido acorde a ambas acciones y actualizamos el form.
        {
            string[] trozos = mensaje.Split('-');

            int mov = Convert.ToInt32(trozos[0]); //Movimiento del rival.

            if ((tuMOV == 1) && (mov == 1))
            {
                media = new SoundPlayer("Recursos//11.wav");
                media.Play();
            }
            else if (((tuMOV == 1) && (mov == 2)) || ((tuMOV == 2) && (mov == 1)))
            {
                media = new SoundPlayer("Recursos//12.wav");
                media.Play();
            }
            else if (((tuMOV == 1) && (mov == 3)) || ((tuMOV == 3) && (mov == 1)))
            {
                media = new SoundPlayer("Recursos//13.wav");
                media.Play();
            }
            else if (((tuMOV == 2) && (mov == 3)) || ((tuMOV == 3) && (mov == 2)))
            {
                media = new SoundPlayer("Recursos//23.wav");
                media.Play();
            }
            else if ((tuMOV == 2) && (mov == 2))
            {
                media = new SoundPlayer("Recursos//22.wav");
                media.Play();
            }
            else if ((tuMOV == 3) && (mov == 3))
            {
                media = new SoundPlayer("Recursos//33.wav");
                media.Play();
            }

            vida1_lbl.Text = trozos[1]; //Vida1.
            
            vida2_lbl.Text = trozos[2]; //Vida2.

            muncion_lbl.Text = trozos[3].Split('\0')[0]; //Balas.

            tuMOV = 0;
            onCard = true;

            cartaSeleccionada = false;

            c1.Location = new Point(318, 475);
            c2.Location = new Point(539, 475);
            c3.Location = new Point(760, 475);
        }

        public void conclusion(string mensaje) //Procedimiento que sirve para finalizar con la partida, mostrando una pantalla final con el resultado.
        {
            OcultarTodosLosComponentes();

            FI_lbl.Text = "S A L I R";
            FI_lbl.BackColor = Color.FromArgb(76, 58, 38);
            FI_lbl.Location = new Point(584, 395);
            FI_lbl.Font = new System.Drawing.Font("Microsoft YaHei UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            FI_lbl.ForeColor = System.Drawing.Color.White;

            if (mensaje == "e")
            {
                this.BackgroundImage = System.Drawing.Image.FromFile("Recursos//E.png");

                //info_lbl.Text = "EMPATE";
            }
            else if(mensaje == "p")
            {
                this.BackgroundImage = System.Drawing.Image.FromFile("Recursos//D.png");

                //info_lbl.Text = "DERROTA";
            }
            else if (mensaje == "g")
            {
                this.BackgroundImage = System.Drawing.Image.FromFile("Recursos//V.png");

                //info_lbl.Text = "VICTORIA";
            }
            else if (mensaje == "r")
            {
                this.BackgroundImage = System.Drawing.Image.FromFile("Recursos//R.png");

                //info_lbl.Text = "TU RIVAL SE HA RENDIDO";
            }

            pFin = true;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e) //Si el form se cierra y la partida aún no ha terminado el rival envía rendición.
        {
            if (pFin == false)
            {
                byte[] msg = System.Text.Encoding.ASCII.GetBytes("9/" + idPartida + "/4");
                server.Send(msg);
            }
        }

        private void OcultarTodosLosComponentes()
        {
            vida1_lbl.Hide(); vida2_lbl.Hide(); c1.Hide(); c2.Hide(); c3.Hide(); enviar_btn.Hide();
            muncion_lbl.Hide(); jug1_lbl.Hide(); jug2_lbl.Hide(); chat_lbx.Hide(); oc_chat_btn.Hide(); consulta_tbx.Hide();
        }

        private void enviar_btn_MouseDown(object sender, MouseEventArgs e)
        {
            enviar_btn.Location = new Point(enviar_btn.Location.X, enviar_btn.Location.Y - 3);
        }

        private void enviar_btn_MouseUp(object sender, MouseEventArgs e) //Envíamos el mensaje del chat al servidor.
        {
            enviar_btn.Location = new Point(enviar_btn.Location.X, enviar_btn.Location.Y + 3);

            byte[] msg = System.Text.Encoding.ASCII.GetBytes("8/" + idPartida + "/" + usuario + ": " + consulta_tbx.Text);
            server.Send(msg);

            consulta_tbx.Text = "ESCRIBE ALGO...";

            aux1 = 0;
        }

        //FUNCIONES PARA CUANDO EL RATÓN SE PONE SOBRE UNA CARTA.

        private void c1_MouseLeave(object sender, EventArgs e)
        {
            if (onCard == true) { c1.Location = new Point(c1.Location.X, c1.Location.Y + 70); }
        }

        private void c1_MouseEnter(object sender, EventArgs e)
        {
            if (onCard == true) { c1.Location = new Point(c1.Location.X, c1.Location.Y - 70); }
        }

        private void c2_MouseEnter(object sender, EventArgs e)
        {
            if (onCard == true) { c2.Location = new Point(c2.Location.X, c2.Location.Y - 70); }
        }

        private void c2_MouseLeave(object sender, EventArgs e)
        {
            if (onCard == true) { c2.Location = new Point(c2.Location.X, c2.Location.Y + 70); }
        }

        private void c3_MouseEnter(object sender, EventArgs e)
        {
            if (onCard == true) { c3.Location = new Point(c3.Location.X, c3.Location.Y - 70); }
        }

        private void c3_MouseLeave(object sender, EventArgs e)
        {
            if (onCard == true) { c3.Location = new Point(c3.Location.X, c3.Location.Y + 70); }
        }

        public void chat(string mensaje)
        {
            chat_lbx.Items.Add(mensaje);
        }

        private void consulta_tbx_MouseClick(object sender, MouseEventArgs e)
        {
            if (aux1 == 0) { consulta_tbx.Text = ""; aux1 = 1; }
        }

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

        protected override CreateParams CreateParams //Cosas raras de Windows para poner sombreado al Form.
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= 0x20000;
                return cp;
            }
        }

        private void FI_lbl_MouseDown(object sender, MouseEventArgs e)
        {
            if (pFin == true)
            {
                FI_lbl.Location = new Point(FI_lbl.Location.X, FI_lbl.Location.Y - 3);
            }
        }

        private void FI_lbl_MouseUp(object sender, MouseEventArgs e)
        {
            if (pFin == true)
            {
                FI_lbl.Location = new Point(FI_lbl.Location.X, FI_lbl.Location.Y + 3);

                this.Close();
                this.Dispose();
            }
        }

        private void cerrar_btn_MouseClick(object sender, MouseEventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void oc_chat_btn_MouseDown(object sender, MouseEventArgs e)
        {
            oc_chat_btn.Location = new Point(oc_chat_btn.Location.X, oc_chat_btn.Location.Y - 3);
        }

        private void oc_chat_btn_MouseUp(object sender, MouseEventArgs e)
        {
            oc_chat_btn.Location = new Point(oc_chat_btn.Location.X, oc_chat_btn.Location.Y + 3);

            if (aux2 == 0)
            {
                this.BackgroundImage = System.Drawing.Image.FromFile("Recursos//J2.png");
                oc_chat_btn.BackgroundImage = System.Drawing.Image.FromFile("Recursos//show.png");
                oc_chat_btn.Location = new Point(oc_chat_btn.Location.X + 407, oc_chat_btn.Location.Y);

                chat_lbx.Hide(); consulta_tbx.Hide(); enviar_btn.Hide();

                aux2 = 1;
            }
            else
            {
                this.BackgroundImage = System.Drawing.Image.FromFile("Recursos//J1.png");
                oc_chat_btn.BackgroundImage = System.Drawing.Image.FromFile("Recursos//hide.png");
                oc_chat_btn.Location = new Point(oc_chat_btn.Location.X - 407, oc_chat_btn.Location.Y);

                chat_lbx.Show(); consulta_tbx.Show(); enviar_btn.Show();

                aux2 = 0;
            }
        }

        private void min_btn_MouseClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
