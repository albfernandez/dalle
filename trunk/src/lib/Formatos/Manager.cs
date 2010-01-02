/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Manager - 	
		Provides a single access point for all supported formats.
	
    Copyright (C) 2003-2009  Alberto Fernández <infjaf00@yahoo.es>

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
using System.Collections;
using System.Reflection;
using System.IO;
using System.Threading;


namespace Dalle.Formatos
{

	/// <summary> 
	/// Clase que se encarga de gestionar y centralizar el acceso a los
	/// distintos formatos.
	/// </summary>
	
	public class Manager 
	{


		/// <summary>
		/// Lista con todos los formatos soportados.
		/// </summary>
		
		private ArrayList formatos = new ArrayList();
		
		/// <summary>
		/// Tabla que relaciona cada hilo con su instancia de la clase
		/// correspondiente.
		/// </summary>

		private static Hashtable threadsTable = new Hashtable();
				
		/// <summary>
		/// Crea una nueva instancia de la clase.
		/// </summary>
				
		private Manager ()
		{
			formatos.Add (new Dalle.Formatos.Astrotite.Astrotite());
			formatos.Add (new Dalle.Formatos.Axman.Axman3());
			formatos.Add (new Dalle.Formatos.Zip.Zip());
			formatos.Add (new Dalle.Formatos.MaxSplitter.MaxSplitter());
			formatos.Add (new Dalle.Formatos.EasyFileSplitter.EasyFileSplitter());
			formatos.Add (new Dalle.Formatos.SF.ParteSF());
			formatos.Add (new Dalle.Formatos.Kamaleon.ParteKamaleon2());
			formatos.Add (new Dalle.Formatos.Kamaleon.ParteKamaleon());
			formatos.Add (new Dalle.Formatos.Camouflage.Camouflage());
			formatos.Add (new Dalle.Formatos.FileSplit.FileSplit());
			// De momento anulado (tampoco es que sea muy usado)
			//formatos.Add (new Dalle.Formatos.SplitFile.SplitFile_v1());
			formatos.Add (new Dalle.Formatos.Hacha.ParteHacha_v2());
			formatos.Add (new Dalle.Formatos.Hacha.ParteHacha_v1());
			formatos.Add (new Dalle.Formatos.Hacha.ParteHachaPro());
			//formatos.Add (new Dalle.Formatos.Cutter.Cutter());	
			//formatos.Add (new Dalle.Formatos.HJSplit.HJSplit());
			//formatos.Add (new Dalle.Formatos.UnixSplit.UnixSplit());			
			formatos.Add (new Dalle.Formatos.Generico.ParteGenerico());

			foreach (Parte p in formatos){
				p.Progress += new ProgressEventHandler (this.OnProgress);
			}
		}
		
		
		
		/// <summary>
		/// Obtiene la única instancia de la clase.</summary>
		/// <returns>La única instancia de la clase.</returns>
		
		public static Manager Instance{
			get { return GetInstance(Thread.CurrentThread); }
		}
		
		/// <summary>Versión de la librería.</summary>
		
		public static Version Version{
			get{ return Assembly.GetExecutingAssembly().GetName().Version; }
		}
		
		/// <summary>
		/// Obtiene la única instancia de la clase.</summary>
		/// <returns>La única instancia de la clase.</returns>
		
		public static Manager GetInstance ()
		{
			return GetInstance (Thread.CurrentThread);
		}		
		
		/// <summary>
		/// Obtiene la instancia de la clase para un hilo dado.
		/// </summary>
		/// <param name="t">El hilo para el que queremos obtener una
		/// instancia.</param>
		/// <returns>La instancia de la clase para t.</returns>
		
		public static Manager GetInstance (Thread t)
		{
			if (!threadsTable.ContainsKey (t))
				threadsTable.Add (t, new Manager());
			return threadsTable[t] as Manager;
		}
				
		/// <summary>
		/// Obtiene una lista de formatos que estan soportados para partir.
		/// </summary>
		/// <returns>
		/// Una lista de IPartes con los que se pueden partir ficheros.
		/// </returns>
		
		public ArrayList GetFormatosParte ()
		{

			ArrayList ret = new ArrayList();
			foreach (IParte p in formatos)
				if (p.ParteFicheros)
					ret.Add(p);

			ret.Sort (ParteComparer.Instance);
			return ret;
		}

		/// <summary>
		/// Obtiene una lista de formatos que estan soportados para unir.
		/// </summary>
		/// <returns>
		/// Una lista de IPartes con los que se pueden unir ficheros.
		/// </returns>

		public ArrayList GetFormatosUne()
		{
			ArrayList ret = new ArrayList();
			foreach (IParte p in formatos)
				ret.Add(p);
			ret.Sort (ParteComparer.Instance);
			return ret;
		}
		
		/// <summary>
		/// Obtiene el objeto IParte que puede unir el fichero indicado,
		/// null si el fichero no pertenece a un formato conocido.
		/// </summary>
		/// <param name="fichero">El nombre del fichero.</param>
		/// <returns>El IParte para unir fichero, o null si no hay ninguno.
		/// </returns>
		
		public IParte GetFormatoFichero (String fichero)
		{
			Console.WriteLine("Formatos soportados " + formatos.Count);
			foreach (IParte p in formatos){
				Console.WriteLine("testeando parte " + p.Nombre + " " + p);
				if (p.PuedeUnir(fichero)){
					return p;
				}
			}
			return null;
		}
		
