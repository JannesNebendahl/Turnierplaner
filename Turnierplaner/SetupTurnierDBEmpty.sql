--Setup tables
CREATE TABLE Mannschaften
(
    Id              INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    Name            TEXT    NOT NULL UNIQUE,
    Kuerzel         TEXT    NOT NULL UNIQUE,
    Entstehungsjahr DATE,
    Kapitan         INTEGER,
    Punktestand     INTEGER
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
    AuswaertsmannschaftsId  INTEGER NOT NULL REFERENCES Mannschaften,
    ErgebnisEingetragen     INTEGER NOT NULL
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
