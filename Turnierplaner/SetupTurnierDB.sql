--Setup tables
CREATE TABLE Mannschaften
(
    Id              INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    Name            TEXT    NOT NULL UNIQUE,
    Kuerzel         TEXT    NOT NULL UNIQUE,
    Entstehungsjahr DATE,
    Kapitan         INTEGER
);

CREATE TABLE Positionen
(
    Id      INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    Name    TEXT    NOT NULL UNIQUE,
    Kuerzel TEXT    NOT NULL UNIQUE
);

CREATE TABLE Schiedsrichter
(
    Id       INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    Vorname  TEXT    NOT NULL,
    Nachname TEXT    NOT NULL
);

CREATE TABLE Spieler
(
    Id            INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    Vorname       TEXT    NOT NULL,
    Nachname      TEXT    NOT NULL,
    Trikotnummer  INTEGER,
    MannschaftsId INTEGER REFERENCES Mannschaften
);

CREATE TABLE SpieltAuf
(
    SpielerId  INTEGER NOT NULL REFERENCES Spieler,
    PositionId INTEGER NOT NULL REFERENCES Positionen
);

CREATE TABLE Spiel
(
    Id                      INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    Datum                   DATE NOT NULL,
    Spieltag                INTEGER NOT NULL,
    Zuschaueranzahl         INTEGER,
    HeimmannschaftsId       INTEGER NOT NULL REFERENCES Mannschaften,
    AuswaertsmannschaftsId  INTEGER NOT NULL REFERENCES Mannschaften
);

CREATE TABLE Trainer
(
    TrainerID   INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    Vorname     VARCHAR NOT NULL,
    Nachname    VARCHAR NOT NULL,
    Amtsantritt DATE,
    Mannschaft  INTEGER NOT NULL
);

CREATE TABLE Pfeift
(
    SchiedsrichterId INTEGER NOT NULL REFERENCES Schiedsrichter,
    SpielId INTEGER NOT NULL REFERENCES Spiel
);

CREATE TABLE Tor
(
    TorID       INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    Zeitstempel INTEGER,
    Spieler     INTEGER,
    Mannschaft  INTEGER NOT NULL,
    Typ         INTEGER,
    SpielID     INTEGER NOT NULL
);
create table Fairnesstabelle
(
    SpielerId integer not null,
    Karte     varchar not null,
    SpielId   integer not null
);


--Insert default Data
INSERT INTO Positionen (Id, Name, Kuerzel) VALUES (1, 'Torwart', 'TW');
INSERT INTO Positionen (Id, Name, Kuerzel) VALUES (2, 'Rechtsverteidiger', 'RV');
INSERT INTO Positionen (Id, Name, Kuerzel) VALUES (3, 'Innenverteidiger', 'IV');
INSERT INTO Positionen (Id, Name, Kuerzel) VALUES (4, 'Linksverteidiger', 'LV');
INSERT INTO Positionen (Id, Name, Kuerzel) VALUES (5, 'Defensivesmittelfeld', 'CDM');
INSERT INTO Positionen (Id, Name, Kuerzel) VALUES (6, 'Centralesmittelfeld', 'CM');
INSERT INTO Positionen (Id, Name, Kuerzel) VALUES (7, 'Offensivesmittelfeld', 'COM');
INSERT INTO Positionen (Id, Name, Kuerzel) VALUES (8, 'Rechtesmittelfeld', 'RM');
INSERT INTO Positionen (Id, Name, Kuerzel) VALUES (9, 'Linkesmittelfels', 'LM');
INSERT INTO Positionen (Id, Name, Kuerzel) VALUES (10, 'Rechterflügel', 'RF');
INSERT INTO Positionen (Id, Name, Kuerzel) VALUES (11, 'Linkerflügel', 'LF');
INSERT INTO Positionen (Id, Name, Kuerzel) VALUES (12, 'Mittelstürmer', 'MS');
INSERT INTO Positionen (Id, Name, Kuerzel) VALUES (13, 'Stürmer', 'ST');


