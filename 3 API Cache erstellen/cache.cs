using System;
using System.Net;  
namespace cacheAnlegen
{
	class Program
	{
		static void Main(string[] args)
		{
/*Variabeln*/ int GesamtMenge = 2500000, Teiler = 1000;
/*Variabeln*/ int GiveID=1; 
			try
			{		int TeileFilmMenge = GesamtMenge / Teiler;
					for(int i = 0; i < TeileFilmMenge; i++)
					{	DateTime StartZeit = DateTime.Now;  // (B) Zeitmessung
						GiveID = loadCache(Teiler*i, GiveID);
						DateTime EndZeit = DateTime.Now;	// (E) Zeitmessung
						TimeSpan GemessendeZeit= EndZeit - StartZeit;
					if (GemessendeZeit.Minutes < 1) Console.WriteLine ("< {0} (ID:{1}) {2}s",(GesamtMenge-Teiler*(i+1)),GiveID, GemessendeZeit.Seconds);
					else {	Console.WriteLine ("< {0} (j={1}) {2}m",(GesamtMenge-Teiler*(i+1)),GiveID, GemessendeZeit.Minutes);}	
					}
			} catch { Console.WriteLine ("ERROR"); }
		}
/*  Methode: loadCache
 *  Übergeben: int Anzahl, Erledigte Teilmenge
 *  Rückgabe: GiveID - die abgearbeitete Anzahl
 *  Info: Füllt den Cache Ordner mit XML Datein zur schnelleren Bearbeitung im späteren Verlauf
*/	 public static int loadCache(int Anzahl, int gID)
	 { 
			int wechselURL=0, countTimeout=0;
			string[] URL = {"http://ofdbgw.geeksphere.de/movie/", "http://ofdbgw.metawave.ch/movie/","http://ofdbgw.h1915283.stratoserver.net/movie/","http://ofdbgw.johann-scharl.de/movie/"};
			string ProblemFall = "cache.xml";
			string meineProbleme ="";
			if (System.IO.File.Exists(ProblemFall))
			{
				meineProbleme = System.IO.File.ReadAllText(ProblemFall);
			}
			int i = 0; int j=gID;

			while (j<Anzahl)
			{  if (j >= 30000) j= 1;
				string html="",myCacheFile = "cache/"+ j.ToString() +".xml";
				if (!System.IO.File.Exists(myCacheFile))
				{ 	
					if (meineProbleme.Contains("id:"+j.ToString()+":")) { html=""; }
					else
					{
						WebClient site = new WebClient ();
						Uri uri = new Uri (URL[wechselURL] + j); 
						html = site.DownloadString (uri);
						site.Dispose();
					}
				}
				if ((html != "") && html.Contains ("<rcodedesc>Ok</rcodedesc>") && !html.Contains ("<titel></titel>") && !html.Contains ("<imdbid></imdbid>") && !html.Contains ("<beschreibung></beschreibung>"))  
				{	i++; // Inhalt vorhanden:
					if (!System.IO.File.Exists(myCacheFile))
						{
							using (System.IO.StreamWriter sw2 = System.IO.File.AppendText (myCacheFile))
							{	
								sw2.WriteLine(html);
								Console.WriteLine("schreibe "+j+".");
							}
						}
				} 
				else 
				{	i--;
					if (html.Contains("<rcodedesc>Keine Resultate zu Anfrage gefunden</rcodedesc>") && (!meineProbleme.Contains("1 id:"+j+":")))
					{
						using (System.IO.StreamWriter sw = System.IO.File.AppendText  (ProblemFall))
						{
							sw.WriteLine ("Error 1 id:"+j+":Keine Resultate");
						}
					}
					else if(html.Contains("<titel></titel>") && (!meineProbleme.Contains("2 id:"+j+":")))
				{
						using (System.IO.StreamWriter sw = System.IO.File.AppendText  (ProblemFall)) 	
					{
							sw.WriteLine ("Error 2 id:"+j+":Titel fehlt");
					}
				}					
					else if(html.Contains("<beschreibung></beschreibung>") && (!meineProbleme.Contains("3 id:"+j+":")))
				{
						using (System.IO.StreamWriter sw = System.IO.File.AppendText  (ProblemFall)) 	
					{
							sw.WriteLine ("Error 3 id:"+j+":Beschreibung fehlt");
					}
				}
					if(html.Contains("<rcodedesc>Fehler oder Timeout bei OFDB Anfrage</rcodedesc>")) countTimeout++;
					if (countTimeout>=30){countTimeout=0;	wechselURL++; if (wechselURL >= 3) {wechselURL = 0;	}}
				}
			j++;	
			}	
			return(j);	
		} // (E) loadCache
	} // (E) Program
} // (E) cacheAnlegen
