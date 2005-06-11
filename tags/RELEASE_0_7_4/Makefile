# This program is free software; you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation; either version 2 of the License, or
# (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU Library General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program; if not, write to the Free Software
# Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.


BASEPATH=.
include rules.make


PIXMAPS=$(BUILDDIR)/Pixmaps.resources
RESOURCES=-resource:$(PIXMAPS),Pixmaps.resources

MCS_FLAGS+= -r ICSharpCode.SharpZipLib.dll

APIDOCS = $(APIDOCSBASE)/libDalle

LANGS=en es
LANGDIR=src/lib/languages
TRAD=$(LANGDIR)/strings.resources $(addprefix $(LANGDIR)/strings., $(addsuffix .resources, $(LANGS)))
RESOURCES=$(PIXMAPS) $(TRAD)
RESS= $(foreach res,$(TRAD), $(addprefix -resource:,$(res)),$(notdir $(res)))





REC_TRAD= $(foreach res,$(TRAD), $(addprefix -resource:,$(res)),$(notdir $(res)))


all: $(LIBRARY) interfaces tests


install: all
	@echo $(LINEA)
	@echo Instalando...
	@echo $(LINEA)
	$(MKDIR) $(DESTDIR)/$(PREFIX)/bin
	$(MKDIR) $(DESTDIR)/$(PREFIX)/lib
	$(INSTALL) $(LIBRARY) $(DESTDIR)/$(PREFIX)/lib
	@list='$(INTERFACES)'; for d in $$list ; do \
	    (cd src/ui/$$d && $(MAKE) install) || exit 1 ; \
	done

uninstall:
	@echo $(LINEA)
	@echo Desinstalando...
	@echo $(LINEA)
	$(DELETE) $(DESTDIR)/$(PREFIX)/lib/$(LIBNAME)
	@list='$(INTERFACES)'; for d in $$list ; do \
	    (cd src/ui/$$d && $(MAKE) uninstall) || exit 1 ; \
	done


$(LIBRARY) : $(PIXMAPS) $(TRAD) $(shell find src/lib -name "*.cs")
	install -d $(BUILDDIR)
	@echo $(LINEA)
	@echo "Compilando la libreria $(LIBRARY)."
	@echo $(LINEA)
	$(MCS) $(MCS_FLAGS) -target:library $(RESS) -resource:$(PIXMAPS),Pixmaps.resources -o $(LIBRARY) -recurse:src/lib/*.cs

#Construcción de los recursos de imágenes.

$(PIXMAPS): $(AUXRESGEN) pixmaps/Piel-01.jpg
	@echo $(LINEA)
	@echo "Generando los recursos de imágenes."
	@echo $(LINEA)
	$(MONO) $(MONO_FLAGS) $(AUXRESGEN) pixmaps/Piel-01.jpg $(PIXMAPS)

$(AUXRESGEN): src/soporte/Files2Resource.cs
	@echo $(LINEA)
	@echo "Compilando generador de recursos."
	@echo $(LINEA)
	install -d $(BUILDDIR)
	$(MCS) $(MCS_FLAGS) -o $(AUXRESGEN) src/soporte/Files2Resource.cs

#Construccion de las distintas interfaces de usuario.
interfaces:
	@list='$(INTERFACES)'; for d in $$list ; do \
	    (cd src/ui/$$d && $(MAKE)) || exit 1 ; \
	done

tests:
	(cd src/tests && $(MAKE)) || exit 1
	


#Construcción de los paquetes del lenguaje.

$(TRAD):$(LANGDIR)/strings%resources: $(LANGDIR)/strings%txt
	@echo $(LINEA)
	@echo "Generando los recursos de lenguajes: $@."
	@echo $(LINEA)
	$(RESGEN) $< $@
	
$(LANGDIR)/strings.txt: $(shell find src/lib/ -name "*.cs")
	$(GETSTRINGS) $(shell find src/lib/ -name "*.cs") > $@


#Documentación
docs:
	install -d $(APIDOCS)
	rm -f $(APIDOCS)/doc.xml
ifeq ($(CSDOC),csdoc)
		csdoc -w -fprivate -l ICSharpCode.SharpZipLib.dll -o $(APIDOCS)/doc.xml $(shell find src/lib -name "*.cs")
		csdoc2html -ftitle=libDalle -o $(APIDOCS) $(APIDOCS)/doc.xml
else
		@echo No puede generar la documentacion.
		@echo Necesita Portable.NET.
endif

clean-docs:
	rm -rf $(APIDOCS)/*
wc:
	wc $(shell find src -name "*.cs")

zip:
	install -d ../backup
	zip -9r ../backup/$(shell date +%y%m%d_%H%M%s.zip) src doc* Makefile* configure* rules.make* TODO Changelog *.txt

#Elimina los archivos generados al compilar
clean:
	rm -rf build/*
	rm -rf api-docs/
	rm -f $(LANGDIR)/*.resources
	
	@list='$(INTERFACES)'; for d in $$list ; do \
	    (cd src/ui/$$d && $(MAKE) clean) || exit 1 ; \
	done
	(cd src/tests && $(MAKE) clean) || exit 1

distclean: clean
	echo "" > rules.make
	rm -f $(shell find . -name "*.resources")
	
.PHONY: clean docs zip interfaces uninstall dist-clean tests
