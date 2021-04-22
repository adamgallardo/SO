DROP DATABASE IF EXISTS juego;
CREATE DATABASE juego;
USE juego;
CREATE TABLE jugador (
   usuario VARCHAR(16) PRIMARY KEY NOT NULL,
   contrasena  VARCHAR(16) NOT NULL
)ENGINE = InnoDB;

CREATE TABLE partida (
   ID INTEGER PRIMARY KEY  NOT NULL AUTO_INCREMENT,
   ganador TEXT NOT NULL,
   fecha DATE NOT NULL,
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

INSERT INTO jugador VALUES('MrCapitan', 'mimara');
INSERT INTO jugador VALUES('Legyonaryus', 'suppgap');
INSERT INTO jugador VALUES('Athax', 'si');

INSERT INTO partida VALUES(1, 'Legyonaryus', '2021-03-14', 52);
INSERT INTO partida VALUES(2, 'MrCapitan', '2021-03-16', 29);
INSERT INTO partida VALUES(3, 'Athax', '2021-03-17', 43);


INSERT INTO juego VALUES(1, 'MrCapitan', 'Legyonaryus');
INSERT INTO juego VALUES(2, 'Legyonaryus', 'Athax');
INSERT INTO juego VALUES(3, 'Athax', 'MrCapitan');