/*

	Dalle - A split/join file utility library
	Dalle.MakeWeb - Makes the project web page.		
	
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
	
	public class MakeWeb
	{
		public static XmlNamespaceManager nsmgr;
		public static string SourceDir;
		// Parámetros <xml con contenidos> <plantilla> <directorio destino>
		public static void Main (string [] args)
		{		
			string input = args[0];
			string template = args[1];			
			string destDir = args[2];
			SourceDir = new FileInfo(args[0]).DirectoryName;
			XmlDocument document = new XmlDocument();
			document.Load (input);
			nsmgr = new XmlNamespaceManager (document.NameTable);
			nsmgr.AddNamespace ("t", "https://github.com/albfernandez/dalle/web.xsd");
			XmlNodeList paginas = document.SelectNodes ("/t:web/t:page", nsmgr);
			
			Pagina[] list = new Pagina [paginas.Count];
			
			int count = 0;
			foreach (XmlNode node in paginas){
				list[count++] = new Pagina (node);
			}
			
			MakeAllPages (list, template, destDir);
		}
		
		static string GenerarIndice (Pagina[] list)
		{
			String ret = "<ul style='list-style-type: none;margin-left:0; padding-left:0;'>\n";
			foreach (Pagina p in list){
				ret +=
					String.Format (
						"<li><a href=\"{0}.html\">{1}</a></li>\n",
						p.FileName, 
						p.Name
					);
			}
			ret += "</ul>\n";
			return ret;
		}
		
		static void MakeAllPages (Pagina[] list, string template, string destDir)
		{
			string plantilla = Fichero.CargarTexto (template);
			string indice = GenerarIndice(list);
			
			for (int i=0; i < list.Length; i++){
				Pagina p = list[i];
				Console.WriteLine (p.Name);
				
				string salida = plantilla.Replace ("#TITLE#", "Dalle - " + p.Name);
				salida = salida.Replace ("#CONTENT#", p.Content);
				salida = salida.Replace ("#MENU#", indice);				
				salida = salida.Replace (" xmlns=\"https://github.com/albfernandez/dalle/web.xsd\"", "");
				Fichero.GuardarTexto (destDir +Path.DirectorySeparatorChar +p.FileName + ".html", salida);
			}
				
		}
		


		public static void ComprobarParametros (string[] pars)
		{
			if (pars.Length != 3){
				throw new Exception ("El número de parámetros debe ser 3");
			}
			if (! File.Exists(pars[0])){
				throw new Exception("No se encontró el archivo de plantilla");
			}
			if (! Directory.Exists(pars[1])){
				throw new Exception ("No se encontró el directorio de fuentes");
			}
			if (! Directory.Exists(pars[2])){
				throw new Exception ("No se encontró el directorio de destino");
			}
		}
	}


	
	class Pagina 
	{
		private string name;
		private string fileName;
		private string content;
		
		public Pagina (XmlNode node)
		{
			name = GetField (node, "t:name");
			fileName = GetField (node, "t:filename");
			if (name == "FAQ"){
				content = MakeFAQ (node);
				
			}
			else if (name == "News"){
				content = MakeNews ();
			}
			else{
				content = GetInnerXml(node, "t:content");
			}
		}
		public string Name{
			get { return name; }
		}
		public string FileName {
			get { return (fileName == "") ? Name : fileName; }
		}
		public string Content{
			get { return content; }
		}
		
		public string MakeNews()
		{
			String ret = "";
			ret += "<h1>News</h1>\n";
			ret += "<pre>\n";
			try{
				ret += Fichero.CargarTexto (MakeWeb.SourceDir + Path.DirectorySeparatorChar +"NEWS");
			}
			catch (Exception){}
			ret += "<pre>\n";
			return ret;
		}
		public string MakeFAQ (XmlNode node)
		{
		
			string ret= "";
			ret += "<h1>FAQ</h1>\n";			
			
			XmlNodeList preguntas = node.SelectNodes ("t:item", MakeWeb.nsmgr);
			
			Pregunta[] list = new Pregunta [preguntas.Count];
			Console.WriteLine ("Numero de elementos en FAQ = "+ preguntas.Count);
			int count = 0;
			foreach (XmlNode _n_ in preguntas){
				Console.WriteLine ("" + _n_.OuterXml );
				list[count++] = new Pregunta (_n_);
			}
			ret += Pregunta.MakeIndex(list);
			ret += "<hr /> \n";
			count=1;
			foreach (Pregunta p in list){
				ret += String.Format (
					"<a name=\"{0}\"></a>\n<p><b>{1}</b></p>\n<p>{2}</p>\n",
					count++,
					p.Question,
					p.Answer
					);	
			}			
			return ret;
		}
		public static string GetField (XmlNode node, string selector)
		{
                XmlNode result = node.SelectSingleNode (selector, MakeWeb.nsmgr);

                if (result == null)
                        return String.Empty;

                return result.InnerText;
        }
		
		public static string GetInnerXml (XmlNode node, string selector)
		{
			XmlNode result = node.SelectSingleNode (selector, MakeWeb.nsmgr);
			if (result == null)
				return String.Empty;
			return result.InnerXml;
		}	
	}
	
	class Pregunta
	{
		private string pregunta;
		private string respuesta;
		
		public Pregunta (XmlNode node)
		{
			Console.WriteLine ("---\n" + node.OuterXml);
			this.pregunta = Pagina.GetInnerXml (node, "t:question");

			Console.WriteLine (this.pregunta);





			this.respuesta = Pagina.GetInnerXml (node, "t:answer");
		}
		
		public string Question{
			get { return pregunta; }
		}
		public string Answer {
			get { return respuesta; }
		}
		
		public static string MakeIndex (Pregunta[] list)
		{
			string ret = "";
			int count=1;
			foreach (Pregunta p in list){
				ret += String.Format ("<a href=\"#faq{0}\">{1}</a><br>\n", count++, p.Question);
			}
			return ret;		
		}
	}
	

	/// <remarks>Esta clase contiene métodos para leer y escribir ficheros.
	/// </remarks>
	
	class Fichero
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
			catch (ArgumentException){
				TextReader file = new StreamReader (archivo, System.Text.Encoding.Default);
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
