using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.DataModels.ManualWorksXMLs
{
    public class xmlSet
    {
        public xmlBook Book { get; set; } = new xmlBook();
        public xmlChapter Chapter { get; set; }  = new xmlChapter();
        public xmlElement ImageElement { get; set; } = new xmlElement() { Config = new xmlImageConfig() };
        public xmlElement OrderedListElement { get; set; } = new xmlElement() { Config = new xmlOrderListConfig() };
        public xmlElement TableElement { get; set; } = new xmlElement() { Config = new xmlTableConfig() };
        public xmlElement TextElement { get; set; } = new xmlElement() { Config = new xmlNormalTextConfig() };
        public xmlElement UnorderedListElement { get; set; } = new xmlElement() { Config = new xmlUnOrderListConfig() };
        public xmlElement Heading1Element { get; set; } = new xmlElement() {Config = new xmlHeading1Config() };
        public xmlElement Heading2Element { get; set; } = new xmlElement();
        public xmlElement Heading3Element { get; set; } = new xmlElement();
        public xmlElement Heading4Element { get; set; } = new xmlElement();
        public xmlElement Heading5Element { get; set; } = new xmlElement() { Config = new xmlHeading1Config() };
        public xmlElement NoteElement { get; set; } = new xmlElement() { Config = new xmlNoteCongif() };

    }
}
