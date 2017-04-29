using System;
using System.Net;  
using System.Xml;  
using System.Collections.Generic;

namespace www.omdbapi.com
{
	public class JahresListe
	{	public string imdb_ID { get; set; } 
		public int omdbJahr { get; set; } 
		public int xmlJahr { get; set; } 
	}
	class Program
	{
		static void Main(string[] args)
		{
			string xmlFileName1 = "database_programmes.xml";
			XmlDocument altXML = new XmlDocument ();
			altXML.Load (xmlFileName1);

			XmlTextReader reader = new XmlTextReader (xmlFileName1);
			int i = 0, j = 0, firstScout, dazwischen;
			string myYear = "", myJahr = "", myIMDB_ID = "", myaktIMDb_ID = "";
			List<JahresListe> _JL = new List<JahresListe>(); // Jahresliste
			while (reader.Read ()) 
			{
				if (reader.Name == "data") 
				{	myJahr = reader.GetAttribute ("year");
					if (myIMDB_ID != myaktIMDb_ID)
					{
						myaktIMDb_ID = myIMDB_ID;
						_JL.Add( new JahresListe () {imdb_ID = myIMDB_ID,omdbJahr=Convert.ToInt32(myYear), xmlJahr=Convert.ToInt32(myJahr)});
					Console.WriteLine ("Film#{0} eingelesen.",j);
					}
				}
				if (reader.Name == "programme")
				{i++;
					if (reader.GetAttribute ("imdb_id") != null) 
					{	  myIMDB_ID = reader.GetAttribute ("imdb_id");
						if (myIMDB_ID != "") 
						{	Uri uri = new Uri ("http://www.omdbapi.com/?i=" + myIMDB_ID);
							WebClient site = new WebClient ();
							string html = site.DownloadString (uri);

							if (html.Contains ("Incorrect IMDb ID.")) 
							{
								Console.WriteLine ("Falsche IMDb ID");
							}
							else // Inhalt vorhanden
 							{	
								j++;
								firstScout = html.IndexOf ("Year")+7;
								dazwischen = 4;
								myYear = html.Substring (firstScout, dazwischen);
							}
						}
					}
				}
			}
			i = 0; int a = 0; int d;
			while (i < j) 
			{
				if ((_JL [i].omdbJahr != _JL [i].xmlJahr) && (_JL[i].imdb_ID!=""))
				{
					if (_JL [i].omdbJahr > _JL [i].xmlJahr)
						d = _JL [i].omdbJahr - _JL [i].xmlJahr;
					else
						d = _JL [i].xmlJahr - _JL [i].omdbJahr;
					Console.WriteLine (" IMDB_ID : {0} omdbJahr: {1} xmlJahr: {2} Abweichung: {3} Jahr(e)", _JL [i].imdb_ID, _JL [i].omdbJahr, _JL [i].xmlJahr, d);
					a++;
				}
				i++;
			}
			Console.WriteLine ("{0} Film IDs bei omdbapi.com eingelesen. {1} Abweichungen.", j,a);

		}
	}
}
