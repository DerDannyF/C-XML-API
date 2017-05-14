// Aus dem Cache Filme lesen und in die database_programmes.xml einfügen schauen ob die imdbID schon vorhanden.
// + omdbapi.com Aufruf
using System;
using System.Collections.Generic;
using System.Linq;

namespace FgotoXML
{
	class Program
	{
		static void Main(string[] args)
		{
			
			string FilmDatei = "database_programmes.xml";
			string myDB = System.IO.File.ReadAllText(FilmDatei);

			List<FilmDaten> _FILM = new List<FilmDaten>(); // FilmDaten
		
			int korrekt = 0;
			for (int i = 0; i < 309000; i++) // den Cache durchgehen
			{
			//	Console.WriteLine (i);
				if (System.IO.File.Exists ("cache/" + i + ".xml") && (System.IO.File.Exists ("cache2/" + i + ".xml")) && (System.IO.File.Exists (FilmDatei) )) 
				{ 
					korrekt = leseDaten(_FILM, i);
					if (korrekt == 1)
						schreibeXml (FilmDatei, _FILM, i);
					else {
						Console.ForegroundColor = ConsoleColor.DarkGray;
						Console.Write ("Datensatz ");
						Console.ForegroundColor = ConsoleColor.Red;
						Console.Write (""+i+" Nicht");
						Console.ForegroundColor = ConsoleColor.DarkGray;
						Console.WriteLine (" zu verwerten");

					}
					_FILM.Clear();
				}
			}

			// Endausgabe
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine ("Filme vorher: {0} ",CountStrings (myDB, "product=\"1\""));
			myDB = System.IO.File.ReadAllText(FilmDatei);
			Console.WriteLine ("Filme nachher: {0} ",CountStrings (myDB, "product=\"1\""));
		}

