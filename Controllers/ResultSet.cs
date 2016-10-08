using System.Collections.Generic;

namespace Firefly.Controllers{
    
    public struct MetaData {
        public string Prev;
        public string Next;
        public int CurrentPage;
        public int PerPage;
        public int? Total;
    }
    
    public class ResultSet<T>{
        
        public MetaData Meta { get; set;}
        public IEnumerable<T> Data { get; set;}
    }
}