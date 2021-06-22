DROP DATABASE IF EXISTS bd;
CREATE DATABASE bd;
USE bd;
CREATE TABLE jugador (
   usuario VARCHAR(16) PRIMARY KEY NOT NULL,
   contrasena  VARCHAR(16) NOT NULL,
   victorias INTEGER NOT NULL
)ENGINE = InnoDB;

CREATE TABLE partida (
   ID INTEGER PRIMARY KEY  NOT NULL AUTO_INCREMENT,
   ganador TEXT NOT NULL,
   duracion INTEGER NOT NULL
)ENGINE = InnoDB;

CREATE TABLE juego (
   partida INTEGER NOT NULL,
   FOREIGN KEY (partida) REFERENCES partida(ID),
   jugador1 VARCHAR(16) NOT NULL,
   jugador2 VARCHAR(16) NOT NULL,
   FOREIGN KEY (jugador1) REFERENCES jugador(usuario),
   FOREIGN KEY (jugador2) REFERENCES jugador(usuario)   
)ENGINE = InnoDB;

INSERT INTO jugador VALUES('MrCapitan', 'mimara', 1);
INSERT INTO jugador VALUES('Legyonaryus', 'suppgap', 1);
INSERT INTO jugador VALUES('Athax', 'si', 1);

INSERT INTO partida VALUES(1, 'Legyonaryus', 52);
INSERT INTO partida VALUES(2, 'MrCapitan', 29);
INSERT INTO partida VALUES(3, 'Athax', 43);


INSERT INTO juego VALUES(1, 'MrCapitan', 'Legyonaryus');
INSERT INTO juego VALUES(2, 'Legyonaryus', 'Athax');
INSERT INTO juego VALUES(3, 'Athax', 'MrCapitan');