using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JSport.Lucene.Net.Analysis.En;
using Raven.Client.Documents.Indexes;

namespace MyWebApplication472
{
    public class Employees_En_Gb : AbstractIndexCreationTask<Employee>
    {
        public Employees_En_Gb()
        {
            Map = employees => from doc in employees
                              select new
                              {
                                  LastName = doc.LastName,
                                  FirstName = doc.FirstName,
                                  Title = doc.Title 
                              };
            Index(x => x.Title, FieldIndexing.Search);
            Analyze(x => x.Title, typeof(EnglishAnalyzer).AssemblyQualifiedName);
        }
    }

    public class Employee
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }
    }
}