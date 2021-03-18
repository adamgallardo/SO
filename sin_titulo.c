//Devuelve la ID de las 5 partidas más rápidas
#include <mysql.h>
#include <string.h>
void main ()
{
	MYSQL *conn;
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	conn = mysql_init(NULL);
	if (conn==NULL)
	{
		printf ("Error al crear la conexión: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	conn = mysql_real_connect (conn, "localhost","root", "mysql",
							   "juego",0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexión: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	//Buscamos el primer tiempo mas corto
	
	err=mysql_query (conn, "SELECT duracion FROM partida WHERE duracion = (SELECT MIN(duracion) FROM partida)");
	if (err!=0)
	{
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	resultado=mysql_store_result(conn);
	row = mysql_fetch_row (resultado);
	printf("El tiempo de la partida mas rapida es %s \n",row[0]);
	mysql_close (conn);
}

