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


BASEPATH=../../../..
include $(BASEPATH)/rules.make

PROGNAME=dalle-crcs.exe
WRAPPERNAME=dalle-crcs
TARGET=$(BUILDDIR)/$(PROGNAME)
WRAPPER=$(BUILDDIR)/$(WRAPPERNAME)

LANGDIR=languages
TRADUCCIONES=$(LANGDIR)/Mensajes.en.resources\
		$(LANGDIR)/Mensajes.es.resources

RECURSOS_TRADUCCIONES=-resource:$(LANGDIR)/Mensajes.en.resources,Mensajes.en.resources \
		-resource:$(LANGDIR)/Mensajes.es.resources,Mensajes.es.resources

LANGS=en es
LANGDIR=languages
TRAD=$(LANGDIR)/strings.resources $(addprefix $(LANGDIR)/strings., $(addsuffix .resources, $(LANGS)))
RESOURCES=$(TRAD)
RESS= $(foreach res,$(RESOURCES), $(addprefix -resource:,$(res)),$(notdir $(res)))

all: $(TARGET)


$(TARGET): $(TRAD) src/crcs.cs
	@echo $(LINEA)
	@echo "Compilando comprobador de crcs de prueba."
	@echo $(LINEA)
	$(MCS) $(MCS_FLAGS) $(RESS) -out:$(TARGET) -r:$(LIBRARY) -r:ICSharpCode.SharpZipLib src/crcs.cs
	@echo "#!/bin/sh" > $(WRAPPER)
	@echo "" >> $(WRAPPER)
	@echo -n "$(MONO) $(MONO_FLAGS) $(PREFIX)/lib/dalle/$(PROGNAME) " >>$(WRAPPER)
	@echo -n "\"$$" >> $(WRAPPER)
	@echo -n "@\"" >> $(WRAPPER)
	@echo "" >> $(WRAPPER)

$(TRAD):$(LANGDIR)/strings%resources: $(LANGDIR)/strings%txt
	@echo $(LINEA)
	@echo "Generando los recursos de lenguajes: $@."
	@echo $(LINEA)
	$(RESGEN) $< $@
	
$(LANGDIR)/strings.txt: src/*.cs
	$(GETSTRINGS) src/*.cs > $@

install: $(TARGET)
	#$(MKDIR) $(DESTDIR)/$(PREFIX)/lib/dalle
	#$(INSTALL) $(TARGET) $(DESTDIR)/$(PREFIX)/lib/dalle
	#$(INSTALL) $(WRAPPER) $(DESTDIR)/$(PREFIX)/bin
	@echo No se instala dalle-crcs

uninstall:
	$(DELETE) $(DESTDIR)/$(PREFIX)/bin/$(WRAPPERNAME)
	$(DELETE) $(DESTDIR)/$(PREFIX)/lib/dalle/$(PROGNAME)

clean:
	rm -f $(TARGET) $(WRAPPER) $(LANGDIR)/*.resources

.PHONY: clean uninstall