		/// <summary>Obtiene el objeto IParte correspondiente al 
		/// nombre de formato dado.
		/// </summary>
		/// <param name="name">El nombre del formato.</param>
		/// <returns>El formato pedido, null si no existe.</returns>

		public IParte GetFormatByName (String name)
		{
			foreach (IParte p in formatos)
				if (p.Nombre==name)
					return p;
			return null;

		}

		/// <summary>Parte un fichero</summary>
		/// <param name="formato">
		/// El nombre del formato que se utilizará para partir el fichero.
		/// </param>
		/// <param name="fichero">El fichero a partir.</param>
		/// <param name="salida1">
		/// El nombre del fichero destino, o nombre base de los fragmentos.
		/// No todos los formatos hacen uso de este parametro.
		/// </param>
		/// <param name="dir">
		/// Directorio donde se pondrán los fragmentos obtenidos.
		/// </param>
		/// <param name="kb">
		/// Tamaño de los fragmentos (hay formatos, como zip, que no hacen uso
		/// de este dato). 
		/// </param>
		
		public void Partir (String formato, String fichero, String salida1, String dir, long kb)
		{
			
			this.SetStopRequest (false);
			IParte p = GetFormatByName(formato);
			if (p==null){
				throw new FormatNotSupportedException (formato);
			}
			if (!p.ParteFicheros){
				throw new SplitNotSupportedException (formato);
			}
			try{
				p.Partir (fichero, salida1, dir, kb);
			}
			catch (StoppedByUserException){
				this.SetStopRequest (false);
			}

		}
		
		/// <summary>Parte un fichero</summary>
		/// <param name="formato">
		/// El nombre del formato que se utilizará para partir el fichero.
		/// </param>
		/// <param name="fichero">
		/// El fichero a partir.
		/// </param>
		/// <param name="salida1">
		/// El nombre del fichero destino, o nombre base de los fragmentos.
		/// No todos los formatos hacen uso de este parametro.
		/// </param>
		/// <param name="kb">
		/// Tamaño de los fragmentos (hay formatos, como zip, que no hacen uso
		/// de este dato). 
		/// </param>

		public void Partir (String formato, String fichero, String salida1, long kb)
		{
			Partir (formato, fichero, salida1, new FileInfo(fichero).DirectoryName, kb);
		}
			
	
		/// <summary>
		/// Une el fichero indicado (y sus otros fragmentos), 
		/// dejando el resultado en el directorio indicado.
		/// </summary>
		/// <param name="fichero">El nombre del fragmento.</param>
		/// <param name="dir">
		/// El directorio donde se dejara el archivo resultante.
		/// </param>
		
		public void Unir (String fichero, String dir)
		{
			this.SetStopRequest (false);
			
			IParte f = this.GetFormatoFichero(fichero);
			if (f == null)
				throw new UnknownFormatException (fichero);
				
			try{
				f.Unir (fichero, dir);
			}
			catch (StoppedByUserException){
				this.SetStopRequest (false);
			}
		}
		
		/// <summary>
		/// Une el fichero indicado (y sus otros fragmentos), dejando el 
		/// resultado en el directorio indicado.
		/// </summary>
		/// <param name="fichero">El nombre del fragmento.</param>
		/// <param name="dir">
		/// El directorio donde se dejara el archivo resultante.
		/// </param>

		public void Unir (String fichero)
		{
			Unir (fichero, new FileInfo(fichero).DirectoryName);
		}
		
		/// <summary>Provoca el evento Progress</summary>
		/// <param name="done">La tarea realizada</param>
		/// <param name="total">El total a realizar</param>
		
		protected void OnProgress (long done, long total)
		{
			if (Progress != null)
				Progress (done, total);
		}	
		
		/// <summary>Evento que se provoca para mostrar el progreso
		/// de una operación.</summary>
		
		public event ProgressEventHandler Progress;
		
		/// <summary>Provoca la detención de la operación actual.</summary>

		public void Stop()
		{
			this.SetStopRequest (true);
		}
		
		/// <summary>Cambia el estado de "detener".</summary>
		
		private void SetStopRequest (bool value)
		{			
			foreach (Parte p in formatos)
				p.SetStopRequest (value);			
		}
	}
	
	
	/// <summary>Clase utilizada para comparar dos objetos Parte</summary>
	/// <remarks>Se comparan por el nombre del formato.</remarks>
	
	public class ParteComparer : System.Collections.IComparer 
	{
		/// <summary>
		/// Única instancia de la clase (patrón singleton).
		/// </summary>
		
		private static ParteComparer instance = null;
		
		/// <summary>
		/// Constructor privado. No se permite crear instancias de esta 
		/// clase externamente.
		/// </summary>
		
		private ParteComparer()
		{
		}
		
		/// <summary>La única instancia de la clase.</summary>
		
		public static ParteComparer Instance{
			get{
				if (instance == null)
					instance = new ParteComparer();
				return instance;
			}
		}
		
		
		/// <summary>Compara dos objetos Parte</summary>
		
		public int Compare (object x, object y)
		{
			string s1 = (x as IParte).Nombre;
			string s2 = (y as IParte).Nombre;
			
			return String.Compare (s1, s2);
		}
	}	
}
