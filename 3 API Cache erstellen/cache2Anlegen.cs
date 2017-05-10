/*	
*/
using System;
using System.Net;  
namespace cache2Anlegen
{
	class Program
	{
		static void Main(string[] args)
		{
			/*Variabeln*/ int GesamtMenge = 280000;
			string imdbID = "";
			try
			{		} catch { Console.WriteLine ("ERROR"); }
				for(int i = 0; i < GesamtMenge; i++)
				{
					string alteCacheDatei = "cache/"+i+".xml";

					if (System.IO.File.Exists(alteCacheDatei))
					{
					imdbID = bekommeID (i);
					loadCache(i, imdbID);
					}
				}
		
		}

		public static string bekommeID(int i)
		{
			int firstScout, dazwischen;
			string myIMDbID = "";
			// [3] IMDbID lesen
			string html = System.IO.File.ReadAllText("cache/" + i + ".xml");
			firstScout = html.IndexOf ("<imdbid>") + 8;
			dazwischen = html.IndexOf ("</imdbid>") - html.IndexOf ("<imdbid>") -8;
			myIMDbID = "tt"+html.Substring (firstScout, dazwischen);
			return(myIMDbID);
		}
/*  Methode: loadCache
 *  Übergeben: int Position
 *  Rückgabe: 
 *  Info: Füllt den Cache2 Ordner mit XML Datein zur schnelleren Bearbeitung im späteren Verlauf
*/	 public static void loadCache(int Anzahl, string myID)
		{ 
			
			string URL = "http://www.omdbapi.com/?i="+myID;
			string html="",myCacheFile = "cache2/"+ Anzahl.ToString() +".xml";
				if (!System.IO.File.Exists(myCacheFile))
				{ 	
						WebClient site = new WebClient ();
						Uri uri = new Uri (URL); 
						html = site.DownloadString (uri);
						site.Dispose();
					
				} else {html="";}
			if (html != "") 
				if (!System.IO.File.Exists(myCacheFile))
					if ( (html.Contains("Response\":\"False\"")) || (html.Contains("Incorrect IMDb ID.")))
					{
				Console.ForegroundColor = ConsoleColor.Red;
						Console.Write("Resonse False oder inc. IMDBID");
				Console.ForegroundColor = ConsoleColor.White;
					}
					else
					{
						using (System.IO.StreamWriter sw2 = System.IO.File.AppendText (myCacheFile))
						{	
						sw2.WriteLine(html);
					Console.Write(" schreibe ");
					Console.ForegroundColor = ConsoleColor.Green;
						Console.WriteLine(Anzahl);
					Console.ForegroundColor = ConsoleColor.White;

						}
					}
		} // (E) loadCache
	} // (E) Program
} // (E) cache2Anlegen