		private static int leseDaten(List<FilmDaten> _daten, int i)
		{
			// My_ Variablen
			string myTitle="", myIMDb_ID="", myDesc="",myGenre="", myMainGenre="", mySubGenre="", myLand="";
			int myJahr=0, myRuntime=0, myFlags=0;
			// Celebrities
			string RVorname="", RNachname="";
			float myofdbRating, myomdbRating;
			int scout = 0, scout2 = 0;
			int firstScout, dazwischen, abschneiden; string htmlsub = "";
			string html = System.IO.File.ReadAllText("cache/" + i + ".xml");
			string OMDbhtml = System.IO.File.ReadAllText("cache2/"+i+".xml");

			// (B) Liste füllen

			// [1] Titel lesen
			firstScout = html.IndexOf ("<titel>") + 7;
			dazwischen = html.IndexOf ("</titel>") - html.IndexOf ("<titel>") -7;
			myTitle = html.Substring (firstScout, dazwischen);
			/* Titel Probleme angehen:
			 * 13.05.17
			* Problem 1: [TV Serie] [Serial] [TV-Mini-Serie] im Titel
			* Problem 2: [Kurzfilm] im Titel
			* Problem 3: Alles mit ,Der ,Die...
			*/ 
			// Problem 1: 
			if (myTitle.Contains ("[TV Serie]")) myTitle = "";
			if (myTitle.Contains ("[TV-Serie]")) myTitle = "";
			if (myTitle.Contains ("[Serial]")) myTitle = "";
			if (myTitle.Contains ("[TV-Mini-Serie]")) myTitle = "";
			
			// 14.05.17
			// Problem 2: einfach ignorieren! (wir haben ehh zu viel Filme)
			if (myTitle.Contains ("[Kurzfilm]")) myTitle = "";

			// Problem 3: 
				if (myTitle.EndsWith(", Der")) myTitle = Vorholen (myTitle, ", Der");	
				if (myTitle.EndsWith(", Die")) myTitle = Vorholen (myTitle, ", Die");	
				if (myTitle.EndsWith(", Das")) myTitle = Vorholen (myTitle, ", Das");	
				if (myTitle.EndsWith(", The")) myTitle = Vorholen (myTitle, ", The");	
				if (myTitle.EndsWith(", Eine")) myTitle = Vorholen (myTitle, ", Eine");	
				if (myTitle.EndsWith(", Des")) myTitle = Vorholen (myTitle, ", Des");	
				if (myTitle.EndsWith(", Ein")) myTitle = Vorholen (myTitle, ", Ein");	
			// [E] Titel Problem angehen


			Console.ForegroundColor = ConsoleColor.White;

			Console.WriteLine (myTitle);


			// [2] Jahr lesen
			firstScout = html.IndexOf ("<jahr>") + 6;
			dazwischen = html.IndexOf ("</jahr>") - html.IndexOf ("<jahr>") -6;
			myJahr = Convert.ToInt32(html.Substring (firstScout, dazwischen));
			// [3] IMDbID lesen
			firstScout = html.IndexOf ("<imdbid>") + 8;
			dazwischen = html.IndexOf ("</imdbid>") - html.IndexOf ("<imdbid>") -8;
			myIMDb_ID = "tt"+html.Substring (firstScout, dazwischen);
			// [4] Beschreibung lesen
			firstScout = html.IndexOf ("<beschreibung>") + 14;
			dazwischen = html.IndexOf ("</beschreibung>") - html.IndexOf ("<beschreibung>") -14;
			if (dazwischen<0) Console.WriteLine("J4");
			myDesc = html.Substring (firstScout, dazwischen);
			/*1000 Sterne leuchten raus (Xml nicht valide) 
			*/ if (myIMDb_ID == "tt0053336")	myDesc = "";
			/* Date: 14.05.17
			 * Schrotttext [Italo-Cinema.de] / Filmdienst / Pressetext / Quellenangabe uvm.
			*/ string[] Beschreibungsprobleme = 
			{"Quelle: WDR ","Quelle: WDR", "Quelle: ARD", "Quelle: SWR", "Quelle: MDR", "Quelle: RBB", "Quelle: EinsFestival",
			"Quelle: Premiere", "Quelle: arte","Quelle: HR"," kino.de"," kino.de ", " leisurefoxx.de ", 
			" wicked-vision.com", " dvdmagazin.de", " Prisma-Online.de", " cinefacts.de"," prisma.de", " digitaldvd.de",
			"[Italo-Cinema.de]", " DVD-Forum.at", " dvd-palace.de", "Quelle: filmstarts.de", " DasErste.de", " moviewiki.org",
			" www.movieplot.de"," www.movieplot.de","Quelle: ILLUSIONS UNLTD. films","Quelle: http://filme.disney.de/baymax/story",
			"Quelle: Covertext", "Quelle:  www.movieplot.de", " TV-Spielfilm", "Quelle: Abschrift", " dtm.at", " VMP",
			" TV-Media.at", "ttool", "Quelle:", " Covertext VMP", " Covertext", "(Covertext)", " eigenen Text einstellen) ",
			" Pressetext", " Filmdienst", "(Covertext der deutschen VHS):", " Capitol Videoean ", " Galileo Medien AG",
			" Loyal Video", " TV Movie", " Jacob GmbH", " Frank Trebbin" };
			foreach (string elem in Beschreibungsprobleme) { myDesc = WegDamit (myDesc, elem); }
			
			// [5] Genre lesen
			firstScout = html.IndexOf ("<genre>") + 7;
			dazwischen = html.IndexOf ("</genre>") - html.IndexOf ("<genre>") -7;
			myGenre = html.Substring (firstScout, dazwischen);
			// Anzahl Genres rausfinden
			int c = 0;  string[] sepGenre = { "<titel>" };
			c = myGenre.Split(sepGenre, StringSplitOptions.None).Count() - 1;
			// Genre Anzahl > 0 = MainGenre
			myMainGenre = "";
			if (c>0)
			{
				dazwischen = myGenre.IndexOf ("</titel>")-8;
				myMainGenre = myGenre.Substring(8, dazwischen);
				// Genre Anzahl > 1 = SubGenre
				mySubGenre = "";
				if (c>1)
				{
					abschneiden = myGenre.IndexOf ("</titel>")+8;
					myGenre = myGenre.Substring(abschneiden,myGenre.Length-abschneiden);
					firstScout = myGenre.IndexOf ("<titel>")+7;
					dazwischen = myGenre.IndexOf ("</titel>") -8;
					mySubGenre = myGenre.Substring(firstScout,dazwischen);
				}
				/*GenreKürzel*/			
				if (myMainGenre == "Abenteuer") myMainGenre="1";
				if (myMainGenre == "Action") myMainGenre="2";
				if (myMainGenre == "Animation") myMainGenre="3";
				if (myMainGenre == "Krimi") myMainGenre="4";
				if (myMainGenre == "Komödie") myMainGenre="5";
				if (myMainGenre == "Dokumentation") myMainGenre="6";
				if (myMainGenre == "Biographie") myMainGenre="6";
				if (myMainGenre == "Drama") myMainGenre="7";
				if (myMainGenre == "Katastrophen") myMainGenre="7";
				if (myMainGenre == "Erotik") myMainGenre="8";
				if (myMainGenre == "Sex") myMainGenre="8";
				if (myMainGenre == "Kinder-/Familienfilm") myMainGenre="9";
				if (myMainGenre == "Fantasy") myMainGenre="10";
				if (myMainGenre == "History") myMainGenre="11";
				if (myMainGenre == "Horror") myMainGenre="12";						
				if (myMainGenre == "Splatter") myMainGenre="12";						
				if (myMainGenre == "Monumental") myMainGenre="13";
				if (myMainGenre == "Mystery") myMainGenre="14";
				if (myMainGenre == "Liebe/Romantik") myMainGenre="15";
				if (myMainGenre == "Science-Fiction") myMainGenre="16";
				if (myMainGenre == "Thriller") myMainGenre="17";
				if (myMainGenre == "Western") myMainGenre="18";

				if (mySubGenre == "Abenteuer") mySubGenre="1";
				if (mySubGenre == "Action") mySubGenre="2";
				if (mySubGenre == "Animation") mySubGenre="3";
				if (mySubGenre == "Krimi") mySubGenre="4";
				if (mySubGenre == "Komödie") mySubGenre="5";
				if (mySubGenre == "Dokumentation") mySubGenre="6";
				if (mySubGenre == "Biographie") mySubGenre="6";
				if (mySubGenre == "Drama") mySubGenre="7";
				if (mySubGenre == "Katastrophen") mySubGenre="7";
				if (mySubGenre == "Erotik") mySubGenre="8";
				if (mySubGenre == "Sex") mySubGenre="8";
				if (mySubGenre == "Kinder-/Familienfilm") mySubGenre="9";
				if (mySubGenre == "Fantasy") mySubGenre="10";
				if (mySubGenre == "History") mySubGenre="11";
				if (mySubGenre == "Horror") mySubGenre="12";						
				if (mySubGenre == "Splatter") mySubGenre="12";						
				if (mySubGenre == "Monumental") mySubGenre="13";
				if (mySubGenre == "Mystery") mySubGenre="14";
				if (mySubGenre == "Liebe/Romantik") mySubGenre="15";
				if (mySubGenre == "Science-Fiction") mySubGenre="16";
				if (mySubGenre == "Thriller") mySubGenre="17";
				if (mySubGenre == "Western") mySubGenre="18";

				if (myMainGenre.Length > 2) myMainGenre = "";
				if (mySubGenre.Length > 2) mySubGenre = "";
				if ((myMainGenre == "") && (mySubGenre != ""))
				{
					myMainGenre = mySubGenre;
					mySubGenre = "";
				}
				if (myMainGenre == mySubGenre) mySubGenre = "";
			}
			// [6] Land lesen
			firstScout = html.IndexOf ("<produktionsland>") + 17;
			dazwischen = html.IndexOf ("</produktionsland>") - html.IndexOf ("<produktionsland>") ;
			myLand = html.Substring (firstScout + 7, dazwischen -32);
			// Anzahl der Produktionsländer rausfinden
			string myNewLand="", testLand= myLand;
			/*Länderkürzel ISO3166 1-3*/		
			if (myLand.Contains("Deutschland")) myNewLand = myNewLand+"DE";
			if (myLand.Contains("Italien")) myNewLand = myNewLand+"IT";
			if (myLand.Contains("Hongkong")) myNewLand = myNewLand+"HK";
			if (myLand.Contains("China")) myNewLand = myNewLand+"CN";
			if (myLand.Contains("Frankreich")) myNewLand = myNewLand+"FR";
			if (myLand.Contains("Großbritannien")) myNewLand = myNewLand+"GB";
			if (myLand.Contains("Schweden")) myNewLand = myNewLand+"SE";
			if (myLand.Contains("Neuseeland")) myNewLand = myNewLand+"NZ";
			if (myLand.Contains("Kanada")) myNewLand = myNewLand+"CA";
			if (myLand.Contains("Niederlande")) myNewLand = myNewLand+"NL";
			if (myLand.Contains("Dänemark")) myNewLand = myNewLand+"DK";
			if (myLand.Contains("Spanien")) myNewLand = myNewLand+"ES";
			if (myLand.Contains("Japan")) myNewLand = myNewLand+"JP";
			if (myLand.Contains("Schweiz")) myNewLand = myNewLand+"CH";
			if (myLand.Contains("Österreich")) myNewLand = myNewLand+"AT";
			if (myLand.Contains("Australien")) myNewLand = myNewLand+"AU";
			if (myLand.Contains("Südkorea")) myNewLand = myNewLand+"KR";
			if (myLand.Contains("Panama")) myNewLand = myNewLand+"PA";
			if (myLand.Contains("USA")) myNewLand = myNewLand+"US";
			if (myLand.Contains("Griechenland")) myNewLand = myNewLand+"GR";
			if (myLand.Contains("Norwegen")) myNewLand = myNewLand+"NO";
			if (myLand.Contains("Taiwan")) myNewLand = myNewLand+"TW";
			if (myLand.Contains("Belgien")) myNewLand = myNewLand+"BE";
			if (myLand.Contains("Südafrika")) myNewLand = myNewLand+"ZA";
			if (myLand.Contains("Rumänien")) myNewLand = myNewLand+"RO";
			if (myLand.Contains("Finnland")) myNewLand = myNewLand+"FI";
			if (myLand.Contains("Mexiko")) myNewLand = myNewLand+"MX";
			myLand = "nope";
			if (myNewLand.Length==2) myLand = myNewLand;
			if (myNewLand.Length>=3) myLand = myNewLand.Insert(2, "/");
			if (myNewLand.Length>=6) {myLand = myNewLand.Insert(2, "/"); myLand = myLand.Substring(0,5);}
			if (myLand == "no") Console.WriteLine(myLand+":"+testLand);
			// [7] Rating lesen
			firstScout = html.IndexOf ("<note>") + 6;
			dazwischen = html.IndexOf ("</note>") - html.IndexOf ("<note>") -6 ;
			myofdbRating = Convert.ToSingle(html.Substring (firstScout, dazwischen))/100;
			if (myofdbRating == 0)
				myofdbRating = 5;
			// (B) OMDb Abfrage
			if (OMDbhtml.Contains ("Incorrect IMDb ID.")) 
			{
				Console.WriteLine ("Falsche IMDb ID");
			}
			else // Inhalt vorhanden
			{	
				// [8] Runtime lesen
				if (OMDbhtml.Contains("\"Runtime\":\"N/A\""))
				{ myRuntime = 0; }
				else
				{							
					firstScout = OMDbhtml.IndexOf ("Runtime")+10;
					dazwischen = OMDbhtml.IndexOf ("\",\"Genre")-OMDbhtml.IndexOf ("Runtime\":\"")-13;
					if (dazwischen<=0) {dazwischen=4;}
					// schräges Format ausgleichen
					//"Runtime":"1 h 30 min" Normalangabe: "Runtime": "90min"
					string Hilfsvariable = Convert.ToString(OMDbhtml.Substring (firstScout, dazwischen));
					if (Hilfsvariable.Contains("h\""))
					{	if (Hilfsvariable.Contains ("4 h\"")) {	myRuntime = 240; }
						if (Hilfsvariable.Contains ("3 h\"")) {	myRuntime = 180; }
						if (Hilfsvariable.Contains ("2 h\"")) {	myRuntime = 120; }
						if (Hilfsvariable.Contains ("1 h\"")) {	myRuntime = 60; }
					}
					else if (Hilfsvariable.Contains ("1 h ")) {
							firstScout = Hilfsvariable.IndexOf ("1 h");
							Hilfsvariable = Hilfsvariable.Substring (firstScout + 4, 2);
							myRuntime = Convert.ToInt16 (Hilfsvariable) + 60;
					}
					else {	myRuntime = Convert.ToInt16(OMDbhtml.Substring (firstScout, dazwischen));	}

				}
			}
			// [9] Rating lesen
			if (OMDbhtml.Contains("\"imdbRating\":\"N/A\""))
			{ myomdbRating = 0; }
			else
			{					
				firstScout = OMDbhtml.IndexOf ("imdbRating")+13;
				dazwischen = OMDbhtml.IndexOf ("imdbVotes")-OMDbhtml.IndexOf ("imdbRating")-16;
				myomdbRating = Convert.ToSingle(OMDbhtml.Substring (firstScout, dazwischen))/10;
			}

			// [10] Flags
			if (OMDbhtml.Contains ("\"Rated\":\"R\""))
				myFlags = 64;
			else
				myFlags = 0;
			// Edit 13.05.17 Doku = Kultur
			if (myMainGenre == "6")
				myFlags = 4;

			// (E) OMDb Abfrage

			// [B] Celebrities
			if (html.Contains ("</name>\n</person>\n</regie>")) {	// Regie	
				scout = html.IndexOf ("</name>\n</person>\n</regie>");
				htmlsub = html.Substring (scout - 40, 40);
				scout = htmlsub.IndexOf ("<name>");
				htmlsub = htmlsub.Substring (scout + 6, htmlsub.Length - 6 - scout);
				if (htmlsub.Contains (" ")) {
					scout = htmlsub.IndexOf (" ");
					RVorname = htmlsub.Substring (0, scout);
					RNachname = htmlsub.Substring (1 + scout, htmlsub.Length - (scout + 1));
				}
			}
				// VARIABELN
				int AnzahlDarsteller=1; string htmlsubsub="", name="";
				string D1VN="",D1NN="",D2VN="",D2NN="",D3VN="",D3NN="",D4VN="",D4NN="",D5VN="",D5NN="";


				if (html.Contains ("<besetzung>\n<person>\n<id>")) 
				{ // Darsteller (max5)
					scout = html.IndexOf ("<besetzung>\n<person>\n<id>");
					scout2 = html.IndexOf ("</besetzung>");
					htmlsub = html.Substring (scout, scout2 - scout);
					scout = htmlsub.IndexOf ("<name>");
					htmlsubsub = htmlsub.Substring (scout, htmlsub.Length - scout);
					while ((AnzahlDarsteller < CountStrings (htmlsub, "<name>")) && (AnzahlDarsteller <= 5)) 
					{
						scout = htmlsubsub.IndexOf ("<name>");
						scout2 = htmlsubsub.IndexOf ("</name>");
						name = htmlsubsub.Substring (scout + 6, scout2 - scout - 6);
						htmlsubsub = htmlsubsub.Substring (scout2 + 6, htmlsubsub.Length - (scout2 + 6));			
						if (name.Contains (" ")) {
							scout = name.IndexOf (" ");
							if (AnzahlDarsteller == 1) {
								D1VN = name.Substring (0, scout);
								D1NN = name.Substring (1 + scout, name.Length - (scout + 1));
							}
							if (AnzahlDarsteller == 2) {
								D2VN = name.Substring (0, scout);
								D2NN = name.Substring (1 + scout, name.Length - (scout + 1));
							}
							if (AnzahlDarsteller == 3) {
								D3VN = name.Substring (0, scout);
								D3NN = name.Substring (1 + scout, name.Length - (scout + 1));
							}
							if (AnzahlDarsteller == 4) {
								D4VN = name.Substring (0, scout);
								D4NN = name.Substring (1 + scout, name.Length - (scout + 1));
							}
							if (AnzahlDarsteller == 5) {
								D5VN = name.Substring (0, scout);
								D5NN = name.Substring (1 + scout, name.Length - (scout + 1));
							}
						}
					AnzahlDarsteller++;
					}
					
				}
			// [E] Celebrities
			if ((myRuntime != 0) && (myDesc != "") && (myLand != "nope") && (myMainGenre != "") && (myTitle!="")) {
				_daten.Add (new FilmDaten () {
					// Ordne Daten zu
					ofdbTitle = myTitle,
					ofdbYear = myJahr,
					ofdbIMDb_ID = myIMDb_ID,
					ofdbDesc = myDesc,
					ofdbMainGenre = myMainGenre,
					ofdbSubGenre = mySubGenre,
					ofdbCountry = myLand,
					ofdbRating = myofdbRating,
					omdbRuntime = (myRuntime / 60) + 1,
					omdbRating = myomdbRating,
					omdbFlags = myFlags,
					// Celebrities
						RegisseurVName = RVorname,
						RegisseurNName = RNachname,
						Darsteller1VName = D1VN,
						Darsteller2VName = D2VN,
						Darsteller3VName = D3VN,
						Darsteller4VName = D4VN,
						Darsteller5VName = D5VN,

						Darsteller1NName = D1NN,
						Darsteller2NName = D2NN,
						Darsteller3NName = D3NN,
						Darsteller4NName = D4NN,
						Darsteller5NName = D5NN});
				return(1);
			} else
				return(0);
		}

