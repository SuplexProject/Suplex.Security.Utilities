using System;
using System.Collections.Generic;
using System.IO;

using YamlDotNet.Serialization;

namespace Suplex.Utilities.Serialization
{
    public class YamlHelpers
    {
        #region Serialize/Deserialize
        //public static void Serialize(TextWriter tw, object data, bool serializeAsJson = false, bool emitDefaultValues = false, IYamlTypeConverter converter = null)
        //{
        //    Serialize( tw, data, serializeAsJson, emitDefaultValues, new IYamlTypeConverter[] { converter } );
        //}
        public static void Serialize(TextWriter tw, object data, bool serializeAsJson = false, bool emitDefaultValues = false, params IYamlTypeConverter[] converters)
        {
            Serializer serializer = null;
            SerializerBuilder builder = new SerializerBuilder();

            if( serializeAsJson )
                builder.JsonCompatible();

            if( emitDefaultValues )
                builder.EmitDefaults();

            if( converters != null && converters.Length > 0 )
                foreach( IYamlTypeConverter converter in converters )
                    if( converter != null )
                        builder.WithTypeConverter( converter );

            serializer = builder.Build() as Serializer;

            serializer.Serialize( tw, data );
        }

        public static string Serialize(object data, bool serializeAsJson = false, bool formatJson = true, bool emitDefaultValues = false, params IYamlTypeConverter[] converters)
        {
            string result = null;

            if( !string.IsNullOrWhiteSpace( data?.ToString() ) )
                using( StringWriter writer = new StringWriter() )
                {
                    Serialize( writer, data, serializeAsJson, emitDefaultValues, converters );
                    result = serializeAsJson && formatJson ? JsonHelpers.FormatJson( writer.ToString() ) : writer.ToString();
                }

            return result;
        }

        public static void SerializeFile(string path, object data, bool serializeAsJson = false, bool formatJson = true, bool emitDefaultValues = false, params IYamlTypeConverter[] converters)
        {
            if( !serializeAsJson )
            {
                using( StreamWriter writer = new StreamWriter( path ) )
                    Serialize( writer, data, serializeAsJson, emitDefaultValues, converters );
            }
            else //gets formatted json
            {
                string result = Serialize( data, serializeAsJson, formatJson, emitDefaultValues, converters );
                File.WriteAllText( path, result );
            }
        }

        public static T Deserialize<T>(string yaml, bool ignoreUnmatchedProperties = true, params IYamlTypeConverter[] converters)
        {
            using( StringReader reader = new StringReader( yaml ) )
            {
                DeserializerBuilder builder = new DeserializerBuilder();

                if( ignoreUnmatchedProperties )
                    builder.IgnoreUnmatchedProperties();

                if( converters != null && converters.Length > 0 )
                    foreach( IYamlTypeConverter converter in converters )
                        if( converter != null )
                            builder.WithTypeConverter( converter );

                Deserializer deserializer = builder.Build() as Deserializer;
                return deserializer.Deserialize<T>( reader );
            }
        }

        public static T Deserialize<T>(TextReader reader, bool ignoreUnmatchedProperties = true, params IYamlTypeConverter[] converters)
        {
            DeserializerBuilder builder = new DeserializerBuilder();

            if( ignoreUnmatchedProperties )
                builder.IgnoreUnmatchedProperties();

            if( converters != null && converters.Length > 0 )
                foreach( IYamlTypeConverter converter in converters )
                    if( converter != null )
                        builder.WithTypeConverter( converter );

            Deserializer deserializer = builder.Build() as Deserializer;
            return deserializer.Deserialize<T>( reader );
        }

        public static T DeserializeFile<T>(string path, bool ignoreUnmatchedProperties = true, params IYamlTypeConverter[] converters) where T : class
        {
            T ssc = null;
            using( StreamReader reader = new StreamReader( path ) )
            {
                DeserializerBuilder builder = new DeserializerBuilder();

                if( ignoreUnmatchedProperties )
                    builder.IgnoreUnmatchedProperties();

                if( converters != null && converters.Length > 0 )
                    foreach( IYamlTypeConverter converter in converters )
                        if( converter != null )
                            builder.WithTypeConverter( converter );

                Deserializer deserializer = builder.Build() as Deserializer;
                ssc = deserializer.Deserialize<T>( reader );
            }
            return ssc;
        }

        public static Dictionary<object, object> Deserialize(string source)
        {
            return Deserialize<Dictionary<object, object>>( source );
        }
        #endregion
    }
}