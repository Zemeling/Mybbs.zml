using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace My.Model.Core.Base
{
    [Serializable]
    [DataContract(IsReference = true)]
    [ProtoContract]
    public abstract class Entity : EntityBase
    {
        [ProtoMember(1)]
        [DefaultValue(false)]
        [DataMember]
        public bool IsDeleted
        {
            get;
            set;
        }

        [ProtoMember(2)]
        [DataMember]
        [Display(Name = "创建人")]
        public string CreatedBy
        {
            get;
            set;
        }

        [DataMember]
        [Display(Name = "创建时间")]
        [ProtoMember(3)]
        public DateTime CreatedDate
        {
            get;
            set;
        }

        [DataMember]
        [ProtoMember(4)]
        [Display(Name = "修改人")]
        public string ModifiedBy
        {
            get;
            set;
        }

        [DataMember]
        [ProtoMember(5)]
        [Display(Name = "修改时间")]
        public DateTime ModifiedDate
        {
            get;
            set;
        }

        protected Entity()
        {
            this.IsDeleted = false;
            this.CreatedDate = DateTime.UtcNow;
            this.ModifiedDate = DateTime.UtcNow;
        }

        protected XDocument GetDocumentRoot()
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("Entity");
            doc.Add(root);
            return doc;
        }

        protected XElement GetRootElement(string xml)
        {
            XDocument formXmlDocument = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
            XElement element = formXmlDocument.Root;
            if (element == null || element.Name != (XName)"Entity")
            {
                throw new ArgumentException("Invalid Entity Definition");
            }
            return element;
        }

        protected virtual string GetEnityObjectXml()
        {
            XDocument doc = this.GetDocumentRoot();
            this.SerializeToXml(doc.Root);
            return doc.ToString(SaveOptions.DisableFormatting);
        }

        protected virtual XElement SerializeToXml(XElement element)
        {
            element.Add(new XElement("IsDeleted", this.IsDeleted));
            element.Add(new XElement("CreatedBy", this.CreatedBy));
            element.Add(new XElement("CreatedDate", this.CreatedDate));
            element.Add(new XElement("ModifiedBy", this.ModifiedBy));
            element.Add(new XElement("ModifiedDate", this.ModifiedDate));
            return element;
        }

        protected Entity(string xml)
        {
            try
            {
                XElement element = this.GetRootElement(xml);
                this.IsDeleted = (element.Element("IsDeleted") != null && bool.Parse(element.Element("IsDeleted").Value));
                this.CreatedBy = ((element.Element("CreatedBy") != null) ? element.Element("CreatedBy").Value : string.Empty);
                this.CreatedDate = ((element.Element("CreatedDate") != null) ? DateTime.Parse(element.Element("CreatedDate").Value) : DateTime.UtcNow);
                this.ModifiedBy = ((element.Element("ModifiedBy") != null) ? element.Element("ModifiedBy").Value : string.Empty);
                this.ModifiedDate = ((element.Element("ModifiedDate") != null) ? DateTime.Parse(element.Element("ModifiedDate").Value) : DateTime.UtcNow);
            }
            catch (Exception)
            {
                throw new Exception("Invalid Entity Definition");
            }
        }
    }
}