		private static void schreibeXml (string schreibeDatei, List<FilmDaten> _daten, int DatenSatzNummer)
		{
			string inhaltNeu=""; int myPosition=0;
			string inhalt = System.IO.File.ReadAllText(schreibeDatei);
			if (!inhalt.Contains ("imdb_id=\"" + _daten[0].ofdbIMDb_ID + "\"")) {
				// Film noch nicht vorhanden:
			
				// [B] *****************CSO (in Arbeit)
				int cr=0, sp=0, ou=0;
				Random zufall = new Random(); 
				cr = Convert.ToInt32( (_daten [0].ofdbRating + _daten [0].omdbRating) / 2 * 10);
				sp = cr; ou = cr;

				if ((sp <= 0) || (sp >= 100)) { // Irgendwas ist schief gelaufen
					sp = 50;
				}
				if ((sp <= 25) && (sp >= 0)) { // furchtbar schlechter Film
					cr = cr + Convert.ToInt32 (zufall.Next (0, 10));
					sp = sp + Convert.ToInt32 (zufall.Next (0, (_daten[0].ofdbYear-1900)/5));
					ou = +10 + ou + Convert.ToInt32 (zufall.Next (-10, 35));
				}
				if ((sp <= 50) && (sp >=25)) { // etwas besserer Film
					cr = cr + Convert.ToInt32 (zufall.Next (-10, 20));
					sp = sp + Convert.ToInt32 (zufall.Next (0, (_daten[0].ofdbYear-1900)/5));
					ou = -10 + ou + Convert.ToInt32 (zufall.Next (-10, 35));
				}
				if ((sp <= 75) && (sp >= 50)) { // sehr gute Kritiken
					cr = cr + Convert.ToInt32 (zufall.Next (-10, 10));
					sp = sp + Convert.ToInt32 (zufall.Next (-10, (_daten[0].ofdbYear-1900)/10));
					ou = -30 + ou + Convert.ToInt32 (zufall.Next (-10, 50));
				}
				if ((sp <= 90) && (sp >= 75)) { // Top Film
					cr = cr + Convert.ToInt32 (zufall.Next (-10, 10));
					sp = sp + Convert.ToInt32 (zufall.Next (-10, (_daten[0].ofdbYear-1900)/10));
					ou = -30 + ou + Convert.ToInt32 (zufall.Next (-10, 40));
				}
				if ((sp <= 100) && (sp >= 95)) { // Super Top Film
					cr = cr -10 + Convert.ToInt32 (zufall.Next (-10, 10));
					sp = sp -20 + Convert.ToInt32 (zufall.Next (-10, (_daten[0].ofdbYear-1900)/10));
					ou = ou -10 + Convert.ToInt32 (zufall.Next (-10, 10));
				}
				// [E] *****************CSO

				// schreibe programme-Container
				string neuerFilm = "\n\n<programme id=\"automatic-ID:" + DatenSatzNummer + "\" product=\"1\" imdb_id=\"" + _daten [0].ofdbIMDb_ID + "\"  creator=\"automatic v0.1\">\n" +
				                   "<title>\n" +
				                   "<de>" + _daten [0].ofdbTitle + "</de>\n" +
				                   "</title>\n" +
				                   "<description>\n" +
				                   "<de>" + _daten [0].ofdbDesc + "</de>\n" +
				                   "</description>\n" +
				                   "<staff>\n" +
				                   "<member index=\"0\" function=\"1\">automatic-Celebrities-" + _daten [0].RegisseurVName + "_" + _daten [0].RegisseurNName + "</member>\n";
				if ((_daten [0].Darsteller1VName != "") &&(_daten [0].Darsteller1VName != ""))
					neuerFilm +=	 "<member index=\"1\" function=\"2\">automatic-Celebrities-" + _daten [0].Darsteller1VName + "_" + _daten [0].Darsteller1NName + "</member>\n";
				if ((_daten [0].Darsteller2VName != "") &&(_daten [0].Darsteller2VName != ""))
					neuerFilm +=	"<member index=\"2\" function=\"2\">automatic-Celebrities-" + _daten [0].Darsteller2VName + "_" + _daten [0].Darsteller2NName + "</member>\n";
				if ((_daten [0].Darsteller3VName != "") &&(_daten [0].Darsteller3VName != ""))
					neuerFilm +=	"<member index=\"3\" function=\"2\">automatic-Celebrities-" + _daten [0].Darsteller3VName + "_" + _daten [0].Darsteller3NName + "</member>\n";
				if ((_daten [0].Darsteller4VName != "") &&(_daten [0].Darsteller4VName != ""))
					neuerFilm +=	"<member index=\"4\" function=\"2\">automatic-Celebrities-" + _daten [0].Darsteller4VName + "_" + _daten [0].Darsteller4NName + "</member>\n";
				if ((_daten [0].Darsteller5VName != "") &&(_daten [0].Darsteller5VName != ""))
					neuerFilm +=	"<member index=\"5\" function=\"2\">automatic-Celebrities-" + _daten [0].Darsteller5VName + "_" + _daten [0].Darsteller5NName + "</member>\n";
					neuerFilm +=              "</staff>\n" +
				                    "<groups target_groups=\"0\" pro_pressure_groups=\"0\" contra_pressure_groups=\"0\" />\n" +
					"<data country=\""+_daten[0].ofdbCountry+"\" year=\""+_daten[0].ofdbYear+"\" distribution=\"1\" maingenre=\""+_daten[0].ofdbMainGenre+"\" subgenre=\""+_daten[0].ofdbSubGenre+"\" flags=\""+_daten[0].omdbFlags+"\" blocks=\""+_daten[0].omdbRuntime+"\" price_mod=\"1\" />\n" +
				                    "<ratings critics=\""+cr+"\" speed=\""+sp+"\" outcome=\""+ou+"\" />\n" +
				                    "</programme>\n";
				// und zurechtschnippeln
				myPosition = inhalt.IndexOf ("<allprogrammes>");
				inhaltNeu = inhalt.Substring (0, myPosition + 15);
				inhaltNeu += neuerFilm;
				inhaltNeu += inhalt.Substring (myPosition + 15, inhalt.Length - (myPosition + 15));
				System.IO.StreamWriter outputStreamWriter = System.IO.File.CreateText (schreibeDatei);
				outputStreamWriter.Write (inhaltNeu);
				outputStreamWriter.Close ();

				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.Write ("schreibe ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write ("" + _daten [0].ofdbTitle + "");
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine ("...");


			} else {
				Console.ForegroundColor = ConsoleColor.DarkGray;Console.Write("Film schon ");
				Console.ForegroundColor = ConsoleColor.White;	Console.Write("vorhanden ");
				Console.ForegroundColor = ConsoleColor.DarkGray;Console.WriteLine(""+_daten[0].ofdbTitle+".");
			}
		} // [E] schreibeXml
		public class FilmDaten  // Meine FilmDaten
		{	// Cache
			public string ofdbTitle { get; set; }    	// [1]
			public int ofdbYear { get; set; } 		// [2]
			public string ofdbIMDb_ID { get; set; }   	// [3]
			public string ofdbDesc { get; set; }  		// [4]
			public string ofdbMainGenre {get; set; } 	// [5]
			public string ofdbSubGenre {get; set; }		// [5.1]
			public string ofdbCountry { get; set; } 	// [6]
			public float ofdbRating {get; set; }		// [7]
			// OMDb API Rückgabe tt+[3]			
			public int omdbRuntime {get; set;}		// [8]
			public float omdbRating {get; set; }		// [9] siehe [7]
			public int omdbFlags {get; set;}		// [10]
			// Celebrities
			public string RegisseurVName {get; set;}
			public string RegisseurNName {get; set;}
			public string Darsteller1VName { get; set;}
			public string Darsteller1NName { get; set;}
			public string Darsteller2VName { get; set;}
			public string Darsteller2NName { get; set;}
			public string Darsteller3VName { get; set;}
			public string Darsteller3NName { get; set;}
			public string Darsteller4VName { get; set;}
			public string Darsteller4NName { get; set;}
			public string Darsteller5VName { get; set;}
			public string Darsteller5NName { get; set;}

		}
		
		/// <summary>WegDamit</summary>
		/// <returns>quelle [ohne] problem</returns>
		/// <param name="quelle">string: quelle</param>
		/// <param name="problem">string: problem</param>
		private static string WegDamit(string quelle, string problem)
		{
			bool rausHier = false;
			while (rausHier == false) 
			{
				if (quelle.Contains (problem)) {
					quelle = quelle.Remove (quelle.IndexOf (problem), problem.Length);
				} else rausHier = true;
		/// <summary>Vorholen</summary>
		/// <returns>Holt einen String von hinten des quelle_String nach Vorne und gibt den String zurück
		/// myBsp= "Tag, Der"; myBsp = Vorholen(Bsp,", Der); - myBsp == "Der Tag");</returns>
		/// <param name="quelle">string: quelle</param>
		/// <param name="problem">string: problem</param>
		private static string Vorholen(string quelle, string problem)
		{
			string ziel = quelle;
			int scout = ziel.IndexOf(problem);
			string preFix = ziel.Substring (scout+2, ziel.Length - scout-2);
			string postFix = ziel.Substring (0, ziel.Length - problem.Length);
			return (preFix+ " " +postFix);
		}
		/// <summary>CountSting</summary>
		/// <returns>Zählt die Anzahl der regexStr in str Rückgabe: int</returns>
		/// <param name="str">string: str</param>
		/// <param name="regexStr">string: regexStr</param>
		/// Benötigt: System.Text.RegularExpressions
		private static int CountStrings(string str, string regexStr)
		{
			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(regexStr); 
			return regex.Matches(str).Count;
		}
	} // [E] Class
} // (E) FgotoXML
