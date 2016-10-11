
namespace Firefly.Configuration{

    public class Config
    {
        public Keys Keys { get; set; }
        public DbContextSettings DbContextSettings { get; set; }
        public Resources Resources { get; set; }
    }
    
    public struct Keys {
                public bool OwnCertificate {get;set;}
                public string CertificatePath { get; set; }
                public string CertificatePassword{ get; set; }
            }
    
    public struct DbContextSettings {
        public string ConnectionString {get;set;}
        public string DefaultSchema {get;set;}
    }

    public struct Resources {
        public int DefaultLimit { get; set; }
        public bool ShowTotalCount { get; set; }
    }

}