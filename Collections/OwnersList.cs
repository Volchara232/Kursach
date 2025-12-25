using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sem3_kurs.Model;

namespace Sem3_kurs.Collections
{
    public class OwnersList
    {
        public List<Owner> Owners { get; } = new List<Owner>();

        public void AddOwner(Owner owner) => Owners.Add(owner);
    }

}
