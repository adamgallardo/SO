#include <stdio.h>
#include <mysql.h>
#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <pthread.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>

typedef struct{
	char usuario[20];
	int socket;
} Conectado;

typedef struct{
	Conectado conectados[100];
	int num;
} ListaDeConectados;

//FUNCIONES PARA LA LISTA DE CONECTADOS.

//Agrega a la lista de conectados un nuevo usuario.

void AnadirLista(ListaDeConectados *lista, char usuario[20], int socket) 
{
	strcpy (lista->conectados[lista->num].usuario, usuario);
	lista->conectados[lista->num].socket = socket;
	
	printf("Usuario: %s, socket: %d\n", usuario, socket);
	
	lista->num++;
}

//Elimina un usuario de la lista de conectados.

int EliminarLista(ListaDeConectados *lista, char usuario[20])
{
	int i = 0;
	int encontrado = 0;
	
	while((lista->num > i) && (encontrado == 0))
	{
		if (strcmp(lista->conectados[i].usuario, usuario) == 0)
		{
			while(i < lista->num)
			{
				lista->conectados[i] = lista->conectados[i+1];
			}    
			
			lista->num--;   
			encontrado = 1;
			
			return 0;
		}
		else
		{
			i++;
		}
	}
	
	if(encontrado == 0)
	{
		return 1;
	}
}

//Funcion que retorna un string con los usuarios conectados.

void DameConectados(ListaDeConectados *lista, char conectados[300])
{
	int i = 0;
	
	sprintf(conectados, "%d", lista->num);
	
	while(i < lista->num)
	{
		sprintf(conectados,"%s,%s", conectados, lista->conectados[i].usuario);
		
		i++;
	}
}

//Funcion que retorna la posicion del usuario deseado.

int DamePosicion(ListaDeConectados *lista, char usuario[20]){
	
	int i = 0;
	
	while(i < lista->num)
	{
		if (strcmp(lista->conectados[i].usuario,usuario) == 0)
		{
			return i;
		}
	}
}

/*typedef struct{
Conectado jugadores[2]; //Lista con los jugadores.
} Partida;
#include <stdio.h>
#include <mysql.h>
#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <pthread.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>

typedef struct{
char usuario[20];
int socket;
} Conectado;

typedef struct{
Conectado conectados[100];
int num;
} ListaDeConectados;

//FUNCIONES PARA LA LISTA DE CONECTADOS.

//Agrega a la lista de conectados un nuevo usuario.

void AnadirLista(ListaDeConectados *lista, char usuario[20], int socket) 
{
strcpy (lista->conectados[lista->num].usuario, usuario);
lista->conectados[lista->num].socket = socket;

printf("Usuario: %s, socket: %d\n", usuario, socket);

lista->num++;
}

//Elimina un usuario de la lista de conectados.

int EliminarLista(ListaDeConectados *lista, char usuario[20])
{
int i = 0;
int encontrado = 0;

while((lista->num > i) && (encontrado == 0))
{
if (strcmp(lista->conectados[i].usuario, usuario) == 0)
{
while(i < lista->num)
{
lista->conectados[i] = lista->conectados[i+1];
}    

lista->num--;   
encontrado = 1;

return 0;
}
else
{
i++;
}
}

if(encontrado == 0)
{
return 1;
}
}

//Funcion que retorna un string con los usuarios conectados.

void DameConectados(ListaDeConectados *lista, char conectados[300])
{
int i = 0;

sprintf(conectados, "%d", lista->num);

while(i < lista->num)
{
sprintf(conectados,"%s,%s", conectados, lista->conectados[i].usuario);

i++;
}
}

//Funcion que retorna la posicion del usuario deseado.

int DamePosicion(ListaDeConectados *lista, char usuario[20]){

int i = 0;

while(i < lista->num)
{
if (strcmp(lista->conectados[i].usuario,usuario) == 0)
{
return i;
}
}
}

/*typedef struct{
Conectado jugadores[2]; //Lista con los jugadores.
} Partida;

typedef struct{
Partida partidas[100];
int numPartidas; //Total de partidas en la tabla.
}ListaDePartidas;
*/
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

int i;

int sockets[100]; //Vector de sockets.

ListaDeConectados lista; //Lista de conectados.
//ListaDePartidas partida; //Lista de partidas.

