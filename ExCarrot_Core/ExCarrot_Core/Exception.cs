using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace ExCarrot_Core
{


    public enum InitializeExceptionType
    {
        AlreadyInitalized,
        InvalidArgument,
        DependencyError,
        Unknown,
    }

    public enum DBExceptionType
    {
        InvalidSyntax,
        ConnectionFailed,
        ServerError,
        Unauthorized
    }

    public enum DataExceptionType
    {
        InvalidData,
        InternalError,
        DBNotConnected,
    }

    public enum CloudVExceptionType
    {
        InvalidCall,
        InternalError,
        Unauthorized
    }


       



    public class InitializeException : CloudVException
    {
        public InitializeException() : base() { }
        public InitializeException(string message) : base(message) { }
        public InitializeException(string message, System.Exception inner) : base(message, inner) { }

        protected InitializeException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    public class DataException : CloudVException
    {
        public DataException() : base() { }
        public DataException(string message) : base(message) { }
        public DataException(string message, System.Exception inner) : base(message, inner) { }

        protected DataException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class DBException : CloudVException
    {
        public DBException() : base() { }
        public DBException(string message) : base(message) { }
        public DBException(string message, System.Exception inner) : base(message, inner) { }

        protected DBException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class CloudVException : System.Exception
    {
        public CloudVException() : base(){ }
        public CloudVException(string message) : base(message) { }
        public CloudVException(string message, System.Exception inner) : base(message, inner) { }

        protected CloudVException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
