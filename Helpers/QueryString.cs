using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;

namespace Firefly.Helpers {
    public class QueryString{

        private Dictionary<string,string> store;
        public QueryString(Dictionary<string, string> values){
            store = values;
        }

        public QueryString(){
            store = new Dictionary<string, string>();
        }

        public QueryString Add(string key, string val){
            store.Add(key, val);
            return this;
        }

        public QueryString Replace(string key, string val){
            store[key] = val;
            return this;
        }

        public QueryString Remove(string key){
            store.Remove(key);
            return this;
        }

        public override string ToString() {
            var array = (from pair in store select string.Format("{0}={1}", HtmlEncoder.Default.Encode(pair.Key), HtmlEncoder.Default.Encode(pair.Value)));        
            return "?" + string.Join("&", array);
        }
    }
}