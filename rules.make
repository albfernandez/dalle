

MONO_PATH=:/usr/share/dotnet/mono/1.0/:/usr/share/dotnet/mono/gtk-sharp/

INTERFACES=console gtk glade swf
MAKE = make
LIBNAME= libDalle.dll
BUILDDIR=$(BASEPATH)/build
LIBRARY=$(BUILDDIR)/$(LIBNAME)
PROGRAMA=dalle
MCS=MONO_PATH=$(MONO_PATH) mcs
WARN_LEVEL=4
WARN_OPTIONS=-warn:$(WARN_LEVEL)
MCS_FLAGS=$(WARN_OPTIONS) -g

GTK_RESOURCES=-r gtk-sharp -r glib-sharp -r gdk-sharp
QT_RESOURCES=-r Qt.dll
SWT_RESOURCES=-r ICSharpCode.SWT.dll


MONO=mono
MONO_FLAGS=--debug
RESGEN=monoresgen

AUXRESGEN=$(BUILDDIR)/Files2Resource.exe

LINEA = "======================================================================"

PREFIX=/usr
DEST_DIR=

CSDOC=csdoc
APIDOCSBASE=$(BASEPATH)/api-docs

INSTALL = install -m 755 
MKDIR = install -d
DELETE = rm -f

GETSTRINGS=$(BASEPATH)/src/soporte/getstrings.pl
