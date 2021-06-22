#include <stdio.h>
#include <mysql.h>
#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <pthread.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>

/*Estructura para la lista de conectados, en la cual se 
almacenaran los nombres y los sockets de los usuarios.*/
typedef struct{
	char nombre[20];
	int socket;
} Conectado;

typedef struct{
	Conectado conectados[100];
	int num;
} ListaDeConectados;

/*LISTA DE PARTIDAS.*/
typedef struct{
	int id;
	int socket1; //El jugador numero 1 siempre sera el host(el que invita)
	int socket2;
	char nombre1[20];
	char nombre2[20];
	int jugada1;
	int jugada2;
	int vida1;
	int vida2;
	int bala1;
	int bala2;
	int ronda;
}Partida;

typedef struct{
	Partida partidas[100];
	int num;
} ListaDePartidas;

pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

ListaDeConectados listaC; /*Lista de conectados.*/
ListaDePartidas listaP; /*Lista de partidas.*/

/*FUNCIONES PARA LA LISTA DE CONECTADOS.*/

/*Funcion que aÃ±ade a la lista de conectados un usuario.
Devuelve -2 si el usuario ya esta en la lista, -1 si la 
lista esta llena y 0 si se ha introducido correctamente*/
int AnadirLista (ListaDeConectados *lista, char nombre[20], int socket)
{
	if (lista->num == 100)
	{
		return -1;
	}
	int i = 0;
	
	while(i < lista->num)
	{
		if(strcmp(lista->conectados[i].nombre, nombre) == 0)
		{
			return -2;
		}
		i++;
	}
	
	lista->conectados[lista->num].socket = socket;
	strcpy (lista->conectados[lista->num].nombre, nombre);
	
	printf("Socket: %d, nombre: %s y posicion de la lista: %d.\n", socket, nombre, lista->num);
	lista->num++;
	
	return 0;
}

/*Funcion que elimina un usuario de la lista de conectados, quita el socket de la lista y devuelve 1.*/
int EliminarLista(ListaDeConectados *lista, char nombre[20], int socket)
{	
	int i = DamePosicion(lista, nombre);
	
	while(i < lista->num - 1)
	{
		lista->conectados[i] = lista->conectados[i+1];
		i++;
	}
	lista->num--;
	
	return 1;
}

/*Funcion que retorna un string con los usuarios conectados.*/
void DameConectados(ListaDeConectados *lista, char conectados[300])
{
	int i = 0;
	
	sprintf(conectados, "%d", lista->num);
	
	while(i < lista->num)
	{
		sprintf(conectados,"%s,%s", conectados, lista->conectados[i].nombre);
		
		i++;
	}
}

/*Funcion que retorna la posicion del usuario deseado en la lista de conectados.*/
int DamePosicion(ListaDeConectados *lista, char nombre[20])
{
	int i = 0;
	
	while(i < lista->num)
	{
		if (strcmp(lista->conectados[i].nombre, nombre) == 0)
		{
			return i;
		}
		i++;
	}
}

/*Funcion que retorna el numero de socket de un 
usuario en concreto en la lista de conectados.*/
int DameSocket(ListaDeConectados *lista, char nombre[20])
{
	int i = 0;
	
	while(i < lista->num)
	{
		if (strcmp(lista->conectados[i].nombre, nombre) == 0)
		{
			return lista->conectados[i].socket;
		}
		i++;
	}
}

