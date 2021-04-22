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
	
	while(encontrado == 0)
	{
		if (strcmp(lista->conectados[i].usuario, usuario) == 0)
		{
			while(i < lista->num)
			{
				lista->conectados[i] = lista->conectados[i+1];
			}    
			
			lista->num--;   
		}
		else
		{
			i++;
		}
	}
}

//Funcion que retorna un string con los usuarios conectados.

void DameConectados (ListaDeConectados *lista, char conectados[300])
{
	int i = 0;
	
	while(i < lista->num)
	{
		sprintf (conectados,"%s,%s",conectados,lista->conectados[i].usuario);
		
		i++;
	}
}

//VARIABLES.

//Acceso excluyente.

pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

int i;
int sockets[100]; //Vector de sockets.

ListaDeConectados lista; //Lista de conectados.

//Funcion que proporciona atencion al cliente.

void *AtenderCliente (void *socket)
{
	//Nos conectamos al servidor SQL.
	
	int err;
	MYSQL *conn;
	
	conn = mysql_init(NULL); //Creamos una conexion al servidor MYSQL. 
	
	int sock_conn;
	int *s;
	s = (int *) socket;
	sock_conn= *s;
	
	int terminar = 0;
	
	char peticion[512];
	char respuesta[512];
	
	int ret;
	
	// Estructura especial para almacenar resultados de consultas 
	//Entramos en un bucle para atender todas las peticiones de estecliente
	//hasta que se desconecte
	conn = mysql_init(NULL);
	
	if (conn==NULL) 
	{
		printf ("Error al crear la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "juego" ,0, NULL, 0); //Inicializar la conexion.
	
	if (conn==NULL) 
	{
		printf ("Error al inicializar la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	printf("Inician las consultas.");
	
	while(terminar == 0)
	{
		ret = read(sock_conn, peticion, sizeof(peticion)); //Ahora recibimos su mensaje, que dejamos en buff.
		printf ("Recibido.\n");
		
		peticion[ret] = '\0'; //Tenemos que añadirle la marca de fin de string para que no escriba lo que hay despues en el buffer.
		
		printf ("Se ha conectado: %s\n", peticion); //Escribimos el nombre en la consola.
		
		char *p = strtok(peticion, "/");
		int codigo =  atoi (p);
		
		if (codigo == 0)
		{
			//Desconectar.
			
			printf("Usuario desconectado.\n");
			
			char *usuario[20];
			
			p = strtok(NULL,"/"); 
			strcpy(usuario, p);
			
			//Eliminamos el usuario de la lista de conectados.
			
			pthread_mutex_lock(&mutex);
			EliminarLista(&lista, usuario);
			pthread_mutex_unlock(&mutex);
			
			terminar = 1;
		}
		
		if (codigo == 1)
		{
			//Iniciar sesion.
			
			MYSQL_ROW row;
			MYSQL_RES *resultado;
			char consulta[100];
			
			char *usuario[20];
			char *contra[20];
			
			p = strtok(NULL, "/");
			
			strcpy(usuario,p);
			
			while (p != NULL)
			{
				strcpy(contra, p);
				p = strtok(NULL,"/");
			}
			
			sprintf(consulta, "SELECT usuario FROM jugador WHERE usuario = '%s' AND contrasena = '%s';", usuario, contra);
			
			printf("%s\n",consulta);
			
			int err = mysql_query (conn, consulta);
			
			if (err!=0) {
				
				printf ("Error al consultar la base de datos. %u %s\n", mysql_errno(conn), mysql_error(conn));
				
				exit (1);
			}
			
			resultado = mysql_store_result(conn);
			row = mysql_fetch_row(resultado);
			
			if(row == NULL)
			{
				sprintf(respuesta, "0");
			}
			else{
				sprintf(respuesta, "1");
				printf("%s conectado.", usuario);
			}
			
			pthread_mutex_lock(&mutex);
			AnadirLista(&lista, usuario, sock_conn);
			pthread_mutex_unlock(&mutex);
			
			write (sock_conn, respuesta, strlen(respuesta)); //Enviamos.
		}
		
		if (codigo == 2)
		{
			//Registrarse.
			
			char usuario[20];
			char contra[20];
			char consulta[100];
			
			p = strtok(NULL, "/");
			strcpy(usuario,p);
			
			p = strtok(NULL,"/");
			strcpy(contra, p);
			
			printf("Se va a registrar: %s %s \n", usuario, contra);
			
			sprintf (consulta, "INSERT INTO jugador(usuario, contrasena, ganadas) VALUES('%s', '%s', '%d')", usuario, contra, 0);
			mysql_query (conn, consulta);
		}
		
		if (codigo == 3)
		{
			//Partida mas rapida.
			
			pthread_mutex_lock(&mutex);
			MYSQL_ROW row;
			MYSQL_RES *resultado;
			
			int err = mysql_query (conn, "SELECT duracion FROM partida WHERE duracion = (SELECT MIN(duracion) FROM partida)");
			
			if (err!=0)
			{
				printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			
			resultado = mysql_store_result(conn);
			row = mysql_fetch_row (resultado);
			
			if(row == NULL)
			{
				printf("No se han obtenido los datos de la consulta \n");
			}
			
			else
			{
				printf("El tiempo de la partida mas rapida es %s \n", row[0]);
			}
			pthread_mutex_unlock(&mutex);
			write (sock_conn, row[0], strlen(row[0])); //Enviamos.
		}
		
		if (codigo == 4)
		{
			//Decir ganador de una partida.
			
			int id;
			MYSQL_ROW row;
			MYSQL_RES *resultado;
			
			int p2 = atoi(p = strtok(NULL, "/"));
			id = p2;
			
			char consulta[80];
			
			sprintf (consulta,"SELECT partida.ganador FROM partida WHERE partida.ID = '%d'", id);
			
			int err = mysql_query (conn, consulta);
			
			if (err!=0)
			{
				printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			
			resultado = mysql_store_result(conn);
			row = mysql_fetch_row (resultado);
			
			if(row == NULL)
			{
				printf("No se han obtenido los datos de la consulta \n");
			}
			
			else
			{
				printf("El tiempo de la partida mas rapida es %s \n", row[0]);
			}
			
			write (sock_conn, row[0], strlen(row[0])); //Enviamos.
		}
		
		if (codigo == 5)
		{
			//Quien haya ganado mas partidas.
			
			MYSQL_ROW row;
			MYSQL_RES *resultado;
			
			char usuario[25];
			char consulta[255];
			
			p = strtok(NULL, "/");
			
			strcpy (usuario, p);
			
			// Ya tenemos el nombre
			printf ("Codigo: %d, Usuario: %s\n", codigo, usuario);
			
			sprintf(consulta, "SELECT jugador.ganadas FROM jugador WHERE jugador.usuario = '%s'", usuario);
			
			int err = mysql_query (conn, consulta);
			
			if (err!=0)
			{
				printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			
			resultado = mysql_store_result(conn);
			row = mysql_fetch_row (resultado);
			
			if(row == NULL)
			{
				printf("No se han obtenido los datos de la consulta \n");
			}
			
			else
			{
				printf("El tiempo de la partida mas rapida es %s \n", row[0]);
			}
			
			write (sock_conn, row[0], strlen(row[0])); //Enviamos.
		}
		
		if (codigo == 6)
		{
			//Enviar lista de los jugadores conectados.
			
			char conectados[300];
			
			pthread_mutex_lock(&mutex);
			DameConectados(&lista, conectados);
			pthread_mutex_unlock(&mutex);
			
			write (sock_conn, conectados, strlen(conectados));
		}
	}
	
	//Se desconecta el usuario.
}

int main(int argc, char *argv[])
{
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	
	char peticion[512];
	char respuesta[512];	
	
	lista.num = 0;
	
	//INICIAR SOCKET
	
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0) //Abrimos el socket.
	{
		printf("Error creando el socket.");
	}
	
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0) //Hacemos bind al puerto.
	{
		printf("Error creando el socket.");
	}
	
	memset(&serv_adr, 0, sizeof(serv_adr)); //Inicialitza el zero serv_addr.
	serv_adr.sin_family = AF_INET;
	
	//Asocia el socket a cualquiera de las IP de la maquina. 
	//htonl formatea el numero que recibe al formato necesario.
	
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	
	serv_adr.sin_port = htons(9042); //Escuchamos en el port 9050
	
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
	{
		printf ("Error en el bind.");
	}
	
	if (listen(sock_listen, 2) < 0) //La cola de peticiones pendientes no podra ser superior a 4.
	{
		printf("Error en el Listen.");
	}
	
	pthread_t thread[100];
	
	for(;; i++)
	{
		printf("Escuchando...\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf("Conexion recibida.\n");
		
		//sock_conn es el socket que usaremos para este cliente.
		sockets[i] = sock_conn;
		
		//Crear thread y decir lo que tiene que hacer.
		pthread_create (&thread[i], NULL, AtenderCliente, &sockets[i]);
		
		//Reiniciamos el contador de sockets.
		
		if (i==99)
		{
			i=0;
		}
	}
	
	return 0;
}