//Funcion que proporciona atencion al cliente.

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
	
	int P = 0;
	char *rival[20];
	
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
		
		peticion[ret]='\0';
		
		printf ("Recibido.\n");
		
		//Escribimos el nombre en la consola.
		printf ("Se ha conectado: %s.\n", peticion);
		
		char *p = strtok( peticion, "/");
		
		int codigo =  atoi (p);
		
		int RLD = 0;
		
		if (codigo == 0) //Desconectar usuario.
		{
			printf("punto 3");
			pthread_mutex_lock(&mutex);
			//Eliminamos el usuario de la lista de conectados.
			terminar = EliminarLista(&lista, usuario);
			pthread_mutex_unlock(&mutex);
			
			if(terminar == 0)
			{
				RLD = 1;
			}
			
			printf("%s se ha desconectado.\n", usuario);
		}
		else
		{
			if (codigo == 1) //Iniciar sesion.
			{		
				p = strtok(NULL, "/");
				strcpy(usuario, p);
				p=strtok(NULL, "/");
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
				char u[20];
				p = strtok(NULL, "/");
				strcpy(u, p);
				
				PartidasGanadas(u, respuesta, conn);
			}
			
			else if (codigo == 6) //Invitar a un jugar.
			{ 
				char u[20];
				p = strtok(NULL, "/");
				strcpy(u, p);
				
				strcpy(rival, u);
				
				printf("%s invita a %s.\n", usuario, u);
				
				InvitarJugador(usuario, u, sock_conn);
			}
			
			else if (codigo == 7) //Responder a un jugador.
			{ 
				printf("Ahora responderemos la notificacion.\n");
				
				char u[20];
				p = strtok(NULL, "/");
				strcpy(u, p);
				
				strcpy(rival, u);
				
				printf("El usuario es %s y ", u);
				
				char r[20];
				p = strtok(NULL, "/");
				strcpy(r, p);
				
				printf("la respuesta es %s.\n", r);
				
				RespuestaInvitacion(u, r);
			}
			
			else if (codigo == 9)
			{
				int j;
				
				char mensaje[200];
				p = strtok(NULL, "/");
				sprintf(mensaje, "9/%s", p);
				
				
				for(int j = 0; j<lista.num; j++)
				{
					write (lista.conectados[j].socket, mensaje, strlen(mensaje));
				}
			}
			
			if (codigo == 1 || codigo == 2 || codigo == 3 || codigo == 4 || codigo == 5)
			{
				write (sock_conn, respuesta, strlen(respuesta));
			}
		}
		
		if(RLD == 1)
		{
			int j;
			char notificacion[200];
			char conectados[200];
			pthread_mutex_lock(&mutex);
			DameConectados(&lista, conectados);
			pthread_mutex_unlock(&mutex);
			
			printf("Los usuarios conectados son: %s.\n", conectados);
			
			sprintf (notificacion, "6/%s", conectados);
			
			for(int j=0;j<lista.num;j++)
			{
				write (lista.conectados[j].socket, notificacion, strlen(notificacion));
			}
			
			RLD = 0;	
		}
	}
	close(sock_conn);
}

int IniciarSesion(char usuario[20], char contra[20], char respuesta[100], MYSQL *conn, int sock_conn)
{
	MYSQL_ROW row;
	MYSQL_RES *resultado;
	
	char consulta[100];
	
	sprintf(consulta, "SELECT usuario FROM jugador WHERE usuario = '%s' AND contrasena = '%s';", usuario, contra);
	
	printf("%s\n", consulta);
	
	int err = mysql_query (conn, consulta);
	
	if (err != 0)
	{
		printf ("Error al consultar la base de datos. %u %s\n", mysql_errno(conn), mysql_error(conn));
		
		exit (1);
	}
	
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	
	if(row == NULL)
	{
		sprintf(respuesta, "1/0");
		return 1;
	}
	else
	{		
		pthread_mutex_lock(&mutex);
		AnadirLista(&lista, usuario, sock_conn);
		pthread_mutex_unlock(&mutex);
		sprintf(respuesta, "1/1");
		return 0;
		
		//Podriamos hacer que AnadirLista devolviese un valor para saber si esta ya en la lista.
	}
}

int Registrarse(char usuario[20], char contra[20], char respuesta[100], MYSQL *conn, int sock_conn)
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
		strcpy(respuesta,"2/0");
		return 1;
	}
	else
	{
		pthread_mutex_lock (&mutex);
		AnadirLista(&lista, usuario, sock_conn);
		pthread_mutex_unlock (&mutex);
		strcpy(respuesta,"2/1");
		return 0;
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
	}
	else
	{
		printf("El tiempo de la partida mas rapida es %s.\n", row[0]);
		sprintf(respuesta, "3/%s", row[0]);
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
	}	
	else
	{
		printf("El ganador de la partida es %s.\n", row[0]);
		sprintf(respuesta, "4/%s", row[0]);
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
	}		
	else
	{
		printf("Dicho usuario ha ganado %s partidas.\n", row[0]);
		sprintf(respuesta, "5/%s", row[0]);
	}
}

void InvitarJugador(char usuario[20], char invitado[20])
{
	char notificacion[512];
	
	pthread_mutex_lock(&mutex);
	int i = DamePosicion(&lista, invitado);
	pthread_mutex_unlock(&mutex);
	
	sprintf(notificacion, "7/%s", usuario);
	
	printf("Vamos a enviar notificacion %s.\n", notificacion);
	
	write (lista.conectados[i].socket, notificacion, strlen(notificacion));
}

void RespuestaInvitacion(char usuario[20], char respuesta[20])
{
	char notificacion[512];
	
	printf("Vamos a enviar la respuesta de la invitacion.\n");
	
	pthread_mutex_lock(&mutex);
	int i = DamePosicion(&lista, usuario);
	pthread_mutex_unlock(&mutex);
	
	if(strcmp(respuesta, "Si") == 0)
	{
		strcpy(notificacion, "8/SolicitudAceptada");
		
		printf("Enviamos respuesta %s.\n", notificacion);
	}
	else{
		strcpy(notificacion, "8/SolicitudDenegada");
		
		printf("Enviamos respuesta %s.\n", notificacion);
	}
	write (lista.conectados[i].socket, notificacion, strlen(notificacion));
}

int main(int argc, char *argv[])
{
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	
	char peticion[512];
	char respuesta[512];
	
	lista.num = 0;
	
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
	
	//Escuchamos en el puerto 9050.
	serv_adr.sin_port = htons(9010);
	
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0){
		
		printf ("Error en el bind.\n");
	}
	
	//La cola de peticiones pendientes no podra ser superior a 4.
	if (listen(sock_listen, 4) < 0)
	{
		printf("Error al escuchar.\n");
	}
	
	pthread_t thread[100];
	
	for(;;i++)
	{
		printf("Escuchando...\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf("Conexion recibida.\n");
		
		sockets[i] = sock_conn;
		
		//Creamos un thread por cliente y lo atendemos.
		pthread_create (&thread[i], NULL, AtenderCliente, &sockets[i]);
		
		//Reiniciamos el contador de sockets.
		
		if (i == 99)
		{
			i = 0;
		}
	}
}