/*Funcion que proporciona atencion continua al cliente.*/
void *AtenderCliente (void *socket)
{
	int err;
	MYSQL *conn;
	
	int sock_conn;
	int *s;
	s = (int *) socket;
	sock_conn= *s;
	
	int terminar = 0;
	
	char peticion[512];
	char respuesta[512];
	
	int ret;
	
	char *usuario[20];
	char *contra[20];
	
	conn = mysql_init(NULL);
	
	if (conn == NULL)
	{
		printf ("Error al crear la conexion: %u %s.\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost", "root", "mysql", "bd", 0, NULL, 0);
	
	if (conn == NULL)
	{
		printf ("Error al inicializar la conexion: %u %s.\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	while (terminar == 0)
	{
		//Ahora recibimos su mensaje, que dejamos en buff.
		ret = read(sock_conn, peticion, sizeof(peticion));
		
		peticion[ret] = '\0';
		
		printf ("Recibido.\n");
		
		//Escribimos el nombre en la consola.
		printf ("Se ha registrado una peticion: %s.\n", peticion);
		
		char *p = strtok( peticion, "/");
		int codigo =  atoi (p);
		
		char idPartida[10];
		
		int RLD = 0;
		
		if (codigo == 0) //Desconectar usuario.
		{
			pthread_mutex_lock(&mutex);
			//Eliminamos el usuario de la lista de conectados.
			terminar = EliminarLista(&listaC, usuario,sock_conn);
			pthread_mutex_unlock(&mutex);
			
			printf("%s se ha desconectado.\n", usuario);
			
			if(terminar == 1)
			{
				RLD = 1;
			}
		}
		else
		{
			if (codigo == 1) //Iniciar sesion.
			{		
				p = strtok(NULL, "/");
				strcpy(usuario, p);
				p = strtok(NULL, "/");
				strcpy(contra, p);
				
				terminar = IniciarSesion(usuario, contra, respuesta, conn, sock_conn);
				
				if(terminar == 0)
				{
					RLD = 1;
				}
			}
			
			else if (codigo == 2) //Registrar usuario.
			{ 
				p = strtok(NULL, "/");
				strcpy(usuario, p);
				
				p = strtok(NULL, "/");
				strcpy(contra, p);
				
				printf("Se va a registrar %s con la contrasena %s.\n", usuario, contra);
				
				terminar = Registrarse(usuario, contra, respuesta, conn, sock_conn);
				
				if(terminar == 0)
				{
					RLD = 1;
				}
			}
			
			else if (codigo == 3) //Partida mas rapida.
			{ 
				PartidaRapida(respuesta, conn);
			}
			
			else if (codigo == 4) //Decir ganador de una partida.
			{ 
				p = strtok(NULL, "/");
				int id = atoi(p);
				
				GanadorPartida(id, respuesta, conn);
			}
			
			else if (codigo == 5) //Quien haya ganado mas partidas.
			{
				char ganador[20];
				p = strtok(NULL, "/");
				strcpy(ganador, p);
				
				PartidasGanadas(ganador, respuesta, conn);
			}
			
			else if (codigo == 6) //Invitar a un jugador.
			{ 
				char rival[20];
				p = strtok(NULL, "/");
				strcpy(rival, p);
				
				printf("%s invita a %s.\n", usuario, rival);
				
				InvitarJugador(usuario, rival, sock_conn);
			}
			
			else if (codigo == 7) //Responder a un jugador.
			{ 
				char rival[20];
				char respuesta[20];
				
				printf("Ahora responderemos la notificacion.\n");;
				
				p = strtok(NULL, "/");
				strcpy(rival, p);
				
				printf("El usuario es %s y ", rival);
				
				p = strtok(NULL, "/");
				strcpy(respuesta, p);
				
				printf("la respuesta es %s.\n", respuesta);
				
				pthread_mutex_lock(&mutex);
				RespuestaInvitacion(usuario, rival, respuesta, sock_conn, &listaP);
				pthread_mutex_unlock(&mutex);
			}
			
			else if (codigo == 8) //Chat partida.
			{ 
				char mensaje1[512];
				char mensaje2[512];
				
				int i = 0;
				
				p = strtok(NULL, "/");
				strcpy(idPartida, p);
				
				i = atoi(idPartida);
				
				printf("La ID de la partida es %d ", i);
				
				p = strtok(NULL, "/");
				strcpy(mensaje1, p);
				
				printf("y el mensaje es %s.\n", mensaje1);
				
				sprintf(mensaje2, "9/%s/%s", idPartida, mensaje1);
				
				printf("%s.\n", mensaje2);
				
				write (listaP.partidas[i].socket1, mensaje2, strlen(mensaje2));
				write (listaP.partidas[i].socket2, mensaje2, strlen(mensaje2));
			}
			
			else if (codigo == 9) //Hacer jugada
			{
				int jugada;
				int i = 0;
				
				p = strtok(NULL, "/");
				strcpy(idPartida, p);
				
				i = atoi(idPartida);
				
				p = strtok(NULL, "/");
				
				jugada = atoi(p);
				
				printf("ID: %d y Jugada: %d.\n", i, jugada);
				
				pthread_mutex_lock(&mutex);
				MirarJugadas(&listaP, i, jugada, sock_conn, conn);
				pthread_mutex_unlock(&mutex);
			}
			
			if (codigo == 1 || codigo == 2 || codigo == 3 || codigo == 4 || codigo == 5)
			{
				write (sock_conn, respuesta, strlen(respuesta));
			}			
		}
		if(RLD == 1)
		{
			int i = 0;
			
			char conectados[200];
			char notificacion[200];
			
			pthread_mutex_lock(&mutex);
			DameConectados(&listaC, conectados);
			pthread_mutex_unlock(&mutex);
			
			printf("Los usuarios conectados son: %s.\n", conectados);
			
			sprintf (notificacion, "6/0/%s", conectados);
			
			while(i < listaC.num)
			{
				write (listaC.conectados[i].socket, notificacion, strlen(notificacion));
				
				i++;
			}
			RLD = 0;	
		}
	}
	close(sock_conn);
}

void IniciarSesion(char usuario[20], char contra[20], char respuesta[100], MYSQL *conn, int sock_conn)
{
	MYSQL_ROW row;
	MYSQL_RES *resultado;
	
	char consulta[100];
	
	sprintf(consulta, "SELECT usuario FROM jugador WHERE usuario = '%s' AND contrasena = '%s';", usuario, contra);
	
	printf("%s\n", consulta);
	
	int err = mysql_query (conn, consulta);
	
	if (err != 0)
	{
		printf ("Error al consultar la base de datos %u %s.\n", mysql_errno(conn), mysql_error(conn));
		
		exit (1);
	}
	
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	
	if(row == NULL)
	{
		sprintf(respuesta, "1/0/1"); //Usuario no registrado.
	}
	else
	{		
		pthread_mutex_lock(&mutex);
		int j = AnadirLista(&listaC, usuario, sock_conn);
		pthread_mutex_unlock(&mutex);
		
		if(j == 0)
		{
			printf("%s ha iniciado sesion.\n", usuario);
			sprintf(respuesta, "1/0/0");
		}
		else if(j == -1)
		{
			printf("La lista de conectados esta llena.\n");
			sprintf(respuesta, "1/0/2");
		}
		else
		{
			printf("El usuario %s ya tiene una sesion activa.\n", usuario);
			sprintf(respuesta, "1/0/3");
		}
	}
}

void Registrarse(char usuario[20], char contra[20], char respuesta[100], MYSQL *conn, int sock_conn)
{
	MYSQL_ROW row;
	MYSQL_RES *resultado;
	
	char consulta[100];
	
	sprintf (consulta, "INSERT INTO jugador(usuario, contrasena, victorias) VALUES('%s', '%s', '%d')", usuario, contra, 0);
	int err = mysql_query (conn, consulta);
	
	if (err!=0)
	{
		printf ("Error al introducir datos la base %u %s.\n", mysql_errno(conn), mysql_error(conn));
		
		printf("El usuario %s ya existe.\n", usuario);
		
		strcpy(respuesta,"2/0/1");
	}
	else
	{
		pthread_mutex_lock (&mutex);
		int j = AnadirLista(&listaC, usuario, sock_conn);
		pthread_mutex_unlock (&mutex);
		
		if(j == 0)
		{
			printf("%s ha iniciado sesion.\n", usuario);
			sprintf(respuesta, "2/0/0");
		}
		else
		{
			printf("La lista de conectados esta llena.\n");
			sprintf(respuesta, "2/0/2");
		}
	}
}

void PartidaRapida(char respuesta[100], MYSQL *conn)
{
	MYSQL_ROW row;
	MYSQL_RES *resultado;
	
	int err = mysql_query (conn, "SELECT duracion FROM partida WHERE duracion = (SELECT MIN(duracion) FROM partida)");
	
	if (err!=0)
	{
		printf ("Error al consultar datos de la base %u %s.\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row (resultado);
	
	if (row == NULL){
		
		printf ("No se han obtenido datos en la consulta.\n");
		strcpy(respuesta, "3/0/0");
	}
	else
	{
		printf("El tiempo de la partida mas rapida es de %s turnos.\n", row[0]);
		sprintf(respuesta, "3/0/%s", row[0]);
	}
}

void GanadorPartida(int id, char respuesta[100], MYSQL *conn)
{
	MYSQL_ROW row;
	MYSQL_RES *resultado;
	
	char consulta[100];
	
	sprintf (consulta,"SELECT partida.ganador FROM partida WHERE partida.ID = '%d'", id);
	
	int err = mysql_query (conn, consulta);
	
	if (err!=0)
	{
		printf ("Error al consultar datos de la base %u %s.\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row (resultado);
	
	if(row == NULL)
	{
		printf("No se han obtenido los datos de la consulta.\n");
		strcpy(respuesta, "4/0/0");
	}	
	else
	{
		printf("El ganador de la partida es %s.\n", row[0]);
		sprintf(respuesta, "4/0/%s", row[0]);
	}
}

void PartidasGanadas(char usuario[20], char respuesta[100], MYSQL *conn)
{
	MYSQL_ROW row;
	MYSQL_RES *resultado;
	
	char consulta[100];
	
	sprintf(consulta, "SELECT jugador.victorias FROM jugador WHERE jugador.usuario = '%s'", usuario);
	
	int err = mysql_query (conn, consulta);
	
	if (err != 0)
	{
		printf ("Error al consultar datos de la base %u %s.\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row (resultado);
	
	if(row == NULL)
	{
		printf("No se han obtenido los datos de la consulta.\n");
		sprintf(respuesta, "5/0/0");
	}		
	else
	{
		printf("Dicho usuario ha ganado %s partidas.\n", row[0]);
		sprintf(respuesta, "5/0/%s", row[0]);
	}
}

void InvitarJugador(char usuario[20], char invitado[20])
{
	char notificacion[512];
	
	int i = DamePosicion(&listaC, invitado);
	
	sprintf(notificacion, "7/0/%s", usuario);
	
	printf("Vamos a enviar notificacion %s.\n", notificacion);
	
	write (listaC.conectados[i].socket, notificacion, strlen(notificacion));
}

//Recibimos la respuesta, si ha aceptado rellenamos la lista de partidas con la informacion de los jugadores y mandamos un mensaje al que ha invitado informandole si han aceptado o no.
void RespuestaInvitacion(char usuario[20], char rival[20], char respuesta[20], int sock_conn, ListaDePartidas *lista)
{
	char notificacion[512];
	
	printf("Vamos a enviar la respuesta de la invitacion.\n");
	
	int i = DamePosicion(&listaC, rival);
	
	if(strcmp(respuesta, "SI") == 0)
	{
		//Inicializamos todos los datos de la lista.
		lista->partidas[lista->num].id = lista->num;
		lista->partidas[lista->num].socket1 = listaC.conectados[i].socket;
		lista->partidas[lista->num].socket2 = sock_conn;
		strcpy(lista->partidas[lista->num].nombre1, listaC.conectados[i].nombre);
		strcpy(lista->partidas[lista->num].nombre2, usuario);
		lista->partidas[lista->num].vida1 = 5;
		lista->partidas[lista->num].vida2 = 5;
		lista->partidas[lista->num].jugada1 = 0;
		lista->partidas[lista->num].jugada2 = 0;
		lista->partidas[lista->num].bala1 = 0;
		lista->partidas[lista->num].bala2 = 0;
		
		printf("Guardamos cada dato de la partida, %s, %s y mas...\n", lista->partidas[lista->num].nombre1, lista->partidas[lista->num].nombre2);
		
		sprintf(notificacion, "8/%d/0", lista->num);
		
		printf("Enviamos respuesta %s.\n", notificacion);
		
		lista->num++;
	}
	else
	{
		strcpy(notificacion, "8/0/1");
		
		printf("Enviamos respuesta %s.\n", notificacion);
	}
	
	write (listaC.conectados[i].socket, notificacion, strlen(notificacion));
	write (sock_conn, notificacion, strlen(notificacion));
}

void MirarJugadas(ListaDePartidas *listaP, int idPartida, int jugada, int socket, MYSQL *conn)
{
	char respuesta[512];
	
	printf("ID: %d, tipo de jugada: %d y socket: %d.\n", idPartida, jugada, socket);
	
	if (listaP->partidas[idPartida].socket1 == socket)
	{
		listaP->partidas[idPartida].jugada1 = jugada;
		
		printf("Registramos primera jugada del jugador1: %d.\n", jugada);
	}
	
	if (listaP->partidas[idPartida].socket2 == socket)
	{
		listaP->partidas[idPartida].jugada2 = jugada;
		
		printf("Registramos primera jugada del jugador2: %d.\n", jugada);
	}
	
	if(listaP->partidas[idPartida].jugada1 == 4)
	{
		char solucion[512];
		sprintf(solucion, "11/%d/r", idPartida); //Ganador.
		write(listaP->partidas[idPartida].socket2, solucion, strlen(solucion));
	}
	
	if(listaP->partidas[idPartida].jugada2 == 4)
	{
		char solucion[512];
		sprintf(solucion, "11/%d/r", idPartida); //Ganador.
		write(listaP->partidas[idPartida].socket1, solucion, strlen(solucion));
	}
	
	if ((listaP->partidas[idPartida].jugada1 != 0) && (listaP->partidas[idPartida].jugada2 != 0))
	{
		if ((listaP->partidas[idPartida].jugada1 == 1) && (listaP->partidas[idPartida].jugada2 == 1))
		{
			listaP->partidas[idPartida].vida1 = listaP->partidas[idPartida].vida1 - 1;
			listaP->partidas[idPartida].vida2 = listaP->partidas[idPartida].vida2 - 1;
			listaP->partidas[idPartida].bala1 = listaP->partidas[idPartida].bala1 - 1;
			listaP->partidas[idPartida].bala2 = listaP->partidas[idPartida].bala2 - 1;
		}
		
		if ((listaP->partidas[idPartida].jugada1 == 1) && (listaP->partidas[idPartida].jugada2 == 2))
		{
			listaP->partidas[idPartida].bala1 = listaP->partidas[idPartida].bala1 - 1;
		}
		
		if ((listaP->partidas[idPartida].jugada1 == 1) && (listaP->partidas[idPartida].jugada2 == 3))
		{
			listaP->partidas[idPartida].vida2 = listaP->partidas[idPartida].vida2 - 1;
			listaP->partidas[idPartida].bala1 = listaP->partidas[idPartida].bala1 - 1;
			listaP->partidas[idPartida].bala2 = listaP->partidas[idPartida].bala2 + 1;
		}
		
		if ((listaP->partidas[idPartida].jugada1 == 2) && (listaP->partidas[idPartida].jugada2 == 1))
		{
			listaP->partidas[idPartida].bala2 = listaP->partidas[idPartida].bala2 - 1;
		}
		
		if ((listaP->partidas[idPartida].jugada1 == 2) && (listaP->partidas[idPartida].jugada2 == 3))
		{
			listaP->partidas[idPartida].bala2 = listaP->partidas[idPartida].bala2 + 1;
		}
		
		if ((listaP->partidas[idPartida].jugada1 == 3) && (listaP->partidas[idPartida].jugada2 == 1))
		{
			listaP->partidas[idPartida].vida1 = listaP->partidas[idPartida].vida1 - 1;
			listaP->partidas[idPartida].bala1 = listaP->partidas[idPartida].bala1 + 1;
			listaP->partidas[idPartida].bala2 = listaP->partidas[idPartida].bala2 - 1;
		}
		
		if ((listaP->partidas[idPartida].jugada1 == 3) && (listaP->partidas[idPartida].jugada2 == 2))
		{
			listaP->partidas[idPartida].bala1 = listaP->partidas[idPartida].bala1 + 1;
		}
		
		if ((listaP->partidas[idPartida].jugada1 == 3) && (listaP->partidas[idPartida].jugada2 == 3))
		{
			listaP->partidas[idPartida].bala1 = listaP->partidas[idPartida].bala1 + 1;
			listaP->partidas[idPartida].bala2 = listaP->partidas[idPartida].bala2 + 1;
		}
		
		listaP->partidas[idPartida].ronda = listaP->partidas[idPartida].ronda + 1;
		
		//11/numform1/jugada_riva1l/vida1/vida2/bala1/
		
		sprintf(respuesta, "10/%d/%d-%d-%d-%d", idPartida, listaP->partidas[idPartida].jugada2, listaP->partidas[idPartida].vida1, listaP->partidas[idPartida].vida2, listaP->partidas[idPartida].bala1);
		write(listaP->partidas[idPartida].socket1, respuesta, strlen(respuesta));
		
		sprintf(respuesta, "10/%d/%d-%d-%d-%d", idPartida, listaP->partidas[idPartida].jugada1, listaP->partidas[idPartida].vida1, listaP->partidas[idPartida].vida2, listaP->partidas[idPartida].bala2);
		write(listaP->partidas[idPartida].socket2, respuesta, strlen(respuesta));
		
		listaP->partidas[idPartida].jugada1 = 0;
		
		listaP->partidas[idPartida].jugada2 = 0;
	}
	
	if((listaP->partidas[idPartida].vida1 == 0) || (listaP->partidas[idPartida].vida2 == 0))
	{
		TerminarPartida(listaP->partidas[idPartida].vida1, listaP->partidas[idPartida].vida2, listaP->partidas[idPartida].socket1, listaP->partidas[idPartida].socket2, idPartida, conn);
	}
}

void TerminarPartida(int vida1, int vida2, int socket1, int socket2, int idPartida)
{
	char respuesta1[512];
	char respuesta2[512];
	
	if((vida1 == 0) && (vida2 != 0))
	{
		sprintf(respuesta1, "11/%d/p", idPartida); //Perdedor.
		write(socket1, respuesta1, strlen(respuesta1));
		sprintf(respuesta2, "11/%d/g", idPartida); //Ganador.
		write(socket2, respuesta2, strlen(respuesta2));
	}
	
	if((vida2 == 0) && (vida1 != 0))
	{
		sprintf(respuesta2, "11/%d/p", idPartida); //Perdedor.
		write(socket2, respuesta2, strlen(respuesta2));
		sprintf(respuesta1, "11/%d/g", idPartida); //Ganador.
		write(socket1, respuesta1, strlen(respuesta1));
	}
	
	if((vida2 == 0) && (vida1 == 0))
	{
		sprintf(respuesta1, "11/%d/e", idPartida); //Empate.
		write(socket1, respuesta1, strlen(respuesta1));
		write(socket2, respuesta1, strlen(respuesta1));
	}
	
	/*		MYSQL_ROW row;*/
	/*		MYSQL_RES *resultado;*/
	
	/*		char consulta[100];*/
	
	/*		sprintf (consulta, "INSERT INTO partida(ganador, duracion) VALUES('%s', '%d')", contra, 0);*/
	/*		int err = mysql_query (conn, consulta);*/
	
	/*		if (err!=0)*/
	/*		{*/
	/*			printf ("Error al introducir datos la base %u %s.\n", mysql_errno(conn), mysql_error(conn));*/
	/*		}*/
	/*		else*/
	/*		{*/
	
	/*		}*/
}

int main(int argc, char *argv[])
{
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	
	char peticion[512];
	char respuesta[512];
	
	listaC.num = 0;
	listaP.num = 0;
	
	//INICIAR SOCKET
	
	//Abrimos el socket.
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
	{
		
		printf("Error creando en el socket.\n");
	}
	
	//Inicialitza el zero serv_addr.
	memset(&serv_adr, 0, sizeof(serv_adr));
	
	//Asocia el socket a cualquiera de las IP de la maquina. 
	//htonl formatea el numero que recibe al formato necesario.
	
	serv_adr.sin_family = AF_INET;
	
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	
	//Escuchamos en el puerto.
	serv_adr.sin_port = htons(9000);
	
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0){
		
		printf ("Error en el bind.\n");
	}
	
	//La cola de peticiones pendientes no podra ser superior a 4.
	if (listen(sock_listen, 4) < 0)
	{
		printf("Error al escuchar.\n");
	}
	
	pthread_t thread;
	
	int pos_socket = 0;
	
	for(;;)
	{
		printf("Escuchando...\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf("Conexion recibida.\n");
		
		if (listaC.conectados[pos_socket].socket == NULL)
		{
			listaC.conectados[pos_socket].socket = sock_conn;
			
			//Creamos un thread por cliente y lo atendemos.
			pthread_create (&thread, NULL, AtenderCliente, &listaC.conectados[pos_socket].socket);
			
			pos_socket = pos_socket + 1;
		}
		else
		{
			printf("Lista de conectados llena.\n");
		}
	}
}