/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Manager - 	
		Provides a single access point for all supported formats.
	
    Copyright (C) 2003  Alberto Fern�ndez <infjaf00@yahoo.es>

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

*/

using System;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Threading;

using Dalle.Formatos;
using Dalle.Formatos.Hacha;
using Dalle.Formatos.SplitFile;
using Dalle.Formatos.FileSplit;
using Dalle.Formatos.Kamaleon;
using Dalle.Formatos.SF;
using Dalle.Formatos.EasyFileSplitter;
using Dalle.Formatos.MaxSplitter;
using Dalle.Formatos.Generico;
using Dalle.Formatos.Zip;
using Dalle.Formatos.Axman;

using I = Dalle.I18N.GetText;

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
			
			formatos.Add (new Axman3());
			formatos.Add (new Zip());
			formatos.Add (new MaxSplitter());
			formatos.Add (new EasyFileSplitter());
			formatos.Add (new ParteSF());
			formatos.Add (new ParteKamaleon2());
			formatos.Add (new ParteKamaleon());
			formatos.Add (new FileSplit());
			formatos.Add (new SplitFile_v1());
			formatos.Add (new ParteHacha_v2());
			formatos.Add (new ParteHacha_v1());
			formatos.Add (new ParteHachaPro());
			formatos.Add (new ParteGenerico());

			foreach (Parte p in formatos)
				p.Progress += new ProgressEventHandler (this.OnProgress);
		}
		
		
		
		/// <summary>
		/// Obtiene la �nica instancia de la clase.</summary>
		/// <returns>La �nica instancia de la clase.</returns>
		
		public static Manager Instance{
			get { return GetInstance(Thread.CurrentThread); }
		}
		
		/// <summary>Versi�n de la librer�a.</summary>
		
		public static Version Version{
			get{ return Assembly.GetExecutingAssembly().GetName().Version; }
		}
		
		/// <summary>
		/// Obtiene la �nica instancia de la clase.</summary>
		/// <returns>La �nica instancia de la clase.</returns>
		
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
			foreach (IParte p in formatos)
				if (p.PuedeUnir(fichero))
					return p;
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
		/// El nombre del formato que se utilizar� para partir el fichero.
		/// </param>
		/// <param name="fichero">El fichero a partir.</param>
		/// <param name="salida1">
		/// El nombre del fichero destino, o nombre base de los fragmentos.
		/// No todos los formatos hacen uso de este parametro.
		/// </param>
		/// <param name="dir">
		/// Directorio donde se pondr�n los fragmentos obtenidos.
		/// </param>
		/// <param name="kb">
		/// Tama�o de los fragmentos (hay formatos, como zip, que no hacen uso
		/// de este dato). 
		/// </param>
		
		public void Partir (String formato, String fichero, String salida1, String dir, long kb)
		{
			this.SetStopRequest (false);
			IParte p = GetFormatByName(formato);
			if (p==null)
				throw new FormatNotSupportedException (formato);
			if (!p.ParteFicheros)
				throw new SplitNotSupportedException (formato);
			try{
				p.Partir (fichero, salida1, dir, kb);
			}
			catch (StoppedByUserException){
				this.SetStopRequest (false);
			}

		}
		
		/// <summary>Parte un fichero</summary>
		/// <param name="formato">
		/// El nombre del formato que se utilizar� para partir el fichero.
		/// </param>
		/// <param name="fichero">
		/// El fichero a partir.
		/// </param>
		/// <param name="salida1">
		/// El nombre del fichero destino, o nombre base de los fragmentos.
		/// No todos los formatos hacen uso de este parametro.
		/// </param>
		/// <param name="kb">
		/// Tama�o de los fragmentos (hay formatos, como zip, que no hacen uso
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
			IParte f = null;
			foreach (IParte p in formatos){
				if (p.PuedeUnir(fichero)){
					f = p;
					break;
				}
			}
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
		/// de una operaci�n.</summary>
		
		public event ProgressEventHandler Progress;
		
		/// <summary>Provoca la detenci�n de la operaci�n actual.</summary>

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
		/// �nica instancia de la clase (patr�n singleton).
		/// </summary>
		
		private static ParteComparer instance = null;
		
		/// <summary>
		/// Constructor privado. No se permite crear instancias de esta 
		/// clase externamente.
		/// </summary>
		
		private ParteComparer()
		{
		}
		
		/// <summary>La �nica instancia de la clase.</summary>
		
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