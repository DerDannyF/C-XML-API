[__Zurück zu CSharp-XML-API__](https://github.com/DerDannyF/CSharp-XML-API)




omdbapi.com ist ein freier Web Service der folgenden String (für Asterix - Sieg über Cäsar) zurückliefert:
![Screenshot](https://images-na.ssl-images-amazon.com/images/M/MV5BMTI5NDYzNjc3MV5BMl5BanBnXkFtZTcwNTM1NzUyMQ@@._V1_SX300.jpg)
`
{"Title":"Asterix and Caesar","Year":"1985","Rated":"N/A","Released":"11 Dec 1985","Runtime":"79 min","Genre":"Animation, Adventure, Comedy","Director":"Gaëtan Brizzi, Paul Brizzi","Writer":"René Goscinny (comic), Albert Uderzo (comic), Pierre Tchernia (adaptation)","Actors":"Roger Carel, Pierre Tornade, Pierre Mondy, Serge Sauvion","Plot":"Obelix falls for a new arrival in his home village in Gaul, but is heartbroken when her true love arrives to visit her. However, the lovers are kidnapped by Romans; Asterix and Obelix set ...","Language":"French","Country":"France","Awards":"1 win.","Poster":"https://images-na.ssl-images-amazon.com/images/M/MV5BMTI5NDYzNjc3MV5BMl5BanBnXkFtZTcwNTM1NzUyMQ@@._V1_SX300.jpg","Ratings":[{"Source":"Internet Movie Database","Value":"6.8/10"}],"Metascore":"N/A","imdbRating":"6.8","imdbVotes":"6,495","imdbID":"tt0088748","Type":"movie","DVD":"N/A","BoxOffice":"N/A","Production":"N/A","Website":"N/A","Response":"True"}
`


Diesen String lese ich ein und speichere die Jahreszahl `"Year":"1985"` in eine Liste ab in der ich auch die Jahreszahl aus der XML Datei database_programmes.xml abspeichere. 

Am Ende vergleiche ich beide Zahlen und Abweichungen werden ausgegeben.


![Screenshot](http://up.picr.de/29045475iz.png)

__Linux__
- Compilieren mit `mcs main.cs`
- Ausführen mit `./main.exe`
