using System.Data;
using System.Data.Common;
using WorldManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldManagementApi.Repos
{
    public class PeopleRepo
    {
        private WorldContext db;

        public PeopleRepo(WorldContext db)
        {
            this.db = db;
        }

        public Person AddPerson(Person person){
            if (db != null)
            {
                var addedPerson = db.People.Add(person);
                    db.SaveChanges();
                
                return addedPerson.Entity;
            }
            return null;
        }

        public Person UpdatePerson(Person person)
        {
            if (db != null)
            {
                var updatedPerson = db.People.Update(person);
                db.SaveChanges();
                
                return updatedPerson.Entity;
            }
            return null;
        }
    }
}