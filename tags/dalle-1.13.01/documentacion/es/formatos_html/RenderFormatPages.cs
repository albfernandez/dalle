/*

	Dalle - A split/join file utility library
	Dalle.RenderFormatPages - Render format description Web Pages.		
	
    Copyright (C) 2003-2010  Alberto Fernández <infjaf@gmail.com>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.IO;
using System.Xml;
using System.Collections;

namespace Dalle
{

	/// <summary>Clase que construye un sitio web sencillo a partir de una 
	/// plantilla y unos archivos fuentes.</summary>	
	
	public class RenderFormatPages
	{
		
		public static XmlNamespaceManager nsmgr;
		
		public static void MakeAllPages (Formato[] list, string template, string destDir)
		{
			string plantilla = Fichero.CargarTexto (template);
			string indice = GenerarIndice(list);
			
			for (int i=0; i < list.Length; i++){
				Formato f = list[i];
				Console.WriteLine (f.Name);
				
				string salida = plantilla.Replace ("#TITLE#", "Dalle - " + f.Name);
				salida = salida.Replace ("#CONTENT#", f.Contenido);
				salida = salida.Replace ("#MENU#", indice);
				
				if (i > 0){
					salida = salida.Replace (
						"#PREVIOUS#", 
						String.Format (
							"<a href=\"{0}.html\">Anterior ({0})</a>", 
							list[i-1].Name
						)
					);
				}
				else{
					salida = salida.Replace ("#PREVIOUS#", "&nbsp;");
				}
				if ( i < (list.Length -1)){
					salida = salida.Replace (
						"#NEXT#",
						String.Format (
							"<a href=\"{0}.html\">Siguiente ({0})</a>",
							list[i+1].Name
						)
					);
				}
				else{
					salida = salida.Replace ("#NEXT#", "&nbsp;");
				}
				Fichero.GuardarTexto (destDir + "/" +f.Name + ".html", salida);
			}
				
		}
				
		// Parámetros: <xml con formatos> <plantilla del indice> <plantilla de los formatos>  <directorio de destino>
		public static void Main (string [] args)
		{		
			string input = args[0];
			string template_index = args[1];
			string template = args[2];			
			string destDir = args[3];
			XmlDocument document = new XmlDocument();
			document.Load (input);
			nsmgr = new XmlNamespaceManager (document.NameTable);
			nsmgr.AddNamespace ("t", "http://dalle.sourceforge.net/formats.xsd");
			XmlNodeList formatos = document.SelectNodes ("/t:formats/t:format", nsmgr);
			
			Formato[] list = new Formato [formatos.Count];
			int count = 0;
			foreach (XmlNode node in formatos){
				list[count++] = new Formato(node);
			}
			
			Array.Sort (list, Formato.Comparer);
			MakeIndexPage (list, template_index, destDir);
			MakeAllPages (list, template, destDir);
		}
		
		public static void MakeIndexPage (Formato[] list, string template, string destDir)
		{
			string table = MakeMainTable (list);
			string plantilla = Fichero.CargarTexto (template);
			string indice = GenerarIndice (list);
			
			Console.WriteLine ("index");
			string salida = plantilla.Replace ("#TITLE#", "Dalle - Formatos");
			salida = salida.Replace ("#CONTENT#", table);
			salida = salida.Replace ("#MENU#", indice);
			
			Fichero.GuardarTexto (destDir + Path.DirectorySeparatorChar +"index.html", salida);
			
		}
		public static string GenerarIndice (Formato[] list)
		{
			String ret = "<table>\n";
			foreach (Formato f in list){
				ret += "<tr><td>" + MakeInternalRef (f.Name) + "</td></tr>\n";
			}
			ret += "</table>\n";
			return ret;
		}
		public static string MakeMainTable (Formato[] list)
		{
			String ret;
			ret = "<table border=1>\n";
			ret += "<tr>\n";
			ret += "<td>Nombre</td>";
			ret += "<td>Web</td>\n";
			ret += "<td>Une</td>\n";
			ret += "<td>Corta</td>\n</tr>\n";
			foreach (Formato f in list){
			
				ret += "<tr>\n";
				ret += "<td>" + MakeInternalRef (f.Name) + "</td>\n";
				ret += "<td>" + ((f.Web == "") ? "&nbsp;" : MakeWebRef (f.Web)) + "</td>\n";
				ret += "<td>" + ((f.SupportsJoin=="") ? "&nbsp;" : f.SupportsJoin) + "</td>\n";
				ret += "<td>" + ((f.SupportsSplit=="") ? "&nbsp;" : f.SupportsSplit) + "</td>\n";
				ret += "</tr>\n";
			}
			ret += "</table>\n";
			return ret;
		}
		public static string MakeWebRef (string s)
		{
			return String.Format ("<a href=\"{0}\">{0}</a>", s);
		}
		public static string MakeInternalRef (string s)
		{
			return String.Format ("<a href=\"{0}.html\">{0}</a>", s);
		}
	}
	
	
	
	public class Formato 
	{
	
		private static IComparer _comparer;
		
		private string name;
		private string progname;
		private string web;
		private string license;
		private string popularity;
		private string sSplit;
		private string sJoin;
		private string sinceVersion;
		private string contenido;
		
		public Formato (XmlNode node)
		{
			name = GetField (node, "t:name");
			progname = GetField (node, "t:progname");
			web = GetField(node, "t:web");
			license = GetField (node, "t:license");
			popularity = GetField (node, "t:popularity");
			sSplit = GetField (node, "t:supportssplit");
			sJoin = GetField (node, "t:supportsjoin");
			sinceVersion = GetField (node, "t:sinceversion");
			
			contenido = MakeWebContent (node);
			
			
		}
		public string Name{
			get { return name;}
		}
		public string ProgName{
			get { return progname; }
		}
		public string Web{
			get { return web; }
		}
		public string License{
			get { return (license == "") ? "Unknown" : license; }
		}
		public string Popularity{
			get { return (popularity == "") ? "Unknown": popularity; }
		}
		public string SupportsSplit{
			get { return (sSplit =="0") ? "No": sSplit; }
		}
		public string SupportsJoin{
			get{ return (sJoin == "0") ? "No": sJoin; }
		}
		private string SinceVersion{
			get{ return (sinceVersion == "") ? "Unknown": sinceVersion;	}
		}
		public string Contenido {
			get { return contenido;}

		}
		public static IComparer Comparer{
			get{
				if (_comparer == null)
					_comparer = new MyComparer();
				return _comparer;
			}
		}

		public static string GetField (XmlNode node, string selector)
		{
                XmlNode result = node.SelectSingleNode (selector, RenderFormatsPages.nsmgr);

                if (result == null)
                        return String.Empty;

                return result.InnerText;
        }
		
		public string MakeWebContent (XmlNode node)
		{
		
			string ret = "";
			
			ret += "<h2>General</h2>\n";
			ret += "<blockquote>\n";
			ret += GenerarTablaFormato();
			ret += "</blockquote>\n";
			
			ret += "<h2>Descripci&oacute;n</h2>\n";
			ret += "<blockquote>\n";
			ret += GetInnerXml (node, "t:description");
			ret += "</blockquote>\n";
			
			ret += "<h2>Checksums</h2>";
			ret += "<blockquote>\n";
			ret += GetInnerXml(node, "t:checksums");
			ret += "</blockquote>\n";
			
			ret += "<h2>Naming</h2>";
			ret += "<blockquote>\n";
			ret += GetInnerXml (node, "t:naming");
			ret += "</blockquote>\n";
			
			ret += "<h2>Headers</h2>";
			ret += "<blockquote>\n";
			ret += GetInnerXml (node, "t:headers");
			ret += "</blockquote>\n";
			
			ret += "<h2>Tails</h2>";
			ret += "<blockquote>\n";
			ret += GetInnerXml (node, "t:tails");
			ret += "</blockquote>\n";
			
			ret += "<h2>auxfiles</h2>";
			ret += "<blockquote>\n";
			ret += GetInnerXml (node, "t:auxfiles");
			ret += "</blockquote>\n";
			
			ret += "<h2>Notas</h2>";
			ret += "<blockquote>\n";
			ret += GetInnerXml (node, "t:notes");
			ret += "</blockquote>\n";
			
			ret = ret.Replace (" xmlns=\"http://dalle.sourceforge.net/formats.xsd\"", "");			
			return ret == "" ? ("Contenido " + Name ): ret;
		}
		
		private string GenerarTablaFormato ()
		{
			string ret = "";
			ret += "<table border=1>\n";
			ret += "<tr><td>Nombre </td><td>" + Name + " </td></tr>\n";;
			ret += "<tr><td>Nombre del programa </td><td>" + (ProgName == "" ? "&nbsp;":ProgName)+ " </td></tr>\n";
			ret += "<tr><td>Web </td><td>" + (Web == "" ? "&nbsp;" : RenderFormatsPages.MakeWebRef (Web)) + " </td></tr>\n";
			ret += "<tr><td>Licencia</td><td>" + (License == "" ? "&nbsp;" : License )+ "</td></tr>\n";
			ret += "<tr><td>Une </td><td>" + (SupportsJoin == "" ? "&nbsp;" : SupportsJoin) + "</td></tr>\n";
			ret += "<tr><td>Parte</td><td>" +(SupportsSplit =="" ? "&nbsp;" :  SupportsSplit) + "</td></tr>\n";
			ret += "<tr><td>Dalle lo soporta desde su versi&oacute;n</td><td>" + (SinceVersion == "" ? "&nbsp;" : SinceVersion) + "</td></tr>\n";
			ret += "</table>\n";
			return ret;
		}
		public static string GetInnerXml (XmlNode node, string selector)
		{
			XmlNode result = node.SelectSingleNode (selector, RenderFormatsPages.nsmgr);
			if (result == null)
				return String.Empty;
			return result.InnerXml;
		}		
		
		private class MyComparer : IComparer
		{
			public MyComparer()
			{
			}			
			public int Compare (object x, object y)
			{
				string s1 = (x as Formato).Name.ToUpper();
				string s2 = (y as Formato).Name.ToUpper();			
				return String.Compare (s1, s2);
			}
		}
		
		
	}
	
	

	/// <remarks>Esta clase contiene métodos para leer y escribir ficheros.
	/// </remarks>
	
	public class Fichero
	{

		/// <summary>Constructor privado.  No se permiten crear instancias 
		/// de esta clase.</summary>
		
		private Fichero ()
		{
		}
		
		/// <summary>Carga el texto contenido en un archivo.</summary>
		/// <param name="archivo">La ruta del archivo a leer.
		/// </param>
		/// <returns>El texto contenido en <c>archivo</c>.</returns>
		
		public static String CargarTexto (String archivo)
		{

			try{
				TextReader file = File.OpenText (archivo);
				String texto = file.ReadToEnd ();
				file.Close ();
				return texto;
			}
			catch (ArgumentException ex){

				TextReader file = new StreamReader (
									archivo, System.Text.Encoding.Default);
				String texto = file.ReadToEnd ();
				file.Close ();
				return texto;
			}
		}
		
		/// <summary>Guarda un texto en el archivo indicado.</summary>
		/// <param name="nombreFichero">La ruta del fichero en el que
		/// se guardará el texto. Si no existe se creará.</param>
		/// <param name="texto">El texto a guardar.</param>
		
		public static void GuardarTexto (String nombreFichero, String texto)
		{
			TextWriter file = new StreamWriter (nombreFichero, false,
						  System.Text.Encoding.Default);
			file.Write (texto);
			file.Close ();
		}
	}
}
