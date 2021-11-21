using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.CodeDom;
using System.Reflection;
using Kaushik.Spot.Library;
using System.Net.Sockets;
using System.CodeDom.Compiler;
using System.IO;

namespace Kaushik.Spot.CodeGenerator.Model
{
    public static class ProxyGenerator
    {
        private const short BUFFERSIZE = 5120;
        private const int SOCKETCLIENTTIMEOUT = 50000000;
        private const short CHUNKSIZE = 500;
        private const string COMMENT = "/**********************************************************************************\n This class is generated using Kaushik.Spot.CodeGenerator, \n any change to this file may lead to malfunction. \n **********************************************************************************/\n";

        public static string GenerateProxy(ServiceProxyMetaData serviceProxyMetaData)
        {
            string code = null;

            try
            {
                CodeCompileUnit compileUnit = new CodeCompileUnit();

                #region Create Namespace
                CodeNamespace namespaceName = new CodeNamespace(serviceProxyMetaData.NamespaceName);
                compileUnit.Namespaces.Add(namespaceName);
                namespaceName.Imports.Add(new CodeNamespaceImport("System"));
                namespaceName.Imports.Add(new CodeNamespaceImport("System.Net.Sockets"));
                namespaceName.Imports.Add(new CodeNamespaceImport("Kaushik.Spot.Library"));
                #endregion Create Namespace

                #region Create Class
                CodeTypeDeclaration proxyClass = new CodeTypeDeclaration(serviceProxyMetaData.ProxyClassName);
                proxyClass.IsClass = true;
                proxyClass.TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed;
                namespaceName.Types.Add(proxyClass);
                #endregion Create Class

                #region Create Members
                CodeMemberField server = new CodeMemberField(typeof(System.String), "server");
                proxyClass.Members.Add(server);
                server.Attributes = MemberAttributes.Private;

                CodeMemberField port = new CodeMemberField(typeof(short), "port");
                port.Attributes = MemberAttributes.Private;
                proxyClass.Members.Add(port);

                CodeMemberField bufferSize = new CodeMemberField(typeof(short), "bufferSize");
                bufferSize.Attributes = MemberAttributes.Private;
                proxyClass.Members.Add(bufferSize);

                CodeMemberField socketClientTimeout = new CodeMemberField(typeof(int), "socketClientTimeout");
                socketClientTimeout.Attributes = MemberAttributes.Private;
                proxyClass.Members.Add(socketClientTimeout);

                CodeMemberField chunkSize = new CodeMemberField(typeof(short), "chunkSize");
                chunkSize.Attributes = MemberAttributes.Private;
                proxyClass.Members.Add(chunkSize);

                CodeMemberField serviceName = new CodeMemberField(typeof(string), "serviceName");
                serviceName.Attributes = MemberAttributes.Private;
                proxyClass.Members.Add(serviceName);

                CodeMemberField cipher = new CodeMemberField("ICryptographicServiceProvider", "cipher");
                cipher.Attributes = MemberAttributes.Private;
                proxyClass.Members.Add(cipher);

                CodeMemberField socketClient = new CodeMemberField("ISpotSocketClient", "socketClient");
                socketClient.Attributes = MemberAttributes.Private;
                proxyClass.Members.Add(socketClient);
                #endregion Create Members

                #region Create Default Constructor
                CodeConstructor defaultConstructor = new CodeConstructor();
                defaultConstructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                defaultConstructor.ChainedConstructorArgs.Add(new CodePrimitiveExpression(null));
                defaultConstructor.ChainedConstructorArgs.Add(new CodePrimitiveExpression(serviceProxyMetaData.Server));
                defaultConstructor.ChainedConstructorArgs.Add(new CodePrimitiveExpression(serviceProxyMetaData.Port));
                defaultConstructor.ChainedConstructorArgs.Add(new CodeSnippetExpression("SpotSocketProvider.Tcp"));

                proxyClass.Members.Add(defaultConstructor);

                #endregion Create Default Constructor

                #region Create Constructor
                CodeConstructor constructor = new CodeConstructor();
                constructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                CodeParameterDeclarationExpression cipherParameter = new CodeParameterDeclarationExpression("ICryptographicServiceProvider", "cipher");
                constructor.Parameters.Add(cipherParameter);

                CodeParameterDeclarationExpression serverParameter = new CodeParameterDeclarationExpression(typeof(string), "server");
                constructor.Parameters.Add(serverParameter);

                CodeParameterDeclarationExpression portParameter = new CodeParameterDeclarationExpression(typeof(short), "port");
                constructor.Parameters.Add(portParameter);

                CodeParameterDeclarationExpression providerParameter = new CodeParameterDeclarationExpression("SpotSocketProvider", "provider");
                constructor.Parameters.Add(providerParameter);

                constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), server.Name), new CodeArgumentReferenceExpression(serverParameter.Name)));
                constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), port.Name), new CodeArgumentReferenceExpression(portParameter.Name)));
                constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), bufferSize.Name), new CodePrimitiveExpression(BUFFERSIZE)));
                constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), socketClientTimeout.Name), new CodePrimitiveExpression(SOCKETCLIENTTIMEOUT)));
                constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), chunkSize.Name), new CodePrimitiveExpression(CHUNKSIZE)));
                constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), serviceName.Name), new CodePrimitiveExpression(serviceProxyMetaData.SelectedService.FullName)));
                constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), cipher.Name), new CodeArgumentReferenceExpression(cipherParameter.Name)));


                CodeExpression[] createClientParameters = new CodeExpression[] 
                   { 
                       new CodeArgumentReferenceExpression(server.Name), 
                       new CodeArgumentReferenceExpression(providerParameter.Name), 
                       new CodeArgumentReferenceExpression(port.Name),
                       new CodeArgumentReferenceExpression(bufferSize.Name), 
                       new CodeArgumentReferenceExpression(socketClientTimeout.Name), 
                       new CodeArgumentReferenceExpression(chunkSize.Name) 
                   };

                CodeMethodInvokeExpression createClient = new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("SpotSocketFactory"), "GetClientProvider", createClientParameters);
                constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), socketClient.Name), createClient));

                proxyClass.Members.Add(constructor);

                #endregion Create Constructor

                #region Create Encrypt and Decrypt Methods

                CodeMemberMethod helperMethod = getEncrptAndDecryptMethod(cipher, "encrypt", "Encrypt");
                proxyClass.Members.Add(helperMethod);

                helperMethod = getEncrptAndDecryptMethod(cipher, "decrypt", "Decrypt");
                proxyClass.Members.Add(helperMethod);

                #endregion Create Encrypt and Decrypt Methods

                #region Create Service Methods

                if (serviceProxyMetaData.SelectedService != null)
                {
                    foreach (MethodInfo method in serviceProxyMetaData.SelectedService.GetMethods())
                    {
                        addSpotMethod(proxyClass, method, socketClient, serviceName);
                    }
                }
                #endregion Create Service Methods


                CodeGeneratorOptions options = new CodeGeneratorOptions();
                CodeDomProvider provider = null;

                if (serviceProxyMetaData.OutputLanguage == OutputLanguage.CSharp)
                {
                    provider = CodeDomProvider.CreateProvider("CSharp");
                    options.BracingStyle = "C";
                }
                else if (serviceProxyMetaData.OutputLanguage == OutputLanguage.VB)
                {
                    provider = CodeDomProvider.CreateProvider("VB");
                    options.BracingStyle = "VB";
                }

                TextWriter textWriter = new StringWriter();

                provider.GenerateCodeFromCompileUnit(compileUnit, textWriter, options);

                code = COMMENT + textWriter.ToString();

                return code;
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
                throw;
            }
        }

        private static void addSpotMethod(CodeTypeDeclaration proxyClass, MethodInfo method, CodeMemberField socketClient, CodeMemberField serviceName)
        {
            if (/*method.DeclaringType.FullName == serviceAssembly.FullName
                        &&*/ method.ReturnType.FullName != "System.Void"
                && method.GetCustomAttributesData().FirstOrDefault(a => a.ToString() == "[Kaushik.Spot.Library.SpotMethodAttribute()]") != null)
            {
                string functionParameters = String.Empty;
                string functionParameterNames = String.Empty;

                CodeMemberMethod spotMethod = new CodeMemberMethod();
                spotMethod.Name = method.Name;
                spotMethod.ReturnType = new CodeTypeReference(method.ReturnType);
                spotMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                CodeVariableDeclarationStatement request = new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(string)), "request");
                spotMethod.Statements.Add(request);

                CodeVariableDeclarationStatement command = new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(ServiceCommand)), "command");
                spotMethod.Statements.Add(command);

                CodeVariableDeclarationStatement result = new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(ServiceResult)), "result");
                spotMethod.Statements.Add(result);

                CodeVariableDeclarationStatement response = new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(string)), "response");
                spotMethod.Statements.Add(response);

                CodeVariableDeclarationStatement finalResult = new CodeVariableDeclarationStatement(new CodeTypeReference(method.ReturnType), "finalResult");
                spotMethod.Statements.Add(finalResult);

                CodeVariableDeclarationStatement socket = new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(Socket)), "socket");
                spotMethod.Statements.Add(socket);


                spotMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(request.Name), new CodePrimitiveExpression(null)));
                spotMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(command.Name), new CodeObjectCreateExpression(new CodeTypeReference(typeof(ServiceCommand)))));
                spotMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(result.Name), new CodeObjectCreateExpression(new CodeTypeReference(typeof(ServiceResult)))));
                spotMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(response.Name), new CodePrimitiveExpression(null)));
                spotMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(finalResult.Name), new CodeDefaultValueExpression(new CodeTypeReference(method.ReturnType))));

                spotMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(socket.Name), new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), socketClient.Name), "CreateClient")));

                CodeTryCatchFinallyStatement executionBlock = new CodeTryCatchFinallyStatement();

                #region Create Finally Section
                CodeBinaryOperatorExpression condition = new CodeBinaryOperatorExpression(
                        new CodeVariableReferenceExpression(socket.Name),
                        CodeBinaryOperatorType.IdentityInequality,
                        new CodePrimitiveExpression(null));

                CodeStatement[] trueStatements = new CodeStatement[] 
                { 
                    new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression(socket.Name), "Dispose"))),
                    new CodeAssignStatement(new CodeVariableReferenceExpression(socket.Name), new CodePrimitiveExpression(null))
                };

                CodeConditionStatement ifStatement = new CodeConditionStatement(condition, trueStatements);

                executionBlock.FinallyStatements.Add(ifStatement);
                #endregion Create Finally Section

                spotMethod.Statements.Add(executionBlock);


                executionBlock.TryStatements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(command.Name), "ServiceName"),
                        new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), serviceName.Name)));

                executionBlock.TryStatements.Add(
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(command.Name), "Method"),
                        new CodePrimitiveExpression(method.Name)));

                ParameterInfo[] parameters = method.GetParameters();

                if (parameters != null)
                {
                    executionBlock.TryStatements.Add(
                       new CodeAssignStatement(
                           new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(command.Name), "Parameters"),
                           new CodeArrayCreateExpression(typeof(Object), parameters.Count())));

                    int counter = 0;

                    foreach (ParameterInfo parameter in parameters)
                    {
                        CodeParameterDeclarationExpression spotMethodParameter = new CodeParameterDeclarationExpression();
                        spotMethodParameter.Type = new CodeTypeReference(parameter.ParameterType.FullName);
                        spotMethodParameter.Name = parameter.Name;
                        spotMethod.Parameters.Add(spotMethodParameter);

                        executionBlock.TryStatements.Add(new CodeAssignStatement(
                            new CodeArrayIndexerExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(command.Name), "Parameters"), new CodePrimitiveExpression(counter)),
                            new CodeVariableReferenceExpression(parameter.Name)));

                        counter++;
                    }

                    executionBlock.TryStatements.Add(new CodeAssignStatement(
                        new CodeVariableReferenceExpression(request.Name),
                        new CodeMethodInvokeExpression(
                                new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "encrypt"),
                                new CodeExpression[] { 
                                    new CodeMethodInvokeExpression( new  CodeMethodReferenceExpression(new CodeVariableReferenceExpression(command.Name), "Serialize"))
                                })
                        ));

                    executionBlock.TryStatements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), socketClient.Name), "Send"), new CodeVariableReferenceExpression(request.Name))));

                    executionBlock.TryStatements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), socketClient.Name), "Receive")
                        , new CodeDirectionExpression(FieldDirection.Out, new CodeVariableReferenceExpression(response.Name)))));

                    executionBlock.TryStatements.Add(new CodeExpressionStatement(
                        new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression(result.Name), "Deserialize"),
                            new CodeExpression[] { 
                                    new CodeMethodInvokeExpression( new  CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "decrypt"), new CodeVariableReferenceExpression(response.Name))
                                }
                            )));
                }

                CodeBinaryOperatorExpression errorCondition = new CodeBinaryOperatorExpression(
                    new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(string)), "IsNullOrEmpty"), new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(result.Name), "Error")),
                    CodeBinaryOperatorType.IdentityInequality,
                    new CodePrimitiveExpression(true));

                CodeStatement[] throwError = new CodeStatement[] 
                { 
                    new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference(typeof(Exception)), new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(result.Name), "Error")))
                };

                CodeConditionStatement throwExceptionCondition = new CodeConditionStatement(errorCondition, throwError);

                executionBlock.TryStatements.Add(throwExceptionCondition);

                CodeStatement parseResult = null;

                if (method.ReturnType == typeof(String))
                {
                    parseResult = new CodeAssignStatement(new CodeVariableReferenceExpression(finalResult.Name), new CodeMethodInvokeExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(result.Name), "Result"), "ToString"));
                }
                else
                {
                    parseResult = new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(method.ReturnType), "TryParse"),
                        new CodeMethodInvokeExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(result.Name), "Result"), "ToString"),
                        new CodeDirectionExpression(FieldDirection.Out, new CodeVariableReferenceExpression(finalResult.Name))));
                }

                CodeBinaryOperatorExpression checkResult = new CodeBinaryOperatorExpression(
                    new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(result.Name), "Result"),
                    CodeBinaryOperatorType.IdentityInequality,
                    new CodePrimitiveExpression(null));

                CodeConditionStatement parseResultCondition = new CodeConditionStatement(checkResult, parseResult);

                executionBlock.TryStatements.Add(parseResultCondition);

                spotMethod.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(finalResult.Name)));

                proxyClass.Members.Add(spotMethod);
            }
        }

        public static ServiceProxyMetaData LoadAssembly(ServiceProxyMetaData serviceProxyMetaData)
        {
            Assembly serviceAssembly = null;

            try
            {
                serviceAssembly = Assembly.LoadFile(serviceProxyMetaData.AssemblyPath);
                Type[] allTypes = serviceAssembly.GetTypes();

                serviceProxyMetaData.Services = (from t in allTypes
                                                 where t.GetCustomAttributesData().FirstOrDefault(a => a.ToString() == "[Kaushik.Spot.Library.SpotServiceAttribute()]") != null
                                                 select t).ToList();
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
                throw;
            }

            return serviceProxyMetaData;
        }

        private static CodeMemberMethod getEncrptAndDecryptMethod(CodeMemberField cipher, string methodName, string cipherMethodName)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Private;
            method.Name = methodName;

            method.ReturnType = new CodeTypeReference(typeof(string));

            CodeParameterDeclarationExpression textParameter = new CodeParameterDeclarationExpression(typeof(string), "text");
            method.Parameters.Add(textParameter);

            CodeBinaryOperatorExpression condition = new CodeBinaryOperatorExpression(
                        new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), cipher.Name),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodePrimitiveExpression(null));

            CodeStatement[] trueStatements = new CodeStatement[] { new CodeMethodReturnStatement(new CodeArgumentReferenceExpression(textParameter.Name)) };
            CodeStatement[] falseStatements = new CodeStatement[] { new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), cipher.Name), cipherMethodName, new CodeExpression[] { new CodeArgumentReferenceExpression(textParameter.Name) })) };

            CodeConditionStatement ifStatement = new CodeConditionStatement(condition, trueStatements, falseStatements);

            method.Statements.Add(ifStatement);
            return method;
        }
    }
}
