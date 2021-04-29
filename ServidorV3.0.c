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

void EliminarLista(ListaDeConectados *lista, char usuario[20])
{	
	int i = 0;
	int encontrado = 0;
	
	while((encontrado == 0) && (lista->num > i))
	{
		if (strcmp(lista->conectados[i].usuario, usuario) == 0)
		{
			encontrado = 1;  
		}
		else
		{
			i++;
		}
	}

	if(encontrado == 1)
	{
		while(i < lista->num)
		{
			lista->conectados[i] = lista->conectados[i+1];
			i++;
		}    
		
		lista->num--; 
	}
}

//Funcion que retorna un string con los usuarios conectados.

void DameConectados(ListaDeConectados *lista, char conectados[300])
{
	int i = 0;
	
	while(i < lista->num)
	{
		sprintf(conectados,"%s%s,", conectados, lista->conectados[i].usuario);
		
		i++;
	}
}

pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

int i;

int sockets[100]; //Vector de sockets.

ListaDeConectados lista; //Lista de conectados.

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
		printf ("Codigo y peticion: %s.\n", peticion);
		
		char *p = strtok( peticion, "/");
		
		int codigo =  atoi (p);
		
		if (codigo == 0) //Desconectar usuario.
		{
			pthread_mutex_lock(&mutex);
			//Eliminamos el usuario de la lista de conectados.
			EliminarLista(&lista, usuario);
			pthread_mutex_unlock(&mutex);
			
			printf("%s se ha desconectado.\n", usuario);
			
			terminar = 1;
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
			}
			
			else if (codigo == 2) //Registrar usuario.
			{ 
				p = strtok(NULL, "/");
				strcpy(usuario, p);
				
				p = strtok(NULL, "/");
				strcpy(contra, p);
				
				printf("Se va a registrar %s con la contrasena %s.\n", usuario, contra);
				
				terminar = Registrarse(usuario, contra, respuesta, conn, sock_conn);
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
			
			if ((codigo == 1) || (codigo == 2) || (codigo == 3) || (codigo == 4) || (codigo == 5))
			{				
				write (sock_conn, respuesta, strlen(respuesta));
			}
		}
		
		if((codigo == 0) || (codigo == 1) || (codigo == 2))
		{		
			int j;
			char notificacion[200];
			char conectados[200];
			
			pthread_mutex_lock(&mutex);
			DameConectados(&lista, conectados);
			pthread_mutex_unlock(&mutex);
			
			printf("%s\n", conectados);
			
			sprintf (notificacion, "6/%s", conectados);
			
			for(int j = 0; j < lista.num; j++)
			{
				write (lista.conectados[j].socket, notificacion, strlen(notificacion));
			}
		}		
	}
	close(sock_conn);
}

int IniciarSesion(char usuario[20], char contra[20], char respuesta[100], MYSQL *conn, int sock_conn){
	
	MYSQL_ROW row;
	MYSQL_RES *resultado;
	
	char consulta[100];
	
	sprintf(consulta, "SELECT usuario FROM jugador WHERE usuario = '%s' AND contrasena = '%s';", usuario, contra);
	
	printf("%s\n", consulta);
	
	int err = mysql_query (conn, consulta);
	
	if (err != 0){
		printf ("Error al consultar la base de datos. %u %s\n", mysql_errno(conn), mysql_error(conn));
		
		exit (1);
	}
	
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	
	if(row == NULL){
		sprintf(respuesta, "1/0");
		return 1;
	}
	else{		
		pthread_mutex_lock(&mutex);
		AnadirLista(&lista, usuario, sock_conn);
		pthread_mutex_unlock(&mutex);
		sprintf(respuesta, "1/1");
		return 0;
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
	serv_adr.sin_port = htons(9013);
	
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
		printf("Conexi￳n recibida.\n");
		
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