--Default data
INSERT INTO Mannschaften (Id, Name, Kuerzel, Entstehungsjahr, Kapitan) VALUES (1, 'FC Bayern München', 'FCB', '1900-02-27 00:00:00', 1);
INSERT INTO Mannschaften (Id, Name, Kuerzel, Entstehungsjahr, Kapitan) VALUES (2, 'Borussia Dortmund', 'BVB', '1900-12-19 00:00:00', 6);
INSERT INTO Mannschaften (Id, Name, Kuerzel, Entstehungsjahr, Kapitan) VALUES (3, 'Bayer 04 Leverkusen', 'B04', '1904-07-01 00:00:00', 3);
INSERT INTO Mannschaften (Id, Name, Kuerzel, Entstehungsjahr, Kapitan) VALUES (4, 'RB Leipzig', 'RB', '2009-05-19 00:00:00', 3);
INSERT INTO Mannschaften (Id, Name, Kuerzel, Entstehungsjahr, Kapitan) VALUES (5, 'SC Freiburg', 'SCF', '1904-05-30 00:00:00', 3);

INSERT INTO Schiedsrichter (Vorname, Nachname) VALUES ('Deniz', 'Aytekin');
INSERT INTO Schiedsrichter (Vorname, Nachname) VALUES ('Felix', 'Zwayer');
INSERT INTO Schiedsrichter (Vorname, Nachname) VALUES ('Felix', 'Brych');
INSERT INTO Schiedsrichter (Vorname, Nachname) VALUES ('Bastian', 'Dankert');
INSERT INTO Schiedsrichter (Vorname, Nachname) VALUES ('Marco', 'Fritz');

--Spieler
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (1, 'Thomas', 'Müller', 25, 1);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (2, 'Niklas', 'Süle', 4, 1);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (3, 'Alphonso', 'Davies', 19, 1);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (4, 'Robert', 'Lewandowski', 9, 1);

INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (1,8);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (1,12);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (2,3);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (3,4);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (4,13);

INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (5, 'Gregor', 'Kobel', 1, 2);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (6, 'Mats', 'Hummels', 15, 2);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (7, 'Emre', 'Can', 23, 2);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (8, 'Erling', 'Haaland', 9, 2);

INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (5,1);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (6,3);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (7,3);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (7,5);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (7,2);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (8,13);

INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (9, 'Lukas', 'Hradecky', 1, 3);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (10, 'Jeremie', 'Frimpong', 30, 3);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (11, 'Karim', 'Bellarabi', 38, 3);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (12, 'Iker', 'Bravo', 28, 3);

INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (9,1);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (10,2);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (10,8);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (11,8);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (12,13);

INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (13, 'Peter', 'Gulaci', 1, 4);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (14, 'Lukas', 'Klostermann', 16, 4);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (15, 'Dani', 'Olmo', 25, 4);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (16, 'Kevin', 'Kampl', 44, 4);

INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (13,1);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (10,2);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (10,3);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (15,7);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (15,9);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (16,6);

INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (17, 'Mark', 'Flecken', 1, 5);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (18, 'Nico', 'Schlotterbeck', 31, 5);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (19, 'Roland', 'Sallai', 22, 5);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (20, 'Christian', 'Günter', 30, 5);

INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (17,1);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (18,3);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (19,8);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (19,10);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (20,4);

--Trainer
INSERT INTO Trainer(TrainerID, Vorname, Nachname, Amtsantritt, Mannschaft) VALUES (1, 'Julian', 'Nagelsmann', '2014-01-02 00:00:00',1);
INSERT INTO Trainer(TrainerID, Vorname, Nachname, Amtsantritt, Mannschaft) VALUES (2, 'Marco', 'Rose', '2021-5-23 00:00:00',2);
INSERT INTO Trainer(TrainerID, Vorname, Nachname, Amtsantritt, Mannschaft) VALUES (3, 'Geardo', 'Seoane', '2022-01-30 00:00:00',3);
INSERT INTO Trainer(TrainerID, Vorname, Nachname, Amtsantritt, Mannschaft) VALUES (4, 'Domenico', 'Tedesko', '2022-01-30 00:00:00',4);
INSERT INTO Trainer(TrainerID, Vorname, Nachname, Amtsantritt, Mannschaft) VALUES (5, 'Christian', 'Streich', '2010-02-15 00:00:00',5);


