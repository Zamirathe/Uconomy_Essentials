using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Uconomy_Essentials
{
    [XmlRoot("UconomyEConfiguration")]
    public class Group
    {
        [XmlAttribute]
        public string DisplayName { get; set;}
        [XmlAttribute]
        public decimal Salary { get; set; }
    }

    class GroupComparer : IEqualityComparer<Group>
    {
        public bool Equals(Group x, Group y)
        {

            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal. 
            return x.DisplayName == y.DisplayName && x.Salary == y.Salary;
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public int GetHashCode(Group g)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(g, null)) return 0;

            //Get hash code for the Name field if it is not null. 
            int hashGroupName = g.DisplayName == null ? 0 : g.DisplayName.GetHashCode();

            //Get hash code for the Salary field. 
            int hashGroupSalary = g.Salary.GetHashCode();

            //Calculate the hash code for the product. 
            return hashGroupName ^ hashGroupSalary;
        }

    }
}
