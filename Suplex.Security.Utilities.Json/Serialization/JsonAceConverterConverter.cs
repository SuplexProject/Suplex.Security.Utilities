using System;
using Suplex.Security.AclModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Suplex.Utilities.Serialization
{
    
    public class JsonAceConverterConverter : JsonConverter
    {
        public override bool CanConvert(Type type)
        {
            return typeof( IAccessControlEntryConverter ).IsAssignableFrom( type );
        }
        public override object ReadJson(JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
        {
            IAccessControlEntryConverter cv = null; 
            Dictionary<string, string> props = new Dictionary<string, string>();
            JObject cvJson = JObject.Load( reader );
            foreach( JProperty prop in cvJson.Properties() )
                props[prop.Name] = prop.Value.ToString();

            if( props.ContainsKey( RightFields.SourceRightType ) && props.ContainsKey( RightFields.TargetRightType ) )
                cv = AccessControlEntryConverterUtilities.MakeAceConverterFromRightType( props[RightFields.SourceRightType], props[RightFields.TargetRightType], props );

            return cv;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if( value is IAccessControlEntryConverter cv )
            {
                JObject props = new JObject
                {
                    { nameof( cv.UId ), cv.UId },
                    { RightFields.SourceRight, cv.SourceRightName },
                    { RightFields.SourceRightType, cv.SourceRightType.AssemblyQualifiedName },
                    { RightFields.TargetRight, cv.TargetRightName },
                    { RightFields.TargetRightType, cv.TargetRightType.AssemblyQualifiedName },
                    { nameof( cv.Inheritable ), cv.Inheritable }
                };
                props.WriteTo( writer );
            }
        }
    }
}