--Spiele
INSERT INTO Spiel VALUES (1, '2022-04-20 00:00:00', 1, 50000, 1, 4);
INSERT INTO Spiel VALUES (2, '2022-04-20 00:00:00', 1, 76000, 2, 5);
INSERT INTO Spiel VALUES (3, '2022-04-27 00:00:00', 2, 20000, 1, 5);
INSERT INTO Spiel VALUES (4, '2022-04-27 00:00:00', 2, 45000, 2, 3);
INSERT INTO Spiel VALUES (5, '2022-05-04 00:00:00', 3, 50032, 5, 3);
INSERT INTO Spiel VALUES (6, '2022-05-04 00:00:00', 3, 80000, 4, 2);
INSERT INTO Spiel VALUES (7, '2022-05-11 00:00:00', 4, 57435, 1, 3);
INSERT INTO Spiel VALUES (8, '2022-05-11 00:00:00', 4, 55040, 5, 4);
INSERT INTO Spiel VALUES (9, '2022-05-18 00:00:00', 5, 37120, 1, 2);
INSERT INTO Spiel VALUES (10, '2022-05-18 00:00:00', 5, 56790, 3, 4);

--Pfeift
INSERT INTO Pfeift VALUES(4, 1);
INSERT INTO Pfeift VALUES(1, 2);
INSERT INTO Pfeift VALUES(2, 3);
INSERT INTO Pfeift VALUES(3, 4);
INSERT INTO Pfeift VALUES(5, 5);
INSERT INTO Pfeift VALUES(4, 6);
INSERT INTO Pfeift VALUES(1, 7);
INSERT INTO Pfeift VALUES(2, 8);
INSERT INTO Pfeift VALUES(2, 9);
INSERT INTO Pfeift VALUES(5, 10);

--Tore
INSERT INTO Tor VALUES(1, 32, 1, 1, 1, 1);
INSERT INTO Tor VALUES(11, 2, 1, 1, 3, 1);
INSERT INTO Tor VALUES(12, 2, 1, 1, 3, 1);
INSERT INTO Tor VALUES(13, 2, 1, 1, 2, 1);
INSERT INTO Tor VALUES(3, 65, 1, 1, 2, 1);
INSERT INTO Tor VALUES(4, 90, 1, 1, 4, 1);
INSERT INTO Tor VALUES(5, 87, 6, 2, 2, 2);
INSERT INTO Tor VALUES(6, 45, 10, 3, 2, 10);
INSERT INTO Tor VALUES(7, 50, 6, 2, 1, 2);
INSERT INTO Tor VALUES(8, 5, 6, 2, 1, 6);
INSERT INTO Tor VALUES(9, 10, 3, 1, 1, 9);
INSERT INTO Tor VALUES(10, 22, 3, 1, 4, 9);
INSERT INTO Tor VALUES(15, 22, 3, 1, 1, 9);

--Karten
INSERT INTO Fairnesstabelle VALUES (1, 'Gelbe Karte',1);
INSERT INTO Fairnesstabelle VALUES (1, 'Gelbe Karte',1);
INSERT INTO Fairnesstabelle VALUES (20, 'Gelbe Karte',8);
INSERT INTO Fairnesstabelle VALUES (3, 'Gelbe Karte',1);
INSERT INTO Fairnesstabelle VALUES (10, 'Gelbe Karte',4);
INSERT INTO Fairnesstabelle VALUES (10, 'Rote Karte',4);
INSERT INTO Fairnesstabelle VALUES (20, 'Rote Karte',4);
INSERT INTO Fairnesstabelle VALUES (19, 'Rote Karte',8);
INSERT INTO Fairnesstabelle VALUES (19, 'Rote Karte',8);
INSERT INTO Fairnesstabelle VALUES (19, 'Rote Karte',8);
