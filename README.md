# C# Xml API

__0. Vorbereitung__

Umsetzung in C#. Wir brauchen einen C#-Compiler. Linux: `sudo apt install mono-mcs`

- Ein Verzeichniss angelegt.
- Eine Dateiname.cs erstellen und den Scource reinkopieren oder runterladen.
- Internetverbindung (API).
- XML Datei (unter xml).


__0.1 genutzte Namensräume__

- `using System;` System Namensraum
- `using System.Net;` WebClient, Uri
- `using System.Xml;` XmlDocument, Reader
- `using System.Collections.Generic;` Listen


__1 API: omdbapi.com nutzen um die Jahreszahlen aus einer XML Datei mit den zurückgegebenen Jahreszahlen zu vergleichen__
