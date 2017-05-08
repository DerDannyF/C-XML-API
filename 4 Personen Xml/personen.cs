// Aus dem Cache Darsteller und Regisseure lesen und in die database_people.xml einfügen und wenn vorhanden prominence + 1
using System;
using System.Text.RegularExpressions;

namespace PgotoXML
{
	class Program
	{
		static void Main(string[] args)
		{
			int Anzahl = 255500;int j = 0, scout = 0, scout2 = 0, i = 0, PersonenAnzahlVorher = 0, Regisseur = 0, RegisseurPro = 0, Darsteller = 0, DarstellerPro=0;
			string html = "", htmlsub = "", htmlsubsub=""; 
			string vorname, nachname, name;
			string PersonenDatei = "database_people.xml";
			string myDB = System.IO.File.ReadAllText(PersonenDatei);

			PersonenAnzahlVorher = CountStrings (myDB, "</person>");
			while (j < Anzahl) {
			string myCacheFile = "cache/"+ j.ToString() +".xml";
			if (System.IO.File.Exists(myCacheFile))
			{ 	
				myDB = System.IO.File.ReadAllText(PersonenDatei);
				html = System.IO.File.ReadAllText(myCacheFile);
					if (html.Contains ("</name>\n</person>\n</regie>")) 
					{	// Regie		
						scout = html.IndexOf("</name>\n</person>\n</regie>");
						htmlsub = html.Substring(scout-40,40);
						scout = htmlsub.IndexOf ("<name>");
						htmlsub = htmlsub.Substring (scout+6, htmlsub.Length -6 - scout);
						if (htmlsub.Contains (" ")) {
							scout = htmlsub.IndexOf (" ");
							vorname = htmlsub.Substring (0, scout);
							nachname = htmlsub.Substring (1+scout, htmlsub.Length - (scout+1));
							int position = 1;
							if (myDB.Contains ("id=\"automatic-Celebreties-"+vorname+"_"+nachname+"\"")) RegisseurPro++;
							else Regisseur++;
							schreibeXml (vorname,nachname,position,PersonenDatei);
							Console.Write (".");
						}
						
					}
					if (html.Contains("<besetzung>\n<person>\n<id>"))
					{ // Darsteller (max5)
						scout = html.IndexOf ("<besetzung>\n<person>\n<id>");
						scout2 = html.IndexOf ("</besetzung>");
						htmlsub = html.Substring (scout, scout2-scout);
						i = 1;	scout = htmlsub.IndexOf ("<name");
						htmlsubsub = htmlsub.Substring (scout, htmlsub.Length - scout);
						while ((i < CountStrings(htmlsub,"<name>")) && (i <= 5))
						{
							scout = htmlsubsub.IndexOf ("<name>");
							scout2 = htmlsubsub.IndexOf ("</name>");
							name = htmlsubsub.Substring(scout+6, scout2-scout-6);
							htmlsubsub = htmlsubsub.Substring(scout2+6, htmlsubsub.Length-(scout2+6));			
							if (name.Contains (" ")) {
								scout = name.IndexOf (" ");
								vorname = name.Substring (0, scout);
								nachname = name.Substring (1+scout, name.Length - (scout+1));
								int position = 2;
								if (myDB.Contains("automatic-Celebreties-"+vorname+"_"+nachname+""))	DarstellerPro++;
								else Darsteller++;
								schreibeXml (vorname,nachname,position,PersonenDatei);
								Console.Write (".");
							}
						i++;
						}i = 0;
						htmlsub = html.Substring (scout, scout2-scout);
					}
			}
				j++;
			}
			Console.WriteLine ("Celebrity vorher: {0}", PersonenAnzahlVorher);
			Console.WriteLine ("Darsteller neu eingelesen: {0} \nRegisseure neu eingelesen: {1}\n",Darsteller, Regisseur);
			Console.WriteLine ("Darsteller upgrade: {0} \nRegisseur upgrade: {1}\n",DarstellerPro, RegisseurPro);

		}
		private static void schreibeXml (string vname, string nname, int position, string schreibedatei)
		{
			string inhalt = "", inhaltNeu=""; string meinePerson; int myPosition=0, myPosition2=0;
		
			if (System.IO.File.Exists (schreibedatei)) { 	
				inhalt = System.IO.File.ReadAllText(schreibedatei);
				if (!inhalt.Contains ("<first_name>" + vname + "</first_name>\n<last_name>" + nname + "</last_name>")) {
					// Person noch nicht vorhanden:
					// schreibe Personen Container
					meinePerson = "\n\n<person id=\"automatic-Celebreties-" + vname + "_" + nname + "\" imdb_id=\"nm\" created_by=\"automatic v.01\">\n" +
					"<first_name>" + vname + "</first_name>\n" +
					"<last_name>" + nname + "</last_name>\n" +
					"<nick_name></nick_name>\n" +
					"<images />\n" +
					"<details job=\"" + position + "\" gender=\"\" birthday=\"\" deathday=\"\" country=\"\" />\n" +
					"<data prominence=\"1\" skill=\"0\" fame=\"0\" scandalizing=\"0\" price_mod=\"1\" power=\"0\" humor=\"0\" charisma=\"0\" appearance=\"0\" topgenre1=\"0\" topgenre2=\"0\" />\n" +
					"</person>\n";
					// und zurechtschnippeln
					myPosition = inhalt.IndexOf ("<celebritypeople>");
					inhaltNeu = inhalt.Substring (0, myPosition + 17);
					inhaltNeu += meinePerson;
					inhaltNeu += inhalt.Substring (myPosition + 17, inhalt.Length - (myPosition + 17));
					System.IO.StreamWriter outputStreamWriter = System.IO.File.CreateText (schreibedatei);
					outputStreamWriter.Write (inhaltNeu);
					outputStreamWriter.Close ();
				} else {
					// Schon vorhanden? Ja -> Porminence +1 (bis max 9) um später Outcome zu berechnen.
					myPosition2 = inhalt.LastIndexOf ("<first_name>" + vname + "</first_name>\n<last_name>" + nname + "</last_name>\n<nick_name></nick_name>\n<images />");
					inhaltNeu = inhalt.Substring (myPosition2, 500);
					myPosition = inhaltNeu.IndexOf ("prominence=") + 12;
					int prominent = Convert.ToInt32 (inhaltNeu.Substring (myPosition, 1));
					if (prominent <= 8)
						prominent++;
					inhaltNeu = inhalt.Substring (0, myPosition2 + myPosition);
					inhaltNeu += prominent;
					myPosition++;
					inhaltNeu += inhalt.Substring ((myPosition2 + myPosition), inhalt.Length - (myPosition2 + myPosition));

					System.IO.StreamWriter outputStreamWriter2 = System.IO.File.CreateText (schreibedatei);
					outputStreamWriter2.Write (inhaltNeu);
					outputStreamWriter2.Close ();
				}

			}
		}

		private static int CountStrings(string str, string regexStr)
		{
			Regex regex = new Regex(regexStr);
			return regex.Matches(str).Count;
		}
	} // [E] Class
} // (E) PgotoXML
