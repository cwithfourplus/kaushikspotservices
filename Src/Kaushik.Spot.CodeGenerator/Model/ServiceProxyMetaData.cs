using Kaushik.Spot.CodeGenerator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kaushik.Spot.CodeGenerator.Model
{
    public enum OutputLanguage
    {
        [StringValue("CSharp")]
        CSharp,
        [StringValue("VB")]
        VB
    }

    public class ServiceProxyMetaData
    {
        public string AssemblyPath { get; set; }
        public List<Type> Services { get; set; }
        public Type SelectedService { get; set; }
        public string NamespaceName { get; set; }
        public string ProxyClassName { get; set; }
        public string Server { get; set; }
        public short Port { get; set; }
        public OutputLanguage OutputLanguage { get; set; }

        public ServiceProxyMetaData()
        {
            Services = new List<Type>();
            OutputLanguage = Model.OutputLanguage.CSharp;
            Port = 8080;
            NamespaceName = "ServiceProxy";
        }
    }
}
