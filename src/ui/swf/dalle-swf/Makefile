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

PROGNAME=dalle-swf.exe
WRAPPERNAME=dalle-swf
TARGET=$(BUILDDIR)/$(PROGNAME)
WRAPPER=$(BUILDDIR)/$(WRAPPERNAME)
SOURCEDIR=src


LANGS=en es
LANGDIR=languages
TRAD=$(LANGDIR)/strings.resources $(addprefix $(LANGDIR)/strings., $(addsuffix .resources, $(LANGS)))
RESOURCES=$(TRAD) 
RESS= $(foreach res,$(RESOURCES), $(addprefix -resource:,$(res)),$(notdir $(res)))

SWF_RESOURCES=-r System.Windows.Forms -r System.Drawing
all: $(TARGET)


$(TARGET): $(LIBRARY) $(TRAD) $(SOURCEDIR)/*.cs
	@echo "$(LINEA)"
	@echo "Compilando $(PROGRAMA)-swf"
	@echo "$(LINEA)"
	$(MCS) $(MCS_FLAGS) -o $(TARGET) -r $(LIBRARY) $(SWF_RESOURCES) $(RESS)  -recurse:$(SOURCEDIR)/*.cs
	@echo "#!/bin/sh" > $(WRAPPER)
	@echo "" >> $(WRAPPER)
	@echo "$(MONO) $(MONO_FLAGS) $(PREFIX)/bin/$(PROGNAME)" >>$(WRAPPER)
	@echo "" >> $(WRAPPER)

install: all
	$(INSTALL) $(TARGET) $(DESTDIR)/$(PREFIX)/bin
	$(INSTALL) $(WRAPPER) $(DESTDIR)/$(PREFIX)/bin
	
uninstall:
	$(DELETE) $(DESTDIR)/$(PREFIX)/bin/$(WRAPPERNAME)
	$(DELETE) $(DESTDIR)/$(PREFIX)/bin/$(PROGNAME)

$(TRAD):$(LANGDIR)/strings%resources: $(LANGDIR)/strings%txt
	@echo $(LINEA)
	@echo "Generando los recursos de lenguajes: $@."
	@echo $(LINEA)
	$(RESGEN) $< $@
	
$(LANGDIR)/strings.txt: src/*.cs
	$(GETSTRINGS) src/*.cs > $@
	
clean:
	rm -f $(TARGET) $(WRAPPER) $(LANGDIR)/*.resources
	
.PHONY: clean uninstall
