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
    Datum                   DATE,
    Spieltag                INTEGER NOT NULL,
    Zuschauerzahl           INTEGER,
    HeimmannschaftsId       INTEGER NOT NULL REFERENCES Mannschaften,
    AuswaertsmannschaftsId  INTEGER NOT NULL REFERENCES Mannschaften
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

--Default data of Jannes
INSERT INTO Mannschaften (Id, Name, Kuerzel, Entstehungsjahr, Kapitan) VALUES (1, 'FC Bayern München', 'FCB', '1900-02-27 00:00:00', 2);
INSERT INTO Mannschaften (Id, Name, Kuerzel, Entstehungsjahr, Kapitan) VALUES (2, 'Borussia Dortmund', 'BVB', '1900-12-19 00:00:00', 3);
INSERT INTO Mannschaften (Id, Name, Kuerzel, Entstehungsjahr, Kapitan) VALUES (3, 'Hamburger SV', 'HSV', '1887-09-29 00:00:00', 3);

INSERT INTO Schiedsrichter (Vorname, Nachname) VALUES ('Felix', 'Zwayer');

INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (1, 'Thomas', 'Müller', 25, 1);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (2, 'Manuel', 'Neuer', 1, 1);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer, MannschaftsId) VALUES (3, 'Marko', 'Reus', 11, 2);
INSERT INTO Spieler (Id, Vorname, Nachname, Trikotnummer) VALUES (4, 'Max', 'Kruse', 9);

INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (1,8);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (1,12);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (2,1);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (3,7);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (3,9);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (3,11);
INSERT INTO SpieltAuf (SpielerId, PositionId) VALUES (4,12);