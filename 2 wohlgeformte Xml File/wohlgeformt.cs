// MiniProgramm zur Wohlformung von Cache Datein mit Lesefehlern aus dem INET.
namespace wohlgeformt
{
	class Program
	{
		static void Main(string[] args)
		{	string myFileStream = "";
			int scout, scout2;
			string FilesubString = "";

			for (int j = 1; j < 290000; j++) {
				string myCacheFile = "cache/" + j.ToString () + ".xml";

				if (System.IO.File.Exists (myCacheFile)) {
					myFileStream = System.IO.File.ReadAllText(myCacheFile);
					if (!myFileStream.StartsWith ("<?xml version=")) {
						System.Console.WriteLine("Datensatze {0} nicht wohlgeformt", j); 
						if (myFileStream.Contains ("<?xml version=") && myFileStream.Contains ("</ofdbgw>")) {
							System.Console.WriteLine ("Datensatze {0} wird wohlgeformt gemacht", j);
							scout = myFileStream.IndexOf ("</ofdbgw>");
							scout2 = myFileStream.IndexOf ("<?xml version=");
							FilesubString = myFileStream.Substring (scout2, (scout-scout2)+9);
							System.IO.StreamWriter outputStreamWriter = System.IO.File.CreateText(myCacheFile);
							outputStreamWriter.Write(FilesubString);
							outputStreamWriter.Close ();
						}
					}
				} 
			}
		}

	} // [E] Class
} // [E] Namespace
