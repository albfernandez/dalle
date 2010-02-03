/*

	Dalle - A split/join file utility library
	Dalle.Formatos.IParte - Common abstract base class for all formats.
	
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
using System.Collections;	
using System.IO;


namespace Dalle.Formatos
{

	/// <summary>Clase base para todos los formatos.</summary>
		
	public abstract class Parte : IParte
	{
		
		/// <summary>
		/// Nombre del formato.
		/// </summary>
		
		protected string nombre = string.Empty;

		/// <summary>
		/// Breve descripción del formato.
		/// </summary>
		
		protected string descripcion = string.Empty;

		/// <summary>
		/// Dirección de la página oficial del programa original.
		/// </summary>
		
		protected string web = string.Empty;

		/// <summary>
		/// Indica si crea ficheros que pueden unirse con 
		/// copy en DOS y con cat en *nix
		/// </summary>
		
		protected bool compatible = false;

		/// <summary>
		/// Indica si tiene soporte para partir ficheros.
		/// </summary>
		
		protected bool parteFicheros = false;
		
		/// <summary>
		/// Indica si se debe detener la operación.
		/// </summary>
		
		private bool stopRequest = false;

		/// <summary>Obtiene la descripción del formato.</summary>
		
		public String Descripcion{
			get{ return descripcion;}
		}
		/// <summary>Obtiene el nombre del formato.</summary>
		
		public String Nombre{
			get{ return nombre;}
		}

		/// <summary>Obtiene la dirección web del programa original.</summary>
		
		public String Web{
			get{ return web; }
		}
		
		/// <summary>Obtiene si puede partir ficheros.</summary>
		
		public bool ParteFicheros{
			get{ return parteFicheros; }
		}

		/// <summary>
		/// Obtiene si crea ficheros compatibles con copy y cat.
		/// </summary>
		
		public bool Compatible{
			get{ return compatible;	}
		}

		protected Parte ()
		{
		}

		protected Parte (string nombre, string descripcion, string web,
			bool compatible, bool parteFicheros)
		{
			this.nombre = nombre;
			this.descripcion = descripcion;
			this.web = web;
			this.compatible = compatible;
			this.parteFicheros = parteFicheros;
		
		}
		/// <summary>
		/// Método que deben implementar las clases 
		/// herederas para unir ficheros.
		/// </summary>
		/// <param name="fichero">El nombre del fichero a unir</param>
		/// <param name="dirDest">
		/// El directorio donde se pondran los nuevos archivos.
		/// </param>
		
		protected abstract void _Unir (string fichero, string dirDest);
		
		/// <summary>
		/// Método que deben implementar las clases 
		/// herederas para partir ficheros.
		/// </summary>
		/// <param name="fichero">El nombre del fichero a partir.</param>
		/// <param name="sal1">
		/// Nombre del archivo de destino, o nombre base de los fragmentos.
		/// </param>
		/// <param name="dir">
		/// Directorio donde se pondran los nuevos archivos.
		/// </param>
		/// <param name="kb">
		/// Tamaño de los fragmentos generados (en kb).
		/// </param>
		
		protected abstract void _Partir (string fichero, string sal1, string dir, long kb);
		
		/// <summary>
		/// Método que deben implementar las clases herederas 
		/// para indicar si puede unir un determinado fichero.
		/// </summary>
		/// <param name="fichero">
		/// Nombre del fichero que queremos comprobar.
		/// </param>
		/// <returns>
		/// true si el formato puede unir fichero, false en otro caso.
		/// </returns>
		
		public abstract bool PuedeUnir (string fichero);
		
		

		/// <summary>Parte un fichero.</summary>
		/// <param name="fichero">El fichero a partir</param>
		/// <param name="sal1">
		/// Nombre del archivo de destino, o nombre base de los fragmentos.
		/// </param>
		/// <param name="dir">
		/// Directorio donde se pondran los nuevos archivos.
		/// </param>
		/// <param name="kb">
		/// Tamaño de los fragmentos generados (en kb).
		/// </param>
		
		public void Partir (string fichero, string sal1, string dir, long kb)
		{
			if (! Directory.Exists (dir))
				new DirectoryInfo(dir).Create();

			_Partir (fichero, sal1, dir, kb);

		}

		/// <summary>Une un fichero</summary>
		/// <param name="fichero">El nombre del fichero a unir</param>
		/// <param name="dirDest">
		/// El directorio donde se pondran los nuevos archivos.
		/// </param>

		public void Unir (String fichero, String dirDest)
		{
			if (! Directory.Exists (dirDest)){
				//TODO: Poner una excepción personalizada.
				if (File.Exists(dirDest))
					throw new Dalle.Formatos.FileAlreadyExistsException("", dirDest);
				new DirectoryInfo(dirDest).Create();
			}
			_Unir (fichero, dirDest);
		}
		
		/// <summary>Parte un fichero.</summary>
		/// <param name="fichero">El fichero a partir</param>
		/// <param name="sal1">
		/// Nombre del archivo de destino, o nombre base de los fragmentos.
		/// </param>
		/// <param name="kb">
		/// Tamaño de los fragmentos generados (en kb).
		/// </param>

		public void Partir (String fichero, String sal1, long kb)
		{
			this.Partir (
				fichero, 
				new FileInfo(sal1).Name, 
				new FileInfo(fichero).DirectoryName, 
				kb);
		}

		/// <summary>
		/// Método que deben implementar las clases 
		/// herederas para unir ficheros.
		/// </summary>
		/// <param name="fichero">El nombre del fichero a unir</param>
		/// <param name="dirDest">
		/// El directorio donde se pondran los nuevos archivos.
		///</param>

		public void Unir (String fichero)
		{
			this.Unir (fichero, new FileInfo(fichero).DirectoryName);
		}
		
		/// <summary>Provoca el evento Progress</summary>
		/// <param name="done">La tarea realizada</param>
		/// <param name="total">El total a realizar</param>
		
		protected void OnProgress (long done, long total)
		{
			if (Progress != null)
				Progress (done, total);
			if (this.stopRequest)
				throw new StoppedByUserException();
		}
		
		internal void SetStopRequest (bool value)
		{
			stopRequest = value;
		}
		
		/// <summary>Evento que se provoca para mostrar el progreso
		/// de una operación.</summary>
		
		public event ProgressEventHandler Progress;	
	}
}
