using Kaushik.Spot.CodeGenerator.Model;
using Kaushik.Spot.CodeGenerator.View;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;

namespace Kaushik.Spot.CodeGenerator.ViewModel
{
    public class ServiceProxyMetaDataViewModel : ViewModelBase
    {
        private string assemblyPath;
        private ObservableCollection<Type> services;
        private ObservableCollection<OutputLanguage> languageOptions;
        private Type selectedService;
        private string namespaceName;
        private string proxyClassName;
        private string server;
        private short port;
        private OutputLanguage outputLanguage;
        private readonly SimpleRelayCommand browseAssemblyAndLoad;
        private readonly SimpleRelayCommand generateProxy;

        public SimpleRelayCommand GenerateProxy
        {
            get { return generateProxy; }
        }


        public SimpleRelayCommand BrowseAssemblyAndLoad
        {
            get { return browseAssemblyAndLoad; }
        }

        public string AssemblyPath
        {
            get { return assemblyPath; }
            set { SetProperty(ref assemblyPath, value); }
        }

        public ObservableCollection<Type> Services
        {
            get { return services; }
            set
            {
                if (value == null)
                {
                    services.Clear();
                }
                else
                {
                    SetProperty(ref services, value);
                }
            }
        }

        public ObservableCollection<OutputLanguage> LanguageOptions
        {
            get { return languageOptions; }
            set
            {
                if (value == null)
                {
                    languageOptions.Clear();
                }
                else
                {
                    SetProperty(ref languageOptions, value);
                }
            }
        }

        public Type SelectedService
        {
            get { return selectedService; }
            set
            {
                if (SetProperty(ref selectedService, value))
                {
                    if (selectedService.Name.StartsWith("I"))
                    {
                        ProxyClassName = selectedService.Name.Substring(1, selectedService.Name.Length - 1);
                    }
                    else
                    {
                        ProxyClassName = selectedService.Name + "Proxy";
                    }
                }
            }
        }

        public string NamespaceName
        {
            get { return namespaceName; }
            set { SetProperty(ref namespaceName, value); }
        }

        public string ProxyClassName
        {
            get { return proxyClassName; }
            set { SetProperty(ref proxyClassName, value); }
        }

        public string Server
        {
            get { return server; }
            set { SetProperty(ref server, value); }
        }

        public short Port
        {
            get { return port; }
            set { SetProperty(ref port, value); }
        }

        public OutputLanguage OutputLanguage
        {
            get { return outputLanguage; }
            set { SetProperty(ref outputLanguage, value); }
        }


        public ServiceProxyMetaDataViewModel(ServiceProxyMetaData serviceProxyMetaData)
        {
            generateProxy = new SimpleRelayCommand(generateProxyAction);
            browseAssemblyAndLoad = new SimpleRelayCommand(browseAssemblyAndLoadAction);


            assemblyPath = serviceProxyMetaData.AssemblyPath;
            selectedService = serviceProxyMetaData.SelectedService;
            namespaceName = serviceProxyMetaData.NamespaceName;
            proxyClassName = serviceProxyMetaData.ProxyClassName;
            server = serviceProxyMetaData.Server;
            port = serviceProxyMetaData.Port;
            outputLanguage = serviceProxyMetaData.OutputLanguage;
            services = new ObservableCollection<Type>();

            languageOptions = new ObservableCollection<OutputLanguage>();
            languageOptions.Add(Model.OutputLanguage.CSharp);
            languageOptions.Add(Model.OutputLanguage.VB);

            if (serviceProxyMetaData.Services != null)
            {
                foreach (Type service in serviceProxyMetaData.Services)
                {
                    services.Add(service);
                }
            }
        }

        public ServiceProxyMetaData ToModel()
        {
            ServiceProxyMetaData serviceProxyMetaData = new ServiceProxyMetaData();

            serviceProxyMetaData.AssemblyPath = this.AssemblyPath;
            serviceProxyMetaData.SelectedService = this.SelectedService;
            serviceProxyMetaData.NamespaceName = this.NamespaceName;
            serviceProxyMetaData.ProxyClassName = this.ProxyClassName;
            serviceProxyMetaData.Server = this.Server;
            serviceProxyMetaData.Port = this.Port;
            serviceProxyMetaData.OutputLanguage = this.OutputLanguage;

            serviceProxyMetaData.Services.Clear();

            if (this.Services != null)
            {
                foreach (Type service in this.Services)
                {
                    serviceProxyMetaData.Services.Add(service);
                }
            }

            return serviceProxyMetaData;
        }

        private void generateProxyAction(object obj)
        {
            try
            {
                if (SelectedService != null)
                {
                    string proxyCode = Model.ProxyGenerator.GenerateProxy(this.ToModel());

                    ShowProxyCode window = new ShowProxyCode();

                    window.DataContext = proxyCode;

                    window.Show();
                }
                else
                {
                    MessageBox.Show("Please load an Assembly containing SPOT service(s) and select one of them to generate proxy!");
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void browseAssemblyAndLoadAction(object obj)
        {
            OpenFileDialog file = new OpenFileDialog();

            file.Filter = "*.dll|*.exe";

            Nullable<bool> result = file.ShowDialog();

            if (result.HasValue && result.Value)
            {
                AssemblyPath = file.FileName;

                try
                {
                    ServiceProxyMetaData serviceProxyMetaData = Model.ProxyGenerator.LoadAssembly(this.ToModel());

                    services.Clear();

                    if (serviceProxyMetaData.Services != null)
                    {
                        foreach (Type service in serviceProxyMetaData.Services)
                        {
                            services.Add(service);
                        }
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }

            }
        }
    }
}
