﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PedidoLibrary.Servicos {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Mensagens {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Mensagens() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PedidoLibrary.Servicos.Mensagens", typeof(Mensagens).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Informe um produto válido..
        /// </summary>
        public static string M01_PRODUTO_INVALIDO {
            get {
                return ResourceManager.GetString("M01_PRODUTO_INVALIDO", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Informe uma quantidade de produtos acima de zero..
        /// </summary>
        public static string M02_QTDE_PRODUTOS_INVALIDA {
            get {
                return ResourceManager.GetString("M02_QTDE_PRODUTOS_INVALIDA", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to O pedido informado não existe..
        /// </summary>
        public static string M03_PEDIDO_NAO_EXISTE {
            get {
                return ResourceManager.GetString("M03_PEDIDO_NAO_EXISTE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Informe um cliente válido..
        /// </summary>
        public static string M04_CLIENTE_INVALIDO {
            get {
                return ResourceManager.GetString("M04_CLIENTE_INVALIDO", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Seu pedido foi finalizado.
        /// </summary>
        public static string M05_TITULO_EMAIL_PEDIDO_FINALIZADO {
            get {
                return ResourceManager.GetString("M05_TITULO_EMAIL_PEDIDO_FINALIZADO", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parabéns! Seu pedido foi finalizado com sucesso. Em breve você receberá seus produtos..
        /// </summary>
        public static string M06_CORPO_EMAIL_PEDIDO_FINALIZADO {
            get {
                return ResourceManager.GetString("M06_CORPO_EMAIL_PEDIDO_FINALIZADO", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ocorreu um erro no envio do e-mail, mas não se preocupe! Seu pedido foi finalizado com sucesso..
        /// </summary>
        public static string M07_ERRO_ENVIO_EMAIL {
            get {
                return ResourceManager.GetString("M07_ERRO_ENVIO_EMAIL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Este produto não existe no seu pedido..
        /// </summary>
        public static string M08_PRODUTO_NAO_EXISTE_NO_PEDIDO {
            get {
                return ResourceManager.GetString("M08_PRODUTO_NAO_EXISTE_NO_PEDIDO", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Informe um pedido válido..
        /// </summary>
        public static string M09_PEDIDO_INVALIDO {
            get {
                return ResourceManager.GetString("M09_PEDIDO_INVALIDO", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Este pedido é para entrega Express e o produto informado não está disponível para este tipo de entrega..
        /// </summary>
        public static string M10_PRODUTO_NAO_DISPONIVEL_EXPRESS {
            get {
                return ResourceManager.GetString("M10_PRODUTO_NAO_DISPONIVEL_EXPRESS", resourceCulture);
            }
        }
    }
}
