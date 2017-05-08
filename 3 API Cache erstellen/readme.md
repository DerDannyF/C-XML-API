[__Zurück zu CSharp-XML-API__](https://github.com/DerDannyF/CSharp-XML-API)

Dieses Programm hilft uns, indem es einen Cache anlegt welches das arbeiten mit der ofdb API erträglich gestaltet.
Beinhaltet 4 Server die bei TimeOut ständig wechseln. 

Es wird eine Xml Datein erstellt: **cache.xml** (Beinhaltet 3 mögliche Fehler die die API Rückgabe für uns nicht verwertbar machen) 
- Kein Resultat(1)
- Titel(2) 
- Beschreibung(3) fehlt 

Alle IDs in der cache.Xml werden bei erneuten Durchlauf ignoriert.



Diese 4 Dinge helfen uns die Lücken in unserer Db zu schließen:

/*Variabeln*/ int GesamtMenge = 150000, Teiler = 1000;
/*Variabeln*/ int GiveID=1; 
sowie: die Abfrage if (j >= 15000) j= 1;

- Gesamtmenge: das sind die Durchläufe die das Programm macht. 
- Teiler: nach diesen Durchläufen zeigt das Programm die übrige Anzahl, die Position und die Zeit an
- GiveID: das ist die StartID, wenn man Bereiche scannen will, die nicht bei 0 beginnen
- Abfrage: in dem Fall wird an der Positon 15000 wieder bei 1 begonnen, um TimeOut Fehler zu beheben.


Die 4 Server die ständig wechseln: 

- http://ofdbgw.geeksphere.de/movie/, 
- http://ofdbgw.metawave.ch/movie/,
- http://ofdbgw.h1915283.stratoserver.net/movie/ und 
- http://ofdbgw.johann-scharl.de/movie/


[__Zurück zu CSharp-XML-API__](https://github.com/DerDannyF/CSharp-XML-API)
