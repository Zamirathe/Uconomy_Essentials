using System.Xml.Serialization;

namespace ZaupUconomyEssentials
{
    [XmlRoot("UconomyEConfiguration")]
    public class Group
    {
        [XmlAttribute] public string DisplayName { get; set; }
        [XmlAttribute] public decimal Salary { get; set; }
    }
}