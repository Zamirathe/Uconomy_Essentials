using System.Collections.Generic;

namespace ZaupUconomyEssentials
{
    internal class GroupComparer : IEqualityComparer<Group>
    {
        public bool Equals(Group x, Group y)
        {
            //Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.DisplayName == y.DisplayName && x.Salary == y.Salary;
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(Group g)
        {
            //Get hash code for the Name field if it is not null.
            var hashGroupName = g.DisplayName == null ? 0 : g.DisplayName.GetHashCode();

            //Get hash code for the Salary field.
            var hashGroupSalary = g.Salary.GetHashCode();

            //Calculate the hash code for the product.
            return hashGroupName ^ hashGroupSalary;
        }
    }
}